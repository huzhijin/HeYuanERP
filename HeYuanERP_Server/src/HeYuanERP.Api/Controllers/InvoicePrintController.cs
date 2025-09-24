using System;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Api.Options;
using HeYuanERP.Application.Invoices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 发票打印接口：调用 P11 渲染模板并返回 PDF。
/// </summary>
[ApiController]
[Route("api/invoices")]
public class InvoicePrintController : ControllerBase
{
    private readonly InvoicePrintService _printService;
    private readonly PrintingOptions _options;

    public InvoicePrintController(InvoicePrintService printService, IOptions<PrintingOptions> options)
    {
        _printService = printService;
        _options = options.Value ?? new PrintingOptions();
    }

    /// <summary>
    /// 按发票 Id 渲染 PDF。
    /// </summary>
    /// <param name="id">发票 Id。</param>
    /// <param name="templateCode">模板代码（可选，默认使用配置）。</param>
    /// <param name="downloadName">下载文件名（可选，不含扩展名）。</param>
    [HttpGet("{id:guid}/print/pdf")]
    public async Task<IActionResult> PrintPdfAsync([FromRoute] Guid id, [FromQuery] string? templateCode, [FromQuery] string? downloadName, CancellationToken ct)
    {
        var code = string.IsNullOrWhiteSpace(templateCode) ? _options.InvoiceTemplateCode : templateCode!;
        var bytes = await _printService.PrintInvoiceAsync(id, code, ct);
        var fileName = string.IsNullOrWhiteSpace(downloadName) ? $"{_options.DownloadNamePrefix}{DateTime.UtcNow:yyyyMMddHHmmss}.pdf" : $"{downloadName}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}

