using HeYuanERP.Api.Common;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Orders;

// 订单服务实现：CRUD + 确认/反审，含价格汇总与基本校验
public class OrdersService : IOrdersService
{
    private readonly AppDbContext _db;
    private readonly PricingService _pricing;
    private readonly InventoryGuard _guard;

    public OrdersService(AppDbContext db, PricingService pricing, InventoryGuard guard)
    {
        _db = db;
        _pricing = pricing;
        _guard = guard;
    }

    public async Task<Pagination<OrderListItemDto>> QueryAsync(OrderListQueryDto req, CancellationToken ct)
    {
        var q = _db.SalesOrders.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var kw = req.Keyword.Trim();
            q = q.Where(o => o.OrderNo.Contains(kw));
        }
        if (!string.IsNullOrWhiteSpace(req.AccountId))
        {
            q = q.Where(o => o.AccountId == req.AccountId);
        }
        if (req.From.HasValue)
        {
            q = q.Where(o => o.OrderDate >= req.From.Value.Date);
        }
        if (req.To.HasValue)
        {
            var to = req.To.Value.Date.AddDays(1).AddTicks(-1);
            q = q.Where(o => o.OrderDate <= to);
        }
        if (!string.IsNullOrWhiteSpace(req.Status))
        {
            q = q.Where(o => o.Status == req.Status);
        }

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, req.Page);
        var size = Math.Clamp(req.Size, 1, 200);

        // 读取行以计算汇总
        var orders = await q
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Include(o => o.Lines)
            .ToListAsync(ct);

        var list = orders.Select(o =>
        {
            var totals = _pricing.Compute(o.Lines.Select(l => (l.Qty, l.UnitPrice, l.Discount, l.TaxRate)));
            return new OrderListItemDto
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                AccountId = o.AccountId,
                OrderDate = o.OrderDate,
                Currency = o.Currency,
                Status = o.Status,
                Remark = o.Remark,
                TotalQty = totals.TotalQty,
                TotalAmount = totals.TotalAmount,
                TotalTax = totals.TotalTax,
                TotalWithTax = totals.TotalWithTax
            };
        }).ToList();

        return new Pagination<OrderListItemDto> { Items = list, Page = page, Size = size, Total = total };
    }

    public async Task<OrderDetailDto?> GetAsync(string id, CancellationToken ct)
    {
        var o = await _db.SalesOrders.AsNoTracking().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (o == null) return null;
        return MapDetail(o);
    }

    public async Task<OrderDetailDto> CreateAsync(OrderCreateDto req, string currentUserId, CancellationToken ct)
    {
        // 账户存在性与产品有效性
        if (!await _db.Accounts.AsNoTracking().AnyAsync(a => a.Id == req.AccountId, ct))
        {
            throw new ApplicationException("客户不存在");
        }
        await _guard.EnsureProductsValidAsync(req.Lines.Select(l => l.ProductId), ct);

        var order = new SalesOrder
        {
            OrderNo = await NextOrderNoAsync(ct),
            AccountId = req.AccountId.Trim(),
            OrderDate = req.OrderDate.Date,
            Currency = req.Currency.Trim(),
            Remark = string.IsNullOrWhiteSpace(req.Remark) ? null : req.Remark!.Trim(),
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId,
        };
        foreach (var l in req.Lines)
        {
            order.Lines.Add(new SalesOrderLine
            {
                ProductId = l.ProductId.Trim(),
                Qty = l.Qty,
                UnitPrice = l.UnitPrice,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                DeliveryDate = l.DeliveryDate,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId,
            });
        }

        _db.SalesOrders.Add(order);
        await _db.SaveChangesAsync(ct);

        // 更新客户最近业务日期
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == order.AccountId, ct);
        if (acc != null)
        {
            acc.LastEventDate = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return MapDetail(order);
    }

    public async Task<OrderDetailDto> UpdateAsync(string id, OrderUpdateDto req, string currentUserId, CancellationToken ct)
    {
        var order = await _db.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到订单");
        if (order.Status != "draft")
        {
            throw new ApplicationException("仅草稿状态可编辑");
        }
        if (!string.Equals(order.AccountId, req.AccountId, StringComparison.Ordinal))
        {
            // 可允许变更客户，但需存在性
            if (!await _db.Accounts.AsNoTracking().AnyAsync(a => a.Id == req.AccountId, ct))
                throw new ApplicationException("客户不存在");
            order.AccountId = req.AccountId.Trim();
        }
        order.OrderDate = req.OrderDate.Date;
        order.Currency = req.Currency.Trim();
        order.Remark = string.IsNullOrWhiteSpace(req.Remark) ? null : req.Remark!.Trim();
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = currentUserId;

        // 行处理：根据 Id 匹配，支持新增/更新/删除
        var incoming = req.Lines;
        await _guard.EnsureProductsValidAsync(incoming.Where(x => x._deleted != true).Select(x => x.ProductId), ct);

        var dict = order.Lines.ToDictionary(l => l.Id, l => l);
        foreach (var l in incoming)
        {
            if (l._deleted == true && !string.IsNullOrWhiteSpace(l.Id) && dict.ContainsKey(l.Id!))
            {
                _db.SalesOrderLines.Remove(dict[l.Id!]);
                continue;
            }
            if (!string.IsNullOrWhiteSpace(l.Id) && dict.TryGetValue(l.Id!, out var exist))
            {
                exist.ProductId = l.ProductId.Trim();
                exist.Qty = l.Qty;
                exist.UnitPrice = l.UnitPrice;
                exist.Discount = l.Discount;
                exist.TaxRate = l.TaxRate;
                exist.DeliveryDate = l.DeliveryDate;
                exist.UpdatedAt = DateTime.UtcNow;
                exist.UpdatedBy = currentUserId;
            }
            else
            {
                order.Lines.Add(new SalesOrderLine
                {
                    ProductId = l.ProductId.Trim(),
                    Qty = l.Qty,
                    UnitPrice = l.UnitPrice,
                    Discount = l.Discount,
                    TaxRate = l.TaxRate,
                    DeliveryDate = l.DeliveryDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId,
                });
            }
        }

        await _db.SaveChangesAsync(ct);
        return MapDetail(order);
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        var order = await _db.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到订单");
        if (order.Status != "draft")
            throw new ApplicationException("仅草稿状态可删除");
        _db.SalesOrders.Remove(order);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ConfirmAsync(string id, string currentUserId, CancellationToken ct)
    {
        var order = await _db.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到订单");
        if (order.Status != "draft")
            throw new ApplicationException("仅草稿状态可确认");
        if (order.Lines.Count == 0)
            throw new ApplicationException("至少需要一条订单行");

        // 此处不做库存操作，交货环节处理；仅设置状态
        order.Status = "confirmed";
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = currentUserId;
        await _db.SaveChangesAsync(ct);
    }

    public async Task ReverseAsync(string id, string currentUserId, string? reason, CancellationToken ct)
    {
        var order = await _db.SalesOrders.FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到订单");
        if (order.Status != "confirmed")
            throw new ApplicationException("仅已确认状态可反审");

        // 反审标记
        order.Status = "reversed";
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = currentUserId;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            order.Remark = string.IsNullOrWhiteSpace(order.Remark) ? reason!.Trim() : (order.Remark + " | Reverse: " + reason!.Trim());
        }
        await _db.SaveChangesAsync(ct);
    }

    private async Task<string> NextOrderNoAsync(CancellationToken ct)
    {
        // 简易编号：SOyyyyMMdd-随机四位，重试避免冲突
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        for (int i = 0; i < 5; i++)
        {
            var rnd = Random.Shared.Next(0, 9999).ToString("D4");
            var no = $"SO{date}-{rnd}";
            var exists = await _db.SalesOrders.AsNoTracking().AnyAsync(o => o.OrderNo == no, ct);
            if (!exists) return no;
        }
        // 兜底：时间戳
        return $"SO{date}-{DateTime.UtcNow:HHmmssfff}";
    }

    private OrderDetailDto MapDetail(SalesOrder o)
    {
        var totals = _pricing.Compute(o.Lines.Select(l => (l.Qty, l.UnitPrice, l.Discount, l.TaxRate)));
        return new OrderDetailDto
        {
            Id = o.Id,
            OrderNo = o.OrderNo,
            AccountId = o.AccountId,
            OrderDate = o.OrderDate,
            Currency = o.Currency,
            Status = o.Status,
            Remark = o.Remark,
            Lines = o.Lines.OrderBy(l => l.CreatedAt).Select(l => new OrderLineDto
            {
                Id = l.Id,
                ProductId = l.ProductId,
                Qty = l.Qty,
                UnitPrice = l.UnitPrice,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                DeliveryDate = l.DeliveryDate
            }).ToList(),
            CreatedAt = o.CreatedAt,
            CreatedBy = o.CreatedBy,
            UpdatedAt = o.UpdatedAt,
            UpdatedBy = o.UpdatedBy,
            TotalQty = totals.TotalQty,
            TotalAmount = totals.TotalAmount,
            TotalTax = totals.TotalTax,
            TotalWithTax = totals.TotalWithTax
        };
    }
}

