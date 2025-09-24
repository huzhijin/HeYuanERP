namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 采购异常记录 - 记录采购过程中的各种异常情况
/// </summary>
public class PurchaseException
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 异常编号
    /// </summary>
    public string ExceptionNo { get; set; } = string.Empty;

    /// <summary>
    /// 采购订单ID
    /// </summary>
    public string? PurchaseOrderId { get; set; }

    /// <summary>
    /// 收货单ID（如果相关）
    /// </summary>
    public string? ReceiptId { get; set; }

    /// <summary>
    /// 产品ID
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    public string? SupplierId { get; set; }

    /// <summary>
    /// 异常类型
    /// </summary>
    public PurchaseExceptionType Type { get; set; }

    /// <summary>
    /// 异常级别
    /// </summary>
    public ExceptionLevel Level { get; set; } = ExceptionLevel.Medium;

    /// <summary>
    /// 异常标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 异常描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 预期数量
    /// </summary>
    public decimal ExpectedQuantity { get; set; }

    /// <summary>
    /// 实际数量
    /// </summary>
    public decimal ActualQuantity { get; set; }

    /// <summary>
    /// 差异数量
    /// </summary>
    public decimal DifferenceQuantity { get; set; }

    /// <summary>
    /// 预期金额
    /// </summary>
    public decimal ExpectedAmount { get; set; }

    /// <summary>
    /// 实际金额
    /// </summary>
    public decimal ActualAmount { get; set; }

    /// <summary>
    /// 差异金额
    /// </summary>
    public decimal DifferenceAmount { get; set; }

    /// <summary>
    /// 预期交货日期
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// 实际交货日期
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }

    /// <summary>
    /// 延期天数
    /// </summary>
    public int DelayDays { get; set; }

    /// <summary>
    /// 异常状态
    /// </summary>
    public PurchaseExceptionStatus Status { get; set; } = PurchaseExceptionStatus.Open;

    /// <summary>
    /// 处理方案
    /// </summary>
    public string? Resolution { get; set; }

    /// <summary>
    /// 处理人
    /// </summary>
    public string? HandledBy { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? HandledAt { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    public string? HandledRemark { get; set; }

    /// <summary>
    /// 影响评估
    /// </summary>
    public string? ImpactAssessment { get; set; }

    /// <summary>
    /// 预防措施
    /// </summary>
    public string? PreventiveMeasures { get; set; }

    /// <summary>
    /// 根本原因分析
    /// </summary>
    public string? RootCauseAnalysis { get; set; }

    /// <summary>
    /// 质量问题详情（针对质量异常）
    /// </summary>
    public QualityIssueDetails? QualityDetails { get; set; }

    /// <summary>
    /// 扩展数据（JSON格式）
    /// </summary>
    public Dictionary<string, object> ExtensionData { get; set; } = new();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 导航属性：关联采购订单
    /// </summary>
    public PurchaseOrder? PurchaseOrder { get; set; }

    /// <summary>
    /// 导航属性：关联供应商（复用 Account 实体）
    /// </summary>
    public Account? Supplier { get; set; }

    /// <summary>
    /// 导航属性：关联产品
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// 导航属性：异常处理记录
    /// </summary>
    public List<PurchaseExceptionHandlingRecord> HandlingRecords { get; set; } = new();
}

/// <summary>
/// 质量问题详情
/// </summary>
public class QualityIssueDetails
{
    /// <summary>
    /// 质量问题类型
    /// </summary>
    public QualityIssueType IssueType { get; set; }

    /// <summary>
    /// 不合格数量
    /// </summary>
    public decimal DefectiveQuantity { get; set; }

    /// <summary>
    /// 不合格描述
    /// </summary>
    public string DefectDescription { get; set; } = string.Empty;

    /// <summary>
    /// 检验标准
    /// </summary>
    public string InspectionStandard { get; set; } = string.Empty;

    /// <summary>
    /// 检验员
    /// </summary>
    public string? Inspector { get; set; }

    /// <summary>
    /// 检验时间
    /// </summary>
    public DateTime? InspectionDate { get; set; }

    /// <summary>
    /// 照片/文档附件URL列表
    /// </summary>
    public List<string> AttachmentUrls { get; set; } = new();

    /// <summary>
    /// 处理建议
    /// </summary>
    public string? ProcessingSuggestion { get; set; }
}

/// <summary>
/// 采购异常处理记录
/// </summary>
public class PurchaseExceptionHandlingRecord
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 异常ID
    /// </summary>
    public string ExceptionId { get; set; } = string.Empty;

    /// <summary>
    /// 处理动作
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 处理描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 处理人
    /// </summary>
    public string HandledBy { get; set; } = string.Empty;

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime HandledAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 状态变更：从
    /// </summary>
    public PurchaseExceptionStatus? StatusFrom { get; set; }

    /// <summary>
    /// 状态变更：到
    /// </summary>
    public PurchaseExceptionStatus? StatusTo { get; set; }

    /// <summary>
    /// 附件URL列表
    /// </summary>
    public List<string> AttachmentUrls { get; set; } = new();

    /// <summary>
    /// 导航属性：关联异常
    /// </summary>
    public PurchaseException? Exception { get; set; }
}

/// <summary>
/// 采购异常类型
/// </summary>
public enum PurchaseExceptionType
{
    /// <summary>
    /// 超收异常（实际收货数量超过订单数量）
    /// </summary>
    OverReceive = 1,

    /// <summary>
    /// 短收异常（实际收货数量少于订单数量）
    /// </summary>
    ShortReceive = 2,

    /// <summary>
    /// 质量异常（质量不符合要求）
    /// </summary>
    QualityIssue = 3,

    /// <summary>
    /// 价格差异（实际价格与采购价格不符）
    /// </summary>
    PriceDifference = 4,

    /// <summary>
    /// 交期异常（延期交货）
    /// </summary>
    DeliveryDelay = 5,

    /// <summary>
    /// 供应商异常（供应商相关问题）
    /// </summary>
    SupplierIssue = 6,

    /// <summary>
    /// 包装异常（包装不符合要求）
    /// </summary>
    PackagingIssue = 7,

    /// <summary>
    /// 规格差异（产品规格与订单不符）
    /// </summary>
    SpecificationMismatch = 8,

    /// <summary>
    /// 其他异常
    /// </summary>
    Other = 99
}

/// <summary>
/// 质量问题类型
/// </summary>
public enum QualityIssueType
{
    /// <summary>
    /// 外观缺陷
    /// </summary>
    AppearanceDefect = 1,

    /// <summary>
    /// 尺寸偏差
    /// </summary>
    DimensionDeviation = 2,

    /// <summary>
    /// 功能故障
    /// </summary>
    FunctionalFailure = 3,

    /// <summary>
    /// 材质问题
    /// </summary>
    MaterialIssue = 4,

    /// <summary>
    /// 安全隐患
    /// </summary>
    SafetyHazard = 5,

    /// <summary>
    /// 标识错误
    /// </summary>
    LabelingError = 6,

    /// <summary>
    /// 包装破损
    /// </summary>
    PackagingDamage = 7,

    /// <summary>
    /// 其他质量问题
    /// </summary>
    Other = 99
}

/// <summary>
/// 异常级别
/// </summary>
public enum ExceptionLevel
{
    /// <summary>
    /// 低级别（影响小）
    /// </summary>
    Low = 1,

    /// <summary>
    /// 中级别（一般影响）
    /// </summary>
    Medium = 2,

    /// <summary>
    /// 高级别（严重影响）
    /// </summary>
    High = 3,

    /// <summary>
    /// 紧急级别（极严重影响）
    /// </summary>
    Critical = 4
}

/// <summary>
/// 采购异常状态
/// </summary>
public enum PurchaseExceptionStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Open = 1,

    /// <summary>
    /// 处理中
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// 待确认
    /// </summary>
    PendingConfirmation = 3,

    /// <summary>
    /// 已解决
    /// </summary>
    Resolved = 4,

    /// <summary>
    /// 已关闭
    /// </summary>
    Closed = 5,

    /// <summary>
    /// 重新开放
    /// </summary>
    Reopened = 6,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 7
}
