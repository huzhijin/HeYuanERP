using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Customers;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HeYuanERP.Api.Controllers;

#if false

/// <summary>
/// 客户信用管理接口
/// </summary>
[ApiController]
[Route("api/customers/credit")]
[Authorize(Policy = "Permission")]
public class CustomerCreditController : ControllerBase
{
    private readonly ICustomerCreditService _creditService;
    private readonly ILogger<CustomerCreditController> _logger;

    public CustomerCreditController(
        ICustomerCreditService creditService,
        ILogger<CustomerCreditController> logger)
    {
        _creditService = creditService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // =================== 信用评估 ===================

    /// <summary>
    /// 计算客户信用评估（预览，不保存）
    /// </summary>
    [HttpPost("{customerId}/calculate")]
    [RequirePermission("customers.credit.calculate")]
    public async Task<IActionResult> CalculateCreditAssessmentAsync(
        [FromRoute] string customerId,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _creditService.CalculateCreditAssessmentAsync(customerId, ct);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        CustomerId = result.CustomerId,
                        RecommendedRating = result.RecommendedRating.ToString(),
                        CurrentRating = result.CurrentRating?.ToString(),
                        CreditScore = result.CreditScore,
                        RecommendedCreditLimit = result.RecommendedCreditLimit,
                        CurrentCreditLimit = result.CurrentCreditLimit,
                        RiskLevel = result.RiskLevel.ToString(),
                        ScoreDetails = new
                        {
                            PaymentHistoryScore = result.ScoreDetails.PaymentHistoryScore,
                            CreditUtilizationScore = result.ScoreDetails.CreditUtilizationScore,
                            CreditHistoryLengthScore = result.ScoreDetails.CreditHistoryLengthScore,
                            TransactionStabilityScore = result.ScoreDetails.TransactionStabilityScore,
                            FinancialConditionScore = result.ScoreDetails.FinancialConditionScore,
                            WeightedTotalScore = result.ScoreDetails.CalculateWeightedScore()
                        },
                        Recommendations = result.Recommendations,
                        RiskWarnings = result.RiskWarnings,
                        FinancialMetrics = result.FinancialMetrics != null ? new
                        {
                            TotalTransactionAmount = result.FinancialMetrics.TotalTransactionAmount,
                            TransactionCount = result.FinancialMetrics.TransactionCount,
                            OnTimePaymentRate = result.FinancialMetrics.OnTimePaymentRate,
                            AveragePaymentDelay = result.FinancialMetrics.AveragePaymentDelay,
                            MaxOverdueDays = result.FinancialMetrics.MaxOverdueDays,
                            CooperationDays = result.FinancialMetrics.CooperationDays,
                            CreditUtilizationRate = result.FinancialMetrics.CreditUtilizationRate,
                            AnnualGrowthRate = result.FinancialMetrics.AnnualGrowthRate
                        } : null
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
            _logger.LogError(ex, "Error calculating credit assessment for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "计算信用评估时发生错误" });
        }
    }

    /// <summary>
    /// 执行客户信用评估
    /// </summary>
    [HttpPost("{customerId}/assess")]
    [RequirePermission("customers.credit.assess")]
    public async Task<IActionResult> AssessCustomerCreditAsync(
        [FromRoute] string customerId,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _creditService.AssessCustomerCreditAsync(customerId, CurrentUserId, ct);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        CustomerId = result.CustomerId,
                        CreditRating = result.RecommendedRating.ToString(),
                        CreditScore = result.CreditScore,
                        CreditLimit = result.RecommendedCreditLimit,
                        RiskLevel = result.RiskLevel.ToString()
                    },
                    Message = "客户信用评估完成"
                });
            }
            else
            {
                return BadRequest(new { Success = false, Message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing customer credit for {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "执行信用评估时发生错误" });
        }
    }

    /// <summary>
    /// 批量信用评估
    /// </summary>
    [HttpPost("batch-assess")]
    [RequirePermission("customers.credit.batch")]
    public async Task<IActionResult> BatchAssessAsync(
        [FromBody] BatchCreditAssessRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _creditService.BatchAssessCustomerCreditsAsync(
                request.CustomerIds, CurrentUserId, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    SuccessCount = result.SuccessCount,
                    FailureCount = result.FailureCount,
                    RatingChangedCount = result.RatingChangedCount,
                    LimitChangedCount = result.LimitChangedCount,
                    RatingDistribution = result.RatingDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    RiskDistribution = result.RiskDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    ProcessingTime = result.EndTime - result.StartTime
                },
                Message = $"批量评估完成，成功处理 {result.SuccessCount} 个客户"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch assessing customer credits");
            return StatusCode(500, new { Success = false, Message = "批量评估时发生错误" });
        }
    }

    /// <summary>
    /// 重新评估所有客户信用
    /// </summary>
    [HttpPost("reassess-all")]
    [RequirePermission("customers.credit.admin")]
    public async Task<IActionResult> ReassessAllAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _creditService.ReassessAllCustomerCreditsAsync(CurrentUserId, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    SuccessCount = result.SuccessCount,
                    FailureCount = result.FailureCount,
                    RatingChangedCount = result.RatingChangedCount,
                    LimitChangedCount = result.LimitChangedCount,
                    RatingDistribution = result.RatingDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    RiskDistribution = result.RiskDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value)
                },
                Message = $"全量重评完成，处理了 {result.SuccessCount + result.FailureCount} 个客户"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reassessing all customer credits");
            return StatusCode(500, new { Success = false, Message = "重评所有客户信用时发生错误" });
        }
    }

    // =================== 信用额度管理 ===================

    /// <summary>
    /// 设置客户信用额度
    /// </summary>
    [HttpPost("{customerId}/set-limit")]
    [RequirePermission("customers.credit.set_limit")]
    public async Task<IActionResult> SetCreditLimitAsync(
        [FromRoute] string customerId,
        [FromBody] SetCreditLimitRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _creditService.SetCreditLimitAsync(
                customerId, request.CreditLimit, request.Reason, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "信用额度设置成功"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "客户信用记录不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting credit limit for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "设置信用额度时发生错误" });
        }
    }

    /// <summary>
    /// 冻结客户信用
    /// </summary>
    [HttpPost("{customerId}/freeze")]
    [RequirePermission("customers.credit.freeze")]
    public async Task<IActionResult> FreezeCreditAsync(
        [FromRoute] string customerId,
        [FromBody] FreezeCreditRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _creditService.FreezeCreditAsync(
                customerId, request.Reason, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "客户信用冻结成功"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "客户信用记录不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error freezing credit for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "冻结客户信用时发生错误" });
        }
    }

    /// <summary>
    /// 检查信用额度可用性
    /// </summary>
    [HttpPost("{customerId}/check-availability")]
    [RequirePermission("customers.credit.check")]
    public async Task<IActionResult> CheckCreditAvailabilityAsync(
        [FromRoute] string customerId,
        [FromBody] CheckCreditAvailabilityRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var (isAvailable, availableAmount, message) = await _creditService.CheckCreditAvailabilityAsync(
                customerId, request.RequestAmount, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    IsAvailable = isAvailable,
                    AvailableAmount = availableAmount,
                    RequestAmount = request.RequestAmount,
                    Message = message
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking credit availability for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "检查信用额度时发生错误" });
        }
    }

    // =================== 查询和管理 ===================

    /// <summary>
    /// 获取客户信用信息
    /// </summary>
    [HttpGet("{customerId}")]
    [RequirePermission("customers.credit.read")]
    public async Task<IActionResult> GetCustomerCreditAsync(
        [FromRoute] string customerId,
        CancellationToken ct = default)
    {
        try
        {
            var credit = await _creditService.GetCustomerCreditAsync(customerId, ct);

            if (credit != null)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        Id = credit.Id,
                        CustomerId = credit.CustomerId,
                        CreditRating = credit.CreditRating.ToString(),
                        CreditScore = credit.CreditScore,
                        CreditLimit = credit.CreditLimit,
                        UsedCredit = credit.UsedCredit,
                        AvailableCredit = credit.AvailableCredit,
                        CreditUtilizationRate = credit.CreditUtilizationRate,
                        PaymentTerms = credit.PaymentTerms,
                        Status = credit.Status.ToString(),
                        RiskLevel = credit.RiskLevel.ToString(),
                        IsFrozen = credit.IsFrozen,
                        FreezeReason = credit.FreezeReason,
                        LastAssessmentDate = credit.LastAssessmentDate,
                        NextAssessmentDate = credit.NextAssessmentDate,
                        AssessedBy = credit.AssessedBy,
                        ScoreDetails = new
                        {
                            PaymentHistoryScore = credit.ScoreDetails.PaymentHistoryScore,
                            CreditUtilizationScore = credit.ScoreDetails.CreditUtilizationScore,
                            CreditHistoryLengthScore = credit.ScoreDetails.CreditHistoryLengthScore,
                            TransactionStabilityScore = credit.ScoreDetails.TransactionStabilityScore,
                            FinancialConditionScore = credit.ScoreDetails.FinancialConditionScore,
                            WeightedTotalScore = credit.ScoreDetails.CalculateWeightedScore()
                        },
                        GuaranteeInfo = credit.GuaranteeInfo != null ? new
                        {
                            Type = credit.GuaranteeInfo.Type.ToString(),
                            Amount = credit.GuaranteeInfo.Amount,
                            GuarantorName = credit.GuaranteeInfo.GuarantorName,
                            EffectiveDate = credit.GuaranteeInfo.EffectiveDate,
                            ExpiryDate = credit.GuaranteeInfo.ExpiryDate,
                            Status = credit.GuaranteeInfo.Status.ToString()
                        } : null,
                        CreatedAt = credit.CreatedAt,
                        UpdatedAt = credit.UpdatedAt
                    }
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "客户信用记录不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer credit for {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "获取客户信用信息时发生错误" });
        }
    }

    /// <summary>
    /// 获取客户信用历史
    /// </summary>
    [HttpGet("{customerId}/history")]
    [RequirePermission("customers.credit.read")]
    public async Task<IActionResult> GetCreditHistoryAsync(
        [FromRoute] string customerId,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        try
        {
            var history = await _creditService.GetCreditHistoryAsync(customerId, limit, ct);

            var historyList = history.Select(h => new
            {
                Id = h.Id,
                ChangeType = h.ChangeType.ToString(),
                ChangedField = h.ChangedField,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                ChangeReason = h.ChangeReason,
                ChangedAt = h.ChangedAt,
                ChangedBy = h.ChangedBy,
                ApprovalStatus = h.ApprovalStatus.ToString(),
                ApprovedBy = h.ApprovedBy,
                ApprovedAt = h.ApprovedAt,
                ApprovalNotes = h.ApprovalNotes
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = historyList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credit history for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "获取信用历史时发生错误" });
        }
    }

    /// <summary>
    /// 计算客户财务指标
    /// </summary>
    [HttpGet("{customerId}/financial-metrics")]
    [RequirePermission("customers.credit.read")]
    public async Task<IActionResult> GetFinancialMetricsAsync(
        [FromRoute] string customerId,
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        try
        {
            var metrics = await _creditService.CalculateFinancialMetricsAsync(
                customerId, periodStart, periodEnd, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    CustomerId = metrics.CustomerId,
                    PeriodStart = metrics.PeriodStart,
                    PeriodEnd = metrics.PeriodEnd,
                    TotalTransactionAmount = metrics.TotalTransactionAmount,
                    AverageOrderAmount = metrics.AverageOrderAmount,
                    TransactionCount = metrics.TransactionCount,
                    OnTimePaymentCount = metrics.OnTimePaymentCount,
                    OverduePaymentCount = metrics.OverduePaymentCount,
                    OnTimePaymentRate = metrics.OnTimePaymentRate,
                    AveragePaymentDelay = metrics.AveragePaymentDelay,
                    MaxOverdueDays = metrics.MaxOverdueDays,
                    OutstandingAmount = metrics.OutstandingAmount,
                    CooperationDays = metrics.CooperationDays,
                    DisputeCount = metrics.DisputeCount,
                    ReturnRate = metrics.ReturnRate,
                    CreditUtilizationRate = metrics.CreditUtilizationRate,
                    AnnualGrowthRate = metrics.AnnualGrowthRate
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting financial metrics for customer {CustomerId}", customerId);
            return StatusCode(500, new { Success = false, Message = "获取财务指标时发生错误" });
        }
    }

    // =================== 系统管理 ===================

    /// <summary>
    /// 获取信用评估规则列表
    /// </summary>
    [HttpGet("assessment-rules")]
    [RequirePermission("customers.credit.rules.read")]
    public async Task<IActionResult> GetAssessmentRulesAsync(
        [FromQuery] bool enabledOnly = true,
        CancellationToken ct = default)
    {
        try
        {
            var rules = await _creditService.GetAssessmentRulesAsync(enabledOnly, ct);

            var ruleList = rules.Select(r => new
            {
                Id = r.Id,
                RuleName = r.RuleName,
                Description = r.Description,
                TargetRating = r.TargetRating.ToString(),
                MinCreditScore = r.MinCreditScore,
                MaxCreditScore = r.MaxCreditScore,
                Priority = r.Priority,
                IsEnabled = r.IsEnabled,
                ScoreWeights = new
                {
                    PaymentHistoryWeight = r.ScoreWeights.PaymentHistoryWeight,
                    CreditUtilizationWeight = r.ScoreWeights.CreditUtilizationWeight,
                    CreditHistoryLengthWeight = r.ScoreWeights.CreditHistoryLengthWeight,
                    TransactionStabilityWeight = r.ScoreWeights.TransactionStabilityWeight,
                    FinancialConditionWeight = r.ScoreWeights.FinancialConditionWeight
                },
                RiskThresholds = new
                {
                    LowRiskThreshold = r.RiskThresholds.LowRiskThreshold,
                    MediumRiskThreshold = r.RiskThresholds.MediumRiskThreshold,
                    HighRiskThreshold = r.RiskThresholds.HighRiskThreshold,
                    OverdueDaysWarningThreshold = r.RiskThresholds.OverdueDaysWarningThreshold,
                    CreditUtilizationWarningThreshold = r.RiskThresholds.CreditUtilizationWarningThreshold
                },
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
            _logger.LogError(ex, "Error getting assessment rules");
            return StatusCode(500, new { Success = false, Message = "获取评估规则时发生错误" });
        }
    }

    /// <summary>
    /// 获取信用等级说明
    /// </summary>
    [HttpGet("rating-definitions")]
    [RequirePermission("customers.credit.read")]
    public IActionResult GetCreditRatingDefinitions()
    {
        var definitions = new[]
        {
            new { Rating = "AAA", Name = "信用极好", Description = "付款记录优秀，财务状况稳定，风险极低", ScoreRange = "900-1000" },
            new { Rating = "AA", Name = "信用优良", Description = "付款记录良好，财务状况良好，风险低", ScoreRange = "800-899" },
            new { Rating = "A", Name = "信用良好", Description = "付款记录较好，财务状况稳定，风险较低", ScoreRange = "700-799" },
            new { Rating = "BBB", Name = "信用一般", Description = "付款记录一般，财务状况正常，风险中等", ScoreRange = "600-699" },
            new { Rating = "BB", Name = "信用较差", Description = "付款记录较差，存在一定风险", ScoreRange = "500-599" },
            new { Rating = "B", Name = "信用差", Description = "付款记录差，存在较高风险", ScoreRange = "400-499" },
            new { Rating = "C", Name = "信用极差", Description = "付款记录极差，存在高风险", ScoreRange = "300-399" },
            new { Rating = "D", Name = "违约", Description = "存在违约记录，风险极高", ScoreRange = "0-299" }
        };

        return Ok(new
        {
            Success = true,
            Data = definitions
        });
    }
}
#endif

// =================== DTO类 ===================

/// <summary>
/// 批量信用评估请求
/// </summary>
public class BatchCreditAssessRequest
{
    public List<string>? CustomerIds { get; set; }
}

/// <summary>
/// 设置信用额度请求
/// </summary>
public class SetCreditLimitRequest
{
    public decimal CreditLimit { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 冻结信用请求
/// </summary>
public class FreezeCreditRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 检查信用额度可用性请求
/// </summary>
public class CheckCreditAvailabilityRequest
{
    public decimal RequestAmount { get; set; }
}
