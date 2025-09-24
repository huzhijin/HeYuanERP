using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 对账差异记录 - 记录发票与发货的差异 (P1扩展)
/// </summary>
[Table("ReconciliationDifferences")]
public class ReconciliationDifference
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 差异编号
    /// </summary>
    [Required]
    [StringLength(50)]
    public string DifferenceNo { get; set; } = string.Empty;

    /// <summary>
    /// 对账记录ID
    /// </summary>
    public int? ReconciliationRecordId { get; set; }

    [ForeignKey("ReconciliationRecordId")]
    public virtual ReconciliationRecord? ReconciliationRecord { get; set; }

    /// <summary>
    /// 订单ID
    /// </summary>
    public string? OrderId { get; set; }

    /// <summary>
    /// 发货单ID
    /// </summary>
    public string? DeliveryId { get; set; }

    /// <summary>
    /// 发票ID
    /// </summary>
    public string? InvoiceId { get; set; }

    /// <summary>
    /// 产品ID
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// 差异类型
    /// </summary>
    public ReconciliationDifferenceType Type { get; set; }

    /// <summary>
    /// 差异描述
    /// </summary>
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 发货数量
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal DeliveryQuantity { get; set; }

    /// <summary>
    /// 发票数量
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal InvoiceQuantity { get; set; }

    /// <summary>
    /// 差异数量
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal DifferenceQuantity { get; set; }

    /// <summary>
    /// 发货金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DeliveryAmount { get; set; }

    /// <summary>
    /// 发票金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal InvoiceAmount { get; set; }

    /// <summary>
    /// 差异金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DifferenceAmount { get; set; }

    /// <summary>
    /// 差异状态
    /// </summary>
    public ReconciliationStatus Status { get; set; } = ReconciliationStatus.Pending;

    /// <summary>
    /// 处理方案
    /// </summary>
    [StringLength(1000)]
    public string? Resolution { get; set; }

    /// <summary>
    /// 处理人
    /// </summary>
    [StringLength(100)]
    public string? HandledBy { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? HandledAt { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    [StringLength(2000)]
    public string? HandledRemark { get; set; }

    // P1扩展字段
    /// <summary>
    /// 严重程度
    /// </summary>
    [StringLength(50)]
    public string Severity { get; set; } = "Medium"; // Low, Medium, High, Critical

    /// <summary>
    /// 根本原因分类
    /// </summary>
    [StringLength(50)]
    public string RootCauseCategory { get; set; } = string.Empty;

    /// <summary>
    /// 根本原因分析
    /// </summary>
    [StringLength(1000)]
    public string RootCauseAnalysis { get; set; } = string.Empty;

    /// <summary>
    /// 解决类型
    /// </summary>
    [StringLength(50)]
    public string ResolutionType { get; set; } = string.Empty; // Adjustment, Correction, Waiver

    /// <summary>
    /// 责任部门
    /// </summary>
    [StringLength(100)]
    public string ResponsibleDepartment { get; set; } = string.Empty;

    /// <summary>
    /// 分配给用户ID
    /// </summary>
    public int? AssignedToUserId { get; set; }

    /// <summary>
    /// 分配给用户名
    /// </summary>
    [StringLength(100)]
    public string AssignedToUserName { get; set; } = string.Empty;

    /// <summary>
    /// 目标解决日期
    /// </summary>
    public DateTime? TargetResolutionDate { get; set; }

    /// <summary>
    /// 实际解决日期
    /// </summary>
    public DateTime? ActualResolutionDate { get; set; }

    /// <summary>
    /// 影响评估
    /// </summary>
    [StringLength(1000)]
    public string ImpactAssessment { get; set; } = string.Empty;

    /// <summary>
    /// 财务影响
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal FinancialImpact { get; set; }

    /// <summary>
    /// 需要审批
    /// </summary>
    public bool RequiresApproval { get; set; } = false;

    /// <summary>
    /// 审批人ID
    /// </summary>
    public int? ApprovalManagerId { get; set; }

    /// <summary>
    /// 审批人名称
    /// </summary>
    [StringLength(100)]
    public string ApprovalManagerName { get; set; } = string.Empty;

    /// <summary>
    /// 审批日期
    /// </summary>
    public DateTime? ApprovalDate { get; set; }

    /// <summary>
    /// 调整分录ID
    /// </summary>
    public int? AdjustmentEntryId { get; set; }

    /// <summary>
    /// 调整分录号
    /// </summary>
    [StringLength(50)]
    public string AdjustmentEntryNumber { get; set; } = string.Empty;

    /// <summary>
    /// 优先级
    /// </summary>
    [StringLength(50)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High

    /// <summary>
    /// 操作历史
    /// </summary>
    [StringLength(2000)]
    public string ActionHistory { get; set; } = string.Empty; // JSON格式

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    [StringLength(100)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 导航属性：关联订单
    /// </summary>
    public SalesOrder? Order { get; set; }

    /// <summary>
    /// 导航属性：关联发货单
    /// </summary>
    public Delivery? Delivery { get; set; }

    /// <summary>
    /// 导航属性：关联发票
    /// </summary>
    public Invoice? Invoice { get; set; }

    /// <summary>
    /// 导航属性：关联产品
    /// </summary>
    public Product? Product { get; set; }

    // 计算属性
    public bool IsResolved => Status == ReconciliationStatus.Resolved;

    public bool IsOverdue => TargetResolutionDate.HasValue &&
                            DateTime.UtcNow > TargetResolutionDate.Value &&
                            !IsResolved;

    public decimal AbsoluteDifferenceAmount => Math.Abs(DifferenceAmount);

    public bool RequiresUrgentAttention => Severity == "Critical" ||
                                          (Severity == "High" && IsOverdue);
}

/// <summary>
/// 对账差异类型
/// </summary>
public enum ReconciliationDifferenceType
{
    /// <summary>
    /// 数量差异
    /// </summary>
    QuantityDifference = 1,

    /// <summary>
    /// 金额差异
    /// </summary>
    AmountDifference = 2,

    /// <summary>
    /// 税率差异
    /// </summary>
    TaxRateDifference = 3,

    /// <summary>
    /// 超量开票
    /// </summary>
    OverInvoicing = 4,

    /// <summary>
    /// 超额开票
    /// </summary>
    OverAmount = 5,

    /// <summary>
    /// 产品不匹配
    /// </summary>
    ProductMismatch = 6,

    /// <summary>
    /// 价格差异
    /// </summary>
    PriceDifference = 7
}

/// <summary>
/// 对账状态
/// </summary>
public enum ReconciliationStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 处理中
    /// </summary>
    Processing = 2,

    /// <summary>
    /// 已解决
    /// </summary>
    Resolved = 3,

    /// <summary>
    /// 已忽略
    /// </summary>
    Ignored = 4,

    /// <summary>
    /// 需要调整
    /// </summary>
    RequiresAdjustment = 5
}