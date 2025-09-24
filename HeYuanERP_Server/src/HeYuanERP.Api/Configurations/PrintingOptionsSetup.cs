using System;
using System.IO;
using HeYuanERP.Domain.Printing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HeYuanERP.Api.Configurations;

/// <summary>
/// PrintOptions 选项绑定：读取环境变量并结合配置文件，为打印功能提供运行参数。
/// 优先级：环境变量 > appsettings(Printing 节) > 内置默认值。
/// 环境变量键：
/// - PRINT_ENGINE（playwright/puppeteer）
/// - PRINT_TEMPLATES_ROOT（模板根目录）
/// - PRINT_SNAPSHOT_DIR（快照存储目录）
/// - PRINT_DEFAULT_TEMPLATE（默认模板名）
/// </summary>
public class PrintingOptionsSetup : IConfigureOptions<PrintOptions>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PrintingOptionsSetup> _logger;

    public PrintingOptionsSetup(IConfiguration configuration, ILogger<PrintingOptionsSetup> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void Configure(PrintOptions options)
    {
        var section = _configuration.GetSection("Printing");

        // Engine
        options.Engine =
            GetEnv("PRINT_ENGINE") ??
            section["Engine"] ??
            options.Engine ??
            "playwright";

        // TemplatesRoot（默认：<AppBase>/assets/templates）
        options.TemplatesRoot =
            GetEnv("PRINT_TEMPLATES_ROOT") ??
            section["TemplatesRoot"] ??
            options.TemplatesRoot ??
            Path.Combine(AppContext.BaseDirectory, "assets", "templates");

        // SnapshotsRoot（默认：<AppBase>/var/print-snapshots）
        options.SnapshotsRoot =
            GetEnv("PRINT_SNAPSHOT_DIR") ??
            section["SnapshotsRoot"] ??
            options.SnapshotsRoot ??
            Path.Combine(AppContext.BaseDirectory, "var", "print-snapshots");

        // DefaultTemplate（默认：default）
        options.DefaultTemplate =
            GetEnv("PRINT_DEFAULT_TEMPLATE") ??
            section["DefaultTemplate"] ??
            options.DefaultTemplate ??
            "default";

        _logger.LogInformation(
            "打印配置：Engine={Engine}, TemplatesRoot={TemplatesRoot}, SnapshotsRoot={SnapshotsRoot}, DefaultTemplate={DefaultTemplate}",
            options.Engine, options.TemplatesRoot, options.SnapshotsRoot, options.DefaultTemplate);
    }

    private static string? GetEnv(string key)
        => Environment.GetEnvironmentVariable(key);
}

