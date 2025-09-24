// 版权所有(c) HeYuanERP
// 说明：报表快照服务。用于保存导出快照（文件URI+参数+审计），便于追溯与复用（中文注释）。

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Domain.Reports;

namespace HeYuanERP.Application.Reports.Snapshots;

/// <summary>
/// 报表快照服务。
/// </summary>
public class ReportSnapshotService
{
    private readonly IReportSnapshotRepository _snapshots;

    public ReportSnapshotService(IReportSnapshotRepository snapshots)
    {
        _snapshots = snapshots;
    }

    /// <summary>
    /// 创建快照记录。
    /// </summary>
    public async Task<ReportSnapshot> CreateAsync(ReportType type, IDictionary<string, object?> parameters, string fileUri, string? createdBy, string? clientIp, string? userAgent, string? correlationId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fileUri)) throw new ArgumentException("文件URI不能为空。", nameof(fileUri));

        var json = JsonSerializer.Serialize(parameters, JsonOptions);
        var hash = ComputeSha256(json);

        var snapshot = new ReportSnapshot
        {
            Type = type,
            ParametersJson = json,
            FileUri = fileUri,
            ParamHash = hash,
            CreatedBy = createdBy,
            ClientIp = clientIp,
            UserAgent = userAgent,
            CorrelationId = correlationId
        };

        await _snapshots.AddAsync(snapshot, ct);
        return snapshot;
    }

    /// <summary>
    /// 通过 Id 获取快照。
    /// </summary>
    public Task<ReportSnapshot?> FindAsync(Guid id, CancellationToken ct = default)
        => _snapshots.FindAsync(id, ct);

    /// <summary>
    /// 按参数哈希与类型查找最近一次快照（用于命中）。
    /// </summary>
    public Task<ReportSnapshot?> FindByHashAsync(ReportType type, string paramHash, CancellationToken ct = default)
        => _snapshots.FindByHashAsync(type, paramHash, ct);

    private static string ComputeSha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };
}

/// <summary>
/// 报表快照仓储接口（由基础设施层实现）。
/// </summary>
public interface IReportSnapshotRepository
{
    Task AddAsync(ReportSnapshot snapshot, CancellationToken ct = default);
    Task<ReportSnapshot?> FindAsync(Guid id, CancellationToken ct = default);
    Task<ReportSnapshot?> FindByHashAsync(ReportType type, string paramHash, CancellationToken ct = default);
}

