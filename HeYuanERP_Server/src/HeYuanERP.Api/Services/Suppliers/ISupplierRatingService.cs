using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Api.Services.Suppliers;

public interface ISupplierRatingService
{
    Task<SupplierRating?> GetSupplierRatingAsync(string supplierId);
    Task<SupplierRating> CreateSupplierRatingAsync(SupplierRating rating);
    Task<SupplierRating> UpdateSupplierRatingAsync(SupplierRating rating);
    Task<bool> DeleteSupplierRatingAsync(string ratingId);
    Task<List<SupplierRating>> GetSupplierRatingsByLevelAsync(SupplierLevel level);
    Task<List<SupplierRating>> SearchSupplierRatingsAsync(string searchTerm, int skip = 0, int take = 20);
    Task<Dictionary<SupplierLevel, int>> GetSupplierLevelDistributionAsync();
    Task<List<SupplierRating>> GetTopSuppliersAsync(int count = 10);
    Task<List<SupplierRating>> GetHighRiskSuppliersAsync();
    Task<List<SupplierRating>> GetSuppliersRequiringReviewAsync();

    Task<SupplierRating> RecalculateSupplierRatingAsync(string supplierId);
    Task<List<SupplierRating>> BatchRecalculateRatingsAsync(List<string> supplierIds);
    Task<SupplierRating> UpdateSupplierScoreAsync(string supplierId, string criterion, decimal score, string reason);
    Task<bool> ApproveSupplierLevelChangeAsync(string ratingId, string approverId, string comments);
    Task<bool> RejectSupplierLevelChangeAsync(string ratingId, string approverId, string reason);

    Task<SupplierEvaluation> CreateSupplierEvaluationAsync(SupplierEvaluation evaluation);
    Task<SupplierEvaluation> UpdateSupplierEvaluationAsync(SupplierEvaluation evaluation);
    Task<bool> DeleteSupplierEvaluationAsync(string evaluationId);
    Task<List<SupplierEvaluation>> GetSupplierEvaluationsAsync(string supplierId, DateTime? startDate = null, DateTime? endDate = null);
    Task<SupplierEvaluation?> GetLatestSupplierEvaluationAsync(string supplierId);
    Task<List<SupplierEvaluation>> GetPendingEvaluationsAsync();
    Task<bool> ApproveSupplierEvaluationAsync(string evaluationId, string approverId, string comments);
    Task<bool> RejectSupplierEvaluationAsync(string evaluationId, string approverId, string reason);

    Task<SupplierPerformanceReport> GeneratePerformanceReportAsync(string supplierId, DateTime startDate, DateTime endDate);
    Task<SupplierRatingTrend> GetSupplierRatingTrendAsync(string supplierId, int months = 12);
    Task<List<SupplierComparison>> CompareSupplierPerformanceAsync(List<string> supplierIds);
    Task<SupplierBenchmarkReport> GenerateBenchmarkReportAsync(string categoryId = "");

    Task<List<SupplierRatingHistory>> GetSupplierRatingHistoryAsync(string supplierId, int skip = 0, int take = 20);
    Task<SupplierRatingStatistics> GetRatingStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<SupplierRatingAlert>> GetSupplierRatingAlertsAsync();
    Task<bool> MarkAlertAsReadAsync(string alertId, string userId);

    Task<SupplierRiskProfile> GetSupplierRiskProfileAsync(string supplierId);
    Task<List<SupplierRating>> GetSuppliersByRiskLevelAsync(RiskLevel riskLevel);
    Task<SupplierRiskReport> GenerateRiskReportAsync(DateTime reportDate);
    Task<bool> UpdateRiskAssessmentAsync(string supplierId, SupplierRiskAssessment assessment);

    Task<bool> SetSupplierRatingRulesAsync(SupplierRatingRules rules);
    Task<SupplierRatingRules> GetSupplierRatingRulesAsync();
    Task<bool> ValidateSupplierRatingAsync(SupplierRating rating);
    Task<List<string>> GetSupplierRatingValidationErrorsAsync(SupplierRating rating);

    Task<byte[]> ExportSupplierRatingsAsync(ExportFormat format, SupplierRatingExportOptions options);
    Task<bool> ImportSupplierRatingsAsync(byte[] data, ImportFormat format, ImportOptions options);
    Task<SupplierRatingImportResult> ValidateImportDataAsync(byte[] data, ImportFormat format);
}

public class SupplierPerformanceReport
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SupplierPerformanceMetrics Metrics { get; set; } = new();
    public List<PerformanceIndicator> Indicators { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> WeakAreas { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public decimal OverallScore { get; set; }
    public SupplierLevel RecommendedLevel { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class PerformanceIndicator
{
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Target { get; set; }
    public string Unit { get; set; } = string.Empty;
    public PerformanceStatus Status { get; set; } = PerformanceStatus.Normal;
    public string Description { get; set; } = string.Empty;
}

public class SupplierRatingTrend
{
    public string SupplierId { get; set; } = string.Empty;
    public List<RatingDataPoint> DataPoints { get; set; } = new();
    public decimal Slope { get; set; }
    public TrendDirection Direction { get; set; } = TrendDirection.Stable;
    public decimal VarianceScore { get; set; }
}

public class RatingDataPoint
{
    public DateTime Date { get; set; }
    public decimal Score { get; set; }
    public SupplierLevel Level { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class SupplierComparison
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal OverallScore { get; set; }
    public Dictionary<string, decimal> CategoryScores { get; set; } = new();
    public int Rank { get; set; }
    public List<string> Advantages { get; set; } = new();
    public List<string> Disadvantages { get; set; } = new();
}

public class SupplierBenchmarkReport
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int TotalSuppliers { get; set; }
    public decimal AverageScore { get; set; }
    public decimal MedianScore { get; set; }
    public decimal TopQuartileScore { get; set; }
    public decimal BottomQuartileScore { get; set; }
    public List<BenchmarkMetric> Metrics { get; set; } = new();
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
}

public class BenchmarkMetric
{
    public string Name { get; set; } = string.Empty;
    public decimal Average { get; set; }
    public decimal Best { get; set; }
    public decimal Worst { get; set; }
    public decimal StandardDeviation { get; set; }
}

public class SupplierRatingStatistics
{
    public int TotalSuppliers { get; set; }
    public Dictionary<SupplierLevel, int> LevelDistribution { get; set; } = new();
    public Dictionary<RiskLevel, int> RiskDistribution { get; set; } = new();
    public decimal AverageScore { get; set; }
    public int NewRatingsThisMonth { get; set; }
    public int RatingsRequiringReview { get; set; }
    public int HighRiskSuppliers { get; set; }
    public List<TopSupplier> TopPerformers { get; set; } = new();
    public List<BottomSupplier> BottomPerformers { get; set; } = new();
}

public class TopSupplier
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public SupplierLevel Level { get; set; }
}

public class BottomSupplier
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public SupplierLevel Level { get; set; }
    public List<string> Issues { get; set; } = new();
}

public class SupplierRatingAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public AlertType Type { get; set; } = AlertType.RatingDecline;
    public AlertSeverity Severity { get; set; } = AlertSeverity.Medium;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public string? ReadBy { get; set; }
    public DateTime? ReadAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SupplierRiskProfile
{
    public string SupplierId { get; set; } = string.Empty;
    public RiskLevel OverallRiskLevel { get; set; } = RiskLevel.Medium;
    public decimal RiskScore { get; set; }
    public Dictionary<string, RiskAssessmentDetail> RiskFactors { get; set; } = new();
    public List<string> RiskMitigationActions { get; set; } = new();
    public DateTime LastAssessmentDate { get; set; }
    public string AssessedBy { get; set; } = string.Empty;
    public DateTime NextReviewDate { get; set; }
}

public class RiskAssessmentDetail
{
    public decimal Score { get; set; }
    public RiskLevel Level { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public List<string> Evidences { get; set; } = new();
}

public class SupplierRiskReport
{
    public DateTime ReportDate { get; set; }
    public int TotalSuppliers { get; set; }
    public Dictionary<RiskLevel, int> RiskDistribution { get; set; } = new();
    public List<HighRiskSupplierDetail> HighRiskSuppliers { get; set; } = new();
    public List<RiskTrend> RiskTrends { get; set; } = new();
    public List<string> RecommendedActions { get; set; } = new();
}

public class HighRiskSupplierDetail
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public RiskLevel RiskLevel { get; set; }
    public decimal RiskScore { get; set; }
    public List<string> PrimaryRiskFactors { get; set; } = new();
    public DateTime LastReviewDate { get; set; }
    public bool RequiresImmediateAction { get; set; }
}

public class RiskTrend
{
    public RiskLevel RiskLevel { get; set; }
    public List<RiskDataPoint> DataPoints { get; set; } = new();
    public TrendDirection Direction { get; set; }
}

public class RiskDataPoint
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class SupplierRatingRules
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, decimal> ScoreWeights { get; set; } = new();
    public Dictionary<SupplierLevel, ScoreRange> LevelThresholds { get; set; } = new();
    public Dictionary<string, ValidationRule> ValidationRules { get; set; } = new();
    public AutoCalculationSettings AutoCalculation { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class ScoreRange
{
    public decimal MinScore { get; set; }
    public decimal MaxScore { get; set; }
}

public class ValidationRule
{
    public string Field { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}

public class AutoCalculationSettings
{
    public bool EnableAutoCalculation { get; set; } = true;
    public int CalculationFrequencyDays { get; set; } = 30;
    public bool RequireApproval { get; set; } = true;
    public List<string> NotificationRecipients { get; set; } = new();
}

public class SupplierRatingExportOptions
{
    public List<string>? SupplierIds { get; set; }
    public SupplierLevel? MinLevel { get; set; }
    public SupplierLevel? MaxLevel { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string> IncludeFields { get; set; } = new();
    public bool IncludeHistory { get; set; } = false;
    public bool IncludeEvaluations { get; set; } = false;
}

public class SupplierRatingImportResult
{
    public bool IsValid { get; set; }
    public int TotalRecords { get; set; }
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public List<ImportWarning> Warnings { get; set; } = new();
    public Dictionary<string, object> Statistics { get; set; } = new();
}

public class ImportError
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class ImportWarning
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;
}

public enum PerformanceStatus
{
    Excellent,
    Good,
    Normal,
    BelowExpectation,
    Poor
}

public enum TrendDirection
{
    Improving,
    Stable,
    Declining
}

public enum AlertType
{
    RatingDecline,
    RiskIncrease,
    ReviewRequired,
    ThresholdExceeded,
    QualityIssue,
    DeliveryDelay,
    ComplianceViolation
}

public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum ExportFormat
{
    Excel,
    CSV,
    JSON,
    PDF
}

public enum ImportFormat
{
    Excel,
    CSV,
    JSON
}

public class ImportOptions
{
    public bool SkipValidation { get; set; } = false;
    public bool AllowUpdates { get; set; } = true;
    public bool CreateIfNotExists { get; set; } = true;
    public string? DefaultCreatedBy { get; set; }
}