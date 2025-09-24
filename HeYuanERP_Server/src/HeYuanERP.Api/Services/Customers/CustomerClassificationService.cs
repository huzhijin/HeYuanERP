using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Customers;

/// <summary>
/// 客户分级管理服务实现
/// </summary>
#if false
public class CustomerClassificationService : ICustomerClassificationService
{
    private readonly AppDbContext _db;
    private readonly ILogger<CustomerClassificationService> _logger;

    public CustomerClassificationService(AppDbContext db, ILogger<CustomerClassificationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<CustomerClassificationResult> CalculateClassificationAsync(string customerId, CancellationToken ct = default)
    {
        try
        {
            // 计算客户指标
            var metrics = await CalculateCustomerMetricsAsync(customerId, DateTime.UtcNow.AddYears(-1), DateTime.UtcNow, ct);

            // 获取分级规则
            var rules = await GetClassificationRulesAsync(true, ct);
            if (!rules.Any())
            {
                return CustomerClassificationResult.Failure("未配置分级规则");
            }

            // 计算评分
            var scoreBreakdown = CalculateScores(metrics, rules.First().Criteria);
            var totalScore = scoreBreakdown.Values.Sum();

            // 确定推荐等级
            var recommendedLevel = DetermineLevel(totalScore, rules);

            var result = CustomerClassificationResult.Success(recommendedLevel, totalScore, scoreBreakdown);
            result.Metrics = metrics;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating classification for customer {CustomerId}", customerId);
            return CustomerClassificationResult.Failure($"计算分级时发生错误: {ex.Message}");
        }
    }

    public async Task<CustomerClassificationResult> ClassifyCustomerAsync(string customerId, string? userId = null, CancellationToken ct = default)
    {
        try
        {
            // 计算分级
            var result = await CalculateClassificationAsync(customerId, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            // 获取当前分级
            var currentClassification = await GetCustomerClassificationAsync(customerId, ct);
            result.CurrentLevel = currentClassification?.CurrentLevel;

            // 检查是否需要更新分级
            if (currentClassification == null || currentClassification.CurrentLevel != result.RecommendedLevel)
            {
                // 创建新分级记录
                var newClassification = new CustomerClassification
                {
                    CustomerId = customerId,
                    CurrentLevel = result.RecommendedLevel,
                    Method = ClassificationMethod.Automatic,
                    AutoScore = result.TotalScore,
                    ScoreDetails = result.ScoreBreakdown,
                    Reason = "系统自动分级",
                    CreatedBy = userId
                };

                if (currentClassification != null)
                {
                    currentClassification.IsActive = false;
                    currentClassification.UpdatedAt = DateTime.UtcNow;
                    currentClassification.UpdatedBy = userId;
                }

                _db.CustomerClassifications.Add(newClassification);

                // 记录分级历史
                var history = new CustomerClassificationHistory
                {
                    CustomerId = customerId,
                    ClassificationId = newClassification.Id,
                    ChangeType = currentClassification == null ? ClassificationChangeType.Initial :
                                (result.RecommendedLevel > currentClassification.CurrentLevel ? ClassificationChangeType.Upgrade : ClassificationChangeType.Downgrade),
                    FromLevel = currentClassification?.CurrentLevel,
                    ToLevel = result.RecommendedLevel,
                    ChangeReason = "自动分级计算",
                    Score = result.TotalScore,
                    ScoreChange = result.TotalScore - (currentClassification?.AutoScore ?? 0),
                    Method = ClassificationMethod.Automatic,
                    ChangedBy = userId
                };

                _db.CustomerClassificationHistories.Add(history);

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Customer {CustomerId} classified as {Level} with score {Score}",
                    customerId, result.RecommendedLevel, result.TotalScore);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error classifying customer {CustomerId}", customerId);
            return CustomerClassificationResult.Failure($"分级客户时发生错误: {ex.Message}");
        }
    }

    public async Task<BatchClassificationResult> BatchClassifyCustomersAsync(List<string>? customerIds = null, string? userId = null, CancellationToken ct = default)
    {
        var result = new BatchClassificationResult
        {
            StartTime = DateTime.UtcNow
        };

        try
        {
            // 如果未指定客户，则处理所有有交易记录的客户
            if (customerIds == null || !customerIds.Any())
            {
                customerIds = await _db.SalesOrders.AsNoTracking()
                    .Where(o => o.CreatedAt >= DateTime.UtcNow.AddYears(-2))
                    .Select(o => o.CustomerId)
                    .Distinct()
                    .ToListAsync(ct);
            }

            foreach (var customerId in customerIds)
            {
                var classificationResult = await ClassifyCustomerAsync(customerId, userId, ct);
                result.Results.Add(classificationResult);

                if (classificationResult.IsSuccess)
                {
                    result.SuccessCount++;
                    if (classificationResult.CurrentLevel != classificationResult.RecommendedLevel)
                    {
                        result.LevelChangedCount++;
                    }

                    // 统计等级分布
                    if (result.LevelDistribution.ContainsKey(classificationResult.RecommendedLevel))
                    {
                        result.LevelDistribution[classificationResult.RecommendedLevel]++;
                    }
                    else
                    {
                        result.LevelDistribution[classificationResult.RecommendedLevel] = 1;
                    }
                }
                else
                {
                    result.FailureCount++;
                }
            }

            result.EndTime = DateTime.UtcNow;

            _logger.LogInformation("Batch classification completed: {Success} success, {Failure} failure, {Changed} level changed",
                result.SuccessCount, result.FailureCount, result.LevelChangedCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch classification");
            result.EndTime = DateTime.UtcNow;
            return result;
        }
    }

    public async Task<bool> SetCustomerLevelAsync(string customerId, CustomerLevel level, string reason, DateTime? expiryDate = null, string? userId = null, CancellationToken ct = default)
    {
        try
        {
            // 获取当前分级
            var currentClassification = await GetCustomerClassificationAsync(customerId, ct);

            if (currentClassification != null)
            {
                currentClassification.IsActive = false;
                currentClassification.UpdatedAt = DateTime.UtcNow;
                currentClassification.UpdatedBy = userId;
            }

            // 创建新分级记录
            var newClassification = new CustomerClassification
            {
                CustomerId = customerId,
                CurrentLevel = level,
                Method = ClassificationMethod.Manual,
                ExpiryDate = expiryDate,
                Reason = reason,
                CreatedBy = userId
            };

            _db.CustomerClassifications.Add(newClassification);

            // 记录分级历史
            var history = new CustomerClassificationHistory
            {
                CustomerId = customerId,
                ClassificationId = newClassification.Id,
                ChangeType = ClassificationChangeType.ManualAdjustment,
                FromLevel = currentClassification?.CurrentLevel,
                ToLevel = level,
                ChangeReason = reason,
                Method = ClassificationMethod.Manual,
                ChangedBy = userId
            };

            _db.CustomerClassificationHistories.Add(history);

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Customer {CustomerId} manually set to level {Level} by {UserId}: {Reason}",
                customerId, level, userId, reason);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting customer level for {CustomerId}", customerId);
            return false;
        }
    }

    public async Task<CustomerClassification?> GetCustomerClassificationAsync(string customerId, CancellationToken ct = default)
    {
        return await _db.CustomerClassifications.AsNoTracking()
            .Include(c => c.Customer)
            .Where(c => c.CustomerId == customerId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<CustomerClassificationHistory>> GetClassificationHistoryAsync(string customerId, int limit = 50, CancellationToken ct = default)
    {
        return await _db.CustomerClassificationHistories.AsNoTracking()
            .Where(h => h.CustomerId == customerId)
            .OrderByDescending(h => h.ChangedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<CustomerMetrics> CalculateCustomerMetricsAsync(string customerId, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken ct = default)
    {
        periodStart ??= DateTime.UtcNow.AddYears(-1);
        periodEnd ??= DateTime.UtcNow;

        var metrics = new CustomerMetrics
        {
            CustomerId = customerId,
            PeriodStart = periodStart.Value,
            PeriodEnd = periodEnd.Value
        };

        try
        {
            // 获取客户信息
            var customer = await _db.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customerId, ct);

            if (customer == null)
            {
                return metrics;
            }

            // 计算合作年限
            metrics.CustomerLoyaltyDays = (int)(DateTime.UtcNow - customer.CreatedAt).TotalDays;

            // 获取订单数据
            var orders = await _db.SalesOrders.AsNoTracking()
                .Where(o => o.CustomerId == customerId &&
                           o.CreatedAt >= periodStart &&
                           o.CreatedAt <= periodEnd)
                .ToListAsync(ct);

            if (orders.Any())
            {
                // 年交易金额
                metrics.AnnualSalesAmount = orders.Sum(o => o.TotalAmount);

                // 交易频次
                metrics.TransactionFrequency = orders.Count;

                // 平均订单金额
                metrics.AverageOrderValue = metrics.AnnualSalesAmount / metrics.TransactionFrequency;

                // 最后交易日期
                metrics.LastTransactionDate = orders.Max(o => o.CreatedAt);

                // 计算增长趋势（对比上一年）
                var lastYearOrders = await _db.SalesOrders.AsNoTracking()
                    .Where(o => o.CustomerId == customerId &&
                               o.CreatedAt >= periodStart.Value.AddYears(-1) &&
                               o.CreatedAt <= periodEnd.Value.AddYears(-1))
                    .ToListAsync(ct);

                if (lastYearOrders.Any())
                {
                    var lastYearAmount = lastYearOrders.Sum(o => o.TotalAmount);
                    metrics.GrowthTrendRate = lastYearAmount > 0
                        ? (metrics.AnnualSalesAmount - lastYearAmount) / lastYearAmount * 100
                        : 100;
                }
            }

            // 计算回款及时性（简化版本）
            var invoices = await _db.Invoices.AsNoTracking()
                .Where(i => i.CustomerId == customerId &&
                           i.InvoiceDate >= periodStart &&
                           i.InvoiceDate <= periodEnd)
                .ToListAsync(ct);

            if (invoices.Any())
            {
                var paidInvoices = invoices.Where(i => i.Status == "paid").Count();
                metrics.PaymentTimelinessRate = (decimal)paidInvoices / invoices.Count * 100;
            }

            // 获取投诉次数（这里需要根据实际的投诉表来计算）
            // 暂时设为0，后续可以扩展
            metrics.ComplaintCount = 0;

            // 计算利润贡献（简化版本）
            metrics.ProfitContribution = metrics.AnnualSalesAmount * 0.2m; // 假设20%利润率

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating metrics for customer {CustomerId}", customerId);
            return metrics;
        }
    }

    public async Task<ClassificationStatistics> GetClassificationStatisticsAsync(CancellationToken ct = default)
    {
        var statistics = new ClassificationStatistics();

        try
        {
            var classifications = await _db.CustomerClassifications.AsNoTracking()
                .Where(c => c.IsActive)
                .ToListAsync(ct);

            statistics.TotalCustomers = classifications.Count;

            // 等级分布
            statistics.LevelDistribution = classifications
                .GroupBy(c => c.CurrentLevel)
                .ToDictionary(g => g.Key, g => g.Count());

            // 计算销售额分布（简化版本）
            foreach (var level in Enum.GetValues<CustomerLevel>())
            {
                var customerIds = classifications
                    .Where(c => c.CurrentLevel == level)
                    .Select(c => c.CustomerId)
                    .ToList();

                if (customerIds.Any())
                {
                    var salesAmount = await _db.SalesOrders.AsNoTracking()
                        .Where(o => customerIds.Contains(o.CustomerId) &&
                                   o.CreatedAt >= DateTime.UtcNow.AddYears(-1))
                        .SumAsync(o => o.TotalAmount, ct);

                    statistics.SalesDistribution[level] = salesAmount;
                    statistics.ProfitDistribution[level] = salesAmount * 0.2m; // 假设20%利润率
                }
            }

            // 最近变更统计
            var recentChanges = await _db.CustomerClassificationHistories.AsNoTracking()
                .Where(h => h.ChangedAt >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(h => h.ChangeType)
                .ToDictionaryAsync(g => g.Key, g => g.Count(), ct);

            statistics.RecentChanges = recentChanges;

            // 平均评分
            var avgScore = classifications
                .Where(c => c.AutoScore.HasValue)
                .Average(c => c.AutoScore!.Value);
            statistics.AverageCustomerScore = avgScore;

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classification statistics");
            return statistics;
        }
    }

    #region Private Helper Methods

    private Dictionary<string, decimal> CalculateScores(CustomerMetrics metrics, CustomerClassificationCriteria criteria)
    {
        var scores = new Dictionary<string, decimal>();

        // 年交易金额评分
        if (criteria.AnnualSalesAmount.IsEnabled)
        {
            var score = CalculateScore(metrics.AnnualSalesAmount, criteria.AnnualSalesAmount);
            scores["AnnualSalesAmount"] = score * criteria.AnnualSalesAmount.Weight;
        }

        // 交易频次评分
        if (criteria.TransactionFrequency.IsEnabled)
        {
            var score = CalculateScore(metrics.TransactionFrequency, criteria.TransactionFrequency);
            scores["TransactionFrequency"] = score * criteria.TransactionFrequency.Weight;
        }

        // 客户忠诚度评分
        if (criteria.CustomerLoyalty.IsEnabled)
        {
            var score = CalculateScore(metrics.CustomerLoyaltyDays, criteria.CustomerLoyalty);
            scores["CustomerLoyalty"] = score * criteria.CustomerLoyalty.Weight;
        }

        // 回款及时性评分
        if (criteria.PaymentTimeliness.IsEnabled)
        {
            var score = CalculateScore(metrics.PaymentTimelinessRate, criteria.PaymentTimeliness);
            scores["PaymentTimeliness"] = score * criteria.PaymentTimeliness.Weight;
        }

        // 平均订单金额评分
        if (criteria.AverageOrderValue.IsEnabled)
        {
            var score = CalculateScore(metrics.AverageOrderValue, criteria.AverageOrderValue);
            scores["AverageOrderValue"] = score * criteria.AverageOrderValue.Weight;
        }

        // 增长趋势评分
        if (criteria.GrowthTrend.IsEnabled)
        {
            var score = CalculateScore(metrics.GrowthTrendRate, criteria.GrowthTrend);
            scores["GrowthTrend"] = score * criteria.GrowthTrend.Weight;
        }

        // 投诉次数评分（负分项）
        if (criteria.ComplaintCount.IsEnabled)
        {
            var score = CalculateScore(metrics.ComplaintCount, criteria.ComplaintCount);
            scores["ComplaintCount"] = -score * criteria.ComplaintCount.Weight; // 负分
        }

        // 利润贡献评分
        if (criteria.ProfitContribution.IsEnabled)
        {
            var score = CalculateScore(metrics.ProfitContribution, criteria.ProfitContribution);
            scores["ProfitContribution"] = score * criteria.ProfitContribution.Weight;
        }

        return scores;
    }

    private decimal CalculateScore(decimal value, ScoreCriteria criteria)
    {
        if (!criteria.Thresholds.Any())
        {
            // 如果没有阈值，使用简单的线性评分
            return Math.Min(value / 100 * criteria.MaxScore, criteria.MaxScore);
        }

        // 使用阈值进行分段评分
        var threshold = criteria.Thresholds
            .Where(t => value >= t.MinValue && value <= t.MaxValue)
            .FirstOrDefault();

        return threshold?.Score ?? 0;
    }

    private CustomerLevel DetermineLevel(decimal totalScore, List<CustomerClassificationRule> rules)
    {
        var applicableRules = rules
            .Where(r => totalScore >= r.MinTotalScore)
            .OrderByDescending(r => r.MinTotalScore)
            .ThenBy(r => r.Priority);

        return applicableRules.FirstOrDefault()?.TargetLevel ?? CustomerLevel.Regular;
    }

    #endregion

    // 简化实现的其他方法
    public Task<BatchClassificationResult> RecalculateAllClassificationsAsync(string? userId = null, CancellationToken ct = default) => BatchClassifyCustomersAsync(null, userId, ct);
    public Task<bool> TemporaryUpgradeCustomerAsync(string customerId, CustomerLevel level, DateTime expiryDate, string reason, string? userId = null, CancellationToken ct = default) => SetCustomerLevelAsync(customerId, level, reason, expiryDate, userId, ct);
    public Task<BatchClassificationResult> BatchSetCustomerLevelsAsync(Dictionary<string, CustomerLevel> customerLevels, string reason, string? userId = null, CancellationToken ct = default) => Task.FromResult(new BatchClassificationResult());
    public Task<(List<CustomerClassification> classifications, int total)> GetCustomersByLevelAsync(CustomerLevel level, int page = 1, int size = 20, CancellationToken ct = default) => Task.FromResult((new List<CustomerClassification>(), 0));
    public Task<(List<CustomerClassification> classifications, int total)> SearchClassificationsAsync(string? keyword = null, CustomerLevel? level = null, ClassificationMethod? method = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int size = 20, CancellationToken ct = default) => Task.FromResult((new List<CustomerClassification>(), 0));
    public Task<CustomerClassificationRule> CreateClassificationRuleAsync(string ruleName, CustomerLevel targetLevel, CustomerClassificationCriteria criteria, decimal minTotalScore, int priority = 100, string? userId = null, CancellationToken ct = default) => Task.FromResult(new CustomerClassificationRule());
    public Task<bool> UpdateClassificationRuleAsync(string ruleId, string? ruleName = null, CustomerClassificationCriteria? criteria = null, decimal? minTotalScore = null, int? priority = null, bool? isEnabled = null, string? userId = null, CancellationToken ct = default) => Task.FromResult(false);
    public async Task<List<CustomerClassificationRule>> GetClassificationRulesAsync(bool enabledOnly = true, CancellationToken ct = default) => await _db.CustomerClassificationRules.AsNoTracking().Where(r => !enabledOnly || r.IsEnabled).OrderBy(r => r.Priority).ToListAsync(ct);
    public Task<bool> DeleteClassificationRuleAsync(string ruleId, string? userId = null, CancellationToken ct = default) => Task.FromResult(false);
    public Task<Dictionary<string, CustomerMetrics>> BatchCalculateCustomerMetricsAsync(List<string> customerIds, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken ct = default) => Task.FromResult(new Dictionary<string, CustomerMetrics>());
    public Task<Dictionary<DateTime, Dictionary<CustomerLevel, int>>> GetLevelTrendAnalysisAsync(DateTime fromDate, DateTime toDate, string groupBy = "month", CancellationToken ct = default) => Task.FromResult(new Dictionary<DateTime, Dictionary<CustomerLevel, int>>());
    public Task<object> AnalyzeCustomerValueDistributionAsync(CancellationToken ct = default) => Task.FromResult<object>(new { });
    public Task<List<string>> CheckCustomersForReclassificationAsync(int daysSinceLastClassification = 90, CancellationToken ct = default) => Task.FromResult(new List<string>());
    public Task<List<CustomerClassification>> CheckExpiringTemporaryUpgradesAsync(int daysBeforeExpiry = 7, CancellationToken ct = default) => Task.FromResult(new List<CustomerClassification>());
    public Task<byte[]> GenerateClassificationReportAsync(DateTime fromDate, DateTime toDate, CustomerLevel? level = null, string format = "excel", CancellationToken ct = default) => Task.FromResult(Array.Empty<byte>());
    public Task<Dictionary<string, CustomerLevel>> PredictCustomerLevelChangesAsync(List<string> customerIds, int forecastDays = 90, CancellationToken ct = default) => Task.FromResult(new Dictionary<string, CustomerLevel>());
    public Task<Dictionary<string, decimal>> AnalyzeCustomerChurnRiskAsync(CustomerLevel? level = null, CancellationToken ct = default) => Task.FromResult(new Dictionary<string, decimal>());
    public Task<List<string>> GetCustomerUpgradeSuggestionsAsync(string customerId, CancellationToken ct = default) => Task.FromResult(new List<string>());
}
#endif
