// 版权所有(c) HeYuanERP
// 说明：CSV 导出器实现（中文注释）。

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using HeYuanERP.Domain.Reports;

namespace HeYuanERP.Application.Reports.Exporters;

/// <summary>
/// CSV 导出器。
/// </summary>
public class CsvExporter : IReportExporter
{
    private readonly ExporterOptions _options;

    public CsvExporter(ExporterOptions options)
    {
        _options = options ?? ExporterOptions.FromEnvironment();
    }

    public Task<string> ExportAsync(ExportPayload payload, ReportExportFormat format, CancellationToken ct = default)
    {
        if (format != ReportExportFormat.Csv)
            throw new NotSupportedException($"CsvExporter 仅支持 CSV 格式，实际为: {format}");

        var root = _options.EnsureOutputRoot();
        var fileName = BuildFileName(payload.Title, "csv");
        var path = Path.Combine(root, fileName);

        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); // 带 BOM，便于 Excel 识别 UTF-8

        // 写入表头
        var columns = (payload.Columns == null || payload.Columns.Count == 0)
            ? DeriveColumns(payload)
            : payload.Columns;

        writer.WriteLine(string.Join(",", columns.Select(c => EscapeCsv(c.Header))));

        // 写入数据行
        foreach (var row in payload.Rows)
        {
            if (ct.IsCancellationRequested) break;
            var values = columns.Select(c => FormatValue(row.TryGetValue(c.Key, out var v) ? v : null));
            writer.WriteLine(string.Join(",", values));
        }
        writer.Flush();

        var uri = _options.ToFileUri(path);
        return Task.FromResult(uri);
    }

    private static IReadOnlyList<ExportColumn> DeriveColumns(ExportPayload payload)
    {
        var first = payload.Rows?.FirstOrDefault();
        if (first == null || first.Count == 0)
            return Array.Empty<ExportColumn>();
        return first.Keys.Select(k => new ExportColumn(k, k)).ToList();
    }

    private static string BuildFileName(string title, string ext)
    {
        var safe = string.Join('_', (title ?? "report").Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim('_');
        if (string.IsNullOrWhiteSpace(safe)) safe = "report";
        return $"{safe}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.{ext}";
    }

    private static string FormatValue(object? v)
    {
        if (v == null) return "";
        return v switch
        {
            DateTime dt => EscapeCsv(dt.ToString("yyyy-MM-dd HH:mm:ss")),
            DateTimeOffset dto => EscapeCsv(dto.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")),
            IFormattable f => EscapeCsv(f.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty),
            _ => EscapeCsv(v.ToString() ?? string.Empty)
        };
    }

    private static string EscapeCsv(string s)
    {
        var needQuote = s.Contains('"') || s.Contains(',') || s.Contains('\n') || s.Contains('\r');
        if (!needQuote) return s;
        var escaped = s.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}

