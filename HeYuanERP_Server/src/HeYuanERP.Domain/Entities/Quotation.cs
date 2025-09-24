using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("Quotations")]
public class Quotation
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string QuotationNumber { get; set; } = string.Empty; // 报价单号

    [Required]
    [StringLength(100)]
    public string QuotationTitle { get; set; } = string.Empty; // 报价单标题

    [StringLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Sent, Accepted, Rejected, Expired

    [StringLength(50)]
    public string Type { get; set; } = "Standard"; // Standard, Tender, RFQ

    // 客户信息
    [Required]
    public int CustomerId { get; set; }

    [StringLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(500)]
    public string CustomerAddress { get; set; } = string.Empty;

    [StringLength(100)]
    public string CustomerContact { get; set; } = string.Empty;

    [StringLength(50)]
    public string CustomerPhone { get; set; } = string.Empty;

    [StringLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;

    // 销售人员信息
    [Required]
    public int SalespersonId { get; set; }

    [StringLength(100)]
    public string SalespersonName { get; set; } = string.Empty;

    [StringLength(100)]
    public string SalespersonDepartment { get; set; } = string.Empty;

    [StringLength(50)]
    public string SalespersonPhone { get; set; } = string.Empty;

    [StringLength(100)]
    public string SalespersonEmail { get; set; } = string.Empty;

    // 日期信息
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;

    public DateTime ValidUntil { get; set; } // 有效期

    public DateTime? SentDate { get; set; } // 发送日期

    public DateTime? ResponseDate { get; set; } // 客户回复日期

    // 价格汇总
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubtotalAmount { get; set; } // 小计金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } // 税额

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } // 总金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } // 总折扣金额

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountRate { get; set; } // 总折扣率

    // 条款信息
    [StringLength(100)]
    public string PaymentTerms { get; set; } = string.Empty; // 付款条件

    [StringLength(100)]
    public string DeliveryTerms { get; set; } = string.Empty; // 交货条件

    [StringLength(100)]
    public string DeliveryTime { get; set; } = string.Empty; // 交货时间

    [StringLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty; // 交货地址

    [StringLength(100)]
    public string Currency { get; set; } = "CNY"; // 币种

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1; // 汇率

    // 备注和说明
    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty; // 备注

    [StringLength(2000)]
    public string TermsAndConditions { get; set; } = string.Empty; // 条款和条件

    [StringLength(1000)]
    public string SpecialInstructions { get; set; } = string.Empty; // 特殊说明

    // 竞争信息
    [StringLength(500)]
    public string CompetitorInfo { get; set; } = string.Empty; // 竞争对手信息

    [StringLength(100)]
    public string WinProbability { get; set; } = string.Empty; // 胜率

    [StringLength(500)]
    public string KeySellingPoints { get; set; } = string.Empty; // 关键卖点

    // 审批信息
    public bool RequiresApproval { get; set; } = false;

    public int? ApproverId { get; set; }

    [StringLength(100)]
    public string ApproverName { get; set; } = string.Empty;

    public DateTime? ApprovalDate { get; set; }

    [StringLength(1000)]
    public string ApprovalComments { get; set; } = string.Empty;

    // 关联订单信息
    public int? ConvertedOrderId { get; set; } // 转换的订单ID

    [StringLength(50)]
    public string ConvertedOrderNumber { get; set; } = string.Empty;

    public DateTime? ConversionDate { get; set; } // 转换日期

    [Column(TypeName = "decimal(5,2)")]
    public decimal ConversionRate { get; set; } // 转换率

    // 修订信息
    public int RevisionNumber { get; set; } = 1; // 修订版本号

    public int? OriginalQuotationId { get; set; } // 原始报价单ID

    [StringLength(500)]
    public string RevisionReason { get; set; } = string.Empty; // 修订原因

    // 模板和格式
    [StringLength(100)]
    public string TemplateId { get; set; } = string.Empty; // 使用的模板ID

    [StringLength(100)]
    public string LanguageCode { get; set; } = "zh-CN"; // 语言代码

    [StringLength(200)]
    public string DocumentPath { get; set; } = string.Empty; // 生成的文档路径

    // 追踪和分析
    public int ViewCount { get; set; } // 查看次数

    public DateTime? LastViewedDate { get; set; } // 最后查看时间

    [StringLength(500)]
    public string CustomerFeedback { get; set; } = string.Empty; // 客户反馈

    [StringLength(500)]
    public string LossReason { get; set; } = string.Empty; // 失败原因

    // 邮件和通信
    [StringLength(200)]
    public string EmailSubject { get; set; } = string.Empty; // 邮件主题

    [StringLength(1000)]
    public string EmailBody { get; set; } = string.Empty; // 邮件正文

    public bool EmailSent { get; set; } = false; // 是否已发送邮件

    public DateTime? EmailSentDate { get; set; } // 邮件发送时间

    // 性能指标
    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedCost { get; set; } // 预估成本

    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedProfit { get; set; } // 预估利润

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProfitMargin { get; set; } // 利润率

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 关联实体
    public virtual ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();

    // 计算属性
    public bool IsExpired => DateTime.UtcNow > ValidUntil;

    public int DaysUntilExpiry => (int)(ValidUntil - DateTime.UtcNow).TotalDays;

    public bool IsOverdue => IsExpired && Status == "Sent";

    public decimal TotalItemsCount => QuotationItems.Sum(i => i.Quantity);

    public string StatusDescription => Status switch
    {
        "Draft" => "草稿",
        "Sent" => "已发送",
        "Accepted" => "已接受",
        "Rejected" => "已拒绝",
        "Expired" => "已过期",
        _ => "未知"
    };

    public string UrgencyLevel => DaysUntilExpiry <= 3 ? "紧急" : DaysUntilExpiry <= 7 ? "重要" : "普通";
}

[Table("QuotationItems")]
public class QuotationItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuotationId { get; set; }

    [ForeignKey("QuotationId")]
    public virtual Quotation Quotation { get; set; } = null!;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [StringLength(100)]
    public string ProductCode { get; set; } = string.Empty;

    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string ProductDescription { get; set; } = string.Empty;

    [StringLength(100)]
    public string Specification { get; set; } = string.Empty;

    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; } // 单价

    [Column(TypeName = "decimal(18,2)")]
    public decimal StandardPrice { get; set; } // 标准价格

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountRate { get; set; } // 折扣率

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } // 折扣金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; } // 行总计

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; } // 税率

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } // 税额

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty; // 备注

    [StringLength(100)]
    public string DeliveryTime { get; set; } = string.Empty; // 交货时间

    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedCost { get; set; } // 预估成本

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProfitMargin { get; set; } // 利润率

    public int SortOrder { get; set; } // 排序

    // 可选项信息
    public bool IsOptional { get; set; } = false; // 是否为可选项

    [StringLength(100)]
    public string OptionGroup { get; set; } = string.Empty; // 选项组

    // 计算属性
    public decimal TotalWithTax => LineTotal + TaxAmount;

    public decimal UnitCost => EstimatedCost / (Quantity > 0 ? Quantity : 1);

    public decimal LineProfit => LineTotal - (EstimatedCost * Quantity);
}