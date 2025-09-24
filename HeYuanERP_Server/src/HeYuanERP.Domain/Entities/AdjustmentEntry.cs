using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("AdjustmentEntries")]
public class AdjustmentEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string AdjustmentNumber { get; set; } = string.Empty; // 调整单号

    [Required]
    [StringLength(50)]
    public string AdjustmentType { get; set; } = string.Empty; // Reconciliation, Inventory, Financial, Tax

    [StringLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved, Rejected, Posted

    public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;

    public DateTime? EffectiveDate { get; set; } // 生效日期

    // 关联信息
    public int? ReconciliationRecordId { get; set; }

    [ForeignKey("ReconciliationRecordId")]
    public virtual ReconciliationRecord? ReconciliationRecord { get; set; }

    public string? ReconciliationDifferenceId { get; set; }

    [ForeignKey("ReconciliationDifferenceId")]
    public virtual ReconciliationDifference? ReconciliationDifference { get; set; }

    // 业务原因
    [Required]
    [StringLength(100)]
    public string ReasonCode { get; set; } = string.Empty; // 调整原因代码

    [Required]
    [StringLength(1000)]
    public string ReasonDescription { get; set; } = string.Empty; // 调整原因描述

    [StringLength(500)]
    public string BusinessJustification { get; set; } = string.Empty; // 业务理由

    // 金额信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAdjustmentAmount { get; set; } // 总调整金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal DebitAmount { get; set; } // 借方金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditAmount { get; set; } // 贷方金额

    [StringLength(10)]
    public string Currency { get; set; } = "CNY"; // 币种

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1; // 汇率

    // 影响评估
    [StringLength(50)]
    public string ImpactArea { get; set; } = string.Empty; // Revenue, Cost, Asset, Liability

    [StringLength(1000)]
    public string ImpactDescription { get; set; } = string.Empty; // 影响描述

    [Column(TypeName = "decimal(18,2)")]
    public decimal FinancialImpact { get; set; } // 财务影响

    public bool AffectsReporting { get; set; } = false; // 影响财务报告

    // 处理人员
    [Required]
    public int PreparedByUserId { get; set; }

    [StringLength(100)]
    public string PreparedByUserName { get; set; } = string.Empty;

    public int? ReviewedByUserId { get; set; }

    [StringLength(100)]
    public string ReviewedByUserName { get; set; } = string.Empty;

    public DateTime? ReviewedDate { get; set; }

    public int? ApprovedByUserId { get; set; }

    [StringLength(100)]
    public string ApprovedByUserName { get; set; } = string.Empty;

    public DateTime? ApprovedDate { get; set; }

    // 审批流程
    [StringLength(50)]
    public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

    [StringLength(1000)]
    public string ApprovalComments { get; set; } = string.Empty;

    public bool RequiresHigherApproval { get; set; } = false;

    public int? HigherApprovalUserId { get; set; }

    [StringLength(100)]
    public string HigherApprovalUserName { get; set; } = string.Empty;

    [StringLength(50)]
    public string ApprovalLevel { get; set; } = string.Empty; // Level1, Level2, Level3

    // 过账信息
    public bool IsPosted { get; set; } = false;

    public DateTime? PostedDate { get; set; }

    public int? PostedByUserId { get; set; }

    [StringLength(100)]
    public string PostedByUserName { get; set; } = string.Empty;

    [StringLength(50)]
    public string JournalEntryNumber { get; set; } = string.Empty; // 记账凭证号

    // 分期信息（如果适用）
    public bool IsRecurring { get; set; } = false;

    [StringLength(50)]
    public string RecurringFrequency { get; set; } = string.Empty; // Monthly, Quarterly, Yearly

    public int RecurringPeriods { get; set; } // 分期期数

    public int ProcessedPeriods { get; set; } // 已处理期数

    public DateTime? NextProcessingDate { get; set; }

    // 撤销信息
    public bool IsReversed { get; set; } = false;

    public DateTime? ReversedDate { get; set; }

    public int? ReversedByUserId { get; set; }

    [StringLength(100)]
    public string ReversedByUserName { get; set; } = string.Empty;

    [StringLength(500)]
    public string ReversalReason { get; set; } = string.Empty;

    public int? ReversalEntryId { get; set; } // 撤销调整单ID

    // 监管合规
    public bool IsRegulatoryRequired { get; set; } = false;

    [StringLength(100)]
    public string RegulatoryReference { get; set; } = string.Empty;

    [StringLength(500)]
    public string ComplianceNotes { get; set; } = string.Empty;

    public bool RequiresExternalReporting { get; set; } = false;

    // 系统集成
    [StringLength(100)]
    public string SourceSystem { get; set; } = string.Empty; // 来源系统

    [StringLength(100)]
    public string SourceTransactionId { get; set; } = string.Empty; // 来源交易ID

    [StringLength(500)]
    public string IntegrationNotes { get; set; } = string.Empty;

    public bool AutoGenerated { get; set; } = false; // 自动生成

    // 工作流
    [StringLength(50)]
    public string WorkflowStatus { get; set; } = string.Empty;

    [StringLength(2000)]
    public string WorkflowHistory { get; set; } = string.Empty; // JSON格式

    // 附件和证据
    [StringLength(1000)]
    public string SupportingDocuments { get; set; } = string.Empty; // JSON格式的文档链接

    [StringLength(2000)]
    public string AuditTrail { get; set; } = string.Empty; // 审计跟踪

    // 备注
    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty;

    [StringLength(2000)]
    public string InternalNotes { get; set; } = string.Empty; // 内部备注

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 关联实体
    public virtual ICollection<AdjustmentEntryLine> AdjustmentLines { get; set; } = new List<AdjustmentEntryLine>();

    // 计算属性
    public bool IsBalanced => Math.Abs(DebitAmount - CreditAmount) < 0.01m;

    public bool IsApproved => ApprovalStatus == "Approved";

    public bool RequiresApproval => TotalAdjustmentAmount > 1000; // 可配置的审批阈值

    public string StatusDescription => Status switch
    {
        "Draft" => "草稿",
        "Submitted" => "已提交",
        "Approved" => "已审批",
        "Rejected" => "已拒绝",
        "Posted" => "已过账",
        _ => "未知"
    };

    public bool IsEditable => Status == "Draft" || Status == "Rejected";

    public bool CanBePosted => IsApproved && IsBalanced && !IsPosted;

    public int DaysInProcess => (int)(DateTime.UtcNow - CreatedAt).TotalDays;

    public bool IsOverdueForApproval => Status == "Submitted" && DaysInProcess > 5; // 可配置

    public decimal AbsoluteTotalAmount => Math.Abs(TotalAdjustmentAmount);
}

[Table("AdjustmentEntryLines")]
public class AdjustmentEntryLine
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AdjustmentEntryId { get; set; }

    [ForeignKey("AdjustmentEntryId")]
    public virtual AdjustmentEntry AdjustmentEntry { get; set; } = null!;

    public int LineNumber { get; set; } // 行号

    [StringLength(50)]
    public string AccountCode { get; set; } = string.Empty; // 科目代码

    [StringLength(200)]
    public string AccountName { get; set; } = string.Empty; // 科目名称

    [StringLength(50)]
    public string AccountType { get; set; } = string.Empty; // Asset, Liability, Equity, Revenue, Expense

    [Column(TypeName = "decimal(18,2)")]
    public decimal DebitAmount { get; set; } // 借方金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditAmount { get; set; } // 贷方金额

    [StringLength(500)]
    public string Description { get; set; } = string.Empty; // 摘要

    [StringLength(100)]
    public string CostCenter { get; set; } = string.Empty; // 成本中心

    [StringLength(100)]
    public string Department { get; set; } = string.Empty; // 部门

    [StringLength(100)]
    public string Project { get; set; } = string.Empty; // 项目

    [StringLength(100)]
    public string CustomerVendor { get; set; } = string.Empty; // 客户/供应商

    [StringLength(100)]
    public string Product { get; set; } = string.Empty; // 产品

    // 辅助核算
    [StringLength(200)]
    public string AuxiliaryAccounting { get; set; } = string.Empty; // JSON格式的辅助核算

    // 外币信息
    [StringLength(10)]
    public string Currency { get; set; } = "CNY";

    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal ForeignDebitAmount { get; set; } // 外币借方金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal ForeignCreditAmount { get; set; } // 外币贷方金额

    // 数量信息
    [Column(TypeName = "decimal(10,2)")]
    public decimal Quantity { get; set; }

    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,6)")]
    public decimal UnitPrice { get; set; }

    // 税务信息
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [StringLength(50)]
    public string TaxCode { get; set; } = string.Empty;

    // 备注
    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    // 计算属性
    public decimal NetAmount => DebitAmount - CreditAmount;

    public decimal AbsoluteAmount => Math.Abs(NetAmount);

    public bool IsDebit => DebitAmount > 0;

    public bool IsCredit => CreditAmount > 0;

    public decimal TotalAmountIncludingTax => AbsoluteAmount + TaxAmount;
}