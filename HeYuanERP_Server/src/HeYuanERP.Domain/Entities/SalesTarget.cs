using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("SalesTargets")]
public class SalesTarget
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TargetName { get; set; } = string.Empty;

    [Required]
    public int UserId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    public int Year { get; set; }

    [Required]
    public int Month { get; set; }

    [Required]
    [StringLength(50)]
    public string Period { get; set; } = string.Empty; // Monthly, Quarterly, Yearly

    [Required]
    [StringLength(50)]
    public string TargetType { get; set; } = string.Empty; // Revenue, Quantity, Visits, Opportunities

    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ActualValue { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal CompletionRate { get; set; } // 完成率百分比

    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Completed, Cancelled

    [StringLength(100)]
    public string ProductCategory { get; set; } = string.Empty; // 产品类别

    [StringLength(100)]
    public string CustomerSegment { get; set; } = string.Empty; // 客户细分

    [StringLength(100)]
    public string Region { get; set; } = string.Empty; // 区域

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(1000)]
    public string ActionPlan { get; set; } = string.Empty; // 行动计划

    [StringLength(500)]
    public string KPIMetrics { get; set; } = string.Empty; // 关键绩效指标

    // 权重（用于复合目标）
    [Column(TypeName = "decimal(5,2)")]
    public decimal Weight { get; set; } = 100.00m;

    // 奖励机制
    [Column(TypeName = "decimal(10,2)")]
    public decimal BonusRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxBonus { get; set; }

    // 风险评估
    [StringLength(50)]
    public string RiskLevel { get; set; } = "Medium"; // Low, Medium, High

    [StringLength(500)]
    public string RiskFactors { get; set; } = string.Empty;

    // 进度跟踪
    public DateTime? LastUpdated { get; set; }

    [StringLength(1000)]
    public string ProgressNotes { get; set; } = string.Empty;

    // 季度里程碑
    [Column(TypeName = "decimal(18,2)")]
    public decimal Q1Target { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Q2Target { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Q3Target { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Q4Target { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Q1Actual { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Q2Actual { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Q3Actual { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Q4Actual { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 关联的销售机会数量
    public int RelatedOpportunities { get; set; }

    // 平均销售周期
    public int AverageSalesCycle { get; set; }
}