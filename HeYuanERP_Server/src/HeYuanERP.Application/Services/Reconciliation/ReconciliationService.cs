using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Application.Services.Reconciliation;

/// <summary>
/// 对账单导出服务接口。
/// </summary>
public interface IReconciliationService
{
    /// <summary>
    /// 导出 CSV 对账单。
    /// 返回：文件字节、文件名、Content-Type。
    /// </summary>
    Task<(byte[] Content, string FileName, string ContentType)> ExportCsvAsync(DateOnly? dateFrom, DateOnly? dateTo, string? method, CancellationToken ct = default);
}

/// <summary>
/// 对账单导出服务实现（CSV）。
/// </summary>
public class ReconciliationService : IReconciliationService
{
    private readonly IPaymentRepository _repo;
    private readonly ILogger<ReconciliationService> _logger;

    public ReconciliationService(IPaymentRepository repo, ILogger<ReconciliationService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<(byte[] Content, string FileName, string ContentType)> ExportCsvAsync(DateOnly? dateFrom, DateOnly? dateTo, string? method, CancellationToken ct = default)
    {
        // 参数规范化
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = dateFrom ?? today.AddDays(-30);
        var to = dateTo ?? today;
        if (from > to) (from, to) = (to, from);

        var page = 1;
        const int pageSize = 200;
        var sb = new StringBuilder();

        // 写入 UTF-8 BOM，便于 Excel 识别中文
        // 注意：BOM 将在最终转字节时添加；此处先写表头
        sb.AppendLine("日期,方式,金额,备注,附件数");

        long totalWritten = 0;
        while (true)
        {
            var qp = new PaymentQueryParameters
            {
                Method = string.IsNullOrWhiteSpace(method) ? null : method!.Trim(),
                DateFrom = from,
                DateTo = to
            };

            var (items, total) = await _repo.QueryAsync(qp, page, pageSize, "paymentdate", true, ct);
            foreach (var p in items)
            {
                var line = string.Join(',', new[]
                {
                    p.PaymentDate.ToString("yyyy-MM-dd"),
                    EscapeCsv(p.Method),
                    p.Amount.ToString("0.00", CultureInfo.InvariantCulture),
                    EscapeCsv(p.Remark ?? string.Empty),
                    (p.Attachments?.Count ?? 0).ToString()
                });
                sb.AppendLine(line);
                totalWritten++;
            }

            if (page * pageSize >= total) break;
            page++;
        }

        var csvBytes = WithBomUtf8(sb.ToString());
        var fileName = $"reconciliation_{from:yyyyMMdd}_{to:yyyyMMdd}.csv";
        _logger.LogInformation("对账单导出完成：{Count} 条，文件 {FileName}", totalWritten, fileName);
        return (csvBytes, fileName, "text/csv; charset=utf-8");
    }

    private static string EscapeCsv(string input)
    {
        if (input.Contains('"') || input.Contains(',') || input.Contains('\n') || input.Contains('\r'))
        {
            return '"' + input.Replace("\"", "\"\"") + '"';
        }
        return input;
    }

    private static byte[] WithBomUtf8(string text)
    {
        var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        return utf8.GetBytes(text);
    }
}
