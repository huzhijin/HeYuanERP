using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("CustomerValueAnalyses")]
public class CustomerValueAnalysis
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    [Required]
    public DateTime AnalysisDate { get; set; }

    [Required]
    [StringLength(50)]
    public string AnalysisPeriod { get; set; } = string.Empty; // Monthly, Quarterly, Yearly

    // 客户生命周期价值 (CLV)
    [Column(TypeName = "decimal(18,2)")]
    public decimal CustomerLifetimeValue { get; set; }

    // 历史交易价值
    [Column(TypeName = "decimal(18,2)")]
    public decimal HistoricalTransactionValue { get; set; }

    // 平均订单价值 (AOV)
    [Column(TypeName = "decimal(18,2)")]
    public decimal AverageOrderValue { get; set; }

    // 购买频率
    [Column(TypeName = "decimal(8,2)")]
    public decimal PurchaseFrequency { get; set; }

    // 客户获取成本 (CAC)
    [Column(TypeName = "decimal(18,2)")]
    public decimal CustomerAcquisitionCost { get; set; }

    // 客户保留率
    [Column(TypeName = "decimal(5,2)")]
    public decimal RetentionRate { get; set; }

    // 流失风险评分 (0-100)
    [Range(0, 100)]
    public int ChurnRiskScore { get; set; }

    // 交叉销售潜力
    [Range(0, 100)]
    public int CrossSellPotential { get; set; }

    // 向上销售潜力
    [Range(0, 100)]
    public int UpsellPotential { get; set; }

    // 推荐潜力
    [Range(0, 100)]
    public int ReferralPotential { get; set; }

    // 客户满意度分数
    [Range(0, 100)]
    public int SatisfactionScore { get; set; }

    // NPS净推荐值
    [Range(-100, 100)]
    public int NetPromoterScore { get; set; }

    // 客户细分
    [StringLength(50)]
    public string CustomerSegment { get; set; } = string.Empty; // VIP, Premium, Standard, Basic

    // 客户价值等级 (A/B/C/D)
    [StringLength(10)]
    public string ValueGrade { get; set; } = string.Empty;

    // 客户成熟度
    [StringLength(50)]
    public string CustomerMaturity { get; set; } = string.Empty; // New, Growing, Mature, Declining

    // 行业影响力
    [Range(0, 100)]
    public int IndustryInfluence { get; set; }

    // 付款行为评分
    [Range(0, 100)]
    public int PaymentBehaviorScore { get; set; }

    // 合作年限
    [Column(TypeName = "decimal(4,1)")]
    public decimal CooperationYears { get; set; }

    // 产品多样性（购买产品种类数）
    public int ProductDiversity { get; set; }

    // 地理重要性
    [Range(0, 100)]
    public int GeographicImportance { get; set; }

    // 战略重要性
    [Range(0, 100)]
    public int StrategicImportance { get; set; }

    // 沟通偏好
    [StringLength(100)]
    public string CommunicationPreference { get; set; } = string.Empty; // Email, Phone, Face-to-face, Digital

    // 决策周期（天）
    public int DecisionCycleDays { get; set; }

    // 价格敏感度
    [Range(0, 100)]
    public int PriceSensitivity { get; set; }

    // 服务需求等级
    [StringLength(50)]
    public string ServiceRequirementLevel { get; set; } = string.Empty; // High, Medium, Low

    // 增长潜力
    [Column(TypeName = "decimal(18,2)")]
    public decimal GrowthPotential { get; set; }

    // 市场份额占比
    [Column(TypeName = "decimal(5,2)")]
    public decimal MarketSharePercentage { get; set; }

    [StringLength(1000)]
    public string AnalysisNotes { get; set; } = string.Empty;

    [StringLength(1000)]
    public string RecommendedActions { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string AnalyzedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 下次分析计划时间
    public DateTime? NextAnalysisDate { get; set; }

    // 分析模型版本
    [StringLength(20)]
    public string AnalysisModelVersion { get; set; } = "1.0";
}