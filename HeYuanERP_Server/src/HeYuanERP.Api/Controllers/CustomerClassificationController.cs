using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Customers;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HeYuanERP.Api.Controllers;

#if false

/// <summary>
/// 客户分级管理接口
/// </summary>
[ApiController]
[Route("api/customers/classification")]
[Authorize(Policy = "Permission")]
public class CustomerClassificationController : ControllerBase
{
    private readonly ICustomerClassificationService _classificationService;
    private readonly ILogger<CustomerClassificationController> _logger;

    public CustomerClassificationController(
        ICustomerClassificationService classificationService,
        ILogger<CustomerClassificationController> logger)
    {
        _classificationService = classificationService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // =================== 自动分级 ===================

    /// <summary>
    /// 计算客户分级（预览，不保存）
    /// </summary>
    [HttpPost("{customerId}/calculate")]
    [RequirePermission("customers.classification.calculate")]
    public async Task<IActionResult> CalculateClassificationAsync(
        [FromRoute] string customerId,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _classificationService.CalculateClassificationAsync(customerId, ct);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        CustomerId = customerId,
                        RecommendedLevel = result.RecommendedLevel.ToString(),
                        CurrentLevel = result.CurrentLevel?.ToString(),
                        TotalScore = result.TotalScore,
                        ScoreBreakdown = result.ScoreBreakdown,
                        Recommendations = result.Recommendations,
                        Metrics = result.Metrics
                    }
                });
            }
            else
            {
                return BadRequest(new { Success = false, Message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating classification for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "计算客户分级时发生错误" });
        }
    }

    /// <summary>
    /// 执行客户自动分级
    /// </summary>
    [HttpPost("{customerId}/classify")]
    [RequirePermission("customers.classification.execute")]
    public async Task<IActionResult> ClassifyCustomerAsync(
        [FromRoute] string customerId,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _classificationService.ClassifyCustomerAsync(customerId, CurrentUserId, ct);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        CustomerId = customerId,
                        Level = result.RecommendedLevel.ToString(),
                        Score = result.TotalScore,
                        ScoreBreakdown = result.ScoreBreakdown
                    },
                    Message = "客户分级完成"
                });
            }
            else
            {
                return BadRequest(new { Success = false, Message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error classifying customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "执行客户分级时发生错误" });
        }
    }

    /// <summary>
    /// 批量自动分级
    /// </summary>
    [HttpPost("batch-classify")]
    [RequirePermission("customers.classification.batch")]
    public async Task<IActionResult> BatchClassifyAsync(
        [FromBody] BatchClassifyRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _classificationService.BatchClassifyCustomersAsync(
                request.CustomerIds, CurrentUserId, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    SuccessCount = result.SuccessCount,
                    FailureCount = result.FailureCount,
                    LevelChangedCount = result.LevelChangedCount,
                    LevelDistribution = result.LevelDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    ProcessingTime = result.EndTime - result.StartTime
                },
                Message = $"批量分级完成，成功处理 {result.SuccessCount} 个客户"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch classifying customers");
            return StatusCode(500, new { Success = false, Message = "批量分级时发生错误" });
        }
    }

    /// <summary>
    /// 重新计算所有客户分级
    /// </summary>
    [HttpPost("recalculate-all")]
    [RequirePermission("customers.classification.admin")]
    public async Task<IActionResult> RecalculateAllAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _classificationService.RecalculateAllClassificationsAsync(CurrentUserId, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    SuccessCount = result.SuccessCount,
                    FailureCount = result.FailureCount,
                    LevelChangedCount = result.LevelChangedCount,
                    LevelDistribution = result.LevelDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value)
                },
                Message = $"全量重算完成，处理了 {result.SuccessCount + result.FailureCount} 个客户"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalculating all classifications");
            return StatusCode(500, new { Success = false, Message = "重算所有客户分级时发生错误" });
        }
    }

    // =================== 手动分级 ===================

    /// <summary>
    /// 手动设置客户等级
    /// </summary>
    [HttpPost("{customerId}/set-level")]
    [RequirePermission("customers.classification.manual")]
    public async Task<IActionResult> SetCustomerLevelAsync(
        [FromRoute] string customerId,
        [FromBody] SetCustomerLevelRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _classificationService.SetCustomerLevelAsync(
                customerId, request.Level, request.Reason, request.ExpiryDate, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "客户等级设置成功"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "客户不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting customer level for {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "设置客户等级时发生错误" });
        }
    }

    /// <summary>
    /// 临时升级客户等级
    /// </summary>
    [HttpPost("{customerId}/temporary-upgrade")]
    [RequirePermission("customers.classification.manual")]
    public async Task<IActionResult> TemporaryUpgradeAsync(
        [FromRoute] string customerId,
        [FromBody] TemporaryUpgradeRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _classificationService.TemporaryUpgradeCustomerAsync(
                customerId, request.Level, request.ExpiryDate, request.Reason, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = $"客户临时升级成功，有效期至 {request.ExpiryDate:yyyy-MM-dd}"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "客户不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error temporary upgrading customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "临时升级客户时发生错误" });
        }
    }

    // =================== 查询和管理 ===================

    /// <summary>
    /// 获取客户当前分级信息
    /// </summary>
    [HttpGet("{customerId}")]
    [RequirePermission("customers.classification.read")]
    public async Task<IActionResult> GetCustomerClassificationAsync(
        [FromRoute] string customerId,
        CancellationToken ct = default)
    {
        try
        {
            var classification = await _classificationService.GetCustomerClassificationAsync(customerId, ct);

            if (classification != null)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        Id = classification.Id,
                        CustomerId = classification.CustomerId,
                        CurrentLevel = classification.CurrentLevel.ToString(),
                        Method = classification.Method.ToString(),
                        EffectiveDate = classification.EffectiveDate,
                        ExpiryDate = classification.ExpiryDate,
                        Reason = classification.Reason,
                        AutoScore = classification.AutoScore,
                        ScoreDetails = classification.ScoreDetails,
                        IsActive = classification.IsActive,
                        CreatedAt = classification.CreatedAt,
                        CreatedBy = classification.CreatedBy
                    }
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "客户分级信息不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer classification for {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "获取客户分级信息时发生错误" });
        }
    }

    /// <summary>
    /// 获取客户分级历史
    /// </summary>
    [HttpGet("{customerId}/history")]
    [RequirePermission("customers.classification.read")]
    public async Task<IActionResult> GetClassificationHistoryAsync(
        [FromRoute] string customerId,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        try
        {
            var history = await _classificationService.GetClassificationHistoryAsync(customerId, limit, ct);

            var historyList = history.Select(h => new
            {
                Id = h.Id,
                ChangeType = h.ChangeType.ToString(),
                FromLevel = h.FromLevel?.ToString(),
                ToLevel = h.ToLevel.ToString(),
                ChangeReason = h.ChangeReason,
                Score = h.Score,
                ScoreChange = h.ScoreChange,
                Method = h.Method.ToString(),
                ChangedAt = h.ChangedAt,
                ChangedBy = h.ChangedBy
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = historyList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classification history for {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "获取分级历史时发生错误" });
        }
    }

    /// <summary>
    /// 查询指定等级的客户列表
    /// </summary>
    [HttpGet("by-level/{level}")]
    [RequirePermission("customers.classification.read")]
    public async Task<IActionResult> GetCustomersByLevelAsync(
        [FromRoute] CustomerLevel level,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        try
        {
            var (classifications, total) = await _classificationService.GetCustomersByLevelAsync(level, page, size, ct);

            var customerList = classifications.Select(c => new
            {
                Id = c.Id,
                CustomerId = c.CustomerId,
                CurrentLevel = c.CurrentLevel.ToString(),
                AutoScore = c.AutoScore,
                EffectiveDate = c.EffectiveDate,
                ExpiryDate = c.ExpiryDate,
                Method = c.Method.ToString(),
                IsActive = c.IsActive
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Items = customerList,
                    Total = total,
                    Page = page,
                    Size = size,
                    TotalPages = (int)Math.Ceiling((double)total / size)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers by level {Level}", level);
            return StatusCode(500, new { Success = false, Message = "查询客户列表时发生错误" });
        }
    }

    // =================== 统计分析 ===================

    /// <summary>
    /// 获取分级统计信息
    /// </summary>
    [HttpGet("statistics")]
    [RequirePermission("customers.classification.read")]
    public async Task<IActionResult> GetStatisticsAsync(CancellationToken ct = default)
    {
        try
        {
            var statistics = await _classificationService.GetClassificationStatisticsAsync(ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    TotalCustomers = statistics.TotalCustomers,
                    LevelDistribution = statistics.LevelDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    SalesDistribution = statistics.SalesDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    ProfitDistribution = statistics.ProfitDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    RecentChanges = statistics.RecentChanges.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    AverageCustomerScore = statistics.AverageCustomerScore,
                    StatisticsTime = statistics.StatisticsTime
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classification statistics");
            return StatusCode(500, new { Success = false, Message = "获取统计信息时发生错误" });
        }
    }

    /// <summary>
    /// 获取等级趋势分析
    /// </summary>
    [HttpGet("trend-analysis")]
    [RequirePermission("customers.classification.read")]
    public async Task<IActionResult> GetTrendAnalysisAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string groupBy = "month",
        CancellationToken ct = default)
    {
        try
        {
            var trends = await _classificationService.GetLevelTrendAnalysisAsync(fromDate, toDate, groupBy, ct);

            var trendData = trends.ToDictionary(
                kvp => kvp.Key.ToString("yyyy-MM-dd"),
                kvp => kvp.Value.ToDictionary(
                    levelKvp => levelKvp.Key.ToString(),
                    levelKvp => levelKvp.Value));

            return Ok(new
            {
                Success = true,
                Data = trendData
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting level trend analysis");
            return StatusCode(500, new { Success = false, Message = "获取趋势分析时发生错误" });
        }
    }

    // =================== 规则管理 ===================

    /// <summary>
    /// 获取分级规则列表
    /// </summary>
    [HttpGet("rules")]
    [RequirePermission("customers.classification.rules.read")]
    public async Task<IActionResult> GetRulesAsync(
        [FromQuery] bool enabledOnly = true,
        CancellationToken ct = default)
    {
        try
        {
            var rules = await _classificationService.GetClassificationRulesAsync(enabledOnly, ct);

            var ruleList = rules.Select(r => new
            {
                Id = r.Id,
                RuleName = r.RuleName,
                Description = r.Description,
                TargetLevel = r.TargetLevel.ToString(),
                MinTotalScore = r.MinTotalScore,
                Priority = r.Priority,
                IsEnabled = r.IsEnabled,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = ruleList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classification rules");
            return StatusCode(500, new { Success = false, Message = "获取分级规则时发生错误" });
        }
    }

    /// <summary>
    /// 创建分级规则
    /// </summary>
    [HttpPost("rules")]
    [RequirePermission("customers.classification.rules.create")]
    public async Task<IActionResult> CreateRuleAsync(
        [FromBody] CreateClassificationRuleRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var rule = await _classificationService.CreateClassificationRuleAsync(
                request.RuleName, request.TargetLevel, request.Criteria,
                request.MinTotalScore, request.Priority, CurrentUserId, ct);

            return Ok(new
            {
                Success = true,
                Data = new { Id = rule.Id },
                Message = "分级规则创建成功"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating classification rule");
            return StatusCode(500, new { Success = false, Message = "创建分级规则时发生错误" });
        }
    }

    // =================== 业务洞察 ===================

    /// <summary>
    /// 检查即将过期的临时升级
    /// </summary>
    [HttpGet("expiring-upgrades")]
    [RequirePermission("customers.classification.read")]
    public async Task<IActionResult> GetExpiringUpgradesAsync(
        [FromQuery] int daysBeforeExpiry = 7,
        CancellationToken ct = default)
    {
        try
        {
            var expiringUpgrades = await _classificationService.CheckExpiringTemporaryUpgradesAsync(daysBeforeExpiry, ct);

            var upgradeList = expiringUpgrades.Select(c => new
            {
                Id = c.Id,
                CustomerId = c.CustomerId,
                CurrentLevel = c.CurrentLevel.ToString(),
                ExpiryDate = c.ExpiryDate,
                Reason = c.Reason,
                DaysUntilExpiry = c.ExpiryDate.HasValue
                    ? (int)(c.ExpiryDate.Value - DateTime.UtcNow).TotalDays
                    : 0
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = upgradeList,
                Message = $"发现 {upgradeList.Count} 个即将过期的临时升级"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expiring upgrades");
            return StatusCode(500, new { Success = false, Message = "获取即将过期升级时发生错误" });
        }
    }

    /// <summary>
    /// 获取客户升级建议
    /// </summary>
    [HttpGet("{customerId}/upgrade-suggestions")]
    [RequirePermission("customers.classification.read")]
    public async Task<IActionResult> GetUpgradeSuggestionsAsync(
        [FromRoute] string customerId,
        CancellationToken ct = default)
    {
        try
        {
            var suggestions = await _classificationService.GetCustomerUpgradeSuggestionsAsync(customerId, ct);

            return Ok(new
            {
                Success = true,
                Data = suggestions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upgrade suggestions for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "获取升级建议时发生错误" });
        }
    }
}
#endif

// =================== DTO类 ===================

/// <summary>
/// 批量分级请求
/// </summary>
public class BatchClassifyRequest
{
    public List<string>? CustomerIds { get; set; }
}

/// <summary>
/// 设置客户等级请求
/// </summary>
public class SetCustomerLevelRequest
{
    public CustomerLevel Level { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

/// <summary>
/// 临时升级请求
/// </summary>
public class TemporaryUpgradeRequest
{
    public CustomerLevel Level { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 创建分级规则请求
/// </summary>
public class CreateClassificationRuleRequest
{
    public string RuleName { get; set; } = string.Empty;
    public CustomerLevel TargetLevel { get; set; }
    public CustomerClassificationCriteria Criteria { get; set; } = new();
    public decimal MinTotalScore { get; set; }
    public int Priority { get; set; } = 100;
}
