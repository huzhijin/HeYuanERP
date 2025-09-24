using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("SalesOpportunities")]
public class SalesOpportunity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string OpportunityName { get; set; } = string.Empty;

    [Required]
    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Prospecting"; // Prospecting, Qualification, Proposal, Negotiation, Closed-Won, Closed-Lost

    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedValue { get; set; }

    [Range(0, 100)]
    public int Probability { get; set; } = 10;

    [StringLength(50)]
    public string Stage { get; set; } = "Initial";

    [StringLength(50)]
    public string Source { get; set; } = string.Empty; // 来源：电话营销、网站、推荐等

    public DateTime ExpectedCloseDate { get; set; }

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    [Required]
    public int AssignedToUserId { get; set; }

    [StringLength(100)]
    public string AssignedToUserName { get; set; } = string.Empty;

    [StringLength(50)]
    public string Priority { get; set; } = "Medium"; // High, Medium, Low

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 竞争对手信息
    [StringLength(500)]
    public string Competitors { get; set; } = string.Empty;

    // 决策者信息
    [StringLength(200)]
    public string DecisionMakers { get; set; } = string.Empty;

    // 下次跟进时间
    public DateTime? NextFollowUp { get; set; }

    // 损失原因（如果状态为Closed-Lost）
    [StringLength(500)]
    public string LossReason { get; set; } = string.Empty;

    // 产品类别
    [StringLength(100)]
    public string ProductCategory { get; set; } = string.Empty;

    // 销售周期（天数）
    public int SalesCycleDays { get; set; }
}