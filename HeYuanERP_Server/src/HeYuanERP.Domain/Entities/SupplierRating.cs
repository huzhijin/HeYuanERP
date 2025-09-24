namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 供应商评级记录 - 记录供应商的等级信息和绩效评估
/// </summary>
public class SupplierRating
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 供应商ID
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 供应商等级
    /// </summary>
    public SupplierLevel CurrentLevel { get; set; } = SupplierLevel.Standard;

    /// <summary>
    /// 评级方式
    /// </summary>
    public RatingMethod Method { get; set; } = RatingMethod.Automatic;

    /// <summary>
    /// 评级生效日期
    /// </summary>
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 评级到期日期（可选，用于临时评级）
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// 评级原因/备注
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 综合评分（0-100分）
    /// </summary>
    public decimal OverallScore { get; set; }

    /// <summary>
    /// 各维度评分详情
    /// </summary>
    public SupplierScoreDetails ScoreDetails { get; set; } = new();

    /// <summary>
    /// 绩效指标
    /// </summary>
    public SupplierPerformanceMetrics PerformanceMetrics { get; set; } = new();

    /// <summary>
    /// 风险评估结果
    /// </summary>
    public SupplierRiskAssessment RiskAssessment { get; set; } = new();

    /// <summary>
    /// 改进计划
    /// </summary>
    public string? ImprovementPlan { get; set; }

    /// <summary>
    /// 是否当前有效
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 是否推荐供应商
    /// </summary>
    public bool IsRecommended { get; set; }

    /// <summary>
    /// 黑名单状态
    /// </summary>
    public bool IsBlacklisted { get; set; }

    /// <summary>
    /// 黑名单原因
    /// </summary>
    public string? BlacklistReason { get; set; }

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
    /// 导航属性：关联供应商
    /// </summary>
    public Account? Supplier { get; set; }

    /// <summary>
    /// 导航属性：评级历史记录
    /// </summary>
    public List<SupplierRatingHistory> History { get; set; } = new();

    /// <summary>
    /// 导航属性：评估记录
    /// </summary>
    public List<SupplierEvaluation> Evaluations { get; set; } = new();
}

/// <summary>
/// 供应商评分详情
/// </summary>
public class SupplierScoreDetails
{
    /// <summary>
    /// 质量评分（25%权重）
    /// </summary>
    public decimal QualityScore { get; set; }

    /// <summary>
    /// 交付评分（25%权重）
    /// </summary>
    public decimal DeliveryScore { get; set; }

    /// <summary>
    /// 成本竞争力评分（20%权重）
    /// </summary>
    public decimal CostCompetitivenessScore { get; set; }

    /// <summary>
    /// 服务评分（15%权重）
    /// </summary>
    public decimal ServiceScore { get; set; }

    /// <summary>
    /// 技术能力评分（10%权重）
    /// </summary>
    public decimal TechnicalCapabilityScore { get; set; }

    /// <summary>
    /// 合规性评分（5%权重）
    /// </summary>
    public decimal ComplianceScore { get; set; }

    /// <summary>
    /// 计算加权总分
    /// </summary>
    public decimal CalculateWeightedScore()
    {
        return QualityScore * 0.25m +
               DeliveryScore * 0.25m +
               CostCompetitivenessScore * 0.20m +
               ServiceScore * 0.15m +
               TechnicalCapabilityScore * 0.10m +
               ComplianceScore * 0.05m;
    }
}

/// <summary>
/// 供应商绩效指标
/// </summary>
public class SupplierPerformanceMetrics
{
    /// <summary>
    /// 统计期间开始日期
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// 统计期间结束日期
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// 准时交货率（%）
    /// </summary>
    public decimal OnTimeDeliveryRate { get; set; }

    /// <summary>
    /// 质量合格率（%）
    /// </summary>
    public decimal QualityAcceptanceRate { get; set; }

    /// <summary>
    /// 订单完成率（%）
    /// </summary>
    public decimal OrderFulfillmentRate { get; set; }

    /// <summary>
    /// 平均交货周期（天）
    /// </summary>
    public decimal AverageLeadTime { get; set; }

    /// <summary>
    /// 缺陷率（PPM）
    /// </summary>
    public decimal DefectRatePPM { get; set; }

    /// <summary>
    /// 退货率（%）
    /// </summary>
    public decimal ReturnRate { get; set; }

    /// <summary>
    /// 响应时间（小时）
    /// </summary>
    public decimal ResponseTime { get; set; }

    /// <summary>
    /// 价格竞争力指数
    /// </summary>
    public decimal PriceCompetitivenessIndex { get; set; }

    /// <summary>
    /// 成本节约金额
    /// </summary>
    public decimal CostSavingAmount { get; set; }

    /// <summary>
    /// 创新贡献次数
    /// </summary>
    public int InnovationContributions { get; set; }

    /// <summary>
    /// 投诉次数
    /// </summary>
    public int ComplaintCount { get; set; }

    /// <summary>
    /// 合作项目数
    /// </summary>
    public int CollaborationProjects { get; set; }

    /// <summary>
    /// 培训参与次数
    /// </summary>
    public int TrainingParticipations { get; set; }

    /// <summary>
    /// 审核通过次数
    /// </summary>
    public int AuditPasses { get; set; }

    /// <summary>
    /// 认证证书数量
    /// </summary>
    public int CertificationCount { get; set; }
}

/// <summary>
/// 供应商风险评估
/// </summary>
public class SupplierRiskAssessment
{
    /// <summary>
    /// 总体风险等级
    /// </summary>
    public SupplierRiskLevel OverallRiskLevel { get; set; } = SupplierRiskLevel.Medium;

    /// <summary>
    /// 财务风险等级
    /// </summary>
    public RiskLevel FinancialRisk { get; set; } = RiskLevel.Medium;

    /// <summary>
    /// 运营风险等级
    /// </summary>
    public RiskLevel OperationalRisk { get; set; } = RiskLevel.Medium;

    /// <summary>
    /// 供应链风险等级
    /// </summary>
    public RiskLevel SupplyChainRisk { get; set; } = RiskLevel.Medium;

    /// <summary>
    /// 合规风险等级
    /// </summary>
    public RiskLevel ComplianceRisk { get; set; } = RiskLevel.Medium;

    /// <summary>
    /// 地理位置风险等级
    /// </summary>
    public RiskLevel GeographicalRisk { get; set; } = RiskLevel.Medium;

    /// <summary>
    /// 技术风险等级
    /// </summary>
    public RiskLevel TechnicalRisk { get; set; } = RiskLevel.Medium;

    /// <summary>
    /// 风险因子列表
    /// </summary>
    public List<RiskFactor> RiskFactors { get; set; } = new();

    /// <summary>
    /// 缓解措施
    /// </summary>
    public List<string> MitigationMeasures { get; set; } = new();

    /// <summary>
    /// 监控要点
    /// </summary>
    public List<string> MonitoringPoints { get; set; } = new();

    /// <summary>
    /// 风险评估日期
    /// </summary>
    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 下次评估日期
    /// </summary>
    public DateTime NextAssessmentDate { get; set; } = DateTime.UtcNow.AddMonths(6);
}

/// <summary>
/// 风险因子
/// </summary>
public class RiskFactor
{
    /// <summary>
    /// 风险类型
    /// </summary>
    public RiskFactorType Type { get; set; }

    /// <summary>
    /// 风险描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 风险等级
    /// </summary>
    public RiskLevel Level { get; set; }

    /// <summary>
    /// 影响程度（1-5）
    /// </summary>
    public int Impact { get; set; }

    /// <summary>
    /// 发生概率（1-5）
    /// </summary>
    public int Probability { get; set; }

    /// <summary>
    /// 风险值（影响程度 × 发生概率）
    /// </summary>
    public int RiskValue => Impact * Probability;
}

/// <summary>
/// 供应商评级历史记录
/// </summary>
public class SupplierRatingHistory
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 供应商ID
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 评级记录ID
    /// </summary>
    public string RatingId { get; set; } = string.Empty;

    /// <summary>
    /// 变更类型
    /// </summary>
    public RatingChangeType ChangeType { get; set; }

    /// <summary>
    /// 原等级
    /// </summary>
    public SupplierLevel? FromLevel { get; set; }

    /// <summary>
    /// 新等级
    /// </summary>
    public SupplierLevel ToLevel { get; set; }

    /// <summary>
    /// 原评分
    /// </summary>
    public decimal? FromScore { get; set; }

    /// <summary>
    /// 新评分
    /// </summary>
    public decimal ToScore { get; set; }

    /// <summary>
    /// 变更原因
    /// </summary>
    public string? ChangeReason { get; set; }

    /// <summary>
    /// 变更方式
    /// </summary>
    public RatingMethod Method { get; set; }

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
    /// 导航属性：关联评级记录
    /// </summary>
    public SupplierRating? Rating { get; set; }
}

/// <summary>
/// 供应商评估记录
/// </summary>
public class SupplierEvaluation
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 供应商ID
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 评估类型
    /// </summary>
    public EvaluationType Type { get; set; }

    /// <summary>
    /// 评估标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 评估描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 评估期间开始日期
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// 评估期间结束日期
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// 评估状态
    /// </summary>
    public EvaluationStatus Status { get; set; } = EvaluationStatus.Draft;

    /// <summary>
    /// 评估模板ID
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// 评估项目列表
    /// </summary>
    public List<EvaluationItem> Items { get; set; } = new();

    /// <summary>
    /// 总评分
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// 最大分数
    /// </summary>
    public decimal MaxScore { get; set; }

    /// <summary>
    /// 得分率（%）
    /// </summary>
    public decimal ScoreRate => MaxScore > 0 ? (TotalScore / MaxScore) * 100 : 0;

    /// <summary>
    /// 评估结论
    /// </summary>
    public string? Conclusion { get; set; }

    /// <summary>
    /// 改进建议
    /// </summary>
    public List<string> Recommendations { get; set; } = new();

    /// <summary>
    /// 跟进行动计划
    /// </summary>
    public List<ActionPlan> ActionPlans { get; set; } = new();

    /// <summary>
    /// 评估人员
    /// </summary>
    public string? EvaluatedBy { get; set; }

    /// <summary>
    /// 评估时间
    /// </summary>
    public DateTime? EvaluatedAt { get; set; }

    /// <summary>
    /// 审核人员
    /// </summary>
    public string? ReviewedBy { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// 审核意见
    /// </summary>
    public string? ReviewComments { get; set; }

    /// <summary>
    /// 附件URL列表
    /// </summary>
    public List<string> AttachmentUrls { get; set; } = new();

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
    /// 导航属性：关联供应商
    /// </summary>
    public Account? Supplier { get; set; }
}

/// <summary>
/// 评估项目
/// </summary>
public class EvaluationItem
{
    /// <summary>
    /// 项目ID
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 评估维度
    /// </summary>
    public EvaluationDimension Dimension { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 项目描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 权重（0-1）
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 最大分数
    /// </summary>
    public decimal MaxScore { get; set; }

    /// <summary>
    /// 实际得分
    /// </summary>
    public decimal ActualScore { get; set; }

    /// <summary>
    /// 评分标准
    /// </summary>
    public string? ScoringCriteria { get; set; }

    /// <summary>
    /// 评估方法
    /// </summary>
    public EvaluationMethod Method { get; set; }

    /// <summary>
    /// 数据来源
    /// </summary>
    public string? DataSource { get; set; }

    /// <summary>
    /// 评估备注
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 证据文件
    /// </summary>
    public List<string> EvidenceFiles { get; set; } = new();

    /// <summary>
    /// 改进建议
    /// </summary>
    public string? ImprovementSuggestion { get; set; }
}

/// <summary>
/// 行动计划
/// </summary>
public class ActionPlan
{
    /// <summary>
    /// 计划ID
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 问题描述
    /// </summary>
    public string Issue { get; set; } = string.Empty;

    /// <summary>
    /// 改进措施
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 负责人
    /// </summary>
    public string ResponsiblePerson { get; set; } = string.Empty;

    /// <summary>
    /// 预期完成日期
    /// </summary>
    public DateTime ExpectedCompletionDate { get; set; }

    /// <summary>
    /// 执行状态
    /// </summary>
    public ActionStatus Status { get; set; } = ActionStatus.Planned;

    /// <summary>
    /// 完成日期
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// 完成说明
    /// </summary>
    public string? CompletionNotes { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public ActionPriority Priority { get; set; } = ActionPriority.Medium;
}

/// <summary>
/// 供应商评级规则配置
/// </summary>
public class SupplierRatingRule
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
    public SupplierLevel TargetLevel { get; set; }

    /// <summary>
    /// 最低评分要求
    /// </summary>
    public decimal MinScore { get; set; }

    /// <summary>
    /// 最高评分要求
    /// </summary>
    public decimal MaxScore { get; set; }

    /// <summary>
    /// 评分权重配置
    /// </summary>
    public SupplierScoreWeights ScoreWeights { get; set; } = new();

    /// <summary>
    /// 必要条件
    /// </summary>
    public List<SupplierRequirement> Requirements { get; set; } = new();

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
/// 供应商评分权重配置
/// </summary>
public class SupplierScoreWeights
{
    /// <summary>
    /// 质量权重
    /// </summary>
    public decimal QualityWeight { get; set; } = 0.25m;

    /// <summary>
    /// 交付权重
    /// </summary>
    public decimal DeliveryWeight { get; set; } = 0.25m;

    /// <summary>
    /// 成本竞争力权重
    /// </summary>
    public decimal CostCompetitivenessWeight { get; set; } = 0.20m;

    /// <summary>
    /// 服务权重
    /// </summary>
    public decimal ServiceWeight { get; set; } = 0.15m;

    /// <summary>
    /// 技术能力权重
    /// </summary>
    public decimal TechnicalCapabilityWeight { get; set; } = 0.10m;

    /// <summary>
    /// 合规性权重
    /// </summary>
    public decimal ComplianceWeight { get; set; } = 0.05m;
}

/// <summary>
/// 供应商要求
/// </summary>
public class SupplierRequirement
{
    /// <summary>
    /// 要求类型
    /// </summary>
    public RequirementType Type { get; set; }

    /// <summary>
    /// 要求描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 是否必须满足
    /// </summary>
    public bool IsMandatory { get; set; }

    /// <summary>
    /// 验证方法
    /// </summary>
    public string? VerificationMethod { get; set; }

    /// <summary>
    /// 最小值要求
    /// </summary>
    public decimal? MinValue { get; set; }

    /// <summary>
    /// 证书要求
    /// </summary>
    public List<string> RequiredCertifications { get; set; } = new();
}

// =================== 枚举定义 ===================

/// <summary>
/// 供应商等级
/// </summary>
public enum SupplierLevel
{
    /// <summary>
    /// 战略供应商
    /// </summary>
    Strategic = 1,

    /// <summary>
    /// 优选供应商
    /// </summary>
    Preferred = 2,

    /// <summary>
    /// 合格供应商
    /// </summary>
    Qualified = 3,

    /// <summary>
    /// 标准供应商
    /// </summary>
    Standard = 4,

    /// <summary>
    /// 观察供应商
    /// </summary>
    Observation = 5,

    /// <summary>
    /// 不合格供应商
    /// </summary>
    Unqualified = 6
}

/// <summary>
/// 评级方式
/// </summary>
public enum RatingMethod
{
    /// <summary>
    /// 自动评级
    /// </summary>
    Automatic = 1,

    /// <summary>
    /// 手动评级
    /// </summary>
    Manual = 2,

    /// <summary>
    /// 混合评级
    /// </summary>
    Hybrid = 3,

    /// <summary>
    /// 系统导入
    /// </summary>
    SystemImport = 4
}

/// <summary>
/// 评级变更类型
/// </summary>
public enum RatingChangeType
{
    /// <summary>
    /// 新建评级
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
    /// 定期重评
    /// </summary>
    PeriodicReview = 5,

    /// <summary>
    /// 风险调整
    /// </summary>
    RiskAdjustment = 6,

    /// <summary>
    /// 临时调整
    /// </summary>
    TemporaryAdjustment = 7
}

/// <summary>
/// 供应商风险等级
/// </summary>
public enum SupplierRiskLevel
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
/// 风险因子类型
/// </summary>
public enum RiskFactorType
{
    /// <summary>
    /// 财务风险
    /// </summary>
    Financial = 1,

    /// <summary>
    /// 运营风险
    /// </summary>
    Operational = 2,

    /// <summary>
    /// 供应链风险
    /// </summary>
    SupplyChain = 3,

    /// <summary>
    /// 合规风险
    /// </summary>
    Compliance = 4,

    /// <summary>
    /// 地理风险
    /// </summary>
    Geographical = 5,

    /// <summary>
    /// 技术风险
    /// </summary>
    Technical = 6,

    /// <summary>
    /// 环境风险
    /// </summary>
    Environmental = 7,

    /// <summary>
    /// 社会责任风险
    /// </summary>
    SocialResponsibility = 8
}

/// <summary>
/// 评估类型
/// </summary>
public enum EvaluationType
{
    /// <summary>
    /// 入围评估
    /// </summary>
    PreQualification = 1,

    /// <summary>
    /// 定期评估
    /// </summary>
    Periodic = 2,

    /// <summary>
    /// 专项评估
    /// </summary>
    Special = 3,

    /// <summary>
    /// 年度评估
    /// </summary>
    Annual = 4,

    /// <summary>
    /// 项目评估
    /// </summary>
    Project = 5,

    /// <summary>
    /// 审计评估
    /// </summary>
    Audit = 6
}

/// <summary>
/// 评估状态
/// </summary>
public enum EvaluationStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 进行中
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// 待审核
    /// </summary>
    PendingReview = 3,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 4,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 5
}

/// <summary>
/// 评估维度
/// </summary>
public enum EvaluationDimension
{
    /// <summary>
    /// 质量管理
    /// </summary>
    Quality = 1,

    /// <summary>
    /// 交付能力
    /// </summary>
    Delivery = 2,

    /// <summary>
    /// 成本竞争力
    /// </summary>
    Cost = 3,

    /// <summary>
    /// 服务能力
    /// </summary>
    Service = 4,

    /// <summary>
    /// 技术能力
    /// </summary>
    Technology = 5,

    /// <summary>
    /// 合规性
    /// </summary>
    Compliance = 6,

    /// <summary>
    /// 财务状况
    /// </summary>
    Financial = 7,

    /// <summary>
    /// 管理体系
    /// </summary>
    Management = 8
}

/// <summary>
/// 评估方法
/// </summary>
public enum EvaluationMethod
{
    /// <summary>
    /// 数据分析
    /// </summary>
    DataAnalysis = 1,

    /// <summary>
    /// 现场审核
    /// </summary>
    OnSiteAudit = 2,

    /// <summary>
    /// 文件审查
    /// </summary>
    DocumentReview = 3,

    /// <summary>
    /// 问卷调查
    /// </summary>
    Survey = 4,

    /// <summary>
    /// 第三方认证
    /// </summary>
    ThirdPartyCertification = 5,

    /// <summary>
    /// 客户反馈
    /// </summary>
    CustomerFeedback = 6
}

/// <summary>
/// 行动状态
/// </summary>
public enum ActionStatus
{
    /// <summary>
    /// 计划中
    /// </summary>
    Planned = 1,

    /// <summary>
    /// 进行中
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3,

    /// <summary>
    /// 已延期
    /// </summary>
    Delayed = 4,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 5
}

/// <summary>
/// 行动优先级
/// </summary>
public enum ActionPriority
{
    /// <summary>
    /// 低
    /// </summary>
    Low = 1,

    /// <summary>
    /// 中
    /// </summary>
    Medium = 2,

    /// <summary>
    /// 高
    /// </summary>
    High = 3,

    /// <summary>
    /// 紧急
    /// </summary>
    Urgent = 4
}

/// <summary>
/// 要求类型
/// </summary>
public enum RequirementType
{
    /// <summary>
    /// 绩效要求
    /// </summary>
    Performance = 1,

    /// <summary>
    /// 认证要求
    /// </summary>
    Certification = 2,

    /// <summary>
    /// 财务要求
    /// </summary>
    Financial = 3,

    /// <summary>
    /// 质量要求
    /// </summary>
    Quality = 4,

    /// <summary>
    /// 环保要求
    /// </summary>
    Environmental = 5,

    /// <summary>
    /// 社会责任要求
    /// </summary>
    SocialResponsibility = 6
}