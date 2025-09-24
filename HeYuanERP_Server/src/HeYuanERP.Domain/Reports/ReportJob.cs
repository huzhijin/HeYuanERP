// 版权所有(c) HeYuanERP
// 说明：报表导出异步任务实体（中文注释）。

using System;

namespace HeYuanERP.Domain.Reports;

/// <summary>
/// 报表导出任务（异步）：记录导出状态、文件地址、参数与审计信息。
/// </summary>
public class ReportJob
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
    /// 导出格式，例如 PDF/CSV。
    /// </summary>
    public ReportExportFormat Format { get; set; }

    /// <summary>
    /// 任务状态。
    /// </summary>
    public ReportJobStatus Status { get; set; } = ReportJobStatus.Pending;

    /// <summary>
    /// 请求参数（已通过白名单校验后的 JSON 序列化字符串）。
    /// </summary>
    public string ParametersJson { get; set; } = string.Empty;

    /// <summary>
    /// 导出结果文件的 URI（成功时写入）。
    /// </summary>
    public string? FileUri { get; set; }

    /// <summary>
    /// 错误信息（失败时写入）。
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 关联追踪 Id（用于 OpenTelemetry 关联）。
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// 创建时间（UTC）。
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 创建人（用户标识）。
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 开始处理时间（UTC）。
    /// </summary>
    public DateTimeOffset? StartedAtUtc { get; set; }

    /// <summary>
    /// 完成时间（UTC）。
    /// </summary>
    public DateTimeOffset? CompletedAtUtc { get; set; }

    /// <summary>
    /// 并发控制标记（EF Core RowVersion）。
    /// </summary>
    public byte[]? RowVersion { get; set; }
}

