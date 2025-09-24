// 版权所有(c) HeYuanERP
// 说明：报表任务仓储实现（EF Core，中文注释）。

using System;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Reports;
using HeYuanERP.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Infrastructure.Data.Repositories;

/// <summary>
/// 报表任务仓储实现。
/// </summary>
public class ReportJobRepository : IReportJobRepository
{
    private readonly DbContext _db;

    public ReportJobRepository(DbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ReportJob job, CancellationToken ct = default)
    {
        await _db.Set<ReportJob>().AddAsync(job, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task<ReportJob?> FindAsync(Guid id, CancellationToken ct = default)
        => _db.Set<ReportJob>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task UpdateAsync(ReportJob job, CancellationToken ct = default)
    {
        _db.Set<ReportJob>().Update(job);
        await _db.SaveChangesAsync(ct);
    }
}
