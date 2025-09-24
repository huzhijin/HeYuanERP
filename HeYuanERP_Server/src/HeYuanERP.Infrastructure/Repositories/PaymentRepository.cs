using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Interfaces.Repositories;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Infrastructure.Repositories;

/// <summary>
/// 收款仓储实现（EF Core）。
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly DbContext _db;

    public PaymentRepository(DbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 新增收款（包含附件）。
    /// </summary>
    public async Task<Payment> AddAsync(Payment payment, CancellationToken ct = default)
    {
        await _db.Set<Payment>().AddAsync(payment, ct);
        await _db.SaveChangesAsync(ct);
        return payment;
    }

    /// <summary>
    /// 根据 Id 获取收款（包含附件）。
    /// </summary>
    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _db.Set<Payment>()
            .AsNoTracking()
            .Include(p => p.Attachments)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    /// <summary>
    /// 分页查询（含筛选与排序）。
    /// </summary>
    public async Task<(IReadOnlyList<Payment> Items, long TotalCount)> QueryAsync(
        PaymentQueryParameters query,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var q = _db.Set<Payment>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Method))
        {
            var method = query.Method.Trim();
            q = q.Where(p => p.Method == method);
        }

        if (query.MinAmount.HasValue)
        {
            q = q.Where(p => p.Amount >= query.MinAmount.Value);
        }

        if (query.MaxAmount.HasValue)
        {
            q = q.Where(p => p.Amount <= query.MaxAmount.Value);
        }

        if (query.DateFrom.HasValue)
        {
            var from = query.DateFrom.Value;
            q = q.Where(p => p.PaymentDate >= from);
        }

        if (query.DateTo.HasValue)
        {
            var to = query.DateTo.Value;
            q = q.Where(p => p.PaymentDate <= to);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(p => p.Remark != null && p.Remark.Contains(kw));
        }

        // 排序
        IOrderedQueryable<Payment> oq;
        switch ((sortBy ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "amount":
                oq = sortDesc ? q.OrderByDescending(p => p.Amount) : q.OrderBy(p => p.Amount);
                break;
            case "createdat":
                oq = sortDesc ? q.OrderByDescending(p => p.CreatedAtUtc) : q.OrderBy(p => p.CreatedAtUtc);
                break;
            case "paymentdate":
            default:
                oq = sortDesc ? q.OrderByDescending(p => p.PaymentDate) : q.OrderBy(p => p.PaymentDate);
                break;
        }

        var total = await oq.CountAsync(ct);
        var items = await oq.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}
