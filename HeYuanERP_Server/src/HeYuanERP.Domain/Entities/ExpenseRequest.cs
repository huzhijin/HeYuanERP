using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("ExpenseRequests")]
public class ExpenseRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string RequestNumber { get; set; } = string.Empty; // 费用申请单号

    [Required]
    [StringLength(50)]
    public string RequestType { get; set; } = string.Empty; // Travel, Office, Marketing, Training, Entertainment

    [StringLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved, Rejected, Paid, Closed

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    // 申请人信息
    [Required]
    public int RequesterId { get; set; }

    [StringLength(100)]
    public string RequesterName { get; set; } = string.Empty;

    [StringLength(100)]
    public string RequesterDepartment { get; set; } = string.Empty;

    [StringLength(100)]
    public string RequesterPosition { get; set; } = string.Empty;

    [StringLength(50)]
    public string RequesterPhone { get; set; } = string.Empty;

    [StringLength(100)]
    public string RequesterEmail { get; set; } = string.Empty;

    // 费用基本信息
    [Required]
    [StringLength(200)]
    public string ExpenseTitle { get; set; } = string.Empty; // 费用标题

    [Required]
    [StringLength(1000)]
    public string ExpenseDescription { get; set; } = string.Empty; // 费用说明

    [StringLength(100)]
    public string BusinessPurpose { get; set; } = string.Empty; // 业务目的

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } // 总金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal ApprovedAmount { get; set; } // 批准金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; } // 已支付金额

    [StringLength(10)]
    public string Currency { get; set; } = "CNY"; // 币种

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1; // 汇率

    // 时间信息
    public DateTime ExpenseStartDate { get; set; } // 费用开始日期
    public DateTime ExpenseEndDate { get; set; } // 费用结束日期

    public DateTime? RequiredDate { get; set; } // 需要支付日期

    // 审批信息
    public bool RequiresApproval { get; set; } = true;

    public int? FirstApproverId { get; set; } // 一级审批人

    [StringLength(100)]
    public string FirstApproverName { get; set; } = string.Empty;

    public DateTime? FirstApprovalDate { get; set; }

    [StringLength(50)]
    public string FirstApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

    [StringLength(500)]
    public string FirstApprovalComments { get; set; } = string.Empty;

    public int? SecondApproverId { get; set; } // 二级审批人

    [StringLength(100)]
    public string SecondApproverName { get; set; } = string.Empty;

    public DateTime? SecondApprovalDate { get; set; }

    [StringLength(50)]
    public string SecondApprovalStatus { get; set; } = "Pending";

    [StringLength(500)]
    public string SecondApprovalComments { get; set; } = string.Empty;

    // 财务信息
    public int? FinanceApproverId { get; set; } // 财务审批人

    [StringLength(100)]
    public string FinanceApproverName { get; set; } = string.Empty;

    public DateTime? FinanceApprovalDate { get; set; }

    [StringLength(50)]
    public string FinanceApprovalStatus { get; set; } = "Pending";

    [StringLength(500)]
    public string FinanceApprovalComments { get; set; } = string.Empty;

    // 支付信息
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty; // Cash, BankTransfer, CorporateCard, Check

    [StringLength(100)]
    public string PaymentAccount { get; set; } = string.Empty; // 收款账户

    [StringLength(200)]
    public string PaymentAccountName { get; set; } = string.Empty; // 收款户名

    [StringLength(100)]
    public string PaymentBank { get; set; } = string.Empty; // 收款银行

    public DateTime? PaymentDate { get; set; } // 支付日期

    [StringLength(50)]
    public string PaymentReference { get; set; } = string.Empty; // 支付参考号

    // 预算相关
    [StringLength(100)]
    public string BudgetCode { get; set; } = string.Empty; // 预算代码

    [StringLength(200)]
    public string BudgetName { get; set; } = string.Empty; // 预算名称

    [Column(TypeName = "decimal(18,2)")]
    public decimal BudgetAmount { get; set; } // 预算金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal BudgetUsed { get; set; } // 已使用预算

    [Column(TypeName = "decimal(18,2)")]
    public decimal BudgetRemaining { get; set; } // 剩余预算

    // 项目相关
    [StringLength(100)]
    public string ProjectCode { get; set; } = string.Empty; // 项目代码

    [StringLength(200)]
    public string ProjectName { get; set; } = string.Empty; // 项目名称

    [StringLength(100)]
    public string CostCenter { get; set; } = string.Empty; // 成本中心

    // 差旅信息（如果是差旅费用）
    [StringLength(200)]
    public string TravelDestination { get; set; } = string.Empty; // 出差地点

    [StringLength(500)]
    public string TravelPurpose { get; set; } = string.Empty; // 出差目的

    public int TravelDays { get; set; } // 出差天数

    [StringLength(200)]
    public string AccommodationInfo { get; set; } = string.Empty; // 住宿信息

    // 税务信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } // 税额

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; } // 税率

    public bool IsTaxDeductible { get; set; } = true; // 是否可税前扣除

    [StringLength(100)]
    public string TaxCategory { get; set; } = string.Empty; // 税务类别

    // 单据和附件
    [StringLength(1000)]
    public string AttachmentUrls { get; set; } = string.Empty; // 附件链接（JSON格式）

    [StringLength(500)]
    public string ReceiptNumbers { get; set; } = string.Empty; // 发票号码

    public int ReceiptCount { get; set; } // 发票数量

    public bool HasOriginalReceipts { get; set; } = true; // 是否有原始发票

    // 审计信息
    [StringLength(2000)]
    public string AuditTrail { get; set; } = string.Empty; // 审计跟踪（JSON格式）

    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty; // 备注

    [StringLength(500)]
    public string InternalNotes { get; set; } = string.Empty; // 内部备注

    // 紧急程度
    [StringLength(50)]
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent

    [StringLength(500)]
    public string UrgencyReason { get; set; } = string.Empty; // 紧急原因

    // 关联信息
    [StringLength(100)]
    public string RelatedOrderNumber { get; set; } = string.Empty; // 关联订单号

    [StringLength(100)]
    public string RelatedContractNumber { get; set; } = string.Empty; // 关联合同号

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 关联实体
    public virtual ICollection<ExpenseRequestLine> ExpenseLines { get; set; } = new List<ExpenseRequestLine>();

    // 计算属性
    public bool IsApproved => FirstApprovalStatus == "Approved" &&
                             (SecondApproverId == null || SecondApprovalStatus == "Approved") &&
                             (FinanceApproverId == null || FinanceApprovalStatus == "Approved");

    public bool IsRejected => FirstApprovalStatus == "Rejected" ||
                             SecondApprovalStatus == "Rejected" ||
                             FinanceApprovalStatus == "Rejected";

    public bool IsPaid => Status == "Paid";

    public decimal PendingAmount => ApprovedAmount - PaidAmount;

    public bool IsOverBudget => BudgetAmount > 0 && TotalAmount > BudgetRemaining;

    public int DaysInProcess => (int)(DateTime.UtcNow - RequestDate).TotalDays;

    public bool IsOverdue => RequiredDate.HasValue && DateTime.UtcNow > RequiredDate.Value && !IsPaid;

    public string StatusDescription => Status switch
    {
        "Draft" => "草稿",
        "Submitted" => "已提交",
        "Approved" => "已批准",
        "Rejected" => "已拒绝",
        "Paid" => "已支付",
        "Closed" => "已关闭",
        _ => "未知"
    };

    public bool RequiresHigherApproval => TotalAmount > 10000; // 可配置的高级审批阈值

    public decimal ApprovalVariance => Math.Abs(TotalAmount - ApprovedAmount);

    public bool HasApprovalVariance => ApprovalVariance > 0.01m;
}

[Table("ExpenseRequestLines")]
public class ExpenseRequestLine
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ExpenseRequestId { get; set; }

    [ForeignKey("ExpenseRequestId")]
    public virtual ExpenseRequest ExpenseRequest { get; set; } = null!;

    public int LineNumber { get; set; } // 行号

    [Required]
    [StringLength(100)]
    public string ExpenseCategory { get; set; } = string.Empty; // 费用类别

    [StringLength(200)]
    public string ExpenseSubCategory { get; set; } = string.Empty; // 费用子类别

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty; // 费用描述

    public DateTime ExpenseDate { get; set; } // 费用发生日期

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } // 金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal ApprovedAmount { get; set; } // 批准金额

    [StringLength(10)]
    public string Currency { get; set; } = "CNY";

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal LocalAmount { get; set; } // 本币金额

    // 数量信息
    [Column(TypeName = "decimal(10,2)")]
    public decimal Quantity { get; set; } = 1;

    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,6)")]
    public decimal UnitPrice { get; set; }

    // 地点信息
    [StringLength(200)]
    public string Location { get; set; } = string.Empty; // 费用发生地点

    [StringLength(200)]
    public string Vendor { get; set; } = string.Empty; // 供应商/商家

    [StringLength(100)]
    public string VendorInvoiceNumber { get; set; } = string.Empty; // 供应商发票号

    // 税务信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; }

    [StringLength(50)]
    public string TaxCode { get; set; } = string.Empty;

    public bool IsTaxIncluded { get; set; } = true; // 是否含税

    // 分摊信息
    [StringLength(100)]
    public string AllocationCode { get; set; } = string.Empty; // 分摊代码

    [Column(TypeName = "decimal(5,2)")]
    public decimal AllocationPercentage { get; set; } = 100; // 分摊百分比

    [StringLength(100)]
    public string CostCenter { get; set; } = string.Empty;

    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [StringLength(100)]
    public string Project { get; set; } = string.Empty;

    // 单据信息
    [StringLength(100)]
    public string ReceiptNumber { get; set; } = string.Empty; // 发票号

    [StringLength(50)]
    public string ReceiptType { get; set; } = string.Empty; // 发票类型

    public bool HasReceipt { get; set; } = true; // 是否有发票

    [StringLength(500)]
    public string AttachmentUrl { get; set; } = string.Empty; // 附件链接

    // 审批信息
    [StringLength(500)]
    public string ApprovalComments { get; set; } = string.Empty; // 审批意见

    [StringLength(500)]
    public string RejectionReason { get; set; } = string.Empty; // 拒绝原因

    // 备注
    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    // 计算属性
    public decimal NetAmount => IsTaxIncluded ? Amount - TaxAmount : Amount;

    public decimal TotalAmountWithTax => IsTaxIncluded ? Amount : Amount + TaxAmount;

    public bool HasApprovalVariance => Math.Abs(Amount - ApprovedAmount) > 0.01m;

    public decimal ApprovalVariance => Amount - ApprovedAmount;
}