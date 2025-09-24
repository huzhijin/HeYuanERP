// 版权所有(c) HeYuanERP
// 说明：报表快照仓储实现（EF Core，中文注释）。

using System;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Reports.Snapshots;
using HeYuanERP.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Infrastructure.Data.Repositories;

/// <summary>
/// 报表快照仓储实现。
/// </summary>
public class ReportSnapshotRepository : IReportSnapshotRepository
{
    private readonly DbContext _db;

    public ReportSnapshotRepository(DbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ReportSnapshot snapshot, CancellationToken ct = default)
    {
        await _db.Set<ReportSnapshot>().AddAsync(snapshot, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task<ReportSnapshot?> FindAsync(Guid id, CancellationToken ct = default)
        => _db.Set<ReportSnapshot>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<ReportSnapshot?> FindByHashAsync(ReportType type, string paramHash, CancellationToken ct = default)
        => _db.Set<ReportSnapshot>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Type == type && x.ParamHash == paramHash, ct);
}
