using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("CustomerVisits")]
public class CustomerVisit
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    public int? SalesOpportunityId { get; set; }

    [ForeignKey("SalesOpportunityId")]
    public virtual SalesOpportunity? SalesOpportunity { get; set; }

    [Required]
    public DateTime VisitDate { get; set; }

    [Required]
    [StringLength(50)]
    public string VisitType { get; set; } = string.Empty; // 拜访类型：初访、回访、投标、签约、售后等

    [Required]
    [StringLength(50)]
    public string Purpose { get; set; } = string.Empty; // 拜访目的

    [StringLength(200)]
    public string ContactPerson { get; set; } = string.Empty; // 联系人

    [StringLength(100)]
    public string ContactPosition { get; set; } = string.Empty; // 联系人职位

    [StringLength(50)]
    public string ContactPhone { get; set; } = string.Empty;

    [StringLength(100)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string VisitContent { get; set; } = string.Empty; // 拜访内容

    [StringLength(1000)]
    public string CustomerFeedback { get; set; } = string.Empty; // 客户反馈

    [StringLength(1000)]
    public string NextSteps { get; set; } = string.Empty; // 下一步行动

    public DateTime? NextVisitDate { get; set; } // 下次拜访计划

    [Required]
    [StringLength(50)]
    public string VisitResult { get; set; } = string.Empty; // 拜访结果：成功、一般、失败

    [Range(1, 5)]
    public int CustomerSatisfaction { get; set; } = 3; // 客户满意度 1-5分

    [StringLength(500)]
    public string CompetitorInfo { get; set; } = string.Empty; // 竞争对手信息

    [StringLength(500)]
    public string MarketIntelligence { get; set; } = string.Empty; // 市场情报

    // 拜访花费
    [Column(TypeName = "decimal(10,2)")]
    public decimal ExpenseCost { get; set; }

    [StringLength(200)]
    public string ExpenseDescription { get; set; } = string.Empty;

    [Required]
    public int VisitedByUserId { get; set; }

    [StringLength(100)]
    public string VisitedByUserName { get; set; } = string.Empty;

    // 同行人员
    [StringLength(200)]
    public string Companions { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 拜访地址
    [StringLength(500)]
    public string VisitAddress { get; set; } = string.Empty;

    // 拜访持续时间（分钟）
    public int DurationMinutes { get; set; }

    // 是否达成意向
    public bool IntentionReached { get; set; }

    // 预估成交金额
    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedDealAmount { get; set; }
}