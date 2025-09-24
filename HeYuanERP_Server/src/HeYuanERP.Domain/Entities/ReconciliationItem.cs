using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("ReconciliationItems")]
public class ReconciliationItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ReconciliationRecordId { get; set; }

    [ForeignKey("ReconciliationRecordId")]
    public virtual ReconciliationRecord ReconciliationRecord { get; set; } = null!;

    [StringLength(50)]
    public string ItemType { get; set; } = string.Empty; // Internal, External, Both

    [StringLength(50)]
    public string Status { get; set; } = "Unmatched"; // Matched, Unmatched, Partial, Disputed

    // 内部系统数据
    [StringLength(100)]
    public string InternalTransactionId { get; set; } = string.Empty;

    [StringLength(50)]
    public string InternalReferenceNumber { get; set; } = string.Empty;

    public DateTime? InternalTransactionDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal InternalAmount { get; set; }

    [StringLength(200)]
    public string InternalDescription { get; set; } = string.Empty;

    [StringLength(50)]
    public string InternalTransactionType { get; set; } = string.Empty;

    // 外部系统数据
    [StringLength(100)]
    public string ExternalTransactionId { get; set; } = string.Empty;

    [StringLength(50)]
    public string ExternalReferenceNumber { get; set; } = string.Empty;

    public DateTime? ExternalTransactionDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ExternalAmount { get; set; }

    [StringLength(200)]
    public string ExternalDescription { get; set; } = string.Empty;

    [StringLength(50)]
    public string ExternalTransactionType { get; set; } = string.Empty;

    // 匹配信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal DifferenceAmount { get; set; } // 差异金额

    public int DifferenceInDays { get; set; } // 日期差异天数

    [StringLength(50)]
    public string MatchType { get; set; } = string.Empty; // Exact, Fuzzy, Manual, Automated

    [StringLength(100)]
    public string MatchingCriteria { get; set; } = string.Empty; // Amount+Date, ReferenceNumber, etc.

    [Column(TypeName = "decimal(5,2)")]
    public decimal MatchConfidenceScore { get; set; } // 匹配置信度分数

    // 处理信息
    public DateTime? MatchedDate { get; set; }

    public int? MatchedByUserId { get; set; }

    [StringLength(100)]
    public string MatchedByUserName { get; set; } = string.Empty;

    [StringLength(50)]
    public string MatchingMethod { get; set; } = string.Empty; // Auto, SemiAuto, Manual

    // 差异分析
    [StringLength(50)]
    public string DifferenceType { get; set; } = string.Empty; // Amount, Date, Reference, Description

    [StringLength(500)]
    public string DifferenceReason { get; set; } = string.Empty;

    [StringLength(500)]
    public string ResolutionAction { get; set; } = string.Empty;

    [StringLength(50)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

    // 业务相关信息
    [StringLength(100)]
    public string CustomerVendorName { get; set; } = string.Empty;

    [StringLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [StringLength(50)]
    public string CheckNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string BankAccount { get; set; } = string.Empty;

    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    // 分类和标记
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [StringLength(100)]
    public string SubCategory { get; set; } = string.Empty;

    [StringLength(200)]
    public string Tags { get; set; } = string.Empty; // JSON格式的标签

    // 监管和合规
    public bool RequiresReview { get; set; } = false;

    public bool IsRegulatory { get; set; } = false;

    [StringLength(100)]
    public string ComplianceNotes { get; set; } = string.Empty;

    // 工作流状态
    [StringLength(50)]
    public string WorkflowStatus { get; set; } = string.Empty;

    public int? AssignedToUserId { get; set; }

    [StringLength(100)]
    public string AssignedToUserName { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    // 历史记录
    [StringLength(2000)]
    public string ActionHistory { get; set; } = string.Empty; // JSON格式的操作历史

    [StringLength(1000)]
    public string Comments { get; set; } = string.Empty;

    // 关联调整
    public int? AdjustmentEntryId { get; set; }

    [StringLength(50)]
    public string AdjustmentEntryNumber { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal AdjustmentAmount { get; set; }

    // 时间戳
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 计算属性
    public bool IsMatched => Status == "Matched";

    public bool HasDifference => DifferenceAmount != 0 || DifferenceInDays != 0;

    public decimal AbsoluteDifferenceAmount => Math.Abs(DifferenceAmount);

    public bool IsWithinTolerance => Math.Abs(DifferenceAmount) <=
        (ReconciliationRecord?.ToleranceAmount ?? 0);

    public string StatusDescription => Status switch
    {
        "Matched" => "已匹配",
        "Unmatched" => "未匹配",
        "Partial" => "部分匹配",
        "Disputed" => "争议中",
        _ => "未知"
    };

    public bool IsOverdue => DueDate.HasValue && DateTime.UtcNow > DueDate.Value;

    public int DaysOverdue => IsOverdue ? (int)(DateTime.UtcNow - DueDate!.Value).TotalDays : 0;

    public string UrgencyLevel => Priority switch
    {
        "Critical" => "紧急",
        "High" => "重要",
        "Medium" => "普通",
        "Low" => "低",
        _ => "未设置"
    };
}