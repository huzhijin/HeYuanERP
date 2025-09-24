using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("PriceStrategies")]
public class PriceStrategy
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string StrategyName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string StrategyCode { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string StrategyType { get; set; } = string.Empty; // Fixed, Percentage, Formula, Tiered, Customer-specific

    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Draft

    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // 适用范围
    [StringLength(100)]
    public string ApplicableCustomerType { get; set; } = string.Empty; // All, VIP, Corporate, Retail等

    [StringLength(100)]
    public string ApplicableProductCategory { get; set; } = string.Empty; // 适用产品分类

    [StringLength(100)]
    public string ApplicableRegion { get; set; } = string.Empty; // 适用地区

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinOrderAmount { get; set; } // 最小订单金额

    [Column(TypeName = "decimal(10,2)")]
    public decimal MinOrderQuantity { get; set; } // 最小订单数量

    // 价格规则
    [StringLength(50)]
    public string PriceRule { get; set; } = string.Empty; // Markup, Discount, Fixed

    [Column(TypeName = "decimal(8,4)")]
    public decimal PriceValue { get; set; } // 价格值（百分比或固定值）

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; } // 基准价格

    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxDiscountAmount { get; set; } // 最大折扣金额

    [Column(TypeName = "decimal(5,2)")]
    public decimal MaxDiscountPercent { get; set; } // 最大折扣百分比

    // 阶梯价格规则（JSON格式）
    [StringLength(2000)]
    public string TieredPriceRules { get; set; } = string.Empty;

    // 客户特定价格（JSON格式）
    [StringLength(2000)]
    public string CustomerSpecificPrices { get; set; } = string.Empty;

    // 产品特定价格（JSON格式）
    [StringLength(2000)]
    public string ProductSpecificPrices { get; set; } = string.Empty;

    // 条件规则
    [StringLength(50)]
    public string PaymentTerm { get; set; } = string.Empty; // 付款条件

    [StringLength(50)]
    public string DeliveryTerm { get; set; } = string.Empty; // 交货条件

    public bool RequireApproval { get; set; } = false; // 是否需要审批

    public int? ApproverUserId { get; set; } // 审批人

    [StringLength(100)]
    public string ApproverUserName { get; set; } = string.Empty;

    // 促销信息
    public bool IsPromotional { get; set; } = false;

    [StringLength(200)]
    public string PromotionalMessage { get; set; } = string.Empty;

    [StringLength(100)]
    public string PromotionalCode { get; set; } = string.Empty;

    // 组合销售规则
    public bool AllowCombination { get; set; } = true; // 是否允许与其他策略组合

    [StringLength(500)]
    public string CombinationRules { get; set; } = string.Empty;

    // 优先级
    public int Priority { get; set; } = 1; // 优先级，数字越大优先级越高

    // 使用限制
    public int? MaxUsageCount { get; set; } // 最大使用次数

    public int CurrentUsageCount { get; set; } // 当前使用次数

    public int? MaxUsagePerCustomer { get; set; } // 每客户最大使用次数

    // 时间限制
    [StringLength(100)]
    public string TimeRestriction { get; set; } = string.Empty; // 时间限制（如工作日、周末等）

    [StringLength(100)]
    public string SeasonalRestriction { get; set; } = string.Empty; // 季节性限制

    // 统计信息
    public int UsageCount { get; set; } // 使用次数

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalSavingsAmount { get; set; } // 总节省金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalOrderAmount { get; set; } // 总订单金额

    public DateTime? LastUsedDate { get; set; } // 最后使用时间

    // 性能指标
    [Column(TypeName = "decimal(5,2)")]
    public decimal ConversionRate { get; set; } // 转换率

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProfitMarginImpact { get; set; } // 利润率影响

    [Column(TypeName = "decimal(5,2)")]
    public decimal CustomerSatisfactionScore { get; set; } // 客户满意度

    // 自动化规则
    public bool AutoApply { get; set; } = false; // 是否自动应用

    [StringLength(1000)]
    public string AutoApplyConditions { get; set; } = string.Empty; // 自动应用条件

    // 备注和审计
    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 关联实体
    public virtual ICollection<PriceRequest> PriceRequests { get; set; } = new List<PriceRequest>();

    // 计算属性
    public bool IsActive => Status == "Active" &&
                           DateTime.UtcNow >= EffectiveDate &&
                           (ExpiryDate == null || DateTime.UtcNow <= ExpiryDate);

    public bool IsExpired => ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value;

    public decimal EffectiveDiscountRate => PriceRule == "Discount" ? PriceValue : 0;

    public decimal EffectiveMarkupRate => PriceRule == "Markup" ? PriceValue : 0;
}