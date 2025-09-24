using HeYuanERP.Api.Common;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Inventory;

// 库存服务实现：
// - 库存汇总：来自 InventoryBalances，支持产品/仓库/库位过滤与分页；
// - 库存事务：来自 InventoryTxns，支持多维过滤与时间范围；
// - 平均成本：当前未引入成本字段，返回空（用于报表口径占位）。
public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;

    public InventoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Pagination<InventorySummaryItemDto>> QuerySummaryAsync(InventorySummaryQueryDto req, CancellationToken ct)
    {
        var q = _db.InventoryBalances.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.ProductId))
            q = q.Where(x => x.ProductId == req.ProductId);
        if (!string.IsNullOrWhiteSpace(req.Whse))
            q = q.Where(x => x.Whse == req.Whse);
        if (!string.IsNullOrWhiteSpace(req.Loc))
            q = q.Where(x => x.Loc == req.Loc);

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, req.Page);
        var size = Math.Clamp(req.Size, 1, 200);

        var items = await q
            .OrderBy(x => x.ProductId).ThenBy(x => x.Whse).ThenBy(x => x.Loc)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        // 取产品信息映射（优化：当前页内去重）
        var pids = items.Select(i => i.ProductId).Distinct().ToList();
        var pmap = await _db.Products.AsNoTracking()
            .Where(p => pids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name }, ct);

        var list = items.Select(b => new InventorySummaryItemDto
        {
            ProductId = b.ProductId,
            ProductCode = pmap.TryGetValue(b.ProductId, out var p) ? p.Code : null,
            ProductName = pmap.TryGetValue(b.ProductId, out var p2) ? p2.Name : null,
            Whse = b.Whse,
            Loc = b.Loc,
            OnHand = b.OnHand,
            Reserved = b.Reserved,
            Available = b.Available,
            AvgCost = null // 成本占位
        }).ToList();

        return new Pagination<InventorySummaryItemDto> { Items = list, Page = page, Size = size, Total = total };
    }

    public async Task<Pagination<InventoryTxnItemDto>> QueryTransactionsAsync(InventoryTxnQueryDto req, CancellationToken ct)
    {
        var q = _db.InventoryTxns.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.ProductId))
            q = q.Where(x => x.ProductId == req.ProductId);
        if (!string.IsNullOrWhiteSpace(req.Whse))
            q = q.Where(x => x.Whse == req.Whse);
        if (!string.IsNullOrWhiteSpace(req.Loc))
            q = q.Where(x => x.Loc == req.Loc);
        if (!string.IsNullOrWhiteSpace(req.TxnCode))
            q = q.Where(x => x.TxnCode == req.TxnCode);
        if (req.From.HasValue)
            q = q.Where(x => x.TxnDate >= req.From.Value.Date);
        if (req.To.HasValue)
        {
            var to = req.To.Value.Date.AddDays(1).AddTicks(-1);
            q = q.Where(x => x.TxnDate <= to);
        }

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, req.Page);
        var size = Math.Clamp(req.Size, 1, 200);

        var txns = await q
            .OrderByDescending(x => x.TxnDate).ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        var pids = txns.Select(i => i.ProductId).Distinct().ToList();
        var pmap = await _db.Products.AsNoTracking()
            .Where(p => pids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name }, ct);

        var list = txns.Select(t => new InventoryTxnItemDto
        {
            Id = t.Id,
            TxnCode = t.TxnCode,
            ProductId = t.ProductId,
            ProductCode = pmap.TryGetValue(t.ProductId, out var p) ? p.Code : null,
            ProductName = pmap.TryGetValue(t.ProductId, out var p2) ? p2.Name : null,
            Qty = t.Qty,
            Whse = t.Whse,
            Loc = t.Loc,
            TxnDate = t.TxnDate,
            RefType = t.RefType,
            RefId = t.RefId,
            CreatedAt = t.CreatedAt,
            CreatedBy = t.CreatedBy
        }).ToList();

        return new Pagination<InventoryTxnItemDto> { Items = list, Page = page, Size = size, Total = total };
    }
}

