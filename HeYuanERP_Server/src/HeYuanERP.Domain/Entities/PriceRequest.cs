using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("PriceRequests")]
public class PriceRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string RequestCode { get; set; } = string.Empty; // 申请编号

    [Required]
    [StringLength(50)]
    public string RequestType { get; set; } = string.Empty; // Special, Discount, Quotation, Bulk

    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Expired

    [Required]
    public int RequesterId { get; set; } // 申请人ID

    [StringLength(100)]
    public string RequesterName { get; set; } = string.Empty;

    [StringLength(100)]
    public string RequesterDepartment { get; set; } = string.Empty;

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    public DateTime? ResponseDate { get; set; }

    // 客户信息
    [Required]
    public int CustomerId { get; set; }

    [StringLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(100)]
    public string CustomerType { get; set; } = string.Empty; // VIP, Corporate, Retail等

    [StringLength(100)]
    public string CustomerRegion { get; set; } = string.Empty;

    // 产品信息
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(100)]
    public string ProductCode { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal RequestQuantity { get; set; } // 申请数量

    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;

    // 价格信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal StandardPrice { get; set; } // 标准价格

    [Column(TypeName = "decimal(18,2)")]
    public decimal RequestedPrice { get; set; } // 申请价格

    [Column(TypeName = "decimal(18,2)")]
    public decimal ApprovedPrice { get; set; } // 批准价格

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountRate { get; set; } // 折扣率

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } // 折扣金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } // 总金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedCost { get; set; } // 预估成本

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProfitMargin { get; set; } // 利润率

    // 申请理由和条件
    [Required]
    [StringLength(1000)]
    public string RequestReason { get; set; } = string.Empty; // 申请理由

    [StringLength(500)]
    public string BusinessJustification { get; set; } = string.Empty; // 商业理由

    [StringLength(500)]
    public string CompetitorInfo { get; set; } = string.Empty; // 竞争对手信息

    [StringLength(100)]
    public string PaymentTerm { get; set; } = string.Empty; // 付款条件

    [StringLength(100)]
    public string DeliveryTerm { get; set; } = string.Empty; // 交货条件

    public DateTime? ValidUntil { get; set; } // 有效期

    // 审批信息
    public int? ApproverId { get; set; } // 审批人ID

    [StringLength(100)]
    public string ApproverName { get; set; } = string.Empty;

    [StringLength(100)]
    public string ApproverDepartment { get; set; } = string.Empty;

    [StringLength(1000)]
    public string ApprovalComments { get; set; } = string.Empty; // 审批意见

    [StringLength(500)]
    public string RejectionReason { get; set; } = string.Empty; // 拒绝原因

    public bool RequiresHigherApproval { get; set; } = false; // 是否需要更高级审批

    public int? HigherApproverId { get; set; } // 更高级审批人ID

    [StringLength(100)]
    public string HigherApproverName { get; set; } = string.Empty;

    // 相关文档
    [StringLength(500)]
    public string AttachmentUrls { get; set; } = string.Empty; // 附件链接（JSON格式）

    [StringLength(200)]
    public string ReferenceOrderNumber { get; set; } = string.Empty; // 参考订单号

    [StringLength(200)]
    public string ReferenceQuotationNumber { get; set; } = string.Empty; // 参考报价单号

    // 条件限制
    [Column(TypeName = "decimal(10,2)")]
    public decimal MinOrderQuantity { get; set; } // 最小订单数量

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinOrderAmount { get; set; } // 最小订单金额

    public bool OneTimeOffer { get; set; } = true; // 是否为一次性优惠

    public int? MaxUsageCount { get; set; } // 最大使用次数

    public int CurrentUsageCount { get; set; } // 当前使用次数

    // 市场信息
    [StringLength(500)]
    public string MarketConditions { get; set; } = string.Empty; // 市场条件

    [StringLength(500)]
    public string CustomerFeedback { get; set; } = string.Empty; // 客户反馈

    [Column(TypeName = "decimal(18,2)")]
    public decimal ExpectedOrderValue { get; set; } // 预期订单价值

    [StringLength(100)]
    public string SalesProbability { get; set; } = string.Empty; // 成交概率

    // 风险评估
    [StringLength(50)]
    public string RiskLevel { get; set; } = "Medium"; // Low, Medium, High

    [StringLength(500)]
    public string RiskFactors { get; set; } = string.Empty; // 风险因素

    [StringLength(500)]
    public string MitigationMeasures { get; set; } = string.Empty; // 缓解措施

    // 绩效追踪
    public bool ConvertedToOrder { get; set; } = false; // 是否转化为订单

    [StringLength(100)]
    public string OrderNumber { get; set; } = string.Empty; // 订单号

    [Column(TypeName = "decimal(18,2)")]
    public decimal ActualOrderAmount { get; set; } // 实际订单金额

    public DateTime? OrderDate { get; set; } // 下单日期

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 扩展属性
    [StringLength(2000)]
    public string ExtendedAttributes { get; set; } = string.Empty; // JSON格式扩展属性

    // 关联实体
    public int? PriceStrategyId { get; set; }

    [ForeignKey("PriceStrategyId")]
    public virtual PriceStrategy? PriceStrategy { get; set; }

    // 计算属性
    public decimal SavingsAmount => StandardPrice - RequestedPrice;

    public decimal SavingsRate => StandardPrice > 0 ? ((StandardPrice - RequestedPrice) / StandardPrice) * 100 : 0;

    public bool IsExpired => ValidUntil.HasValue && DateTime.UtcNow > ValidUntil.Value;

    public bool RequiresApproval => DiscountRate > 5 || SavingsAmount > 1000; // 示例规则

    public int DaysToExpiry => ValidUntil.HasValue ? (int)(ValidUntil.Value - DateTime.UtcNow).TotalDays : 0;

    public decimal ExpectedProfitLoss => (RequestedPrice - EstimatedCost) * RequestQuantity;

    public string UrgencyLevel => DaysToExpiry <= 3 ? "High" : DaysToExpiry <= 7 ? "Medium" : "Low";
}