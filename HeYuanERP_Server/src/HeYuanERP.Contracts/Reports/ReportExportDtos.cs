// 版权所有(c) HeYuanERP
// 说明：报表导出相关契约 DTO（与 OpenAPI 对齐，中文注释）。

using System;
using System.Collections.Generic;

namespace HeYuanERP.Contracts.Reports;

/// <summary>
/// 报表导出请求体（/api/reports/{name}/export）。
/// 与 OpenAPI 的 ReportExportRequest 对齐：仅包含 params 动态对象；
/// 额外的 format（pdf/csv）作为可选字段，便于前后端约定。
/// </summary>
public sealed class ReportExportRequestDto
{
    /// <summary>
    /// 导出格式（可选）：pdf/csv。若省略则默认由后端按报表类型决定。
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// 报表参数（已通过白名单校验）。键名与值均需满足后端定义的校验规则。
    /// </summary>
    public IDictionary<string, object?> Params { get; init; } = new Dictionary<string, object?>();
}

/// <summary>
/// 报表任务状态响应（与 OpenAPI ReportTask 对齐）。
/// </summary>
public sealed class ReportTaskDto
{
    /// <summary>任务 Id</summary>
    public string TaskId { get; init; } = string.Empty;

    /// <summary>
    /// 状态：queued|running|completed|failed
    /// </summary>
    public string Status { get; init; } = "queued";

    /// <summary>导出文件的 URI（完成时提供）</summary>
    public string? FileUri { get; init; }

    /// <summary>错误或说明消息</summary>
    public string? Message { get; init; }

    /// <summary>创建时间（UTC）</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>完成时间（UTC，可空）</summary>
    public DateTimeOffset? FinishedAt { get; init; }
}

