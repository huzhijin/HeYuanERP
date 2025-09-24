using HeYuanERP.Api.DTOs.Invoices;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 发票对账管理接口
/// </summary>
[ApiController]
[Route("api/invoice-reconciliation")]
[Authorize(Policy = "Permission")]
public class InvoiceReconciliationController : ControllerBase
{
    private readonly IInvoiceBusinessRuleService _businessRuleService;
    private readonly ILogger<InvoiceReconciliationController> _logger;

    public InvoiceReconciliationController(
        IInvoiceBusinessRuleService businessRuleService,
        ILogger<InvoiceReconciliationController> logger)
    {
        _businessRuleService = businessRuleService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // =================== 发票验证 ===================

    /// <summary>
    /// 验证发票金额是否合规
    /// </summary>
    [HttpPost("validate-amount")]
    [RequirePermission("invoice.read")]
    public async Task<IActionResult> ValidateInvoiceAmountAsync(
        [FromBody] ValidateInvoiceAmountDto request, CancellationToken ct)
    {
        try
        {
            var result = await _businessRuleService.ValidateInvoiceAmountAsync(
                request.OrderId, request.InvoiceAmount, ct);

            return Ok(new { Success = true, Data = result.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating invoice amount for order {OrderId}", request.OrderId);
            return BadRequest(new { Success = false, Message = "验证发票金额时发生错误" });
        }
    }

    /// <summary>
    /// 验证是否可以对指定发货单开票
    /// </summary>
    [HttpPost("validate-delivery")]
    [RequirePermission("invoice.read")]
    public async Task<IActionResult> ValidateDeliveryInvoiceAsync(
        [FromBody] ValidateDeliveryInvoiceDto request, CancellationToken ct)
    {
        try
        {
            var result = await _businessRuleService.CanInvoiceDeliveryAsync(
                request.DeliveryId, request.Amount, ct);

            return Ok(new { Success = true, Data = result.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating delivery invoice for delivery {DeliveryId}", request.DeliveryId);
            return BadRequest(new { Success = false, Message = "验证发货单开票时发生错误" });
        }
    }

    /// <summary>
    /// 验证发票行项目
    /// </summary>
    [HttpPost("validate-items")]
    [RequirePermission("invoice.read")]
    public async Task<IActionResult> ValidateInvoiceItemsAsync(
        [FromBody] ValidateInvoiceItemsDto request, CancellationToken ct)
    {
        try
        {
            var items = request.Items.Select(i => i.ToBusinessObject()).ToList();
            var result = await _businessRuleService.ValidateInvoiceItemsAsync(
                request.OrderId, items, ct);

            return Ok(new { Success = true, Data = result.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating invoice items for order {OrderId}", request.OrderId);
            return BadRequest(new { Success = false, Message = "验证发票行项目时发生错误" });
        }
    }

    // =================== 对账功能 ===================

    /// <summary>
    /// 执行发票与发货的对账
    /// </summary>
    [HttpPost("reconcile-with-delivery")]
    [RequirePermission("invoice.reconcile")]
    public async Task<IActionResult> ReconcileWithDeliveryAsync(
        [FromBody] ReconcileInvoiceDto request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.DeliveryId))
        {
            return BadRequest(new { Success = false, Message = "发货单ID不能为空" });
        }

        try
        {
            var result = await _businessRuleService.ReconcileInvoiceWithDeliveryAsync(
                request.InvoiceId, request.DeliveryId, ct);

            return Ok(new { Success = true, Data = result.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reconciling invoice {InvoiceId} with delivery {DeliveryId}",
                request.InvoiceId, request.DeliveryId);
            return BadRequest(new { Success = false, Message = "执行发票对账时发生错误" });
        }
    }

    /// <summary>
    /// 执行发票与订单的对账
    /// </summary>
    [HttpPost("reconcile-with-order")]
    [RequirePermission("invoice.reconcile")]
    public async Task<IActionResult> ReconcileWithOrderAsync(
        [FromBody] ReconcileInvoiceDto request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.OrderId))
        {
            return BadRequest(new { Success = false, Message = "订单ID不能为空" });
        }

        try
        {
            var result = await _businessRuleService.ReconcileInvoiceWithOrderAsync(
                request.InvoiceId, request.OrderId, ct);

            return Ok(new { Success = true, Data = result.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reconciling invoice {InvoiceId} with order {OrderId}",
                request.InvoiceId, request.OrderId);
            return BadRequest(new { Success = false, Message = "执行发票对账时发生错误" });
        }
    }

    // =================== 统计和查询 ===================

    /// <summary>
    /// 获取订单的发票统计信息
    /// </summary>
    [HttpGet("order-statistics/{orderId}")]
    [RequirePermission("invoice.read")]
    public async Task<IActionResult> GetOrderInvoiceStatisticsAsync(
        [FromRoute] string orderId, CancellationToken ct)
    {
        try
        {
            var statistics = await _businessRuleService.GetOrderInvoiceStatisticsAsync(orderId, ct);
            return Ok(new { Success = true, Data = statistics.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice statistics for order {OrderId}", orderId);
            return BadRequest(new { Success = false, Message = "获取订单发票统计时发生错误" });
        }
    }

    // =================== 发票管理 ===================

    /// <summary>
    /// 检查发票是否可以作废
    /// </summary>
    [HttpGet("{invoiceId}/can-cancel")]
    [RequirePermission("invoice.read")]
    public async Task<IActionResult> CanCancelInvoiceAsync(
        [FromRoute] string invoiceId, CancellationToken ct)
    {
        try
        {
            var result = await _businessRuleService.CanCancelInvoiceAsync(invoiceId, ct);
            return Ok(new { Success = true, Data = result.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if invoice {InvoiceId} can be cancelled", invoiceId);
            return BadRequest(new { Success = false, Message = "检查发票作废条件时发生错误" });
        }
    }

    /// <summary>
    /// 生成红蓝字调整凭证建议
    /// </summary>
    [HttpGet("{invoiceId}/adjustment-suggestions")]
    [RequirePermission("invoice.read")]
    public async Task<IActionResult> GenerateAdjustmentSuggestionsAsync(
        [FromRoute] string invoiceId, CancellationToken ct)
    {
        try
        {
            var suggestions = await _businessRuleService.GenerateAdjustmentSuggestionsAsync(invoiceId, ct);
            var suggestionsDto = suggestions.Select(s => s.ToDto()).ToList();

            return Ok(new { Success = true, Data = suggestionsDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating adjustment suggestions for invoice {InvoiceId}", invoiceId);
            return BadRequest(new { Success = false, Message = "生成调整凭证建议时发生错误" });
        }
    }

    // =================== 差异处理 ===================

    /// <summary>
    /// 处理对账差异
    /// </summary>
    [HttpPost("differences/{differenceId}/handle")]
    [RequirePermission("invoice.reconcile")]
    public async Task<IActionResult> HandleReconciliationDifferenceAsync(
        [FromRoute] string differenceId,
        [FromBody] HandleReconciliationDifferenceDto request, CancellationToken ct)
    {
        try
        {
            var resolution = request.ToBusinessObject();
            var result = await _businessRuleService.HandleReconciliationDifferenceAsync(
                differenceId, resolution, CurrentUserId, ct);

            if (!result)
            {
                return NotFound(new { Success = false, Message = "对账差异记录不存在" });
            }

            return Ok(new { Success = true, Message = "对账差异处理成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling reconciliation difference {DifferenceId}", differenceId);
            return BadRequest(new { Success = false, Message = "处理对账差异时发生错误" });
        }
    }

    // =================== 批量操作 ===================

    /// <summary>
    /// 批量验证多个发票金额
    /// </summary>
    [HttpPost("batch/validate-amounts")]
    [RequirePermission("invoice.read")]
    public async Task<IActionResult> BatchValidateInvoiceAmountsAsync(
        [FromBody] List<ValidateInvoiceAmountDto> requests, CancellationToken ct)
    {
        try
        {
            var results = new List<object>();

            foreach (var request in requests)
            {
                var result = await _businessRuleService.ValidateInvoiceAmountAsync(
                    request.OrderId, request.InvoiceAmount, ct);

                results.Add(new
                {
                    OrderId = request.OrderId,
                    RequestedAmount = request.InvoiceAmount,
                    Validation = result.ToDto()
                });
            }

            return Ok(new { Success = true, Data = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch validate invoice amounts");
            return BadRequest(new { Success = false, Message = "批量验证发票金额时发生错误" });
        }
    }

    /// <summary>
    /// 批量对账
    /// </summary>
    [HttpPost("batch/reconcile")]
    [RequirePermission("invoice.reconcile")]
    public async Task<IActionResult> BatchReconcileAsync(
        [FromBody] List<ReconcileInvoiceDto> requests, CancellationToken ct)
    {
        try
        {
            var results = new List<object>();

            foreach (var request in requests)
            {
                if (!string.IsNullOrEmpty(request.DeliveryId))
                {
                    var result = await _businessRuleService.ReconcileInvoiceWithDeliveryAsync(
                        request.InvoiceId, request.DeliveryId, ct);

                    results.Add(new
                    {
                        InvoiceId = request.InvoiceId,
                        DeliveryId = request.DeliveryId,
                        Reconciliation = result.ToDto()
                    });
                }
                else if (!string.IsNullOrEmpty(request.OrderId))
                {
                    var result = await _businessRuleService.ReconcileInvoiceWithOrderAsync(
                        request.InvoiceId, request.OrderId, ct);

                    results.Add(new
                    {
                        InvoiceId = request.InvoiceId,
                        OrderId = request.OrderId,
                        Reconciliation = result.ToDto()
                    });
                }
            }

            return Ok(new { Success = true, Data = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch reconcile");
            return BadRequest(new { Success = false, Message = "批量对账时发生错误" });
        }
    }
}