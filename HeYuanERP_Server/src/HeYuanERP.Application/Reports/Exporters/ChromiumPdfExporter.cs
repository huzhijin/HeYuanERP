// 版权所有(c) HeYuanERP
// 说明：Chromium Headless PDF 导出器实现（中文注释）。

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Domain.Reports;

namespace HeYuanERP.Application.Reports.Exporters;

/// <summary>
/// 基于 Chromium Headless 的 PDF 导出器。
/// - 当提供 PrintUrl 时，使用 URL 渲染为 PDF；
/// - 当提供 Html 时，写入临时 HTML 文件后渲染为 PDF；
/// - 返回文件 URI（或映射后的公网 URL）。
/// </summary>
public class ChromiumPdfExporter : IReportExporter
{
    private readonly ExporterOptions _options;

    public ChromiumPdfExporter(ExporterOptions options)
    {
        _options = options ?? ExporterOptions.FromEnvironment();
    }

    public async Task<string> ExportAsync(ExportPayload payload, ReportExportFormat format, CancellationToken ct = default)
    {
        if (format != ReportExportFormat.Pdf)
            throw new NotSupportedException($"ChromiumPdfExporter 仅支持 PDF 格式，实际为: {format}");

        var root = _options.EnsureOutputRoot();
        var pdfName = BuildFileName(payload.Title, "pdf");
        var pdfPath = Path.Combine(root, pdfName);

        string? htmlTempPath = null;
        try
        {
            var target = payload.PrintUrl;
            if (string.IsNullOrWhiteSpace(target))
            {
                if (string.IsNullOrWhiteSpace(payload.Html))
                {
                    // 当未提供 PrintUrl/Html 时，退化为根据 Columns/Rows 构造简易 HTML
                    var html = HtmlFromTable(payload);
                    htmlTempPath = Path.Combine(root, Path.GetFileNameWithoutExtension(pdfName) + ".html");
                    await File.WriteAllTextAsync(htmlTempPath, html, ct);
                    target = new Uri(htmlTempPath).AbsoluteUri;
                }
                else
                {
                    // 将 HTML 写入临时文件
                    htmlTempPath = Path.Combine(root, Path.GetFileNameWithoutExtension(pdfName) + ".html");
                    await File.WriteAllTextAsync(htmlTempPath, payload.Html!, ct);
                    target = new Uri(htmlTempPath).AbsoluteUri;
                }
            }

            await RunChromiumAsync(target!, pdfPath, ct);

            if (!File.Exists(pdfPath))
                throw new IOException("PDF 生成失败，目标文件不存在。");

            return _options.ToFileUri(pdfPath);
        }
        finally
        {
            // 清理临时 HTML
            if (htmlTempPath != null)
            {
                try { File.Delete(htmlTempPath); } catch { /* 忽略 */ }
            }
        }
    }

    private async Task RunChromiumAsync(string targetUrlOrFile, string outputPdf, CancellationToken ct)
    {
        var chrome = ResolveChromiumPath();
        var args = BuildChromiumArgs(outputPdf, targetUrlOrFile);

        var psi = new ProcessStartInfo
        {
            FileName = chrome,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
        proc.Start();

        // 简单等待完成
        await proc.WaitForExitAsync(ct);
        if (proc.ExitCode != 0)
        {
            var err = await proc.StandardError.ReadToEndAsync();
            throw new InvalidOperationException($"Chromium 生成 PDF 失败，退出码: {proc.ExitCode}，错误: {err}");
        }
    }

    private static string BuildChromiumArgs(string pdfPath, string target)
    {
        // --no-sandbox 便于容器/CI 环境；--disable-dev-shm-usage 避免共享内存不足
        return $"--headless=new --disable-gpu --no-sandbox --disable-dev-shm-usage --print-to-pdf=\"{pdfPath}\" \"{target}\"";
    }

    private string ResolveChromiumPath()
    {
        if (!string.IsNullOrWhiteSpace(_options.ChromiumPath) && File.Exists(_options.ChromiumPath))
            return _options.ChromiumPath!;

        var env = Environment.GetEnvironmentVariable("CHROMIUM_PATH");
        if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
            return env!;

        // 常见可执行名回退
        string[] candidates = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new[] {
                // Windows 可能的安装路径（若未找到，回退到可执行名，由 PATH 决定）
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google/Chrome/Application/chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google/Chrome/Application/chrome.exe"),
                "chrome.exe", "msedge.exe"
            }
            : new[] { 
                "/usr/bin/google-chrome-stable", "/usr/bin/google-chrome", "/usr/bin/chromium", "/usr/bin/chromium-browser", "google-chrome", "chromium", "chromium-browser" 
            };

        foreach (var c in candidates)
        {
            if (File.Exists(c)) return c;
        }
        // 最终回退：交给系统 PATH
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "chrome.exe" : "chromium";
    }

    private static string BuildFileName(string title, string ext)
    {
        var safe = string.Join('_', (title ?? "report").Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim('_');
        if (string.IsNullOrWhiteSpace(safe)) safe = "report";
        return $"{safe}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.{ext}";
    }

    private static string HtmlFromTable(ExportPayload payload)
    {
        // 生成一个简单的 HTML 表格用于打印（中文注释）。
        var title = string.IsNullOrWhiteSpace(payload.Title) ? "报表" : payload.Title;
        var cols = payload.Columns is { Count: > 0 } ? payload.Columns : new[] { new ExportColumn("", "") };
        var sb = new System.Text.StringBuilder();
        sb.Append("<!DOCTYPE html><html lang=\"zh-CN\"><head><meta charset=\"utf-8\"><title>");
        sb.Append(System.Net.WebUtility.HtmlEncode(title));
        sb.Append("</title><style>body{font-family:Arial,Helvetica,\"Microsoft YaHei\",sans-serif;padding:16px;}table{border-collapse:collapse;width:100%;}th,td{border:1px solid #999;padding:6px 8px;font-size:12px;text-align:left;}th{background:#f2f2f2;}</style></head><body>");
        sb.Append("<h3>");
        sb.Append(System.Net.WebUtility.HtmlEncode(title));
        sb.Append("</h3><table><thead><tr>");
        foreach (var c in cols)
        {
            sb.Append("<th>").Append(System.Net.WebUtility.HtmlEncode(c.Header)).Append("</th>");
        }
        sb.Append("</tr></thead><tbody>");
        foreach (var row in payload.Rows)
        {
            sb.Append("<tr>");
            foreach (var c in cols)
            {
                row.TryGetValue(c.Key, out var v);
                sb.Append("<td>").Append(System.Net.WebUtility.HtmlEncode(v?.ToString() ?? "")).Append("</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody></table></body></html>");
        return sb.ToString();
    }
}
