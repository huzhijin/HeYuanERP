using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Api.Data;
using HeYuanERP.Application.Reports.Queries;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Reports;

/// <summary>
/// 基于 EF Core 的库存报表查询实现（替代应用层占位实现）。
/// 数据来源：InventoryBalances、InventoryTxns。
/// </summary>
public class InventoryQueryEf : IInventoryQuery
{
    private readonly AppDbContext _db;

    public InventoryQueryEf(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<InventorySummaryDto>> QuerySummaryAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
    {
        var q = _db.InventoryBalances.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(args?.ProductId))
            q = q.Where(x => x.ProductId == args!.ProductId);
        if (!string.IsNullOrWhiteSpace(args?.Whse))
            q = q.Where(x => x.Whse == args!.Whse);
        if (!string.IsNullOrWhiteSpace(args?.Loc))
            q = q.Where(x => x.Loc == args!.Loc);

        var list = await q
            .OrderBy(x => x.ProductId).ThenBy(x => x.Whse).ThenBy(x => x.Loc)
            .Select(x => new InventorySummaryDto
            {
                ProductId = x.ProductId,
                Whse = x.Whse,
                Loc = x.Loc ?? string.Empty,
                OnHand = x.OnHand,
                Reserved = x.Reserved,
                Available = x.Available
            })
            .ToListAsync(ct);

        return list;
    }

    public async Task<PagedResult<InventoryTxnDto>> QueryTransactionsAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
    {
        var q = _db.InventoryTxns.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(args?.ProductId))
            q = q.Where(x => x.ProductId == args!.ProductId);
        if (!string.IsNullOrWhiteSpace(args?.Whse))
            q = q.Where(x => x.Whse == args!.Whse);
        if (!string.IsNullOrWhiteSpace(args?.Loc))
            q = q.Where(x => x.Loc == args!.Loc);
        if (args?.Range?.StartUtc != null)
            q = q.Where(x => x.TxnDate >= args.Range!.StartUtc!.Value.UtcDateTime.Date);
        if (args?.Range?.EndUtc != null)
        {
            var to = args.Range!.EndUtc!.Value.UtcDateTime.Date.AddDays(1).AddTicks(-1);
            q = q.Where(x => x.TxnDate <= to);
        }

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, args?.Page ?? 1);
        var size = Math.Clamp(args?.Size ?? 20, 1, 500);

        var rows = await q
            .OrderByDescending(x => x.TxnDate).ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(t => new InventoryTxnDto
            {
                TxnId = t.Id,
                TxnCode = t.TxnCode,
                ProductId = t.ProductId,
                Qty = t.Qty,
                Whse = t.Whse,
                Loc = t.Loc ?? string.Empty,
                TxnDate = t.TxnDate,
                RefType = t.RefType,
                RefId = t.RefId
            })
            .ToListAsync(ct);

        return PagedResult<InventoryTxnDto>.Create(rows, page, size, total);
    }
}
