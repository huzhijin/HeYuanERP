// 版权所有(c) HeYuanERP
// 说明：报表中间件/启动扩展（中文注释）。

using HeYuanERP.Application.Reports.Exporters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Api.Extensions;

/// <summary>
/// 应用构建扩展：用于在启动时确保导出目录存在。
/// </summary>
public static class ApplicationBuilderExtensionsReports
{
    /// <summary>
    /// 启用报表相关的运行时准备（如导出目录）。
    /// </summary>
    public static IApplicationBuilder UseReports(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("ReportsStartup");
        var options = app.ApplicationServices.GetRequiredService<ExporterOptions>();
        var path = options.EnsureOutputRoot();
        logger.LogInformation("报表导出目录已准备：{Path}", path);
        return app;
    }
}

