using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.Services.Customers;

/// <summary>
/// 信用评估结果
/// </summary>
public class CreditAssessmentResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 推荐信用等级
    /// </summary>
    public CreditRating RecommendedRating { get; set; }

    /// <summary>
    /// 当前信用等级
    /// </summary>
    public CreditRating? CurrentRating { get; set; }

    /// <summary>
    /// 计算的信用评分
    /// </summary>
    public decimal CreditScore { get; set; }

    /// <summary>
    /// 推荐信用额度
    /// </summary>
    public decimal RecommendedCreditLimit { get; set; }

    /// <summary>
    /// 当前信用额度
    /// </summary>
    public decimal CurrentCreditLimit { get; set; }

    /// <summary>
    /// 风险等级
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// 评分明细
    /// </summary>
    public CreditScoreDetails ScoreDetails { get; set; } = new();

    /// <summary>
    /// 评估建议
    /// </summary>
    public List<string> Recommendations { get; set; } = new();

    /// <summary>
    /// 风险提示
    /// </summary>
    public List<string> RiskWarnings { get; set; } = new();

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 匹配的评估规则ID
    /// </summary>
    public string? MatchedRuleId { get; set; }

    /// <summary>
    /// 客户财务指标
    /// </summary>
    public CustomerFinancialMetrics? FinancialMetrics { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static CreditAssessmentResult Success(CreditRating rating, decimal score, decimal recommendedLimit)
    {
        return new CreditAssessmentResult
        {
            IsSuccess = true,
            RecommendedRating = rating,
            CreditScore = score,
            RecommendedCreditLimit = recommendedLimit
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static CreditAssessmentResult Failure(string errorMessage)
    {
        return new CreditAssessmentResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// 客户财务指标
/// </summary>
public class CustomerFinancialMetrics
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
    /// 总交易金额
    /// </summary>
    public decimal TotalTransactionAmount { get; set; }

    /// <summary>
    /// 平均订单金额
    /// </summary>
    public decimal AverageOrderAmount { get; set; }

    /// <summary>
    /// 交易次数
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// 按时付款次数
    /// </summary>
    public int OnTimePaymentCount { get; set; }

    /// <summary>
    /// 逾期付款次数
    /// </summary>
    public int OverduePaymentCount { get; set; }

    /// <summary>
    /// 按时付款率
    /// </summary>
    public decimal OnTimePaymentRate => TransactionCount > 0 ?
        (decimal)OnTimePaymentCount / TransactionCount * 100 : 0;

    /// <summary>
    /// 平均付款延迟天数
    /// </summary>
    public decimal AveragePaymentDelay { get; set; }

    /// <summary>
    /// 最大逾期天数
    /// </summary>
    public int MaxOverdueDays { get; set; }

    /// <summary>
    /// 当前未结清金额
    /// </summary>
    public decimal OutstandingAmount { get; set; }

    /// <summary>
    /// 最长合作时间（天数）
    /// </summary>
    public int CooperationDays { get; set; }

    /// <summary>
    /// 争议次数
    /// </summary>
    public int DisputeCount { get; set; }

    /// <summary>
    /// 退货率
    /// </summary>
    public decimal ReturnRate { get; set; }

    /// <summary>
    /// 信用额度使用率
    /// </summary>
    public decimal CreditUtilizationRate { get; set; }

    /// <summary>
    /// 年度增长率
    /// </summary>
    public decimal AnnualGrowthRate { get; set; }

    /// <summary>
    /// 扩展指标
    /// </summary>
    public Dictionary<string, decimal> ExtendedMetrics { get; set; } = new();
}

/// <summary>
/// 批量信用评估结果
/// </summary>
public class BatchCreditAssessmentResult
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
    public int RatingChangedCount { get; set; }

    /// <summary>
    /// 额度变更数量
    /// </summary>
    public int LimitChangedCount { get; set; }

    /// <summary>
    /// 处理结果详情
    /// </summary>
    public List<CreditAssessmentResult> Results { get; set; } = new();

    /// <summary>
    /// 等级分布统计
    /// </summary>
    public Dictionary<CreditRating, int> RatingDistribution { get; set; } = new();

    /// <summary>
    /// 风险分布统计
    /// </summary>
    public Dictionary<RiskLevel, int> RiskDistribution { get; set; } = new();

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
/// 风险预警结果
/// </summary>
public class RiskWarningResult
{
    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 预警类型
    /// </summary>
    public List<RiskWarningType> WarningTypes { get; set; } = new();

    /// <summary>
    /// 风险等级
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// 预警描述
    /// </summary>
    public List<string> WarningMessages { get; set; } = new();

    /// <summary>
    /// 建议措施
    /// </summary>
    public List<string> RecommendedActions { get; set; } = new();

    /// <summary>
    /// 预警时间
    /// </summary>
    public DateTime WarningTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 相关数据
    /// </summary>
    public Dictionary<string, object> RelatedData { get; set; } = new();
}

/// <summary>
/// 信用统计信息
/// </summary>
public class CreditStatistics
{
    /// <summary>
    /// 总客户数
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// 各信用等级分布
    /// </summary>
    public Dictionary<CreditRating, int> RatingDistribution { get; set; } = new();

    /// <summary>
    /// 各风险等级分布
    /// </summary>
    public Dictionary<RiskLevel, int> RiskDistribution { get; set; } = new();

    /// <summary>
    /// 各信用状态分布
    /// </summary>
    public Dictionary<CreditStatus, int> StatusDistribution { get; set; } = new();

    /// <summary>
    /// 总授信额度
    /// </summary>
    public decimal TotalCreditLimit { get; set; }

    /// <summary>
    /// 已使用额度
    /// </summary>
    public decimal TotalUsedCredit { get; set; }

    /// <summary>
    /// 可用额度
    /// </summary>
    public decimal TotalAvailableCredit => TotalCreditLimit - TotalUsedCredit;

    /// <summary>
    /// 总体额度使用率
    /// </summary>
    public decimal OverallUtilizationRate => TotalCreditLimit > 0 ?
        (TotalUsedCredit / TotalCreditLimit) * 100 : 0;

    /// <summary>
    /// 逾期客户数
    /// </summary>
    public int OverdueCustomerCount { get; set; }

    /// <summary>
    /// 总逾期金额
    /// </summary>
    public decimal TotalOverdueAmount { get; set; }

    /// <summary>
    /// 平均信用评分
    /// </summary>
    public decimal AverageCreditScore { get; set; }

    /// <summary>
    /// 最近30天评级变更统计
    /// </summary>
    public Dictionary<CreditChangeType, int> RecentChanges { get; set; } = new();

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 逾期分析结果
/// </summary>
public class OverdueAnalysisResult
{
    /// <summary>
    /// 逾期客户数量
    /// </summary>
    public int OverdueCustomerCount { get; set; }

    /// <summary>
    /// 各逾期等级分布
    /// </summary>
    public Dictionary<OverdueLevel, int> LevelDistribution { get; set; } = new();

    /// <summary>
    /// 各逾期等级金额分布
    /// </summary>
    public Dictionary<OverdueLevel, decimal> AmountDistribution { get; set; } = new();

    /// <summary>
    /// 总逾期金额
    /// </summary>
    public decimal TotalOverdueAmount { get; set; }

    /// <summary>
    /// 平均逾期天数
    /// </summary>
    public decimal AverageOverdueDays { get; set; }

    /// <summary>
    /// 逾期率
    /// </summary>
    public decimal OverdueRate { get; set; }

    /// <summary>
    /// 按行业分析
    /// </summary>
    public Dictionary<string, decimal> IndustryAnalysis { get; set; } = new();

    /// <summary>
    /// 按地区分析
    /// </summary>
    public Dictionary<string, decimal> RegionAnalysis { get; set; } = new();

    /// <summary>
    /// 预测坏账率
    /// </summary>
    public decimal PredictedBadDebtRate { get; set; }

    /// <summary>
    /// 分析时间
    /// </summary>
    public DateTime AnalysisTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 客户信用管理服务接口
/// </summary>
public interface ICustomerCreditService
{
    // =================== 信用评估 ===================

    /// <summary>
    /// 计算客户信用评估（预览，不保存）
    /// </summary>
    Task<CreditAssessmentResult> CalculateCreditAssessmentAsync(string customerId, CancellationToken ct = default);

    /// <summary>
    /// 执行客户信用评估
    /// </summary>
    Task<CreditAssessmentResult> AssessCustomerCreditAsync(string customerId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 批量信用评估
    /// </summary>
    Task<BatchCreditAssessmentResult> BatchAssessCustomerCreditsAsync(List<string>? customerIds = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 重新评估所有客户信用
    /// </summary>
    Task<BatchCreditAssessmentResult> ReassessAllCustomerCreditsAsync(string? userId = null, CancellationToken ct = default);

    // =================== 信用额度管理 ===================

    /// <summary>
    /// 设置客户信用额度
    /// </summary>
    Task<bool> SetCreditLimitAsync(string customerId, decimal creditLimit, string reason, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 调整客户信用额度
    /// </summary>
    Task<bool> AdjustCreditLimitAsync(string customerId, decimal newLimit, string reason, bool requireApproval = true, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 临时调整信用额度
    /// </summary>
    Task<bool> TemporaryAdjustCreditLimitAsync(string customerId, decimal tempLimit, DateTime expiryDate, string reason, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 冻结客户信用
    /// </summary>
    Task<bool> FreezeCreditAsync(string customerId, string reason, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 解冻客户信用
    /// </summary>
    Task<bool> UnfreezeCreditAsync(string customerId, string reason, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 检查信用额度可用性
    /// </summary>
    Task<(bool IsAvailable, decimal AvailableAmount, string? Message)> CheckCreditAvailabilityAsync(string customerId, decimal requestAmount, CancellationToken ct = default);

    /// <summary>
    /// 占用信用额度
    /// </summary>
    Task<bool> AllocateCreditAsync(string customerId, decimal amount, string businessDocumentId, OverdueBusinessType businessType, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 释放信用额度
    /// </summary>
    Task<bool> ReleaseCreditAsync(string customerId, decimal amount, string businessDocumentId, string? userId = null, CancellationToken ct = default);

    // =================== 逾期管理 ===================

    /// <summary>
    /// 创建逾期记录
    /// </summary>
    Task<OverdueRecord> CreateOverdueRecordAsync(string customerId, string businessDocumentId, OverdueBusinessType businessType, decimal amount, DateTime dueDate, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 更新逾期记录状态
    /// </summary>
    Task<bool> UpdateOverdueRecordStatusAsync(string overdueRecordId, OverdueStatus status, string? notes = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 结清逾期记录
    /// </summary>
    Task<bool> SettleOverdueRecordAsync(string overdueRecordId, decimal settleAmount, string? notes = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取客户逾期记录
    /// </summary>
    Task<List<OverdueRecord>> GetCustomerOverdueRecordsAsync(string customerId, OverdueStatus? status = null, int limit = 50, CancellationToken ct = default);

    /// <summary>
    /// 批量扫描逾期记录
    /// </summary>
    Task<List<OverdueRecord>> ScanOverdueRecordsAsync(int daysPastDue = 1, CancellationToken ct = default);

    /// <summary>
    /// 逾期分析
    /// </summary>
    Task<OverdueAnalysisResult> AnalyzeOverdueDataAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);

    // =================== 收款提醒 ===================

    /// <summary>
    /// 创建收款提醒
    /// </summary>
    Task<CollectionReminder> CreateCollectionReminderAsync(string customerId, ReminderType reminderType, ReminderMethod method, string content, DateTime scheduledTime, string? overdueRecordId = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 发送收款提醒
    /// </summary>
    Task<bool> SendCollectionReminderAsync(string reminderId, CancellationToken ct = default);

    /// <summary>
    /// 批量发送到期提醒
    /// </summary>
    Task<int> BatchSendDueRemindersAsync(int daysBeforeDue = 3, CancellationToken ct = default);

    /// <summary>
    /// 批量发送逾期提醒
    /// </summary>
    Task<int> BatchSendOverdueRemindersAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取客户提醒记录
    /// </summary>
    Task<List<CollectionReminder>> GetCustomerRemindersAsync(string customerId, ReminderStatus? status = null, int limit = 50, CancellationToken ct = default);

    // =================== 风险预警 ===================

    /// <summary>
    /// 执行风险扫描
    /// </summary>
    Task<List<RiskWarningResult>> ScanRiskWarningsAsync(CancellationToken ct = default);

    /// <summary>
    /// 分析客户风险等级
    /// </summary>
    Task<RiskWarningResult> AnalyzeCustomerRiskAsync(string customerId, CancellationToken ct = default);

    /// <summary>
    /// 获取高风险客户列表
    /// </summary>
    Task<List<CustomerCredit>> GetHighRiskCustomersAsync(RiskLevel minRiskLevel = RiskLevel.High, CancellationToken ct = default);

    /// <summary>
    /// 预测客户违约风险
    /// </summary>
    Task<Dictionary<string, decimal>> PredictDefaultRiskAsync(List<string> customerIds, int forecastDays = 90, CancellationToken ct = default);

    // =================== 查询和管理 ===================

    /// <summary>
    /// 获取客户信用信息
    /// </summary>
    Task<CustomerCredit?> GetCustomerCreditAsync(string customerId, CancellationToken ct = default);

    /// <summary>
    /// 获取客户信用历史
    /// </summary>
    Task<List<CustomerCreditHistory>> GetCreditHistoryAsync(string customerId, int limit = 50, CancellationToken ct = default);

    /// <summary>
    /// 搜索客户信用信息
    /// </summary>
    Task<(List<CustomerCredit> credits, int total)> SearchCustomerCreditsAsync(
        CreditRating? rating = null,
        CreditStatus? status = null,
        RiskLevel? riskLevel = null,
        decimal? minScore = null,
        decimal? maxScore = null,
        bool? isFrozen = null,
        string? keyword = null,
        int page = 1,
        int size = 20,
        CancellationToken ct = default);

    // =================== 规则管理 ===================

    /// <summary>
    /// 创建信用评估规则
    /// </summary>
    Task<CreditAssessmentRule> CreateAssessmentRuleAsync(string ruleName, CreditRating targetRating, decimal minScore, decimal maxScore, CreditScoreWeights scoreWeights, RiskThresholds riskThresholds, int priority = 100, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 更新信用评估规则
    /// </summary>
    Task<bool> UpdateAssessmentRuleAsync(string ruleId, string? ruleName = null, decimal? minScore = null, decimal? maxScore = null, CreditScoreWeights? scoreWeights = null, RiskThresholds? riskThresholds = null, int? priority = null, bool? isEnabled = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取信用评估规则
    /// </summary>
    Task<List<CreditAssessmentRule>> GetAssessmentRulesAsync(bool enabledOnly = true, CancellationToken ct = default);

    /// <summary>
    /// 删除信用评估规则
    /// </summary>
    Task<bool> DeleteAssessmentRuleAsync(string ruleId, string? userId = null, CancellationToken ct = default);

    // =================== 财务指标计算 ===================

    /// <summary>
    /// 计算客户财务指标
    /// </summary>
    Task<CustomerFinancialMetrics> CalculateFinancialMetricsAsync(string customerId, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken ct = default);

    /// <summary>
    /// 批量计算客户财务指标
    /// </summary>
    Task<Dictionary<string, CustomerFinancialMetrics>> BatchCalculateFinancialMetricsAsync(List<string> customerIds, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken ct = default);

    // =================== 统计分析 ===================

    /// <summary>
    /// 获取信用统计信息
    /// </summary>
    Task<CreditStatistics> GetCreditStatisticsAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取信用趋势分析
    /// </summary>
    Task<Dictionary<DateTime, Dictionary<CreditRating, int>>> GetCreditTrendAnalysisAsync(DateTime fromDate, DateTime toDate, string groupBy = "month", CancellationToken ct = default);

    /// <summary>
    /// 分析信用风险分布
    /// </summary>
    Task<object> AnalyzeCreditRiskDistributionAsync(CancellationToken ct = default);

    // =================== 审批流程 ===================

    /// <summary>
    /// 提交信用调整申请
    /// </summary>
    Task<string> SubmitCreditAdjustmentRequestAsync(string customerId, CreditChangeType changeType, string changeValue, string reason, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 审批信用调整申请
    /// </summary>
    Task<bool> ApproveCreditAdjustmentAsync(string requestId, bool isApproved, string? approvalNotes = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取待审批的信用调整申请
    /// </summary>
    Task<List<CustomerCreditHistory>> GetPendingCreditAdjustmentsAsync(CancellationToken ct = default);

    // =================== 报告生成 ===================

    /// <summary>
    /// 生成客户信用报告
    /// </summary>
    Task<byte[]> GenerateCustomerCreditReportAsync(string customerId, string format = "pdf", CancellationToken ct = default);

    /// <summary>
    /// 生成信用统计报告
    /// </summary>
    Task<byte[]> GenerateCreditStatisticsReportAsync(DateTime fromDate, DateTime toDate, string format = "excel", CancellationToken ct = default);

    /// <summary>
    /// 生成逾期分析报告
    /// </summary>
    Task<byte[]> GenerateOverdueAnalysisReportAsync(DateTime fromDate, DateTime toDate, string format = "excel", CancellationToken ct = default);
}

// =================== 辅助枚举 ===================

/// <summary>
/// 风险预警类型
/// </summary>
public enum RiskWarningType
{
    /// <summary>
    /// 信用额度超限
    /// </summary>
    CreditLimitExceeded = 1,

    /// <summary>
    /// 逾期天数超阈值
    /// </summary>
    OverdueDaysExceeded = 2,

    /// <summary>
    /// 信用评分下降
    /// </summary>
    CreditScoreDecline = 3,

    /// <summary>
    /// 付款延迟增加
    /// </summary>
    PaymentDelayIncrease = 4,

    /// <summary>
    /// 争议次数增加
    /// </summary>
    DisputeCountIncrease = 5,

    /// <summary>
    /// 交易额异常下降
    /// </summary>
    TransactionAmountDrop = 6,

    /// <summary>
    /// 信用额度使用率过高
    /// </summary>
    HighCreditUtilization = 7,

    /// <summary>
    /// 多次逾期
    /// </summary>
    RepeatedOverdue = 8
}