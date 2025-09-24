using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Inventory;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 库存预警管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission")]
public class InventoryAlertsController : ControllerBase
{
    private readonly IInventoryAlertService _alertService;

    public InventoryAlertsController(IInventoryAlertService alertService)
    {
        _alertService = alertService;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // =================== 预警记录管理 ===================

    /// <summary>
    /// 获取活跃预警列表
    /// </summary>
    [HttpGet("active")]
    [RequirePermission("inventory.read")]
    public async Task<IActionResult> GetActiveAlertsAsync(CancellationToken ct)
    {
        var alerts = await _alertService.GetActiveAlertsAsync(ct);
        return Ok(alerts);
    }

    /// <summary>
    /// 获取预警历史记录
    /// </summary>
    [HttpGet("history")]
    [RequirePermission("inventory.read")]
    public async Task<IActionResult> GetAlertHistoryAsync(
        [FromQuery] string? productId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        var history = await _alertService.GetAlertHistoryAsync(productId, fromDate, toDate, page, size, ct);
        return Ok(history);
    }

    /// <summary>
    /// 处理预警（标记为已处理、已忽略等）
    /// </summary>
    [HttpPost("{id}/handle")]
    [RequirePermission("inventory.adjust")]
    public async Task<IActionResult> HandleAlertAsync([FromRoute] string id,
        [FromBody] HandleAlertDto request, CancellationToken ct)
    {
        var result = await _alertService.HandleAlertAsync(id, request, CurrentUserId, ct);
        if (!result)
        {
            return NotFound("预警记录不存在");
        }

        return Ok(new { Success = true, Message = "预警处理成功" });
    }

    /// <summary>
    /// 手动执行库存水平检查
    /// </summary>
    [HttpPost("check")]
    [RequirePermission("inventory.adjust")]
    public async Task<IActionResult> CheckInventoryLevelsAsync(CancellationToken ct)
    {
        var result = await _alertService.CheckInventoryLevelsAsync(ct);
        return Ok(result);
    }

    // =================== 预警配置管理 ===================

    /// <summary>
    /// 获取预警配置列表
    /// </summary>
    [HttpGet("configs")]
    [RequirePermission("inventory.read")]
    public async Task<IActionResult> GetAlertConfigsAsync([FromQuery] string? productId, CancellationToken ct)
    {
        var configs = await _alertService.GetAlertConfigsAsync(productId, ct);
        return Ok(configs);
    }

    /// <summary>
    /// 创建预警配置
    /// </summary>
    [HttpPost("configs")]
    [RequirePermission("inventory.adjust")]
    public async Task<IActionResult> CreateAlertConfigAsync([FromBody] CreateInventoryAlertConfigDto request, CancellationToken ct)
    {
        try
        {
            var config = await _alertService.CreateAlertConfigAsync(request, CurrentUserId, ct);
            return Ok(config);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 更新预警配置
    /// </summary>
    [HttpPut("configs/{id}")]
    [RequirePermission("inventory.adjust")]
    public async Task<IActionResult> UpdateAlertConfigAsync([FromRoute] string id,
        [FromBody] UpdateInventoryAlertConfigDto request, CancellationToken ct)
    {
        try
        {
            var config = await _alertService.UpdateAlertConfigAsync(id, request, CurrentUserId, ct);
            return Ok(config);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("预警配置不存在");
        }
    }

    /// <summary>
    /// 删除预警配置
    /// </summary>
    [HttpDelete("configs/{id}")]
    [RequirePermission("inventory.adjust")]
    public async Task<IActionResult> DeleteAlertConfigAsync([FromRoute] string id, CancellationToken ct)
    {
        var result = await _alertService.DeleteAlertConfigAsync(id, ct);
        if (!result)
        {
            return NotFound("预警配置不存在");
        }

        return Ok(new { Success = true, Message = "预警配置删除成功" });
    }

    /// <summary>
    /// 批量创建预警配置
    /// </summary>
    [HttpPost("configs/batch")]
    [RequirePermission("inventory.adjust")]
    public async Task<IActionResult> BatchCreateAlertConfigsAsync([FromBody] List<CreateInventoryAlertConfigDto> requests, CancellationToken ct)
    {
        var configs = await _alertService.BatchCreateAlertConfigsAsync(requests, CurrentUserId, ct);
        return Ok(new
        {
            Success = true,
            CreatedCount = configs.Count,
            TotalRequested = requests.Count,
            Configs = configs
        });
    }

    // =================== 统计和仪表板 ===================

    /// <summary>
    /// 获取预警统计信息（仪表板用）
    /// </summary>
    [HttpGet("stats")]
    [RequirePermission("inventory.read")]
    public async Task<IActionResult> GetAlertStatsAsync(CancellationToken ct)
    {
        var activeAlerts = await _alertService.GetActiveAlertsAsync(ct);

        var stats = new
        {
            TotalActiveAlerts = activeAlerts.Count,
            CriticalAlerts = activeAlerts.Count(a => a.Level == AlertLevel.Critical),
            WarningAlerts = activeAlerts.Count(a => a.Level == AlertLevel.Warning),
            InfoAlerts = activeAlerts.Count(a => a.Level == AlertLevel.Info),
            LowStockAlerts = activeAlerts.Count(a => a.AlertType == InventoryAlertType.LowStock),
            OverStockAlerts = activeAlerts.Count(a => a.AlertType == InventoryAlertType.OverStock),
            ZeroStockAlerts = activeAlerts.Count(a => a.AlertType == InventoryAlertType.ZeroStock),
            AlertsByLevel = activeAlerts.GroupBy(a => a.Level).ToDictionary(g => g.Key.ToString(), g => g.Count()),
            AlertsByType = activeAlerts.GroupBy(a => a.AlertType).ToDictionary(g => g.Key.ToString(), g => g.Count())
        };

        return Ok(stats);
    }

    /// <summary>
    /// 获取库存预警趋势（最近30天）
    /// </summary>
    [HttpGet("trends")]
    [RequirePermission("inventory.read")]
    public async Task<IActionResult> GetAlertTrendsAsync(CancellationToken ct)
    {
        var fromDate = DateTime.UtcNow.Date.AddDays(-30);
        var alerts = await _alertService.GetAlertHistoryAsync(null, fromDate, null, 1, 1000, ct);

        var dailyStats = alerts
            .GroupBy(a => a.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalAlerts = g.Count(),
                CriticalAlerts = g.Count(a => a.Level == AlertLevel.Critical),
                LowStockAlerts = g.Count(a => a.AlertType == InventoryAlertType.LowStock),
                OverStockAlerts = g.Count(a => a.AlertType == InventoryAlertType.OverStock)
            })
            .OrderBy(s => s.Date)
            .ToList();

        return Ok(new
        {
            Period = new { From = fromDate, To = DateTime.UtcNow.Date },
            DailyStats = dailyStats,
            Summary = new
            {
                TotalAlerts = alerts.Count,
                AverageDailyAlerts = dailyStats.Any() ? Math.Round(dailyStats.Average(s => s.TotalAlerts), 1) : 0,
                PeakDay = dailyStats.OrderByDescending(s => s.TotalAlerts).FirstOrDefault()
            }
        });
    }
}
