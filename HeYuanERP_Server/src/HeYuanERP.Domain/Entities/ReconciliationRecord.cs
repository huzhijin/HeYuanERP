using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("ReconciliationRecords")]
public class ReconciliationRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string ReconciliationNumber { get; set; } = string.Empty; // 对账单号

    [Required]
    [StringLength(50)]
    public string ReconciliationType { get; set; } = string.Empty; // Bank, Customer, Supplier, Inventory, GL

    [StringLength(50)]
    public string Status { get; set; } = "InProgress"; // InProgress, Completed, Failed, Cancelled

    public DateTime ReconciliationDate { get; set; } = DateTime.UtcNow;
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }

    // 关联信息
    public int? AccountId { get; set; } // 关联账户（客户/供应商）

    [StringLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [StringLength(100)]
    public string BankAccount { get; set; } = string.Empty; // 银行账户

    [StringLength(200)]
    public string BankName { get; set; } = string.Empty;

    // 对账金额信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal BookBalance { get; set; } // 账面余额

    [Column(TypeName = "decimal(18,2)")]
    public decimal StatementBalance { get; set; } // 对账单余额

    [Column(TypeName = "decimal(18,2)")]
    public decimal DifferenceAmount { get; set; } // 差异金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal ReconciledAmount { get; set; } // 已对账金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnreconciledAmount { get; set; } // 未对账金额

    // 统计信息
    public int TotalItemsCount { get; set; } // 总项目数

    public int ReconciledItemsCount { get; set; } // 已对账项目数

    public int UnreconciledItemsCount { get; set; } // 未对账项目数

    public int DifferenceItemsCount { get; set; } // 差异项目数

    // 处理信息
    [Required]
    public int ProcessedByUserId { get; set; }

    [StringLength(100)]
    public string ProcessedByUserName { get; set; } = string.Empty;

    public DateTime? CompletedDate { get; set; }

    public int? ReviewedByUserId { get; set; }

    [StringLength(100)]
    public string ReviewedByUserName { get; set; } = string.Empty;

    public DateTime? ReviewedDate { get; set; }

    // 外部数据信息
    [StringLength(500)]
    public string ExternalDataSource { get; set; } = string.Empty; // 外部数据源

    [StringLength(200)]
    public string ExternalFileName { get; set; } = string.Empty; // 外部文件名

    public DateTime? ExternalDataDate { get; set; } // 外部数据日期

    [StringLength(100)]
    public string ExternalDataHash { get; set; } = string.Empty; // 外部数据哈希

    // 自动对账设置
    public bool IsAutoReconciliation { get; set; } = false;

    [StringLength(100)]
    public string AutoReconciliationRule { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal ToleranceAmount { get; set; } // 容差金额

    [Column(TypeName = "decimal(5,2)")]
    public decimal TolerancePercentage { get; set; } // 容差百分比

    // 差异分析
    [StringLength(2000)]
    public string DifferenceAnalysis { get; set; } = string.Empty; // 差异分析

    [StringLength(2000)]
    public string ResolutionActions { get; set; } = string.Empty; // 解决方案

    [StringLength(50)]
    public string DifferenceCategory { get; set; } = string.Empty; // Timing, Amount, Missing, Duplicate

    // 备注和附件
    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty;

    [StringLength(1000)]
    public string AttachmentUrls { get; set; } = string.Empty; // 附件链接（JSON格式）

    // 审计跟踪
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 关联实体
    public virtual ICollection<ReconciliationItem> ReconciliationItems { get; set; } = new List<ReconciliationItem>();
    public virtual ICollection<ReconciliationDifference> ReconciliationDifferences { get; set; } = new List<ReconciliationDifference>();

    // 计算属性
    public decimal ReconciliationRate => TotalItemsCount > 0 ?
        (decimal)ReconciledItemsCount / TotalItemsCount * 100 : 0;

    public bool HasDifferences => DifferenceAmount != 0 || DifferenceItemsCount > 0;

    public bool IsCompleted => Status == "Completed";

    public string StatusDescription => Status switch
    {
        "InProgress" => "进行中",
        "Completed" => "已完成",
        "Failed" => "失败",
        "Cancelled" => "已取消",
        _ => "未知"
    };

    public int DaysToComplete => CompletedDate.HasValue ?
        (int)(CompletedDate.Value - CreatedAt).TotalDays :
        (int)(DateTime.UtcNow - CreatedAt).TotalDays;
}