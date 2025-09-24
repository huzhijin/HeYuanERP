using HeYuanERP.Api.Common;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Orders;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Purchase;

// 采购服务实现：采购单 CRUD + 确认 + 收货入库（事务）
public class PurchaseOrdersService : IPurchaseOrdersService
{
    private readonly AppDbContext _db;
    private readonly InventoryGuard _guard;

    public PurchaseOrdersService(AppDbContext db, InventoryGuard guard)
    {
        _db = db;
        _guard = guard;
    }

    public async Task<Pagination<POListItemDto>> QueryAsync(POListQueryDto req, CancellationToken ct)
    {
        var q = _db.PurchaseOrders.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var kw = req.Keyword.Trim();
            q = q.Where(o => o.PoNo.Contains(kw));
        }
        if (!string.IsNullOrWhiteSpace(req.VendorId))
        {
            q = q.Where(o => o.VendorId == req.VendorId);
        }
        if (req.From.HasValue)
        {
            q = q.Where(o => o.PoDate >= req.From.Value.Date);
        }
        if (req.To.HasValue)
        {
            var to = req.To.Value.Date.AddDays(1).AddTicks(-1);
            q = q.Where(o => o.PoDate <= to);
        }
        if (!string.IsNullOrWhiteSpace(req.Status))
        {
            q = q.Where(o => o.Status == req.Status);
        }

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, req.Page);
        var size = Math.Clamp(req.Size, 1, 200);

        var pos = await q
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Include(o => o.Lines)
            .ToListAsync(ct);

        var items = pos.Select(o => new POListItemDto
        {
            Id = o.Id,
            PoNo = o.PoNo,
            VendorId = o.VendorId,
            PoDate = o.PoDate,
            Status = o.Status,
            Remark = o.Remark,
            TotalQty = o.Lines.Sum(l => l.Qty),
            TotalAmount = o.Lines.Sum(l => l.Qty * l.UnitPrice)
        }).ToList();

        return new Pagination<POListItemDto> { Items = items, Page = page, Size = size, Total = total };
    }

    public async Task<PODetailDto?> GetAsync(string id, CancellationToken ct)
    {
        var po = await _db.PurchaseOrders.AsNoTracking().Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct);
        if (po == null) return null;
        return MapDetail(po);
    }

    public async Task<PODetailDto> CreateAsync(POCreateDto req, string currentUserId, CancellationToken ct)
    {
        // 供应商存在性与产品有效性
        if (!await _db.Accounts.AsNoTracking().AnyAsync(a => a.Id == req.VendorId, ct))
            throw new ApplicationException("供应商不存在");
        await _guard.EnsureProductsValidAsync(req.Lines.Select(l => l.ProductId), ct);

        var po = new PurchaseOrder
        {
            PoNo = await NextPoNoAsync(ct),
            VendorId = req.VendorId.Trim(),
            PoDate = req.PoDate.Date,
            Remark = string.IsNullOrWhiteSpace(req.Remark) ? null : req.Remark!.Trim(),
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };
        foreach (var l in req.Lines)
        {
            po.Lines.Add(new POLine
            {
                ProductId = l.ProductId.Trim(),
                Qty = l.Qty,
                UnitPrice = l.UnitPrice,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            });
        }

        _db.PurchaseOrders.Add(po);
        await _db.SaveChangesAsync(ct);

        // 更新供应商最近业务日期
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == po.VendorId, ct);
        if (acc != null)
        {
            acc.LastEventDate = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return MapDetail(po);
    }

    public async Task<PODetailDto> UpdateAsync(string id, POUpdateDto req, string currentUserId, CancellationToken ct)
    {
        var po = await _db.PurchaseOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到采购单");
        if (po.Status != "draft") throw new ApplicationException("仅草稿状态可编辑");

        if (!string.Equals(po.VendorId, req.VendorId, StringComparison.Ordinal))
        {
            if (!await _db.Accounts.AsNoTracking().AnyAsync(a => a.Id == req.VendorId, ct))
                throw new ApplicationException("供应商不存在");
            po.VendorId = req.VendorId.Trim();
        }
        po.PoDate = req.PoDate.Date;
        po.Remark = string.IsNullOrWhiteSpace(req.Remark) ? null : req.Remark!.Trim();
        po.UpdatedAt = DateTime.UtcNow;
        po.UpdatedBy = currentUserId;

        // 行处理：根据 Id 匹配，支持新增/更新/删除
        var incoming = req.Lines;
        await _guard.EnsureProductsValidAsync(incoming.Where(x => x._deleted != true).Select(x => x.ProductId), ct);

        var dict = po.Lines.ToDictionary(l => l.Id, l => l);
        foreach (var l in incoming)
        {
            if (l._deleted == true && !string.IsNullOrWhiteSpace(l.Id) && dict.ContainsKey(l.Id!))
            {
                _db.POLines.Remove(dict[l.Id!]);
                continue;
            }
            if (!string.IsNullOrWhiteSpace(l.Id) && dict.TryGetValue(l.Id!, out var exist))
            {
                exist.ProductId = l.ProductId.Trim();
                exist.Qty = l.Qty;
                exist.UnitPrice = l.UnitPrice;
                exist.UpdatedAt = DateTime.UtcNow;
                exist.UpdatedBy = currentUserId;
            }
            else
            {
                po.Lines.Add(new POLine
                {
                    ProductId = l.ProductId.Trim(),
                    Qty = l.Qty,
                    UnitPrice = l.UnitPrice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }
        }

        await _db.SaveChangesAsync(ct);
        return MapDetail(po);
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        var po = await _db.PurchaseOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到采购单");
        if (po.Status != "draft") throw new ApplicationException("仅草稿状态可删除");
        _db.PurchaseOrders.Remove(po);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ConfirmAsync(string id, string currentUserId, CancellationToken ct)
    {
        var po = await _db.PurchaseOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到采购单");
        if (po.Status != "draft") throw new ApplicationException("仅草稿状态可确认");
        if (po.Lines.Count == 0) throw new ApplicationException("至少需要一条采购行");
        po.Status = "confirmed";
        po.UpdatedAt = DateTime.UtcNow;
        po.UpdatedBy = currentUserId;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<POReceiveDetailDto> ReceiveAsync(string poId, POReceiveCreateDto req, string currentUserId, CancellationToken ct)
    {
        var po = await _db.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == poId, ct)
            ?? throw new KeyNotFoundException("未找到采购单");
        if (po.Status == "draft")
            throw new ApplicationException("请先确认采购单后再收货");

        await _guard.EnsureProductsValidAsync(req.Lines.Select(l => l.ProductId), ct);

        using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var receive = new POReceive
            {
                PoId = po.Id,
                ReceiveDate = req.ReceiveDate.Date,
                Remark = string.IsNullOrWhiteSpace(req.Remark) ? null : req.Remark!.Trim(),
                Status = "completed",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            };

            foreach (var l in req.Lines)
            {
                var line = new POReceiveLine
                {
                    ProductId = l.ProductId.Trim(),
                    Qty = l.Qty,
                    Whse = l.Whse?.Trim(),
                    Loc = string.IsNullOrWhiteSpace(l.Loc) ? null : l.Loc!.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                };
                receive.Lines.Add(line);
            }

            _db.POReceives.Add(receive);
            await _db.SaveChangesAsync(ct);

            // 入库：写库存事务 + 更新结存
            foreach (var l in receive.Lines)
            {
                var txn = new InventoryTxn
                {
                    TxnCode = "PORECEIVE",
                    ProductId = l.ProductId,
                    Qty = l.Qty,
                    Whse = l.Whse ?? string.Empty,
                    Loc = l.Loc,
                    TxnDate = receive.ReceiveDate,
                    RefType = "po.receive",
                    RefId = receive.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                };
                _db.InventoryTxns.Add(txn);

                await UpsertInventoryBalanceAsync(l.ProductId, l.Whse ?? string.Empty, l.Loc, l.Qty, currentUserId, ct);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            // 映射结果
            var dto = new POReceiveDetailDto
            {
                Id = receive.Id,
                PoId = receive.PoId,
                ReceiveDate = receive.ReceiveDate,
                Status = receive.Status,
                Remark = receive.Remark,
                Lines = receive.Lines.Select(x => new POReceiveLineDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Qty = x.Qty,
                    Whse = x.Whse,
                    Loc = x.Loc
                }).ToList()
            };
            return dto;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private async Task UpsertInventoryBalanceAsync(string productId, string whse, string? loc, decimal deltaQty, string currentUserId, CancellationToken ct)
    {
        // 查找现有结存，无则创建；更新 OnHand 与 Available
        var bal = await _db.InventoryBalances.FirstOrDefaultAsync(b => b.ProductId == productId && b.Whse == whse && b.Loc == loc, ct);
        if (bal == null)
        {
            bal = new InventoryBalance
            {
                ProductId = productId,
                Whse = whse,
                Loc = loc,
                OnHand = 0,
                Reserved = 0,
                Available = 0,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = currentUserId
            };
            _db.InventoryBalances.Add(bal);
        }

        bal.OnHand += deltaQty;
        bal.Available = bal.OnHand - bal.Reserved;
        bal.UpdatedAt = DateTime.UtcNow;
        bal.UpdatedBy = currentUserId;
    }

    private PODetailDto MapDetail(PurchaseOrder po)
    {
        return new PODetailDto
        {
            Id = po.Id,
            PoNo = po.PoNo,
            VendorId = po.VendorId,
            PoDate = po.PoDate,
            Status = po.Status,
            Remark = po.Remark,
            Lines = po.Lines.OrderBy(l => l.CreatedAt).Select(l => new POLineDto
            {
                Id = l.Id,
                ProductId = l.ProductId,
                Qty = l.Qty,
                UnitPrice = l.UnitPrice
            }).ToList(),
            CreatedAt = po.CreatedAt,
            CreatedBy = po.CreatedBy,
            UpdatedAt = po.UpdatedAt,
            UpdatedBy = po.UpdatedBy,
            TotalQty = po.Lines.Sum(l => l.Qty),
            TotalAmount = po.Lines.Sum(l => l.Qty * l.UnitPrice)
        };
    }

    private async Task<string> NextPoNoAsync(CancellationToken ct)
    {
        // 简易编号：POyyyyMMdd-随机四位
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        for (int i = 0; i < 5; i++)
        {
            var rnd = Random.Shared.Next(0, 9999).ToString("D4");
            var no = $"PO{date}-{rnd}";
            var exists = await _db.PurchaseOrders.AsNoTracking().AnyAsync(o => o.PoNo == no, ct);
            if (!exists) return no;
        }
        return $"PO{date}-{DateTime.UtcNow:HHmmssfff}";
    }
}
