namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 客户分级记录 - 记录客户的等级信息和变更历史
/// </summary>
public class CustomerClassification
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 客户等级
    /// </summary>
    public CustomerLevel CurrentLevel { get; set; } = CustomerLevel.Regular;

    /// <summary>
    /// 分级方式
    /// </summary>
    public ClassificationMethod Method { get; set; } = ClassificationMethod.Automatic;

    /// <summary>
    /// 分级生效日期
    /// </summary>
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 分级到期日期（可选，用于临时升级）
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// 分级原因/备注
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 自动分级时的评分
    /// </summary>
    public decimal? AutoScore { get; set; }

    /// <summary>
    /// 评分明细（JSON格式）
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public Dictionary<string, decimal> ScoreDetails { get; set; } = new();

    /// <summary>
    /// 是否当前有效
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 导航属性：关联客户（复用 Account 实体）
    /// </summary>
    public Account? Customer { get; set; }

    /// <summary>
    /// 导航属性：分级历史记录
    /// </summary>
    public List<CustomerClassificationHistory> History { get; set; } = new();
}

/// <summary>
/// 客户分级历史记录
/// </summary>
public class CustomerClassificationHistory
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 分级记录ID
    /// </summary>
    public string ClassificationId { get; set; } = string.Empty;

    /// <summary>
    /// 变更类型
    /// </summary>
    public ClassificationChangeType ChangeType { get; set; }

    /// <summary>
    /// 原等级
    /// </summary>
    public CustomerLevel? FromLevel { get; set; }

    /// <summary>
    /// 新等级
    /// </summary>
    public CustomerLevel ToLevel { get; set; }

    /// <summary>
    /// 变更原因
    /// </summary>
    public string? ChangeReason { get; set; }

    /// <summary>
    /// 变更时的评分
    /// </summary>
    public decimal? Score { get; set; }

    /// <summary>
    /// 评分变化
    /// </summary>
    public decimal? ScoreChange { get; set; }

    /// <summary>
    /// 变更方式
    /// </summary>
    public ClassificationMethod Method { get; set; }

    /// <summary>
    /// 变更时间
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 变更人
    /// </summary>
    public string? ChangedBy { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public Dictionary<string, object> ExtensionData { get; set; } = new();

    /// <summary>
    /// 导航属性：关联分级记录
    /// </summary>
    public CustomerClassification? Classification { get; set; }
}

/// <summary>
/// 客户分级规则配置
/// </summary>
public class CustomerClassificationRule
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 规则名称
    /// </summary>
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// 规则描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 目标等级
    /// </summary>
    public CustomerLevel TargetLevel { get; set; }

    /// <summary>
    /// 规则条件（JSON格式）
    /// </summary>
    public CustomerClassificationCriteria Criteria { get; set; } = new();

    /// <summary>
    /// 最低总分要求
    /// </summary>
    public decimal MinTotalScore { get; set; }

    /// <summary>
    /// 规则优先级（数字越小优先级越高）
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// 客户分级标准
/// </summary>
public class CustomerClassificationCriteria
{
    /// <summary>
    /// 年交易金额权重和阈值
    /// </summary>
    public ScoreCriteria AnnualSalesAmount { get; set; } = new();

    /// <summary>
    /// 交易频次权重和阈值
    /// </summary>
    public ScoreCriteria TransactionFrequency { get; set; } = new();

    /// <summary>
    /// 客户忠诚度权重和阈值（合作年限）
    /// </summary>
    public ScoreCriteria CustomerLoyalty { get; set; } = new();

    /// <summary>
    /// 回款及时性权重和阈值
    /// </summary>
    public ScoreCriteria PaymentTimeliness { get; set; } = new();

    /// <summary>
    /// 订单平均金额权重和阈值
    /// </summary>
    public ScoreCriteria AverageOrderValue { get; set; } = new();

    /// <summary>
    /// 增长趋势权重和阈值
    /// </summary>
    public ScoreCriteria GrowthTrend { get; set; } = new();

    /// <summary>
    /// 投诉次数权重和阈值（负分项）
    /// </summary>
    public ScoreCriteria ComplaintCount { get; set; } = new();

    /// <summary>
    /// 利润贡献权重和阈值
    /// </summary>
    public ScoreCriteria ProfitContribution { get; set; } = new();
}

/// <summary>
/// 评分标准
/// </summary>
public class ScoreCriteria
{
    /// <summary>
    /// 权重（0-1之间）
    /// </summary>
    public decimal Weight { get; set; } = 0.1m;

    /// <summary>
    /// 最高分值
    /// </summary>
    public decimal MaxScore { get; set; } = 100m;

    /// <summary>
    /// 阈值设置（用于分段评分）
    /// </summary>
    public List<ScoreThreshold> Thresholds { get; set; } = new();

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 评分阈值
/// </summary>
public class ScoreThreshold
{
    /// <summary>
    /// 最小值
    /// </summary>
    public decimal MinValue { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    public decimal MaxValue { get; set; }

    /// <summary>
    /// 对应分数
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 客户等级
/// </summary>
public enum CustomerLevel
{
    /// <summary>
    /// 潜在客户
    /// </summary>
    Potential = 1,

    /// <summary>
    /// 普通客户
    /// </summary>
    Regular = 2,

    /// <summary>
    /// 重要客户
    /// </summary>
    Important = 3,

    /// <summary>
    /// VIP客户
    /// </summary>
    VIP = 4,

    /// <summary>
    /// 钻石客户
    /// </summary>
    Diamond = 5,

    /// <summary>
    /// 战略客户
    /// </summary>
    Strategic = 6
}

/// <summary>
/// 分级方式
/// </summary>
public enum ClassificationMethod
{
    /// <summary>
    /// 自动分级
    /// </summary>
    Automatic = 1,

    /// <summary>
    /// 手动分级
    /// </summary>
    Manual = 2,

    /// <summary>
    /// 系统导入
    /// </summary>
    SystemImport = 3,

    /// <summary>
    /// 规则调整
    /// </summary>
    RuleAdjustment = 4
}

/// <summary>
/// 分级变更类型
/// </summary>
public enum ClassificationChangeType
{
    /// <summary>
    /// 新建分级
    /// </summary>
    Initial = 1,

    /// <summary>
    /// 等级升级
    /// </summary>
    Upgrade = 2,

    /// <summary>
    /// 等级降级
    /// </summary>
    Downgrade = 3,

    /// <summary>
    /// 手动调整
    /// </summary>
    ManualAdjustment = 4,

    /// <summary>
    /// 规则重算
    /// </summary>
    RuleRecalculation = 5,

    /// <summary>
    /// 过期失效
    /// </summary>
    Expired = 6
}
