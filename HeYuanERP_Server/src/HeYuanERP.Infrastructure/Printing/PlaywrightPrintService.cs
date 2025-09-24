using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HeYuanERP.Application.Printing;
using HeYuanERP.Domain.Printing;
using HeYuanERP.Domain.Printing.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace HeYuanERP.Infrastructure.Printing;

/// <summary>
/// 基于 Playwright 的 PDF 生成实现（Chromium Headless）。
/// 说明：
/// - 通过 IPrintTemplateRenderer 渲染 HTML，再调用 Playwright 生成 PDF；
/// - 打印参数快照（若存在）用于复现相同渲染结果；
/// - 依赖外部脚本预安装浏览器（参见 scripts/install-playwright-browsers.*）。
/// </summary>
public class PlaywrightPrintService : IPrintService
{
    private readonly IPrintTemplateRenderer _renderer;
    private readonly IPrintSnapshotStore _snapshotStore;
    private readonly IValidator<PrintRequest> _validator;
    private readonly PrintOptions _options;
    private readonly ILogger<PlaywrightPrintService> _logger;

    public PlaywrightPrintService(
        IPrintTemplateRenderer renderer,
        IPrintSnapshotStore snapshotStore,
        IValidator<PrintRequest> validator,
        IOptions<PrintOptions> options,
        ILogger<PlaywrightPrintService> logger)
    {
        _renderer = renderer;
        _snapshotStore = snapshotStore;
        _validator = validator;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<byte[]> GeneratePdfAsync(PrintRequest request, CancellationToken cancellationToken = default)
    {
        // 参数校验
        var result = _validator.Validate(request);
        if (!result.IsValid)
        {
            throw new ArgumentException(string.Join("; ", result.Errors));
        }

        var template = string.IsNullOrWhiteSpace(request.Template) ? _options.DefaultTemplate ?? "default" : request.Template!;

        // 读取快照（若不存在以空对象代替）
        var snapshot = await _snapshotStore.TryGetAsync(request.DocType, request.Id, template, cancellationToken);
        object viewModel = snapshot is null ? new Dictionary<string, object>() : DeserializeToObject(snapshot.DataJson);

        // 渲染 HTML
        var html = await _renderer.RenderHtmlAsync(request.DocType, template, viewModel, cancellationToken);

        var timeoutMs = Math.Max(1000, _options.TimeoutSeconds * 1000);

        // Playwright 生成 PDF
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 }
        });
        var page = await context.NewPageAsync();

        await page.SetContentAsync(html, new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = timeoutMs
        });

        var pdfBytes = await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            PrintBackground = true,
            Margin = new Margin { Top = "10mm", Bottom = "10mm", Left = "10mm", Right = "10mm" }
        });

        _logger.LogInformation("Playwright 已生成 PDF：DocType={DocType}, Id={Id}, Template={Template}, Bytes={Bytes}",
            request.DocType, request.Id, template, pdfBytes?.Length ?? 0);

        return pdfBytes ?? Array.Empty<byte>();
    }

    /// <summary>
    /// 将 JSON 文本转为动态对象（用于简单模板替换）。
    /// </summary>
    private static object DeserializeToObject(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new { };
        try
        {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new Dictionary<string, object>();
        }
        catch
        {
            return new { };
        }
    }
}
