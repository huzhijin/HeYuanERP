using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Domain.Invoices;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Infrastructure.Repositories;

/// <summary>
/// 发票仓储实现（EF Core）。
/// </summary>
public class InvoiceRepository : IInvoiceRepository
{
    private readonly DbContext _db;

    /// <summary>
    /// 构造函数。
    /// </summary>
    public InvoiceRepository(DbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Set<Invoice>()
            .Include(i => i.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Invoice?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(number)) return null;
        return await _db.Set<Invoice>()
            .Include(i => i.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Number == number, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        if (invoice == null) throw new ArgumentNullException(nameof(invoice));
        await _db.Set<Invoice>().AddAsync(invoice, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Invoice invoice)
    {
        if (invoice == null) throw new ArgumentNullException(nameof(invoice));
        _db.Set<Invoice>().Update(invoice);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<Invoice> Items, int TotalCount)> QueryAsync(
        InvoiceStatus? status,
        Guid? customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<Invoice>().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }
        if (customerId.HasValue)
        {
            query = query.Where(i => i.CustomerId == customerId.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .AsNoTracking()
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
