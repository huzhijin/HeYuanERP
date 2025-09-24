// Disabled in minimal build: Quality inspection service not enabled
#if false
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Api.Services.Quality;

public interface IQualityInspectionService
{
    // 检验计划管理
    Task<QualityInspectionPlan?> GetInspectionPlanAsync(string planId);
    Task<QualityInspectionPlan> CreateInspectionPlanAsync(QualityInspectionPlan plan);
    Task<QualityInspectionPlan> UpdateInspectionPlanAsync(QualityInspectionPlan plan);
    Task<bool> DeleteInspectionPlanAsync(string planId);
    Task<List<QualityInspectionPlan>> GetInspectionPlansAsync(int skip = 0, int take = 20);
    Task<List<QualityInspectionPlan>> SearchInspectionPlansAsync(string searchTerm, int skip = 0, int take = 20);
    Task<List<QualityInspectionPlan>> GetInspectionPlansByTypeAsync(InspectionType type);
    Task<List<QualityInspectionPlan>> GetActiveInspectionPlansAsync();
    Task<bool> ActivateInspectionPlanAsync(string planId, string activatedBy);
    Task<bool> DeactivateInspectionPlanAsync(string planId, string deactivatedBy);

    // 检验标准管理
    Task<bool> UpdateInspectionStandardsAsync(string planId, List<InspectionStandard> standards);
    Task<List<InspectionStandard>> GetInspectionStandardsAsync(string planId);
    Task<bool> ValidateInspectionStandardsAsync(List<InspectionStandard> standards);
    Task<List<string>> GetStandardValidationErrorsAsync(List<InspectionStandard> standards);
    Task<List<InspectionStandard>> GetStandardLibraryAsync();
    Task<InspectionStandard> CreateStandardTemplateAsync(InspectionStandard standard);

    // 检验项目管理
    Task<bool> UpdateInspectionItemsAsync(string planId, List<InspectionItem> items);
    Task<List<InspectionItem>> GetInspectionItemsAsync(string planId);
    Task<InspectionItem> AddInspectionItemAsync(string planId, InspectionItem item);
    Task<bool> RemoveInspectionItemAsync(string planId, string itemCode);
    Task<bool> ReorderInspectionItemsAsync(string planId, List<string> itemCodes);
    Task<List<InspectionItem>> GetInspectionItemTemplatesAsync(InspectionCategory category);

    // 质量检验执行
    Task<QualityInspection> CreateInspectionAsync(string planId, string batchNumber, string inspectorId, Dictionary<string, object>? additionalData = null);
    Task<QualityInspection?> GetInspectionAsync(string inspectionId);
    Task<List<QualityInspection>> GetInspectionsAsync(string planId, int skip = 0, int take = 20);
    Task<List<QualityInspection>> GetInspectionsByStatusAsync(InspectionStatus status, int skip = 0, int take = 20);
    Task<List<QualityInspection>> GetMyInspectionsAsync(string inspectorId, int skip = 0, int take = 20);
    Task<List<QualityInspection>> GetPendingInspectionsAsync(string inspectorId);
    Task<bool> StartInspectionAsync(string inspectionId, string inspectorId);
    Task<bool> CompleteInspectionAsync(string inspectionId, string inspectorId, string? comments = null);
    Task<bool> CancelInspectionAsync(string inspectionId, string cancelledBy, string reason);

    // 检验结果管理
    Task<InspectionItemResult> RecordItemResultAsync(string inspectionId, string itemCode, InspectionItemResult result);
    Task<List<InspectionItemResult>> GetItemResultsAsync(string inspectionId);
    Task<InspectionItemResult?> GetItemResultAsync(string inspectionId, string itemCode);
    Task<bool> UpdateItemResultAsync(string resultId, InspectionItemResult result);
    Task<bool> ValidateItemResultAsync(InspectionItemResult result, InspectionItem item);
    Task<List<string>> GetItemResultValidationErrorsAsync(InspectionItemResult result, InspectionItem item);

    // 缺陷管理
    Task<DefectRecord> RecordDefectAsync(string inspectionId, DefectRecord defect);
    Task<List<DefectRecord>> GetDefectsAsync(string inspectionId);
    Task<DefectRecord?> GetDefectAsync(string defectId);
    Task<DefectRecord> UpdateDefectAsync(DefectRecord defect);
    Task<bool> CloseDefectAsync(string defectId, string closedBy, string resolution);
    Task<List<DefectRecord>> GetOpenDefectsAsync();
    Task<List<DefectRecord>> GetDefectsByTypeAsync(DefectType type, int skip = 0, int take = 20);
    Task<DefectStatistics> GetDefectStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);

    // 照片和文档管理
    Task<InspectionPhoto> AddPhotoAsync(string inspectionId, string fileName, string filePath, string takenBy, string? itemResultId = null, string? defectId = null);
    Task<List<InspectionPhoto>> GetPhotosAsync(string inspectionId);
    Task<List<InspectionPhoto>> GetItemPhotosAsync(string itemResultId);
    Task<List<InspectionPhoto>> GetDefectPhotosAsync(string defectId);
    Task<bool> RemovePhotoAsync(string photoId, string removedBy);
    Task<byte[]> GetPhotoContentAsync(string photoId);

    Task<InspectionDocument> AddDocumentAsync(string inspectionId, string fileName, string filePath, string uploadedBy, DocumentType type);
    Task<List<InspectionDocument>> GetDocumentsAsync(string inspectionId);
    Task<bool> RemoveDocumentAsync(string documentId, string removedBy);
    Task<byte[]> GetDocumentContentAsync(string documentId);

    // 审核和批准
    Task<bool> SubmitForReviewAsync(string inspectionId, string submittedBy);
    Task<bool> ApproveInspectionAsync(string inspectionId, string reviewerId, string? comments = null);
    Task<bool> RejectInspectionAsync(string inspectionId, string reviewerId, string reason);
    Task<List<QualityInspection>> GetInspectionsForReviewAsync(string reviewerId);
    Task<List<QualityInspection>> GetInspectionsByReviewStatusAsync(InspectionStatus status);

    // 抽样管理
    Task<SampleSelection> GenerateSampleSelectionAsync(string planId, int lotSize, SamplingParameters parameters);
    Task<bool> ValidateSampleSelectionAsync(SampleSelection selection, InspectionSampling sampling);
    Task<SamplingStatistics> GetSamplingStatisticsAsync(string planId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<SamplingRecommendation>> GetSamplingRecommendationsAsync(string planId);

    // 质量分析
    Task<QualityAnalysisResult> AnalyzeQualityTrendsAsync(QualityAnalysisRequest request);
    Task<List<QualityMetric>> GetQualityMetricsAsync(string? productId = null, string? supplierId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<QualityDashboard> GetQualityDashboardAsync(DashboardTimeRange timeRange = DashboardTimeRange.ThisMonth);
    Task<List<QualityAlert>> GetQualityAlertsAsync();
    Task<bool> MarkAlertAsReadAsync(string alertId, string userId);

    // 质量报告
    Task<QualityReport> GenerateQualityReportAsync(QualityReportRequest request);
    Task<List<QualityReport>> GetQualityReportsAsync(int skip = 0, int take = 20);
    Task<QualityReport?> GetQualityReportAsync(string reportId);
    Task<bool> DeleteQualityReportAsync(string reportId, string deletedBy);
    Task<byte[]> ExportQualityReportAsync(string reportId, ExportFormat format);
    Task<QualityReportTemplate> GetReportTemplateAsync(QualityReportType type);

    // 统计和KPI
    Task<InspectionStatistics> GetInspectionStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<InspectionPerformanceData>> GetInspectionPerformanceAsync(string? inspectorId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<SupplierQualityRanking> GetSupplierQualityRankingAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ProductQualityAnalysis> GetProductQualityAnalysisAsync(string? productId = null, DateTime? startDate = null, DateTime? endDate = null);

    // 历史记录
    Task<List<QualityInspectionHistory>> GetPlanHistoryAsync(string planId, int skip = 0, int take = 20);
    Task<List<QualityInspectionHistory>> GetInspectionHistoryAsync(string inspectionId, int skip = 0, int take = 20);
    Task<bool> AddHistoryEntryAsync(string? planId, string? inspectionId, InspectionHistoryAction action, string performedBy, string description, object? oldValue = null, object? newValue = null);

    // 配置管理
    Task<QualityConfiguration> GetQualityConfigurationAsync();
    Task<bool> UpdateQualityConfigurationAsync(QualityConfiguration configuration);
    Task<List<InspectionMethodInfo>> GetAvailableInspectionMethodsAsync();
    Task<List<DefectTypeInfo>> GetDefectTypesAsync();
    Task<List<QualityStandardInfo>> GetQualityStandardsAsync();

    // 集成和自动化
    Task<bool> TriggerAutomaticInspectionAsync(string planId, string batchNumber, Dictionary<string, object> triggerData);
    Task<AutoInspectionResult> RunAutomaticInspectionAsync(string inspectionId, AutoInspectionSettings settings);
    Task<bool> IntegrateWithProductionAsync(string productionOrderId, string planId);
    Task<bool> NotifyStakeholdersAsync(string inspectionId, NotificationType type, List<string> recipients);

    // 移动端支持
    Task<MobileInspectionSession> StartMobileInspectionAsync(string inspectionId, string inspectorId, string deviceId);
    Task<bool> SyncMobileInspectionDataAsync(string sessionId, MobileInspectionData data);
    Task<MobileInspectionConfiguration> GetMobileInspectionConfigAsync(string planId);
    Task<bool> CompleteMobileInspectionAsync(string sessionId, MobileInspectionResult result);

    // 高级功能
    Task<InspectionOptimization> OptimizeInspectionPlanAsync(string planId, OptimizationCriteria criteria);
    Task<PredictiveAnalysis> GetPredictiveQualityAnalysisAsync(PredictiveAnalysisRequest request);
    Task<List<QualityRecommendation>> GetQualityImprovementRecommendationsAsync(string? entityId = null, RecommendationScope scope = RecommendationScope.All);
    Task<QualityBenchmark> GetQualityBenchmarkAsync(BenchmarkRequest request);

    // 导入导出
    Task<byte[]> ExportInspectionDataAsync(InspectionDataExportRequest request);
    Task<ImportResult> ImportInspectionPlanAsync(byte[] data, ImportFormat format, ImportOptions options);
    Task<ImportValidationResult> ValidateImportDataAsync(byte[] data, ImportFormat format);
    Task<bool> ExportInspectionResultsAsync(string inspectionId, ExportFormat format, string filePath);
}

// 支持类定义
public class SampleSelection
{
    public string PlanId { get; set; } = string.Empty;
    public int LotSize { get; set; }
    public int SampleSize { get; set; }
    public SamplingMethod Method { get; set; }
    public List<SampleItem> SelectedItems { get; set; } = new();
    public SamplingPlan Plan { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string GeneratedBy { get; set; } = string.Empty;
}

public class SampleItem
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public string Location { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class SamplingParameters
{
    public SamplingMethod Method { get; set; } = SamplingMethod.Random;
    public SamplingLevel Level { get; set; } = SamplingLevel.Normal;
    public decimal SamplePercentage { get; set; } = 10;
    public int? MinSampleSize { get; set; }
    public int? MaxSampleSize { get; set; }
    public AQLLevel AQLLevel { get; set; } = AQLLevel.Normal;
    public Dictionary<string, object> CustomParameters { get; set; } = new();
}

public class SamplingStatistics
{
    public string PlanId { get; set; } = string.Empty;
    public int TotalSamples { get; set; }
    public decimal AverageSampleSize { get; set; }
    public Dictionary<SamplingMethod, int> MethodDistribution { get; set; } = new();
    public decimal SamplingEfficiency { get; set; }
    public List<SamplingTrend> Trends { get; set; } = new();
}

public class SamplingTrend
{
    public DateTime Date { get; set; }
    public int SampleCount { get; set; }
    public decimal AverageSize { get; set; }
    public decimal AcceptanceRate { get; set; }
}

public class SamplingRecommendation
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecommendationType Type { get; set; }
    public decimal EstimatedImpact { get; set; }
    public string Rationale { get; set; } = string.Empty;
}

public class QualityAnalysisRequest
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddMonths(-3);
    public DateTime EndDate { get; set; } = DateTime.UtcNow;
    public List<string>? ProductIds { get; set; }
    public List<string>? SupplierIds { get; set; }
    public List<InspectionType>? InspectionTypes { get; set; }
    public bool IncludeTrends { get; set; } = true;
    public bool IncludeForecasting { get; set; } = false;
    public AnalysisGranularity Granularity { get; set; } = AnalysisGranularity.Daily;
}

public class QualityAnalysisResult
{
    public QualityMetrics OverallMetrics { get; set; } = new();
    public List<QualityTrend> Trends { get; set; } = new();
    public List<QualityInsight> Insights { get; set; } = new();
    public QualityForecast? Forecast { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class QualityMetric
{
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public MetricType Type { get; set; } = MetricType.Percentage;
    public TrendDirection Trend { get; set; } = TrendDirection.Stable;
    public decimal TrendValue { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class QualityDashboard
{
    public QualityKPIs KPIs { get; set; } = new();
    public List<QualityChart> Charts { get; set; } = new();
    public List<QualityAlert> Alerts { get; set; } = new();
    public List<RecentActivity> RecentActivities { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class QualityKPIs
{
    public decimal OverallPassRate { get; set; }
    public decimal DefectRate { get; set; }
    public decimal FirstTimePassRate { get; set; }
    public decimal AverageInspectionTime { get; set; }
    public int TotalInspections { get; set; }
    public int CriticalDefects { get; set; }
    public decimal CostOfQuality { get; set; }
    public decimal CustomerSatisfactionScore { get; set; }
}

public class QualityChart
{
    public string Title { get; set; } = string.Empty;
    public ChartType Type { get; set; } = ChartType.Line;
    public List<ChartDataSeries> Series { get; set; } = new();
    public ChartConfiguration Configuration { get; set; } = new();
}

public class ChartDataSeries
{
    public string Name { get; set; } = string.Empty;
    public List<ChartDataPoint> DataPoints { get; set; } = new();
    public string Color { get; set; } = string.Empty;
}

public class ChartDataPoint
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ChartConfiguration
{
    public string XAxisLabel { get; set; } = string.Empty;
    public string YAxisLabel { get; set; } = string.Empty;
    public bool ShowLegend { get; set; } = true;
    public bool ShowDataLabels { get; set; } = false;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class QualityAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public AlertType Type { get; set; } = AlertType.QualityIssue;
    public AlertSeverity Severity { get; set; } = AlertSeverity.Medium;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RelatedEntityId { get; set; }
    public string RelatedEntityType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public string? ReadBy { get; set; }
    public DateTime? ReadAt { get; set; }
    public List<string> Recipients { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class RecentActivity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public ActivityType Type { get; set; } = ActivityType.InspectionCompleted;
    public string Description { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RelatedEntityId { get; set; }
    public string RelatedEntityType { get; set; } = string.Empty;
    public Dictionary<string, object> Details { get; set; } = new();
}

public class QualityReportRequest
{
    public QualityReportType Type { get; set; } = QualityReportType.Inspection;
    public string Title { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; } = DateTime.UtcNow.AddMonths(-1);
    public DateTime PeriodEnd { get; set; } = DateTime.UtcNow;
    public QualityReportScope Scope { get; set; } = new();
    public List<string> IncludeSections { get; set; } = new();
    public ReportFormat Format { get; set; } = ReportFormat.PDF;
    public string RequestedBy { get; set; } = string.Empty;
}

public class QualityReportTemplate
{
    public QualityReportType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ReportSection> Sections { get; set; } = new();
    public ReportStyling Styling { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
}

public class ReportSection
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public SectionType Type { get; set; } = SectionType.Summary;
    public bool IsRequired { get; set; } = true;
    public int Order { get; set; } = 0;
    public Dictionary<string, object> Configuration { get; set; } = new();
}

public class ReportStyling
{
    public string Theme { get; set; } = "Default";
    public string FontFamily { get; set; } = "Arial";
    public int FontSize { get; set; } = 12;
    public string PrimaryColor { get; set; } = "#007ACC";
    public string SecondaryColor { get; set; } = "#F5F5F5";
    public bool IncludeLogo { get; set; } = true;
    public Dictionary<string, object> CustomStyles { get; set; } = new();
}

public class InspectionStatistics
{
    public int TotalInspections { get; set; }
    public int CompletedInspections { get; set; }
    public int PendingInspections { get; set; }
    public int FailedInspections { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageInspectionTime { get; set; }
    public Dictionary<InspectionType, int> TypeDistribution { get; set; } = new();
    public Dictionary<InspectionStatus, int> StatusDistribution { get; set; } = new();
    public Dictionary<string, int> InspectorWorkload { get; set; } = new();
}

public class InspectionPerformanceData
{
    public string InspectorId { get; set; } = string.Empty;
    public string InspectorName { get; set; } = string.Empty;
    public int TotalInspections { get; set; }
    public int CompletedInspections { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageInspectionTime { get; set; }
    public decimal AccuracyRate { get; set; }
    public int DefectsFound { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class SupplierQualityRanking
{
    public List<SupplierQualityScore> Rankings { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalSuppliers { get; set; }
    public QualityRankingCriteria Criteria { get; set; } = new();
}

public class SupplierQualityScore
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal OverallScore { get; set; }
    public int Rank { get; set; }
    public decimal PassRate { get; set; }
    public decimal DefectRate { get; set; }
    public int TotalInspections { get; set; }
    public Dictionary<string, decimal> CategoryScores { get; set; } = new();
    public TrendDirection Trend { get; set; } = TrendDirection.Stable;
}

public class QualityRankingCriteria
{
    public decimal PassRateWeight { get; set; } = 40;
    public decimal DefectRateWeight { get; set; } = 30;
    public decimal TimelinessWeight { get; set; } = 20;
    public decimal ComplianceWeight { get; set; } = 10;
    public Dictionary<string, decimal> CustomWeights { get; set; } = new();
}

public class ProductQualityAnalysis
{
    public string? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public QualityMetrics Metrics { get; set; } = new();
    public List<QualityTrend> Trends { get; set; } = new();
    public List<DefectAnalysis> DefectAnalysis { get; set; } = new();
    public List<QualityRecommendation> Recommendations { get; set; } = new();
    public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;
}

public class DefectAnalysis
{
    public DefectType Type { get; set; }
    public DefectCategory Category { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public TrendDirection Trend { get; set; } = TrendDirection.Stable;
    public List<string> CommonCauses { get; set; } = new();
    public decimal EstimatedCost { get; set; }
}

public class DefectStatistics
{
    public int TotalDefects { get; set; }
    public int CriticalDefects { get; set; }
    public int MajorDefects { get; set; }
    public int MinorDefects { get; set; }
    public Dictionary<DefectType, int> TypeDistribution { get; set; } = new();
    public Dictionary<DefectCategory, int> CategoryDistribution { get; set; } = new();
    public Dictionary<DefectCause, int> CauseDistribution { get; set; } = new();
    public decimal DefectRate { get; set; }
    public List<DefectTrend> Trends { get; set; } = new();
}

public class DefectTrend
{
    public DateTime Date { get; set; }
    public int DefectCount { get; set; }
    public decimal DefectRate { get; set; }
    public DefectType? DefectType { get; set; }
    public DefectCategory? Category { get; set; }
}

public class QualityConfiguration
{
    public QualityThresholds Thresholds { get; set; } = new();
    public NotificationSettings Notifications { get; set; } = new();
    public WorkflowSettings Workflows { get; set; } = new();
    public IntegrationSettings Integrations { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class QualityThresholds
{
    public decimal MinPassRate { get; set; } = 95;
    public decimal MaxDefectRate { get; set; } = 5;
    public decimal CriticalDefectThreshold { get; set; } = 0;
    public decimal MajorDefectThreshold { get; set; } = 2.5m;
    public decimal MinorDefectThreshold { get; set; } = 4.0m;
    public int MaxInspectionTime { get; set; } = 480; // minutes
    public Dictionary<string, decimal> CustomThresholds { get; set; } = new();
}

public class NotificationSettings
{
    public bool EnableEmailNotifications { get; set; } = true;
    public bool EnableSmsNotifications { get; set; } = false;
    public bool EnableInAppNotifications { get; set; } = true;
    public List<NotificationRule> Rules { get; set; } = new();
    public Dictionary<string, object> Templates { get; set; } = new();
}

public class NotificationRule
{
    public string Name { get; set; } = string.Empty;
    public NotificationTrigger Trigger { get; set; } = NotificationTrigger.DefectFound;
    public List<string> Recipients { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public Dictionary<string, object> Conditions { get; set; } = new();
}

public class WorkflowSettings
{
    public bool RequireReviewForFailures { get; set; } = true;
    public bool EnableAutomaticNotifications { get; set; } = true;
    public bool AllowInspectorSelfReview { get; set; } = false;
    public int ReviewTimeoutHours { get; set; } = 24;
    public Dictionary<string, object> CustomWorkflows { get; set; } = new();
}

public class IntegrationSettings
{
    public bool IntegrateWithProduction { get; set; } = false;
    public bool IntegrateWithInventory { get; set; } = false;
    public bool IntegrateWithPurchasing { get; set; } = false;
    public string? ProductionSystemEndpoint { get; set; }
    public string? InventorySystemEndpoint { get; set; }
    public Dictionary<string, object> CustomIntegrations { get; set; } = new();
}

public class InspectionMethodInfo
{
    public InspectionMethod Method { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> RequiredEquipment { get; set; } = new();
    public List<InspectionCategory> ApplicableCategories { get; set; } = new();
    public bool RequiresTraining { get; set; } = false;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class DefectTypeInfo
{
    public DefectType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DefectSeverity DefaultSeverity { get; set; } = DefectSeverity.Medium;
    public List<DefectCategory> ApplicableCategories { get; set; } = new();
    public List<string> CommonCauses { get; set; } = new();
    public List<string> PreventiveMeasures { get; set; } = new();
}

public class QualityStandardInfo
{
    public string StandardId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public StandardType Type { get; set; } = StandardType.National;
    public string Organization { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public StandardScope Scope { get; set; } = StandardScope.Product;
    public string Description { get; set; } = string.Empty;
}

// 枚举定义
public enum AnalysisGranularity
{
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public enum MetricType
{
    Count,
    Percentage,
    Rate,
    Time,
    Score
}

public enum ChartType
{
    Line,
    Bar,
    Pie,
    Area,
    Scatter,
    Gauge
}

public enum ActivityType
{
    InspectionCreated,
    InspectionStarted,
    InspectionCompleted,
    DefectRecorded,
    InspectionReviewed,
    PlanActivated,
    PlanDeactivated
}

public enum ReportFormat
{
    PDF,
    Excel,
    Word,
    HTML
}

public enum SectionType
{
    Summary,
    Metrics,
    Charts,
    Tables,
    Recommendations,
    Appendix
}

public enum DashboardTimeRange
{
    Today,
    ThisWeek,
    ThisMonth,
    ThisQuarter,
    ThisYear,
    Last7Days,
    Last30Days,
    Last90Days,
    Custom
}

public enum NotificationTrigger
{
    DefectFound,
    InspectionFailed,
    InspectionCompleted,
    ReviewRequired,
    ThresholdExceeded,
    PlanActivated
}
// Disabled in minimal build
#endif
