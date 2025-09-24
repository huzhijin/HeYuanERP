namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 客户信用记录 - 记录客户的信用状况和评级信息
/// </summary>
public class CustomerCredit
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
    /// 信用等级
    /// </summary>
    public CreditRating CreditRating { get; set; } = CreditRating.BBB;

    /// <summary>
    /// 信用评分（0-1000分）
    /// </summary>
    public decimal CreditScore { get; set; }

    /// <summary>
    /// 信用额度
    /// </summary>
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// 已使用额度
    /// </summary>
    public decimal UsedCredit { get; set; }

    /// <summary>
    /// 可用额度
    /// </summary>
    public decimal AvailableCredit => CreditLimit - UsedCredit;

    /// <summary>
    /// 信用额度使用率
    /// </summary>
    public decimal CreditUtilizationRate => CreditLimit > 0 ? (UsedCredit / CreditLimit) * 100 : 0;

    /// <summary>
    /// 付款条件（天数）
    /// </summary>
    public int PaymentTerms { get; set; } = 30;

    /// <summary>
    /// 信用状态
    /// </summary>
    public CreditStatus Status { get; set; } = CreditStatus.Normal;

    /// <summary>
    /// 风险等级
    /// </summary>
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;

    /// <summary>
    /// 最后评估时间
    /// </summary>
    public DateTime LastAssessmentDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 下次评估时间
    /// </summary>
    public DateTime NextAssessmentDate { get; set; } = DateTime.UtcNow.AddMonths(3);

    /// <summary>
    /// 评估人员
    /// </summary>
    public string? AssessedBy { get; set; }

    /// <summary>
    /// 评估备注
    /// </summary>
    public string? AssessmentNotes { get; set; }

    /// <summary>
    /// 信用评分明细
    /// </summary>
    public CreditScoreDetails ScoreDetails { get; set; } = new();

    /// <summary>
    /// 是否冻结信用
    /// </summary>
    public bool IsFrozen { get; set; }

    /// <summary>
    /// 冻结原因
    /// </summary>
    public string? FreezeReason { get; set; }

    /// <summary>
    /// 冻结时间
    /// </summary>
    public DateTime? FrozenAt { get; set; }

    /// <summary>
    /// 冻结人员
    /// </summary>
    public string? FrozenBy { get; set; }

    /// <summary>
    /// 担保信息
    /// </summary>
    public GuaranteeInfo? GuaranteeInfo { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, object> ExtensionData { get; set; } = new();

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
    /// 导航属性：关联客户
    /// </summary>
    public Account? Customer { get; set; }

    /// <summary>
    /// 导航属性：信用历史记录
    /// </summary>
    public List<CustomerCreditHistory> History { get; set; } = new();

    /// <summary>
    /// 导航属性：逾期记录
    /// </summary>
    public List<OverdueRecord> OverdueRecords { get; set; } = new();

    /// <summary>
    /// 导航属性：收款提醒记录
    /// </summary>
    public List<CollectionReminder> CollectionReminders { get; set; } = new();
}

/// <summary>
/// 信用评分明细
/// </summary>
public class CreditScoreDetails
{
    /// <summary>
    /// 付款历史评分（35%权重）
    /// </summary>
    public decimal PaymentHistoryScore { get; set; }

    /// <summary>
    /// 信用额度使用率评分（30%权重）
    /// </summary>
    public decimal CreditUtilizationScore { get; set; }

    /// <summary>
    /// 合作历史长度评分（15%权重）
    /// </summary>
    public decimal CreditHistoryLengthScore { get; set; }

    /// <summary>
    /// 交易频率和稳定性评分（10%权重）
    /// </summary>
    public decimal TransactionStabilityScore { get; set; }

    /// <summary>
    /// 财务状况评分（10%权重）
    /// </summary>
    public decimal FinancialConditionScore { get; set; }

    /// <summary>
    /// 计算加权总分
    /// </summary>
    public decimal CalculateWeightedScore()
    {
        return PaymentHistoryScore * 0.35m +
               CreditUtilizationScore * 0.30m +
               CreditHistoryLengthScore * 0.15m +
               TransactionStabilityScore * 0.10m +
               FinancialConditionScore * 0.10m;
    }
}

/// <summary>
/// 担保信息
/// </summary>
public class GuaranteeInfo
{
    /// <summary>
    /// 担保类型
    /// </summary>
    public GuaranteeType Type { get; set; }

    /// <summary>
    /// 担保金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 担保人/机构名称
    /// </summary>
    public string GuarantorName { get; set; } = string.Empty;

    /// <summary>
    /// 担保人联系方式
    /// </summary>
    public string GuarantorContact { get; set; } = string.Empty;

    /// <summary>
    /// 担保文件URL
    /// </summary>
    public List<string> DocumentUrls { get; set; } = new();

    /// <summary>
    /// 担保生效日期
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// 担保到期日期
    /// </summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// 担保状态
    /// </summary>
    public GuaranteeStatus Status { get; set; } = GuaranteeStatus.Active;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 客户信用历史记录
/// </summary>
public class CustomerCreditHistory
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
    /// 信用记录ID
    /// </summary>
    public string CreditId { get; set; } = string.Empty;

    /// <summary>
    /// 变更类型
    /// </summary>
    public CreditChangeType ChangeType { get; set; }

    /// <summary>
    /// 变更字段
    /// </summary>
    public string ChangedField { get; set; } = string.Empty;

    /// <summary>
    /// 变更前值
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// 变更后值
    /// </summary>
    public string NewValue { get; set; } = string.Empty;

    /// <summary>
    /// 变更原因
    /// </summary>
    public string? ChangeReason { get; set; }

    /// <summary>
    /// 变更时间
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 变更人员
    /// </summary>
    public string? ChangedBy { get; set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    /// <summary>
    /// 审批人员
    /// </summary>
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// 审批时间
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// 审批备注
    /// </summary>
    public string? ApprovalNotes { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, object> ExtensionData { get; set; } = new();

    /// <summary>
    /// 导航属性：关联信用记录
    /// </summary>
    public CustomerCredit? Credit { get; set; }
}

/// <summary>
/// 逾期记录
/// </summary>
public class OverdueRecord
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
    /// 信用记录ID
    /// </summary>
    public string CreditId { get; set; } = string.Empty;

    /// <summary>
    /// 关联业务单据ID（订单、发票等）
    /// </summary>
    public string BusinessDocumentId { get; set; } = string.Empty;

    /// <summary>
    /// 业务单据类型
    /// </summary>
    public OverdueBusinessType BusinessType { get; set; }

    /// <summary>
    /// 应收金额
    /// </summary>
    public decimal ReceivableAmount { get; set; }

    /// <summary>
    /// 已收金额
    /// </summary>
    public decimal ReceivedAmount { get; set; }

    /// <summary>
    /// 逾期金额
    /// </summary>
    public decimal OverdueAmount => ReceivableAmount - ReceivedAmount;

    /// <summary>
    /// 应付日期
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// 逾期天数
    /// </summary>
    public int OverdueDays => (DateTime.UtcNow.Date - DueDate.Date).Days;

    /// <summary>
    /// 逾期等级
    /// </summary>
    public OverdueLevel OverdueLevel
    {
        get
        {
            return OverdueDays switch
            {
                <= 30 => OverdueLevel.Level1,
                <= 60 => OverdueLevel.Level2,
                <= 90 => OverdueLevel.Level3,
                <= 180 => OverdueLevel.Level4,
                _ => OverdueLevel.Level5
            };
        }
    }

    /// <summary>
    /// 处理状态
    /// </summary>
    public OverdueStatus Status { get; set; } = OverdueStatus.Active;

    /// <summary>
    /// 最后联系时间
    /// </summary>
    public DateTime? LastContactDate { get; set; }

    /// <summary>
    /// 预计收款日期
    /// </summary>
    public DateTime? ExpectedPaymentDate { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    public string? ProcessingNotes { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    public string? ResponsiblePerson { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 导航属性：关联信用记录
    /// </summary>
    public CustomerCredit? Credit { get; set; }
}

/// <summary>
/// 收款提醒记录
/// </summary>
public class CollectionReminder
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
    /// 信用记录ID
    /// </summary>
    public string CreditId { get; set; } = string.Empty;

    /// <summary>
    /// 逾期记录ID
    /// </summary>
    public string? OverdueRecordId { get; set; }

    /// <summary>
    /// 提醒类型
    /// </summary>
    public ReminderType ReminderType { get; set; }

    /// <summary>
    /// 提醒方式
    /// </summary>
    public ReminderMethod Method { get; set; }

    /// <summary>
    /// 提醒内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 计划发送时间
    /// </summary>
    public DateTime ScheduledTime { get; set; }

    /// <summary>
    /// 实际发送时间
    /// </summary>
    public DateTime? SentTime { get; set; }

    /// <summary>
    /// 发送状态
    /// </summary>
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;

    /// <summary>
    /// 发送结果
    /// </summary>
    public string? SendResult { get; set; }

    /// <summary>
    /// 接收人
    /// </summary>
    public string Recipient { get; set; } = string.Empty;

    /// <summary>
    /// 发送人
    /// </summary>
    public string? SentBy { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 下次重试时间
    /// </summary>
    public DateTime? NextRetryTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 导航属性：关联信用记录
    /// </summary>
    public CustomerCredit? Credit { get; set; }

    /// <summary>
    /// 导航属性：关联逾期记录
    /// </summary>
    public OverdueRecord? OverdueRecord { get; set; }
}

/// <summary>
/// 信用评估规则配置
/// </summary>
public class CreditAssessmentRule
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
    /// 目标信用等级
    /// </summary>
    public CreditRating TargetRating { get; set; }

    /// <summary>
    /// 最低信用评分要求
    /// </summary>
    public decimal MinCreditScore { get; set; }

    /// <summary>
    /// 最高信用评分要求
    /// </summary>
    public decimal MaxCreditScore { get; set; }

    /// <summary>
    /// 评分权重配置
    /// </summary>
    public CreditScoreWeights ScoreWeights { get; set; } = new();

    /// <summary>
    /// 风险阈值配置
    /// </summary>
    public RiskThresholds RiskThresholds { get; set; } = new();

    /// <summary>
    /// 规则优先级
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
/// 信用评分权重配置
/// </summary>
public class CreditScoreWeights
{
    /// <summary>
    /// 付款历史权重
    /// </summary>
    public decimal PaymentHistoryWeight { get; set; } = 0.35m;

    /// <summary>
    /// 信用额度使用率权重
    /// </summary>
    public decimal CreditUtilizationWeight { get; set; } = 0.30m;

    /// <summary>
    /// 合作历史长度权重
    /// </summary>
    public decimal CreditHistoryLengthWeight { get; set; } = 0.15m;

    /// <summary>
    /// 交易稳定性权重
    /// </summary>
    public decimal TransactionStabilityWeight { get; set; } = 0.10m;

    /// <summary>
    /// 财务状况权重
    /// </summary>
    public decimal FinancialConditionWeight { get; set; } = 0.10m;
}

/// <summary>
/// 风险阈值配置
/// </summary>
public class RiskThresholds
{
    /// <summary>
    /// 低风险阈值
    /// </summary>
    public decimal LowRiskThreshold { get; set; } = 750m;

    /// <summary>
    /// 中风险阈值
    /// </summary>
    public decimal MediumRiskThreshold { get; set; } = 500m;

    /// <summary>
    /// 高风险阈值
    /// </summary>
    public decimal HighRiskThreshold { get; set; } = 300m;

    /// <summary>
    /// 逾期天数预警阈值
    /// </summary>
    public int OverdueDaysWarningThreshold { get; set; } = 30;

    /// <summary>
    /// 信用额度使用率预警阈值
    /// </summary>
    public decimal CreditUtilizationWarningThreshold { get; set; } = 80m;
}

// =================== 枚举定义 ===================

/// <summary>
/// 信用等级
/// </summary>
public enum CreditRating
{
    /// <summary>
    /// AAA级 - 信用极好
    /// </summary>
    AAA = 1,

    /// <summary>
    /// AA级 - 信用优良
    /// </summary>
    AA = 2,

    /// <summary>
    /// A级 - 信用良好
    /// </summary>
    A = 3,

    /// <summary>
    /// BBB级 - 信用一般
    /// </summary>
    BBB = 4,

    /// <summary>
    /// BB级 - 信用较差
    /// </summary>
    BB = 5,

    /// <summary>
    /// B级 - 信用差
    /// </summary>
    B = 6,

    /// <summary>
    /// C级 - 信用极差
    /// </summary>
    C = 7,

    /// <summary>
    /// D级 - 违约
    /// </summary>
    D = 8
}

/// <summary>
/// 信用状态
/// </summary>
public enum CreditStatus
{
    /// <summary>
    /// 正常
    /// </summary>
    Normal = 1,

    /// <summary>
    /// 观察中
    /// </summary>
    Monitoring = 2,

    /// <summary>
    /// 预警
    /// </summary>
    Warning = 3,

    /// <summary>
    /// 冻结
    /// </summary>
    Frozen = 4,

    /// <summary>
    /// 黑名单
    /// </summary>
    Blacklisted = 5
}

/// <summary>
/// 风险等级
/// </summary>
public enum RiskLevel
{
    /// <summary>
    /// 低风险
    /// </summary>
    Low = 1,

    /// <summary>
    /// 中风险
    /// </summary>
    Medium = 2,

    /// <summary>
    /// 高风险
    /// </summary>
    High = 3,

    /// <summary>
    /// 极高风险
    /// </summary>
    Critical = 4
}

/// <summary>
/// 担保类型
/// </summary>
public enum GuaranteeType
{
    /// <summary>
    /// 个人担保
    /// </summary>
    Personal = 1,

    /// <summary>
    /// 企业担保
    /// </summary>
    Corporate = 2,

    /// <summary>
    /// 银行担保
    /// </summary>
    Bank = 3,

    /// <summary>
    /// 保险担保
    /// </summary>
    Insurance = 4,

    /// <summary>
    /// 抵押担保
    /// </summary>
    Mortgage = 5
}

/// <summary>
/// 担保状态
/// </summary>
public enum GuaranteeStatus
{
    /// <summary>
    /// 生效中
    /// </summary>
    Active = 1,

    /// <summary>
    /// 已到期
    /// </summary>
    Expired = 2,

    /// <summary>
    /// 已终止
    /// </summary>
    Terminated = 3,

    /// <summary>
    /// 已执行
    /// </summary>
    Executed = 4
}

/// <summary>
/// 信用变更类型
/// </summary>
public enum CreditChangeType
{
    /// <summary>
    /// 信用评级调整
    /// </summary>
    RatingAdjustment = 1,

    /// <summary>
    /// 信用额度调整
    /// </summary>
    LimitAdjustment = 2,

    /// <summary>
    /// 信用冻结
    /// </summary>
    CreditFreeze = 3,

    /// <summary>
    /// 信用解冻
    /// </summary>
    CreditUnfreeze = 4,

    /// <summary>
    /// 付款条件调整
    /// </summary>
    PaymentTermsAdjustment = 5,

    /// <summary>
    /// 风险等级调整
    /// </summary>
    RiskLevelAdjustment = 6,

    /// <summary>
    /// 评估重算
    /// </summary>
    Reassessment = 7
}

/// <summary>
/// 审批状态
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// 待审批
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已批准
    /// </summary>
    Approved = 2,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// 已撤回
    /// </summary>
    Withdrawn = 4
}

/// <summary>
/// 逾期业务类型
/// </summary>
public enum OverdueBusinessType
{
    /// <summary>
    /// 销售订单
    /// </summary>
    SalesOrder = 1,

    /// <summary>
    /// 发票
    /// </summary>
    Invoice = 2,

    /// <summary>
    /// 应收账款
    /// </summary>
    AccountReceivable = 3,

    /// <summary>
    /// 其他
    /// </summary>
    Other = 99
}

/// <summary>
/// 逾期等级
/// </summary>
public enum OverdueLevel
{
    /// <summary>
    /// 一级逾期（1-30天）
    /// </summary>
    Level1 = 1,

    /// <summary>
    /// 二级逾期（31-60天）
    /// </summary>
    Level2 = 2,

    /// <summary>
    /// 三级逾期（61-90天）
    /// </summary>
    Level3 = 3,

    /// <summary>
    /// 四级逾期（91-180天）
    /// </summary>
    Level4 = 4,

    /// <summary>
    /// 五级逾期（180天以上）
    /// </summary>
    Level5 = 5
}

/// <summary>
/// 逾期处理状态
/// </summary>
public enum OverdueStatus
{
    /// <summary>
    /// 活跃逾期
    /// </summary>
    Active = 1,

    /// <summary>
    /// 跟进中
    /// </summary>
    Following = 2,

    /// <summary>
    /// 协商中
    /// </summary>
    Negotiating = 3,

    /// <summary>
    /// 法务处理
    /// </summary>
    Legal = 4,

    /// <summary>
    /// 已结清
    /// </summary>
    Settled = 5,

    /// <summary>
    /// 坏账处理
    /// </summary>
    BadDebt = 6
}

/// <summary>
/// 提醒类型
/// </summary>
public enum ReminderType
{
    /// <summary>
    /// 到期提醒
    /// </summary>
    DueReminder = 1,

    /// <summary>
    /// 逾期提醒
    /// </summary>
    OverdueReminder = 2,

    /// <summary>
    /// 催收提醒
    /// </summary>
    CollectionReminder = 3,

    /// <summary>
    /// 最终通知
    /// </summary>
    FinalNotice = 4
}

/// <summary>
/// 提醒方式
/// </summary>
public enum ReminderMethod
{
    /// <summary>
    /// 邮件
    /// </summary>
    Email = 1,

    /// <summary>
    /// 短信
    /// </summary>
    SMS = 2,

    /// <summary>
    /// 电话
    /// </summary>
    Phone = 3,

    /// <summary>
    /// 系统通知
    /// </summary>
    SystemNotification = 4,

    /// <summary>
    /// 微信
    /// </summary>
    WeChat = 5
}

/// <summary>
/// 提醒状态
/// </summary>
public enum ReminderStatus
{
    /// <summary>
    /// 待发送
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已发送
    /// </summary>
    Sent = 2,

    /// <summary>
    /// 发送失败
    /// </summary>
    Failed = 3,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// 重试中
    /// </summary>
    Retrying = 5
}