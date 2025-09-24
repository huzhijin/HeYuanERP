using HeYuanERP.Api.DTOs.Purchase;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Purchase;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 采购异常处理管理接口
/// </summary>
[ApiController]
[Route("api/purchase-exceptions")]
[Authorize(Policy = "Permission")]
public class PurchaseExceptionsController : ControllerBase
{
    private readonly IPurchaseExceptionService _exceptionService;
    private readonly ILogger<PurchaseExceptionsController> _logger;

    public PurchaseExceptionsController(
        IPurchaseExceptionService exceptionService,
        ILogger<PurchaseExceptionsController> logger)
    {
        _exceptionService = exceptionService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // =================== 异常管理 ===================

    /// <summary>
    /// 创建采购异常
    /// </summary>
    [HttpPost]
    [RequirePermission("po.create")]
    public async Task<IActionResult> CreateExceptionAsync(
        [FromBody] CreatePurchaseExceptionDto request, CancellationToken ct)
    {
        try
        {
            var businessRequest = request.ToBusinessObject();
            var exception = await _exceptionService.CreateExceptionAsync(businessRequest, CurrentUserId, ct);

            return Ok(new { Success = true, Data = exception.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase exception");
            return BadRequest(new { Success = false, Message = "创建采购异常时发生错误" });
        }
    }

    /// <summary>
    /// 自动检测采购异常（收货时调用）
    /// </summary>
    [HttpPost("auto-detect/{receiptId}")]
    [RequirePermission("po.receive")]
    public async Task<IActionResult> AutoDetectExceptionsAsync(
        [FromRoute] string receiptId, CancellationToken ct)
    {
        try
        {
            var exceptions = await _exceptionService.AutoDetectExceptionsAsync(receiptId, ct);
            var exceptionsDto = exceptions.Select(e => e.ToDto()).ToList();

            return Ok(new
            {
                Success = true,
                Data = exceptionsDto,
                Count = exceptionsDto.Count,
                Message = $"检测到 {exceptionsDto.Count} 个异常"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error auto-detecting exceptions for receipt {ReceiptId}", receiptId);
            return BadRequest(new { Success = false, Message = "自动检测异常时发生错误" });
        }
    }

    /// <summary>
    /// 获取异常详情
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GetExceptionAsync(
        [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var exception = await _exceptionService.GetExceptionAsync(id, ct);
            if (exception == null)
            {
                return NotFound(new { Success = false, Message = "异常记录不存在" });
            }

            return Ok(new { Success = true, Data = exception.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "获取异常详情时发生错误" });
        }
    }

    /// <summary>
    /// 查询异常列表
    /// </summary>
    [HttpGet]
    [RequirePermission("po.read")]
    public async Task<IActionResult> QueryExceptionsAsync(
        [FromQuery] QueryPurchaseExceptionDto query, CancellationToken ct)
    {
        try
        {
            var (exceptions, total) = await _exceptionService.QueryExceptionsAsync(
                query.Type, query.Status, query.Level, query.SupplierId, query.ProductId,
                query.FromDate, query.ToDate, query.Page, query.Size, ct);

            var exceptionsDto = exceptions.Select(e => e.ToDto()).ToList();

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Items = exceptionsDto,
                    Total = total,
                    Page = query.Page,
                    Size = query.Size,
                    TotalPages = (int)Math.Ceiling((double)total / query.Size)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying exceptions");
            return BadRequest(new { Success = false, Message = "查询异常列表时发生错误" });
        }
    }

    // =================== 异常处理 ===================

    /// <summary>
    /// 处理采购异常
    /// </summary>
    [HttpPost("{id}/handle")]
    [RequirePermission("po.create")]
    public async Task<IActionResult> HandleExceptionAsync(
        [FromRoute] string id,
        [FromBody] HandlePurchaseExceptionDto request, CancellationToken ct)
    {
        try
        {
            var businessRequest = request.ToBusinessObject();
            var result = await _exceptionService.HandleExceptionAsync(id, businessRequest, CurrentUserId, ct);

            return Ok(new { Success = result.IsSuccess, Data = result.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "处理异常时发生错误" });
        }
    }

    /// <summary>
    /// 批量处理异常
    /// </summary>
    [HttpPost("batch/handle")]
    [RequirePermission("po.create")]
    public async Task<IActionResult> BatchHandleExceptionsAsync(
        [FromBody] BatchHandleExceptionsDto request, CancellationToken ct)
    {
        try
        {
            var businessRequest = request.Request.ToBusinessObject();
            var results = await _exceptionService.BatchHandleExceptionsAsync(
                request.ExceptionIds, businessRequest, CurrentUserId, ct);

            var resultsDto = results.Select(r => r.ToDto()).ToList();

            return Ok(new
            {
                Success = true,
                Data = resultsDto,
                SuccessCount = resultsDto.Count(r => r.IsSuccess),
                FailureCount = resultsDto.Count(r => !r.IsSuccess)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch handling exceptions");
            return BadRequest(new { Success = false, Message = "批量处理异常时发生错误" });
        }
    }

    /// <summary>
    /// 升级异常级别
    /// </summary>
    [HttpPost("{id}/escalate")]
    [RequirePermission("po.create")]
    public async Task<IActionResult> EscalateExceptionAsync(
        [FromRoute] string id,
        [FromBody] EscalateExceptionDto request, CancellationToken ct)
    {
        try
        {
            var result = await _exceptionService.EscalateExceptionAsync(
                id, request.NewLevel, request.Reason, CurrentUserId, ct);

            if (!result)
            {
                return NotFound(new { Success = false, Message = "异常记录不存在或升级失败" });
            }

            return Ok(new { Success = true, Message = "异常级别升级成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error escalating exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "升级异常级别时发生错误" });
        }
    }

    /// <summary>
    /// 关闭异常
    /// </summary>
    [HttpPost("{id}/close")]
    [RequirePermission("po.create")]
    public async Task<IActionResult> CloseExceptionAsync(
        [FromRoute] string id,
        [FromBody] CloseExceptionDto request, CancellationToken ct)
    {
        try
        {
            var result = await _exceptionService.CloseExceptionAsync(
                id, request.Reason, CurrentUserId, ct);

            if (!result)
            {
                return NotFound(new { Success = false, Message = "异常记录不存在或关闭失败" });
            }

            return Ok(new { Success = true, Message = "异常关闭成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "关闭异常时发生错误" });
        }
    }

    /// <summary>
    /// 重新开放异常
    /// </summary>
    [HttpPost("{id}/reopen")]
    [RequirePermission("po.create")]
    public async Task<IActionResult> ReopenExceptionAsync(
        [FromRoute] string id,
        [FromBody] ReopenExceptionDto request, CancellationToken ct)
    {
        try
        {
            var result = await _exceptionService.ReopenExceptionAsync(
                id, request.Reason, CurrentUserId, ct);

            if (!result)
            {
                return NotFound(new { Success = false, Message = "异常记录不存在或重新开放失败" });
            }

            return Ok(new { Success = true, Message = "异常重新开放成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reopening exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "重新开放异常时发生错误" });
        }
    }

    // =================== 查询和分析 ===================

    /// <summary>
    /// 获取异常统计信息
    /// </summary>
    [HttpGet("statistics")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GetExceptionStatisticsAsync(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? supplierId,
        CancellationToken ct)
    {
        try
        {
            var statistics = await _exceptionService.GetExceptionStatisticsAsync(
                fromDate, toDate, supplierId, ct);

            return Ok(new { Success = true, Data = statistics.ToDto() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exception statistics");
            return BadRequest(new { Success = false, Message = "获取异常统计时发生错误" });
        }
    }

    /// <summary>
    /// 获取处理建议
    /// </summary>
    [HttpGet("{id}/suggestions")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GetProcessingSuggestionsAsync(
        [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var suggestions = await _exceptionService.GenerateProcessingSuggestionsAsync(id, ct);

            return Ok(new { Success = true, Data = suggestions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processing suggestions for exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "获取处理建议时发生错误" });
        }
    }

    /// <summary>
    /// 获取供应商异常历史
    /// </summary>
    [HttpGet("supplier/{supplierId}/history")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GetSupplierExceptionHistoryAsync(
        [FromRoute] string supplierId,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        try
        {
            var exceptions = await _exceptionService.GetSupplierExceptionHistoryAsync(supplierId, limit, ct);
            var exceptionsDto = exceptions.Select(e => e.ToDto()).ToList();

            return Ok(new { Success = true, Data = exceptionsDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supplier exception history for {SupplierId}", supplierId);
            return BadRequest(new { Success = false, Message = "获取供应商异常历史时发生错误" });
        }
    }

    /// <summary>
    /// 获取产品异常历史
    /// </summary>
    [HttpGet("product/{productId}/history")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GetProductExceptionHistoryAsync(
        [FromRoute] string productId,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        try
        {
            var exceptions = await _exceptionService.GetProductExceptionHistoryAsync(productId, limit, ct);
            var exceptionsDto = exceptions.Select(e => e.ToDto()).ToList();

            return Ok(new { Success = true, Data = exceptionsDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product exception history for {ProductId}", productId);
            return BadRequest(new { Success = false, Message = "获取产品异常历史时发生错误" });
        }
    }

    /// <summary>
    /// 获取异常处理记录
    /// </summary>
    [HttpGet("{id}/handling-records")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GetHandlingRecordsAsync(
        [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var records = await _exceptionService.GetHandlingRecordsAsync(id, ct);
            var recordsDto = records.Select(r => r.ToDto()).ToList();

            return Ok(new { Success = true, Data = recordsDto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting handling records for exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "获取处理记录时发生错误" });
        }
    }

    // =================== 通知和报告 ===================

    /// <summary>
    /// 发送异常通知
    /// </summary>
    [HttpPost("{id}/notify")]
    [RequirePermission("po.create")]
    public async Task<IActionResult> SendExceptionNotificationAsync(
        [FromRoute] string id,
        [FromBody] SendNotificationDto request, CancellationToken ct)
    {
        try
        {
            var result = await _exceptionService.SendExceptionNotificationAsync(
                id, request.NotificationType, ct);

            if (!result)
            {
                return BadRequest(new { Success = false, Message = "发送通知失败" });
            }

            return Ok(new { Success = true, Message = "通知发送成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification for exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "发送通知时发生错误" });
        }
    }

    /// <summary>
    /// 导出异常报告
    /// </summary>
    [HttpPost("export")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> ExportExceptionReportAsync(
        [FromBody] ExportExceptionReportDto request, CancellationToken ct)
    {
        try
        {
            var reportData = await _exceptionService.ExportExceptionReportAsync(
                request.FromDate, request.ToDate, request.Type, request.SupplierId, ct);

            var fileName = $"采购异常报告_{request.FromDate:yyyyMMdd}_{request.ToDate:yyyyMMdd}.xlsx";

            return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting exception report");
            return BadRequest(new { Success = false, Message = "导出报告时发生错误" });
        }
    }

    // =================== 预防措施 ===================

    /// <summary>
    /// 生成预防措施建议
    /// </summary>
    [HttpGet("preventive-measures")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GeneratePreventiveMeasuresAsync(
        [FromQuery] PurchaseExceptionType type,
        [FromQuery] string? supplierId,
        CancellationToken ct)
    {
        try
        {
            var measures = await _exceptionService.GeneratePreventiveMeasuresAsync(type, supplierId, ct);

            return Ok(new { Success = true, Data = measures });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating preventive measures");
            return BadRequest(new { Success = false, Message = "生成预防措施时发生错误" });
        }
    }

    /// <summary>
    /// 计算影响评估
    /// </summary>
    [HttpGet("{id}/impact-assessment")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> CalculateImpactAssessmentAsync(
        [FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var assessment = await _exceptionService.CalculateImpactAssessmentAsync(id, ct);

            return Ok(new { Success = true, Data = assessment });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating impact assessment for exception {ExceptionId}", id);
            return BadRequest(new { Success = false, Message = "计算影响评估时发生错误" });
        }
    }
}