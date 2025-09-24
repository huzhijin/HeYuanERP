// 版权所有(c) HeYuanERP
// 说明：报表导出服务。负责创建异步任务、参数白名单过滤与审计信息记录（中文注释）。

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;

namespace HeYuanERP.Application.Reports;

/// <summary>
/// 报表导出服务（应用层）。
/// </summary>
public class ReportExportService
{
    private readonly IReportParameterWhitelist _whitelist;
    private readonly IReportJobRepository _jobs;
    private readonly IReportExportQueue? _queue;

    public ReportExportService(IReportParameterWhitelist whitelist, IReportJobRepository jobs, IReportExportQueue? queue = null)
    {
        _whitelist = whitelist;
        _jobs = jobs;
        _queue = queue;
    }

    /// <summary>
    /// 创建导出任务并入队（若提供队列）。
    /// </summary>
    public async Task<ReportTaskDto> EnqueueAsync(string name, ReportExportRequestDto req, string? createdBy, string? clientIp, string? userAgent, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("报表名称不能为空。", nameof(name));

        // 名称到类型的映射（可根据实际口径扩展同义词）
        var type = MapNameToType(name);

        // 1) 参数白名单过滤
        var rawParams = req?.Params ?? new Dictionary<string, object?>();
        var safeParams = _whitelist.Filter(type, rawParams);

        // 2) 格式解析
        var format = ParseFormatOrDefault(req?.Format);

        // 3) 创建任务记录
        var job = new ReportJob
        {
            Type = type,
            Format = format,
            Status = ReportJobStatus.Queued,
            ParametersJson = JsonSerializer.Serialize(safeParams, JsonSerializerOptions),
            CreatedBy = createdBy,
            CorrelationId = Activity.Current?.Id,
            FileUri = null,
            ErrorMessage = null
        };

        await _jobs.AddAsync(job, ct);

        // 4) 入队（可选，由 API 层队列实现）
        _queue?.Enqueue(job.Id);

        return new ReportTaskDto
        {
            TaskId = job.Id.ToString(),
            Status = "queued",
            FileUri = null,
            Message = null,
            CreatedAt = job.CreatedAtUtc,
            FinishedAt = null
        };
    }

    /// <summary>
    /// 按任务 Id 获取导出任务状态。
    /// </summary>
    public async Task<ReportTaskDto?> GetTaskAsync(Guid id, CancellationToken ct = default)
    {
        var job = await _jobs.FindAsync(id, ct);
        if (job == null) return null;

        return new ReportTaskDto
        {
            TaskId = job.Id.ToString(),
            Status = MapStatus(job.Status),
            FileUri = job.FileUri,
            Message = job.ErrorMessage,
            CreatedAt = job.CreatedAtUtc,
            FinishedAt = job.CompletedAtUtc
        };
    }

    private static ReportType MapNameToType(string name)
    {
        var key = name.Trim().ToLowerInvariant();
        return key switch
        {
            "sales" or "salesstat" or "sales-stat" => ReportType.SalesStat,
            "invoice" or "invoicestat" or "invoice-stat" => ReportType.InvoiceStat,
            "po" or "poquery" or "po-query" => ReportType.POQuery,
            "inventory" => ReportType.Inventory,
            _ => throw new ArgumentOutOfRangeException(nameof(name), $"不支持的报表名称: {name}")
        };
    }

    private static ReportExportFormat ParseFormatOrDefault(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return ReportExportFormat.Pdf; // 默认 PDF
        var key = s.Trim().ToLowerInvariant();
        return key switch
        {
            "pdf" => ReportExportFormat.Pdf,
            "csv" => ReportExportFormat.Csv,
            _ => ReportExportFormat.Pdf
        };
    }

    private static string MapStatus(ReportJobStatus status) => status switch
    {
        ReportJobStatus.Queued => "queued",
        ReportJobStatus.Running => "running",
        ReportJobStatus.Succeeded => "completed",
        ReportJobStatus.Failed => "failed",
        ReportJobStatus.Canceled => "failed",
        _ => "queued"
    };

    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };
}

/// <summary>
/// 报表任务仓储接口（由基础设施层实现）。
/// </summary>
public interface IReportJobRepository
{
    Task AddAsync(ReportJob job, CancellationToken ct = default);
    Task<ReportJob?> FindAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(ReportJob job, CancellationToken ct = default);
}

/// <summary>
/// 报表导出入队接口（由 API 背景任务队列实现）。
/// </summary>
public interface IReportExportQueue
{
    void Enqueue(Guid jobId);
}

