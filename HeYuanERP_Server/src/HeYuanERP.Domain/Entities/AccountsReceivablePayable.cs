using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

// 应收账款主表
[Table("AccountsReceivable")]
public class AccountReceivable
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string DocumentNumber { get; set; } = string.Empty; // 原始单据号

    [Required]
    [StringLength(50)]
    public string DocumentType { get; set; } = string.Empty; // Invoice, Order, Delivery

    [Required]
    public int CustomerId { get; set; }

    [StringLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(100)]
    public string CustomerCode { get; set; } = string.Empty;

    public DateTime DocumentDate { get; set; } // 单据日期

    public DateTime DueDate { get; set; } // 到期日期

    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalAmount { get; set; } // 原始金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; } // 已收金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAmount { get; set; } // 余额

    [StringLength(10)]
    public string Currency { get; set; } = "CNY";

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1;

    [StringLength(50)]
    public string Status { get; set; } = "Outstanding"; // Outstanding, PartiallyPaid, FullyPaid, WrittenOff

    [StringLength(50)]
    public string AgingBucket { get; set; } = "Current"; // Current, 1-30, 31-60, 61-90, Over90

    public int OverdueDays { get; set; } // 逾期天数

    [StringLength(50)]
    public string RiskLevel { get; set; } = "Low"; // Low, Medium, High

    [Column(TypeName = "decimal(5,2)")]
    public decimal BadDebtProbability { get; set; } // 坏账概率

    // 业务分类
    [StringLength(100)]
    public string BusinessCategory { get; set; } = string.Empty;

    [StringLength(100)]
    public string ProductCategory { get; set; } = string.Empty;

    [StringLength(100)]
    public string SalesChannel { get; set; } = string.Empty;

    [StringLength(100)]
    public string SalesRepresentative { get; set; } = string.Empty;

    // 关联业务信息
    [StringLength(100)]
    public string RelatedOrderNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string RelatedInvoiceNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string RelatedDeliveryNumber { get; set; } = string.Empty;

    // 催收信息
    public DateTime? LastCollectionDate { get; set; } // 最后催收日期

    [StringLength(100)]
    public string LastCollectionMethod { get; set; } = string.Empty;

    public DateTime? NextCollectionDate { get; set; } // 下次催收日期

    [StringLength(200)]
    public string CollectionNotes { get; set; } = string.Empty;

    public int CollectionAttempts { get; set; } // 催收次数

    // 法务信息
    public bool IsLegalAction { get; set; } = false; // 是否启动法务程序

    public DateTime? LegalActionDate { get; set; }

    [StringLength(200)]
    public string LegalActionNotes { get; set; } = string.Empty;

    // 坏账信息
    public bool IsWrittenOff { get; set; } = false; // 是否已核销

    public DateTime? WriteOffDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WriteOffAmount { get; set; }

    [StringLength(500)]
    public string WriteOffReason { get; set; } = string.Empty;

    [StringLength(100)]
    public string WriteOffApprovedBy { get; set; } = string.Empty;

    // 备注信息
    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty;

    [StringLength(2000)]
    public string InternalNotes { get; set; } = string.Empty;

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();
    public virtual ICollection<PaymentApplication> PaymentApplications { get; set; } = new List<PaymentApplication>();

    // 计算属性
    public decimal CollectionRate => OriginalAmount > 0 ? PaidAmount / OriginalAmount * 100 : 0;

    public bool IsOverdue => DateTime.UtcNow > DueDate && BalanceAmount > 0;

    public string AgingDescription => AgingBucket switch
    {
        "Current" => "当期",
        "1-30" => "1-30天",
        "31-60" => "31-60天",
        "61-90" => "61-90天",
        "Over90" => "90天以上",
        _ => "未知"
    };

    public string StatusDescription => Status switch
    {
        "Outstanding" => "未收款",
        "PartiallyPaid" => "部分收款",
        "FullyPaid" => "已收款",
        "WrittenOff" => "已核销",
        _ => "未知"
    };

    public bool RequiresAttention => IsOverdue && OverdueDays > 30;

    public decimal DailyInterest => BalanceAmount * 0.0001m; // 万分之一日息
}

// 应付账款主表
[Table("AccountsPayable")]
public class AccountPayable
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string DocumentNumber { get; set; } = string.Empty; // 原始单据号

    [Required]
    [StringLength(50)]
    public string DocumentType { get; set; } = string.Empty; // PurchaseOrder, Invoice, Receipt

    [Required]
    public int SupplierId { get; set; }

    [StringLength(200)]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(100)]
    public string SupplierCode { get; set; } = string.Empty;

    public DateTime DocumentDate { get; set; } // 单据日期

    public DateTime DueDate { get; set; } // 到期日期

    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalAmount { get; set; } // 原始金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; } // 已付金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAmount { get; set; } // 余额

    [StringLength(10)]
    public string Currency { get; set; } = "CNY";

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1;

    [StringLength(50)]
    public string Status { get; set; } = "Outstanding"; // Outstanding, PartiallyPaid, FullyPaid

    [StringLength(50)]
    public string AgingBucket { get; set; } = "Current"; // Current, 1-30, 31-60, 61-90, Over90

    public int OverdueDays { get; set; } // 逾期天数

    [StringLength(50)]
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent

    // 业务分类
    [StringLength(100)]
    public string PurchaseCategory { get; set; } = string.Empty;

    [StringLength(100)]
    public string ProductCategory { get; set; } = string.Empty;

    [StringLength(100)]
    public string PurchaseRep { get; set; } = string.Empty;

    [StringLength(100)]
    public string CostCenter { get; set; } = string.Empty;

    // 关联业务信息
    [StringLength(100)]
    public string RelatedPONumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string RelatedReceiptNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string RelatedInvoiceNumber { get; set; } = string.Empty;

    // 付款条件
    [StringLength(100)]
    public string PaymentTerms { get; set; } = string.Empty;

    [StringLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Column(TypeName = "decimal(5,2)")]
    public decimal EarlyPaymentDiscount { get; set; } // 早付折扣率

    public DateTime? EarlyPaymentDate { get; set; } // 早付截止日期

    // 审批信息
    public bool RequiresApproval { get; set; } = false;

    [StringLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    public DateTime? ApprovedDate { get; set; }

    [StringLength(50)]
    public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

    // 付款计划
    public bool HasPaymentSchedule { get; set; } = false;

    public DateTime? ScheduledPaymentDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ScheduledPaymentAmount { get; set; }

    // 税务信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; }

    [StringLength(50)]
    public string TaxCode { get; set; } = string.Empty;

    public bool IsVATDeductible { get; set; } = true;

    // 备注信息
    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty;

    [StringLength(2000)]
    public string InternalNotes { get; set; } = string.Empty;

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual ICollection<PaymentSchedule> PaymentSchedules { get; set; } = new List<PaymentSchedule>();

    // 计算属性
    public decimal PaymentRate => OriginalAmount > 0 ? PaidAmount / OriginalAmount * 100 : 0;

    public bool IsOverdue => DateTime.UtcNow > DueDate && BalanceAmount > 0;

    public string AgingDescription => AgingBucket switch
    {
        "Current" => "当期",
        "1-30" => "1-30天",
        "31-60" => "31-60天",
        "61-90" => "61-90天",
        "Over90" => "90天以上",
        _ => "未知"
    };

    public string StatusDescription => Status switch
    {
        "Outstanding" => "未付款",
        "PartiallyPaid" => "部分付款",
        "FullyPaid" => "已付款",
        _ => "未知"
    };

    public bool CanGetEarlyDiscount => EarlyPaymentDate.HasValue && DateTime.UtcNow <= EarlyPaymentDate.Value;

    public decimal EarlyDiscountAmount => CanGetEarlyDiscount ? BalanceAmount * EarlyPaymentDiscount / 100 : 0;
}

// 催收记录
[Table("CollectionRecords")]
public class CollectionRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AccountReceivableId { get; set; }

    [ForeignKey("AccountReceivableId")]
    public virtual AccountReceivable AccountReceivable { get; set; } = null!;

    public DateTime ContactDate { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(50)]
    public string ContactMethod { get; set; } = string.Empty; // Phone, Email, Visit, Letter, SMS

    [StringLength(100)]
    public string ContactPerson { get; set; } = string.Empty; // 联系人

    [StringLength(100)]
    public string ContactTitle { get; set; } = string.Empty; // 联系人职位

    [StringLength(100)]
    public string CollectorName { get; set; } = string.Empty; // 催收人

    public int CollectorId { get; set; } // 催收人ID

    [Required]
    [StringLength(2000)]
    public string ContactNotes { get; set; } = string.Empty; // 联系记录

    [StringLength(50)]
    public string ContactResult { get; set; } = string.Empty; // Success, NoAnswer, Promised, Disputed, Refused

    [StringLength(2000)]
    public string CustomerResponse { get; set; } = string.Empty; // 客户回应

    [StringLength(2000)]
    public string NextAction { get; set; } = string.Empty; // 下一步行动

    public DateTime? NextContactDate { get; set; } // 下次联系日期

    [Column(TypeName = "decimal(18,2)")]
    public decimal PromisedAmount { get; set; } // 承诺付款金额

    public DateTime? PromisedDate { get; set; } // 承诺付款日期

    [StringLength(50)]
    public string CollectionStage { get; set; } = "Initial"; // Initial, Follow-up, Escalated, Legal

    public bool IsSuccessful { get; set; } = false; // 是否成功

    [StringLength(1000)]
    public string AttachmentUrls { get; set; } = string.Empty; // 附件链接

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    // 计算属性
    public string ContactMethodDescription => ContactMethod switch
    {
        "Phone" => "电话",
        "Email" => "邮件",
        "Visit" => "拜访",
        "Letter" => "信函",
        "SMS" => "短信",
        _ => "其他"
    };

    public string ContactResultDescription => ContactResult switch
    {
        "Success" => "成功联系",
        "NoAnswer" => "无人接听",
        "Promised" => "承诺付款",
        "Disputed" => "有争议",
        "Refused" => "拒绝付款",
        _ => "未知"
    };

    public bool IsPromiseKept => PromisedDate.HasValue && DateTime.UtcNow > PromisedDate.Value;
}

// 收款核销记录
[Table("PaymentApplications")]
public class PaymentApplication
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AccountReceivableId { get; set; }

    [ForeignKey("AccountReceivableId")]
    public virtual AccountReceivable AccountReceivable { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string PaymentNumber { get; set; } = string.Empty; // 收款单号

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal AppliedAmount { get; set; } // 核销金额

    [StringLength(10)]
    public string Currency { get; set; } = "CNY";

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1;

    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty; // Cash, BankTransfer, Check, CreditCard

    [StringLength(200)]
    public string PaymentReference { get; set; } = string.Empty; // 银行流水号等

    [StringLength(100)]
    public string PaymentAccount { get; set; } = string.Empty; // 收款账户

    [StringLength(50)]
    public string ApplicationType { get; set; } = "Normal"; // Normal, Advance, Overpayment

    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty;

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    // 计算属性
    public string PaymentMethodDescription => PaymentMethod switch
    {
        "Cash" => "现金",
        "BankTransfer" => "银行转账",
        "Check" => "支票",
        "CreditCard" => "信用卡",
        _ => "其他"
    };
}

// 付款计划
[Table("PaymentSchedules")]
public class PaymentSchedule
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AccountPayableId { get; set; }

    [ForeignKey("AccountPayableId")]
    public virtual AccountPayable AccountPayable { get; set; } = null!;

    public int SequenceNumber { get; set; } // 期次

    public DateTime ScheduledDate { get; set; } // 计划付款日期

    [Column(TypeName = "decimal(18,2)")]
    public decimal ScheduledAmount { get; set; } // 计划付款金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal ActualAmount { get; set; } // 实际付款金额

    public DateTime? ActualDate { get; set; } // 实际付款日期

    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, Cancelled

    [StringLength(100)]
    public string PaymentReference { get; set; } = string.Empty; // 付款参考号

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 计算属性
    public bool IsOverdue => DateTime.UtcNow > ScheduledDate && Status == "Pending";

    public decimal VarianceAmount => ActualAmount - ScheduledAmount;

    public int DelayDays => ActualDate.HasValue ?
        (int)(ActualDate.Value - ScheduledDate).TotalDays :
        (Status == "Pending" ? (int)(DateTime.UtcNow - ScheduledDate).TotalDays : 0);
}