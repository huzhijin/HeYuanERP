// 版权所有(c) HeYuanERP
// 说明：报表导出后台 Worker（中文注释）。

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Reports;
using HeYuanERP.Application.Reports.Exporters;
using HeYuanERP.Application.Reports.Snapshots;
using HeYuanERP.Domain.Reports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Api.BackgroundWorkers;

/// <summary>
/// 报表导出后台任务：从队列读取任务 Id，调用引擎与导出器生成文件并更新任务状态。
/// </summary>
public class ReportExportWorker : BackgroundService
{
    private readonly ILogger<ReportExportWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ReportExportQueue _queue;
    private readonly ReportExportWorkerOptions _options;

    public ReportExportWorker(ILogger<ReportExportWorker> logger, IServiceScopeFactory scopeFactory, ReportExportQueue queue, ReportExportWorkerOptions options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _queue = queue;
        _options = options ?? new ReportExportWorkerOptions();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workers = Math.Max(1, _options.MaxDegreeOfParallelism);
        var tasks = new List<Task>(workers);
        for (var i = 0; i < workers; i++)
        {
            tasks.Add(Task.Run(() => RunLoopAsync(stoppingToken), stoppingToken));
        }
        return Task.WhenAll(tasks);
    }

    private async Task RunLoopAsync(CancellationToken ct)
    {
        _logger.LogInformation("报表导出 Worker 启动，线程：{ThreadId}", Environment.CurrentManagedThreadId);
        var reader = _queue.Reader;
        while (!ct.IsCancellationRequested)
        {
            Guid jobId;
            try
            {
                jobId = await reader.ReadAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取导出任务队列失败。");
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
                continue;
            }

            try
            {
                await ProcessJobAsync(jobId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理导出任务失败，JobId={JobId}", jobId);
            }
        }
    }

    private async Task ProcessJobAsync(Guid jobId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var jobs = scope.ServiceProvider.GetRequiredService<IReportJobRepository>();
        var engine = scope.ServiceProvider.GetRequiredService<IReportEngine>();
        var snapshots = scope.ServiceProvider.GetRequiredService<ReportSnapshotService>();
        var pdf = scope.ServiceProvider.GetRequiredService<ChromiumPdfExporter>();
        var csv = scope.ServiceProvider.GetRequiredService<CsvExporter>();

        var job = await jobs.FindAsync(jobId, ct);
        if (job == null)
        {
            _logger.LogWarning("未找到导出任务，JobId={JobId}", jobId);
            return;
        }

        try
        {
            job.Status = ReportJobStatus.Running;
            job.StartedAtUtc = DateTimeOffset.UtcNow;
            job.ErrorMessage = null;
            await jobs.UpdateAsync(job, ct);

            var parameters = DeserializeParameters(job.ParametersJson);
            var payload = await engine.BuildExportPayloadAsync(job.Type, parameters, job.Format, ct);
            string uri = job.Format == ReportExportFormat.Pdf
                ? await pdf.ExportAsync(payload, job.Format, ct)
                : await csv.ExportAsync(payload, job.Format, ct);

            job.FileUri = uri;
            job.Status = ReportJobStatus.Succeeded;
            job.CompletedAtUtc = DateTimeOffset.UtcNow;
            await jobs.UpdateAsync(job, ct);

            // 记录快照
            await snapshots.CreateAsync(job.Type, parameters, uri, job.CreatedBy, null, null, job.CorrelationId, ct);
        }
        catch (Exception ex)
        {
            job.Status = ReportJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAtUtc = DateTimeOffset.UtcNow;
            await jobs.UpdateAsync(job, ct);
            throw;
        }
    }

    private static IDictionary<string, object?> DeserializeParameters(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new Dictionary<string, object?>();
        try
        {
            var elem = JsonSerializer.Deserialize<Dictionary<string, object?>>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            if (elem is null) return new Dictionary<string, object?>();
            // 将 JsonElement 递归转换为字典/基础类型
            return Normalize(elem);
        }
        catch
        {
            return new Dictionary<string, object?>();
        }
    }

    private static IDictionary<string, object?> Normalize(Dictionary<string, object?> dict)
    {
        var result = new Dictionary<string, object?>();
        foreach (var kv in dict)
        {
            result[kv.Key] = NormalizeValue(kv.Value);
        }
        return result;
    }

    private static object? NormalizeValue(object? v)
    {
        if (v is null) return null;
        if (v is JsonElement je)
        {
            switch (je.ValueKind)
            {
                case JsonValueKind.Object:
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (var prop in je.EnumerateObject())
                    {
                        dict[prop.Name] = NormalizeValue(prop.Value);
                    }
                    return dict;
                }
                case JsonValueKind.Array:
                {
                    var list = new List<object?>();
                    foreach (var item in je.EnumerateArray())
                        list.Add(NormalizeValue(item));
                    return list;
                }
                case JsonValueKind.String:
                    return je.GetString();
                case JsonValueKind.Number:
                    if (je.TryGetInt64(out var l)) return l;
                    if (je.TryGetDouble(out var d)) return d;
                    return je.ToString();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    return null;
            }
        }
        return v;
    }
}

