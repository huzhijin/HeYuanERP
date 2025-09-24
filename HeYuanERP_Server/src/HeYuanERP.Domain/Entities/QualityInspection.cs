#if false
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 质量检验计划实体
/// </summary>
public class QualityInspectionPlan
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public InspectionType Type { get; set; } = InspectionType.Incoming;

    [Required]
    public InspectionScope Scope { get; set; } = InspectionScope.Sample;

    [MaxLength(50)]
    public string? ProductId { get; set; }

    [MaxLength(50)]
    public string? SupplierId { get; set; }

    [MaxLength(50)]
    public string? CategoryId { get; set; }

    public InspectionSchedule Schedule { get; set; } = new();

    public List<InspectionStandard> Standards { get; set; } = new();

    public List<InspectionItem> Items { get; set; } = new();

    public InspectionSampling Sampling { get; set; } = new();

    public QualityRequirements Requirements { get; set; } = new();

    public bool IsActive { get; set; } = true;

    public int Priority { get; set; } = 1;

    [Required]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    [MaxLength(1000)]
    public string Tags { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; set; } = string.Empty;

    public Dictionary<string, object> ExtensionData { get; set; } = new();

    // 导航属性
    public List<QualityInspection> Inspections { get; set; } = new();
    public List<QualityInspectionHistory> History { get; set; } = new();
}

/// <summary>
/// 检验调度配置
/// </summary>
public class InspectionSchedule
{
    public bool IsEnabled { get; set; } = true;

    public ScheduleFrequency Frequency { get; set; } = ScheduleFrequency.OnDemand;

    public ScheduleTrigger Trigger { get; set; } = ScheduleTrigger.Manual;

    public List<string> TriggerEvents { get; set; } = new();

    public int IntervalDays { get; set; } = 1;

    public TimeSpan PreferredTime { get; set; } = new(9, 0, 0);

    public bool AutoCreate { get; set; } = false;

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 检验标准
/// </summary>
public class InspectionStandard
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public StandardType Type { get; set; } = StandardType.National;

    [MaxLength(100)]
    public string Version { get; set; } = string.Empty;

    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiryDate { get; set; }

    public bool IsRequired { get; set; } = true;

    public StandardScope Scope { get; set; } = StandardScope.Product;

    public List<StandardRequirement> Requirements { get; set; } = new();

    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 标准要求
/// </summary>
public class StandardRequirement
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public RequirementType Type { get; set; } = RequirementType.Measurement;

    public object? MinValue { get; set; }

    public object? MaxValue { get; set; }

    public object? TargetValue { get; set; }

    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    public decimal Tolerance { get; set; } = 0;

    public bool IsCritical { get; set; } = false;

    public CriteriaType CriteriaType { get; set; } = CriteriaType.Range;

    public List<string> AcceptableValues { get; set; } = new();
}

/// <summary>
/// 检验项目
/// </summary>
public class InspectionItem
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public InspectionMethod Method { get; set; } = InspectionMethod.Visual;

    [Required]
    public InspectionCategory Category { get; set; } = InspectionCategory.Appearance;

    public bool IsRequired { get; set; } = true;

    public bool IsCritical { get; set; } = false;

    public int Sequence { get; set; } = 0;

    public InspectionItemConfiguration Configuration { get; set; } = new();

    public List<InspectionCriteria> Criteria { get; set; } = new();

    public List<string> RequiredTools { get; set; } = new();

    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 检验项目配置
/// </summary>
public class InspectionItemConfiguration
{
    public int SampleSize { get; set; } = 1;

    public bool AllowSkip { get; set; } = false;

    public bool RequirePhotos { get; set; } = false;

    public bool RequireComments { get; set; } = false;

    public int TimeoutMinutes { get; set; } = 30;

    public decimal PassThreshold { get; set; } = 100;

    public List<string> DefaultValues { get; set; } = new();

    public Dictionary<string, object> ValidationRules { get; set; } = new();
}

/// <summary>
/// 检验标准
/// </summary>
public class InspectionCriteria
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public CriteriaType Type { get; set; } = CriteriaType.Range;

    public object? ExpectedValue { get; set; }

    public object? MinValue { get; set; }

    public object? MaxValue { get; set; }

    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    public decimal Tolerance { get; set; } = 0;

    public bool IsCritical { get; set; } = false;

    public AcceptanceCriteria AcceptanceCriteria { get; set; } = AcceptanceCriteria.MustPass;

    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 检验抽样配置
/// </summary>
public class InspectionSampling
{
    public SamplingMethod Method { get; set; } = SamplingMethod.Random;

    public SamplingLevel Level { get; set; } = SamplingLevel.Normal;

    public int SampleSize { get; set; } = 1;

    public decimal SamplePercentage { get; set; } = 10;

    public int MinSampleSize { get; set; } = 1;

    public int MaxSampleSize { get; set; } = 100;

    public SamplingPlan Plan { get; set; } = new();

    public bool UseStatisticalSampling { get; set; } = false;

    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 抽样计划
/// </summary>
public class SamplingPlan
{
    [MaxLength(50)]
    public string PlanCode { get; set; } = string.Empty;

    public int LotSize { get; set; } = 0;

    public int AcceptanceNumber { get; set; } = 0;

    public int RejectionNumber { get; set; } = 1;

    public AQLLevel AQLLevel { get; set; } = AQLLevel.Normal;

    public decimal AcceptableQualityLevel { get; set; } = 1.0m;

    public InspectionLevel InspectionLevel { get; set; } = InspectionLevel.II;

    public Dictionary<string, object> StatisticalParameters { get; set; } = new();
}

/// <summary>
/// 质量要求
/// </summary>
public class QualityRequirements
{
    public decimal AcceptanceRate { get; set; } = 95;

    public decimal CriticalDefectRate { get; set; } = 0;

    public decimal MajorDefectRate { get; set; } = 2.5m;

    public decimal MinorDefectRate { get; set; } = 4.0m;

    public QualityAction OnFailure { get; set; } = QualityAction.Reject;

    public QualityAction OnCriticalFailure { get; set; } = QualityAction.Stop;

    public bool RequireManagerApproval { get; set; } = false;

    public List<QualityEscalation> Escalations { get; set; } = new();

    public Dictionary<string, object> CustomRequirements { get; set; } = new();
}

/// <summary>
/// 质量升级规则
/// </summary>
public class QualityEscalation
{
    [Required]
    public EscalationTrigger Trigger { get; set; } = EscalationTrigger.CriticalDefect;

    public List<string> Recipients { get; set; } = new();

    [MaxLength(500)]
    public string NotificationTemplate { get; set; } = string.Empty;

    public bool StopProduction { get; set; } = false;

    public bool RequireImmedateAction { get; set; } = false;

    public Dictionary<string, object> Actions { get; set; } = new();
}

/// <summary>
/// 质量检验实体
/// </summary>
public class QualityInspection
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string PlanId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BatchNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? OrderId { get; set; }

    [MaxLength(50)]
    public string? ProductId { get; set; }

    [MaxLength(50)]
    public string? SupplierId { get; set; }

    [Required]
    public InspectionStatus Status { get; set; } = InspectionStatus.Pending;

    public InspectionResult Result { get; set; } = InspectionResult.Unknown;

    public InspectionSummary Summary { get; set; } = new();

    [Required]
    [MaxLength(50)]
    public string InspectorId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ReviewerId { get; set; }

    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public int TotalItems { get; set; } = 0;

    public int InspectedItems { get; set; } = 0;

    public int PassedItems { get; set; } = 0;

    public int FailedItems { get; set; } = 0;

    public decimal PassRate { get; set; } = 0;

    public List<DefectRecord> Defects { get; set; } = new();

    public InspectionEnvironment Environment { get; set; } = new();

    [MaxLength(2000)]
    public string? Comments { get; set; }

    [MaxLength(2000)]
    public string? ReviewComments { get; set; }

    public Dictionary<string, object> ExtensionData { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(PlanId))]
    public QualityInspectionPlan Plan { get; set; } = null!;

    public List<InspectionItemResult> ItemResults { get; set; } = new();
    public List<InspectionPhoto> Photos { get; set; } = new();
    public List<InspectionDocument> Documents { get; set; } = new();
    public List<QualityInspectionHistory> History { get; set; } = new();
}

/// <summary>
/// 检验摘要
/// </summary>
public class InspectionSummary
{
    public int TotalCriteria { get; set; } = 0;

    public int PassedCriteria { get; set; } = 0;

    public int FailedCriteria { get; set; } = 0;

    public int CriticalDefects { get; set; } = 0;

    public int MajorDefects { get; set; } = 0;

    public int MinorDefects { get; set; } = 0;

    public decimal OverallScore { get; set; } = 0;

    public QualityGrade Grade { get; set; } = QualityGrade.Unknown;

    public RecommendedAction RecommendedAction { get; set; } = RecommendedAction.None;

    public Dictionary<string, object> Metrics { get; set; } = new();
}

/// <summary>
/// 缺陷记录
/// </summary>
public class DefectRecord
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DefectType Type { get; set; } = DefectType.Minor;

    [Required]
    public DefectSeverity Severity { get; set; } = DefectSeverity.Low;

    [Required]
    public DefectCategory Category { get; set; } = DefectCategory.Appearance;

    [MaxLength(100)]
    public string? ItemCode { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public int Quantity { get; set; } = 1;

    public DefectCause Cause { get; set; } = DefectCause.Unknown;

    [MaxLength(500)]
    public string? CauseDescription { get; set; }

    public DefectAction Action { get; set; } = DefectAction.Record;

    [MaxLength(500)]
    public string? ActionDescription { get; set; }

    [MaxLength(50)]
    public string? ResponsibleParty { get; set; }

    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string DetectedBy { get; set; } = string.Empty;

    public DefectStatus Status { get; set; } = DefectStatus.Open;

    public List<string> PhotoIds { get; set; } = new();

    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// 检验环境
/// </summary>
public class InspectionEnvironment
{
    public decimal Temperature { get; set; } = 20;

    public decimal Humidity { get; set; } = 50;

    public decimal Pressure { get; set; } = 1013.25m;

    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Equipment { get; set; } = string.Empty;

    public List<string> EnvironmentalConditions { get; set; } = new();

    public Dictionary<string, object> CustomParameters { get; set; } = new();
}

/// <summary>
/// 检验项目结果
/// </summary>
public class InspectionItemResult
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string InspectionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    public InspectionResult Result { get; set; } = InspectionResult.Unknown;

    public object? MeasuredValue { get; set; }

    public object? ExpectedValue { get; set; }

    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    public decimal Deviation { get; set; } = 0;

    public bool IsWithinTolerance { get; set; } = true;

    public List<CriteriaResult> CriteriaResults { get; set; } = new();

    [MaxLength(1000)]
    public string? Comments { get; set; }

    [Required]
    [MaxLength(50)]
    public string InspectorId { get; set; } = string.Empty;

    public DateTime InspectedAt { get; set; } = DateTime.UtcNow;

    public List<string> PhotoIds { get; set; } = new();

    public Dictionary<string, object> RawData { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(InspectionId))]
    public QualityInspection Inspection { get; set; } = null!;
}

/// <summary>
/// 标准结果
/// </summary>
public class CriteriaResult
{
    [Required]
    [MaxLength(100)]
    public string CriteriaName { get; set; } = string.Empty;

    [Required]
    public InspectionResult Result { get; set; } = InspectionResult.Unknown;

    public object? ActualValue { get; set; }

    public object? ExpectedValue { get; set; }

    public decimal Score { get; set; } = 0;

    public bool IsCritical { get; set; } = false;

    [MaxLength(500)]
    public string? Comments { get; set; }
}

/// <summary>
/// 检验照片
/// </summary>
public class InspectionPhoto
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string InspectionId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ItemResultId { get; set; }

    [MaxLength(50)]
    public string? DefectId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; } = 0;

    [MaxLength(500)]
    public string? Description { get; set; }

    public PhotoType Type { get; set; } = PhotoType.Inspection;

    [Required]
    [MaxLength(50)]
    public string TakenBy { get; set; } = string.Empty;

    public DateTime TakenAt { get; set; } = DateTime.UtcNow;

    public PhotoMetadata Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(InspectionId))]
    public QualityInspection Inspection { get; set; } = null!;
}

/// <summary>
/// 照片元数据
/// </summary>
public class PhotoMetadata
{
    public int Width { get; set; } = 0;

    public int Height { get; set; } = 0;

    [MaxLength(100)]
    public string? CameraModel { get; set; }

    public DateTime? TimestampOriginal { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public Dictionary<string, object> Exif { get; set; } = new();
}

/// <summary>
/// 检验文档
/// </summary>
public class InspectionDocument
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string InspectionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; } = 0;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DocumentType Type { get; set; } = DocumentType.Report;

    [Required]
    [MaxLength(50)]
    public string UploadedBy { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(InspectionId))]
    public QualityInspection Inspection { get; set; } = null!;
}

/// <summary>
/// 质量检验历史
/// </summary>
public class QualityInspectionHistory
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [MaxLength(50)]
    public string? PlanId { get; set; }

    [MaxLength(50)]
    public string? InspectionId { get; set; }

    [Required]
    public InspectionHistoryAction Action { get; set; } = InspectionHistoryAction.Created;

    [Required]
    [MaxLength(50)]
    public string PerformedBy { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public object? OldValue { get; set; }

    public object? NewValue { get; set; }

    [MaxLength(2000)]
    public string? Comments { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(PlanId))]
    public QualityInspectionPlan? Plan { get; set; }

    [ForeignKey(nameof(InspectionId))]
    public QualityInspection? Inspection { get; set; }
}

/// <summary>
/// 质量报告
/// </summary>
public class QualityReport
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public QualityReportType Type { get; set; } = QualityReportType.Inspection;

    public DateTime PeriodStart { get; set; } = DateTime.UtcNow.Date;

    public DateTime PeriodEnd { get; set; } = DateTime.UtcNow.Date;

    public QualityReportScope Scope { get; set; } = new();

    public QualityMetrics Metrics { get; set; } = new();

    public List<QualityTrend> Trends { get; set; } = new();

    public List<QualityRecommendation> Recommendations { get; set; } = new();

    [Required]
    [MaxLength(50)]
    public string GeneratedBy { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public QualityReportStatus Status { get; set; } = QualityReportStatus.Draft;

    [MaxLength(500)]
    public string? FilePath { get; set; }

    public Dictionary<string, object> ExtensionData { get; set; } = new();
}

/// <summary>
/// 质量报告范围
/// </summary>
public class QualityReportScope
{
    public List<string> ProductIds { get; set; } = new();

    public List<string> SupplierIds { get; set; } = new();

    public List<string> CategoryIds { get; set; } = new();

    public List<InspectionType> InspectionTypes { get; set; } = new();

    public bool IncludeDefects { get; set; } = true;

    public bool IncludeTrends { get; set; } = true;

    public bool IncludeRecommendations { get; set; } = true;

    public Dictionary<string, object> Filters { get; set; } = new();
}

/// <summary>
/// 质量指标
/// </summary>
public class QualityMetrics
{
    public int TotalInspections { get; set; } = 0;

    public int PassedInspections { get; set; } = 0;

    public int FailedInspections { get; set; } = 0;

    public decimal PassRate { get; set; } = 0;

    public int TotalDefects { get; set; } = 0;

    public int CriticalDefects { get; set; } = 0;

    public int MajorDefects { get; set; } = 0;

    public int MinorDefects { get; set; } = 0;

    public decimal DefectRate { get; set; } = 0;

    public decimal AverageInspectionTime { get; set; } = 0;

    public Dictionary<string, decimal> CategoryMetrics { get; set; } = new();

    public Dictionary<string, decimal> SupplierMetrics { get; set; } = new();

    public Dictionary<string, decimal> ProductMetrics { get; set; } = new();
}

/// <summary>
/// 质量趋势
/// </summary>
public class QualityTrend
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TrendType Type { get; set; } = TrendType.PassRate;

    public List<TrendDataPoint> DataPoints { get; set; } = new();

    public TrendDirection Direction { get; set; } = TrendDirection.Stable;

    public decimal TrendValue { get; set; } = 0;

    [MaxLength(500)]
    public string? Analysis { get; set; }
}

/// <summary>
/// 趋势数据点
/// </summary>
public class TrendDataPoint
{
    public DateTime Date { get; set; }

    public decimal Value { get; set; }

    [MaxLength(100)]
    public string Label { get; set; } = string.Empty;

    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// 质量建议
/// </summary>
public class QualityRecommendation
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public RecommendationType Type { get; set; } = RecommendationType.Process;

    public RecommendationPriority Priority { get; set; } = RecommendationPriority.Medium;

    [MaxLength(100)]
    public string? TargetArea { get; set; }

    public decimal? EstimatedImpact { get; set; }

    [MaxLength(500)]
    public string? ImplementationGuidance { get; set; }

    public List<string> RelatedDefects { get; set; } = new();

    public Dictionary<string, object> Metadata { get; set; } = new();
}
// Disabled non-P0 domain entities for minimal build
#endif
