using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.Services.Customers;

/// <summary>
/// 客户分级结果
/// </summary>
public class CustomerClassificationResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 推荐等级
    /// </summary>
    public CustomerLevel RecommendedLevel { get; set; }

    /// <summary>
    /// 当前等级
    /// </summary>
    public CustomerLevel? CurrentLevel { get; set; }

    /// <summary>
    /// 总评分
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// 各项评分明细
    /// </summary>
    public Dictionary<string, decimal> ScoreBreakdown { get; set; } = new();

    /// <summary>
    /// 分级建议
    /// </summary>
    public List<string> Recommendations { get; set; } = new();

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 匹配的规则
    /// </summary>
    public string? MatchedRuleId { get; set; }

    /// <summary>
    /// 计算基础数据
    /// </summary>
    public CustomerMetrics? Metrics { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static CustomerClassificationResult Success(CustomerLevel recommendedLevel, decimal totalScore, Dictionary<string, decimal> scoreBreakdown)
    {
        return new CustomerClassificationResult
        {
            IsSuccess = true,
            RecommendedLevel = recommendedLevel,
            TotalScore = totalScore,
            ScoreBreakdown = scoreBreakdown
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static CustomerClassificationResult Failure(string errorMessage)
    {
        return new CustomerClassificationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// 客户指标数据
/// </summary>
public class CustomerMetrics
{
    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 统计期间开始日期
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// 统计期间结束日期
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// 年交易金额
    /// </summary>
    public decimal AnnualSalesAmount { get; set; }

    /// <summary>
    /// 交易频次（年订单数）
    /// </summary>
    public int TransactionFrequency { get; set; }

    /// <summary>
    /// 合作年限（天数）
    /// </summary>
    public int CustomerLoyaltyDays { get; set; }

    /// <summary>
    /// 回款及时性（百分比）
    /// </summary>
    public decimal PaymentTimelinessRate { get; set; }

    /// <summary>
    /// 平均订单金额
    /// </summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>
    /// 增长趋势（同比增长率）
    /// </summary>
    public decimal GrowthTrendRate { get; set; }

    /// <summary>
    /// 投诉次数
    /// </summary>
    public int ComplaintCount { get; set; }

    /// <summary>
    /// 利润贡献
    /// </summary>
    public decimal ProfitContribution { get; set; }

    /// <summary>
    /// 最后交易日期
    /// </summary>
    public DateTime? LastTransactionDate { get; set; }

    /// <summary>
    /// 历史最高年销售额
    /// </summary>
    public decimal HistoricalMaxAnnualSales { get; set; }

    /// <summary>
    /// 扩展指标
    /// </summary>
    public Dictionary<string, decimal> ExtendedMetrics { get; set; } = new();
}

/// <summary>
/// 批量分级结果
/// </summary>
public class BatchClassificationResult
{
    /// <summary>
    /// 成功处理数量
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败处理数量
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 等级变更数量
    /// </summary>
    public int LevelChangedCount { get; set; }

    /// <summary>
    /// 处理结果详情
    /// </summary>
    public List<CustomerClassificationResult> Results { get; set; } = new();

    /// <summary>
    /// 等级分布统计
    /// </summary>
    public Dictionary<CustomerLevel, int> LevelDistribution { get; set; } = new();

    /// <summary>
    /// 处理开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 处理结束时间
    /// </summary>
    public DateTime EndTime { get; set; }
}

/// <summary>
/// 分级统计信息
/// </summary>
public class ClassificationStatistics
{
    /// <summary>
    /// 总客户数
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// 各等级客户数量分布
    /// </summary>
    public Dictionary<CustomerLevel, int> LevelDistribution { get; set; } = new();

    /// <summary>
    /// 各等级销售额分布
    /// </summary>
    public Dictionary<CustomerLevel, decimal> SalesDistribution { get; set; } = new();

    /// <summary>
    /// 各等级利润分布
    /// </summary>
    public Dictionary<CustomerLevel, decimal> ProfitDistribution { get; set; } = new();

    /// <summary>
    /// 最近30天等级变更统计
    /// </summary>
    public Dictionary<ClassificationChangeType, int> RecentChanges { get; set; } = new();

    /// <summary>
    /// 平均客户评分
    /// </summary>
    public decimal AverageCustomerScore { get; set; }

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 客户分级服务接口
/// </summary>
public interface ICustomerClassificationService
{
    // =================== 自动分级 ===================

    /// <summary>
    /// 计算客户分级（不保存）
    /// </summary>
    Task<CustomerClassificationResult> CalculateClassificationAsync(string customerId, CancellationToken ct = default);

    /// <summary>
    /// 执行客户自动分级
    /// </summary>
    Task<CustomerClassificationResult> ClassifyCustomerAsync(string customerId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 批量自动分级
    /// </summary>
    Task<BatchClassificationResult> BatchClassifyCustomersAsync(List<string>? customerIds = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 重新计算所有客户分级
    /// </summary>
    Task<BatchClassificationResult> RecalculateAllClassificationsAsync(string? userId = null, CancellationToken ct = default);

    // =================== 手动分级 ===================

    /// <summary>
    /// 手动设置客户等级
    /// </summary>
    Task<bool> SetCustomerLevelAsync(
        string customerId,
        CustomerLevel level,
        string reason,
        DateTime? expiryDate = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 临时升级客户等级
    /// </summary>
    Task<bool> TemporaryUpgradeCustomerAsync(
        string customerId,
        CustomerLevel level,
        DateTime expiryDate,
        string reason,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 批量手动设置客户等级
    /// </summary>
    Task<BatchClassificationResult> BatchSetCustomerLevelsAsync(
        Dictionary<string, CustomerLevel> customerLevels,
        string reason,
        string? userId = null,
        CancellationToken ct = default);

    // =================== 查询和管理 ===================

    /// <summary>
    /// 获取客户当前分级信息
    /// </summary>
    Task<CustomerClassification?> GetCustomerClassificationAsync(string customerId, CancellationToken ct = default);

    /// <summary>
    /// 获取客户分级历史
    /// </summary>
    Task<List<CustomerClassificationHistory>> GetClassificationHistoryAsync(string customerId, int limit = 50, CancellationToken ct = default);

    /// <summary>
    /// 查询指定等级的客户列表
    /// </summary>
    Task<(List<CustomerClassification> classifications, int total)> GetCustomersByLevelAsync(
        CustomerLevel level,
        int page = 1,
        int size = 20,
        CancellationToken ct = default);

    /// <summary>
    /// 搜索客户分级信息
    /// </summary>
    Task<(List<CustomerClassification> classifications, int total)> SearchClassificationsAsync(
        string? keyword = null,
        CustomerLevel? level = null,
        ClassificationMethod? method = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int size = 20,
        CancellationToken ct = default);

    // =================== 规则管理 ===================

    /// <summary>
    /// 创建分级规则
    /// </summary>
    Task<CustomerClassificationRule> CreateClassificationRuleAsync(
        string ruleName,
        CustomerLevel targetLevel,
        CustomerClassificationCriteria criteria,
        decimal minTotalScore,
        int priority = 100,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 更新分级规则
    /// </summary>
    Task<bool> UpdateClassificationRuleAsync(
        string ruleId,
        string? ruleName = null,
        CustomerClassificationCriteria? criteria = null,
        decimal? minTotalScore = null,
        int? priority = null,
        bool? isEnabled = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 获取所有分级规则
    /// </summary>
    Task<List<CustomerClassificationRule>> GetClassificationRulesAsync(bool enabledOnly = true, CancellationToken ct = default);

    /// <summary>
    /// 删除分级规则
    /// </summary>
    Task<bool> DeleteClassificationRuleAsync(string ruleId, string? userId = null, CancellationToken ct = default);

    // =================== 指标计算 ===================

    /// <summary>
    /// 计算客户指标
    /// </summary>
    Task<CustomerMetrics> CalculateCustomerMetricsAsync(string customerId, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken ct = default);

    /// <summary>
    /// 批量计算客户指标
    /// </summary>
    Task<Dictionary<string, CustomerMetrics>> BatchCalculateCustomerMetricsAsync(List<string> customerIds, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken ct = default);

    // =================== 统计分析 ===================

    /// <summary>
    /// 获取分级统计信息
    /// </summary>
    Task<ClassificationStatistics> GetClassificationStatisticsAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取等级趋势分析
    /// </summary>
    Task<Dictionary<DateTime, Dictionary<CustomerLevel, int>>> GetLevelTrendAnalysisAsync(
        DateTime fromDate,
        DateTime toDate,
        string groupBy = "month", // month, week, day
        CancellationToken ct = default);

    /// <summary>
    /// 分析客户价值分布
    /// </summary>
    Task<object> AnalyzeCustomerValueDistributionAsync(CancellationToken ct = default);

    // =================== 预警和监控 ===================

    /// <summary>
    /// 检查需要重新分级的客户
    /// </summary>
    Task<List<string>> CheckCustomersForReclassificationAsync(int daysSinceLastClassification = 90, CancellationToken ct = default);

    /// <summary>
    /// 检查即将过期的临时升级
    /// </summary>
    Task<List<CustomerClassification>> CheckExpiringTemporaryUpgradesAsync(int daysBeforeExpiry = 7, CancellationToken ct = default);

    /// <summary>
    /// 生成分级报告
    /// </summary>
    Task<byte[]> GenerateClassificationReportAsync(
        DateTime fromDate,
        DateTime toDate,
        CustomerLevel? level = null,
        string format = "excel",
        CancellationToken ct = default);

    // =================== 业务洞察 ===================

    /// <summary>
    /// 预测客户等级变化
    /// </summary>
    Task<Dictionary<string, CustomerLevel>> PredictCustomerLevelChangesAsync(List<string> customerIds, int forecastDays = 90, CancellationToken ct = default);

    /// <summary>
    /// 分析客户流失风险
    /// </summary>
    Task<Dictionary<string, decimal>> AnalyzeCustomerChurnRiskAsync(CustomerLevel? level = null, CancellationToken ct = default);

    /// <summary>
    /// 获取客户升级建议
    /// </summary>
    Task<List<string>> GetCustomerUpgradeSuggestionsAsync(string customerId, CancellationToken ct = default);
}