// 版权所有(c) HeYuanERP
// 说明：报表导出器接口与导出负载模型（中文注释）。

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Domain.Reports;

namespace HeYuanERP.Application.Reports.Exporters;

/// <summary>
/// 导出器接口：根据负载与格式，生成文件并返回文件 URI。
/// </summary>
public interface IReportExporter
{
    /// <summary>
    /// 执行导出。
    /// </summary>
    /// <param name="payload">导出负载（包含参数、列、行、可选打印地址/HTML）</param>
    /// <param name="format">导出格式（PDF/CSV）</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>文件 URI</returns>
    Task<string> ExportAsync(ExportPayload payload, ReportExportFormat format, CancellationToken ct = default);
}

/// <summary>
/// 导出列定义。
/// </summary>
public record ExportColumn(string Key, string Header, string? Type = null);

/// <summary>
/// 报表导出负载：统一提供给 CSV/PDF 导出器。
/// </summary>
public class ExportPayload
{
    /// <summary>报表类型。</summary>
    public ReportType Type { get; init; }

    /// <summary>报表标题（用于文件命名与表头）。</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>参数（已通过白名单过滤）。</summary>
    public IDictionary<string, object?> Parameters { get; init; } = new Dictionary<string, object?>();

    /// <summary>列定义（CSV/表格导出）。</summary>
    public IReadOnlyList<ExportColumn> Columns { get; init; } = new List<ExportColumn>();

    /// <summary>行数据（CSV/表格导出）。</summary>
    public IReadOnlyList<IDictionary<string, object?>> Rows { get; init; } = new List<IDictionary<string, object?>>();

    /// <summary>供 PDF 导出器使用的前端打印地址（可选，Chromium 将访问此地址生成 PDF）。</summary>
    public string? PrintUrl { get; init; }

    /// <summary>供 PDF 导出器使用的直出 HTML（可选，二选一）。</summary>
    public string? Html { get; init; }
}

