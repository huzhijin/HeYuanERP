using System;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Api.Models;
using HeYuanERP.Application.Printing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 打印接口控制器：统一 GET /api/print/{docType}/{id}?template=xxx 返回 application/pdf
/// - 打印引擎：Chromium Headless（Playwright/PuppeteerSharp，由 IPrintService 具体实现）
/// - 鉴权：由全局 JWT+RBAC 中间件控制（本控制器只聚焦打印职责）
/// - 响应：直接返回 PDF 二进制流，不包统一响应 Envelope（按 OpenAPI 设计）
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PrintController : ControllerBase
{
    private readonly IPrintService _printService;
    private readonly ILogger<PrintController> _logger;

    public PrintController(IPrintService printService, ILogger<PrintController> logger)
    {
        _printService = printService;
        _logger = logger;
    }

    /// <summary>
    /// 统一打印接口：根据单据类型与标识生成 PDF。
    /// 示例：GET /api/print/order/12345?template=default
    /// </summary>
    /// <param name="docType">单据类型（order/delivery/return/invoice/statement 等）</param>
    /// <param name="id">单据主键（字符串或数值均可）</param>
    /// <param name="query">查询参数（模板名等）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>PDF 二进制流（Content-Type: application/pdf）</returns>
    [HttpGet("{docType}/{id}")]
    [Produces(MediaTypeNames.Application.Pdf)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(
        [FromRoute] string docType,
        [FromRoute] string id,
        [FromQuery] PrintRequestQuery query,
        CancellationToken cancellationToken)
    {
        // 基础参数校验（更全面校验交由 Application 层 Validator 处理）
        if (string.IsNullOrWhiteSpace(docType))
        {
            return BadRequest("docType 不能为空");
        }
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("id 不能为空");
        }

        try
        {
            var request = new PrintRequest
            {
                DocType = docType,
                Id = id,
                Template = query.Template
            };

            var pdfBytes = await _printService.GeneratePdfAsync(request, cancellationToken);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("未生成任何内容或单据不存在");
            }

            var safeDocType = docType.Trim().ToLowerInvariant();
            var fileName = $"{safeDocType}-{id}.pdf";
            return File(pdfBytes, MediaTypeNames.Application.Pdf, fileName);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "打印参数错误：{Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "资源不存在：{Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "打印失败：{Message}", ex.Message);
            // 统一隐藏内部异常细节
            return Problem(title: "打印失败", detail: null, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}

