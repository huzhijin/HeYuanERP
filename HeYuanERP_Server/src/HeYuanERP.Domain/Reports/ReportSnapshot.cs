// 版权所有(c) HeYuanERP
// 说明：报表快照实体（中文注释）。

using System;

namespace HeYuanERP.Domain.Reports;

/// <summary>
/// 报表快照：用于记录指定参数生成的报表文件 URI 与审计信息，便于复用与追溯。
/// </summary>
public class ReportSnapshot
{
    /// <summary>
    /// 主键（Guid）。
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 报表类型。
    /// </summary>
    public ReportType Type { get; set; }

    /// <summary>
    /// 生成时的查询参数（白名单校验后的 JSON）。
    /// </summary>
    public string ParametersJson { get; set; } = string.Empty;

    /// <summary>
    /// 快照文件的 URI（如对象存储或本地持久化路径）。
    /// </summary>
    public string FileUri { get; set; } = string.Empty;

    /// <summary>
    /// 参数哈希（可选，用于去重/命中缓存）。
    /// </summary>
    public string? ParamHash { get; set; }

    /// <summary>
    /// 创建时间（UTC）。
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 创建人（用户标识）。
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 客户端 IP（审计）。
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// User-Agent（审计）。
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 关联追踪 Id（用于 OpenTelemetry 关联）。
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// 并发控制标记（EF Core RowVersion）。
    /// </summary>
    public byte[]? RowVersion { get; set; }
}

