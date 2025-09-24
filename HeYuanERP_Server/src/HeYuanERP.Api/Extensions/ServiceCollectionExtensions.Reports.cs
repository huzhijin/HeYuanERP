// 版权所有(c) HeYuanERP
// 说明：报表相关服务注册扩展（中文注释）。

using System;
using HeYuanERP.Api.BackgroundWorkers;
using HeYuanERP.Application.Reports;
using HeYuanERP.Application.Reports.Exporters;
using HeYuanERP.Application.Reports.Queries;
using HeYuanERP.Application.Reports.Snapshots;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeYuanERP.Api.Extensions;

/// <summary>
/// 报表服务注册扩展。
/// </summary>
public static class ServiceCollectionExtensionsReports
{
    /// <summary>
    /// 注册报表引擎、导出器、仓储与后台任务。
    /// </summary>
    public static IServiceCollection AddReports(this IServiceCollection services, IConfiguration configuration)
    {
        // 1) 导出器配置（环境变量）
        var exporterOptions = ExporterOptions.FromEnvironment();
        services.AddSingleton(exporterOptions);

        // Worker 选项（环境变量）
        var workerOptions = new ReportExportWorkerOptions
        {
            QueueCapacity = ParseEnvInt("REPORTS_QUEUE_CAPACITY", 200),
            MaxDegreeOfParallelism = Math.Max(1, ParseEnvInt("REPORTS_MAX_DOP", 1))
        };
        services.AddSingleton(workerOptions);

        // 2) 导出器
        services.AddSingleton<CsvExporter>();
        services.AddSingleton<ChromiumPdfExporter>();

        // 3) 白名单与引擎
        services.AddSingleton<IReportParameterWhitelist, ReportParameterWhitelist>();
        services.AddScoped<IReportEngine, ReportEngine>();

        // 4) 查询
        // 销售/发票/PO 仍使用应用层占位实现；库存查询改为基于 EF 的实现
        services.AddScoped<ISalesStatQuery, SalesStatQuery>();
        services.AddScoped<IInvoiceStatQuery, InvoiceStatQuery>();
        services.AddScoped<IPOQuery, POQuery>();
        services.AddScoped<IInventoryQuery, HeYuanERP.Api.Services.Reports.InventoryQueryEf>();

        // 5) 导出服务与快照服务
        services.AddScoped<ReportExportService>();
        services.AddScoped<ReportSnapshotService>();

        // 6) 仓储实现（基础设施层）
        services.AddScoped<IReportJobRepository, ReportJobRepository>();
        services.AddScoped<IReportSnapshotRepository, ReportSnapshotRepository>();

        // 7) 队列与后台 worker
        services.AddSingleton<ReportExportQueue>();
        services.AddSingleton<IReportExportQueue>(sp => sp.GetRequiredService<ReportExportQueue>());
        services.AddHostedService<ReportExportWorker>();

        return services;
    }

    private static int ParseEnvInt(string key, int @default)
        => int.TryParse(Environment.GetEnvironmentVariable(key), out var v) ? v : @default;
}
