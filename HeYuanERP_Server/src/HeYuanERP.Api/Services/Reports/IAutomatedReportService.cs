// Disabled in minimal build: Automated report feature not enabled
#if false
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Api.Services.Reports;

public interface IAutomatedReportService
{
    // 报表基础管理
    Task<AutomatedReport?> GetReportAsync(string reportId);
    Task<AutomatedReport> CreateReportAsync(AutomatedReport report);
    Task<AutomatedReport> UpdateReportAsync(AutomatedReport report);
    Task<bool> DeleteReportAsync(string reportId);
    Task<List<AutomatedReport>> GetReportsAsync(int skip = 0, int take = 20);
    Task<List<AutomatedReport>> SearchReportsAsync(string searchTerm, int skip = 0, int take = 20);
    Task<List<AutomatedReport>> GetReportsByTypeAsync(ReportType type);
    Task<List<AutomatedReport>> GetReportsByCategoryAsync(ReportCategory category);
    Task<List<AutomatedReport>> GetPublicReportsAsync();
    Task<List<AutomatedReport>> GetMyReportsAsync(string userId);

    // 报表模板管理
    Task<bool> UpdateReportTemplateAsync(string reportId, ReportTemplate template);
    Task<ReportTemplate> GetReportTemplateAsync(string reportId);
    Task<List<ReportTemplate>> GetTemplateLibraryAsync();
    Task<ReportTemplate> CloneTemplateAsync(string templateId, string newName);
    Task<bool> ValidateTemplateAsync(ReportTemplate template);
    Task<List<string>> GetTemplateValidationErrorsAsync(ReportTemplate template);

    // 数据源管理
    Task<bool> UpdateDataSourceAsync(string reportId, ReportDataSource dataSource);
    Task<ReportDataSource> GetDataSourceAsync(string reportId);
    Task<bool> TestDataSourceConnectionAsync(ReportDataSource dataSource);
    Task<List<string>> GetAvailableTablesAsync(ReportDataSource dataSource);
    Task<List<ColumnInfo>> GetTableColumnsAsync(ReportDataSource dataSource, string tableName);
    Task<DataPreviewResult> PreviewDataAsync(string reportId, int maxRows = 100);

    // 报表执行
    Task<ReportExecution> ExecuteReportAsync(string reportId, Dictionary<string, object>? parameters = null, string? executedBy = null);
    Task<ReportExecution> ExecuteReportWithTemplateAsync(string reportId, string templateId, Dictionary<string, object>? parameters = null);
    Task<bool> CancelExecutionAsync(string executionId);
    Task<ReportExecution?> GetExecutionAsync(string executionId);
    Task<List<ReportExecution>> GetReportExecutionsAsync(string reportId, int skip = 0, int take = 20);
    Task<List<ReportExecution>> GetRunningExecutionsAsync();
    Task<ExecutionStatus> GetExecutionStatusAsync(string executionId);

    // 报表调度管理
    Task<bool> UpdateScheduleAsync(string reportId, ReportSchedule schedule);
    Task<ReportSchedule> GetScheduleAsync(string reportId);
    Task<bool> EnableScheduleAsync(string reportId);
    Task<bool> DisableScheduleAsync(string reportId);
    Task<List<AutomatedReport>> GetScheduledReportsAsync();
    Task<List<ScheduleExecution>> GetUpcomingExecutionsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> TriggerScheduledExecutionAsync(string reportId);

    // 参数管理
    Task<bool> UpdateParametersAsync(string reportId, ReportParameters parameters);
    Task<ReportParameters> GetParametersAsync(string reportId);
    Task<bool> ValidateParametersAsync(string reportId, Dictionary<string, object> parameters);
    Task<List<string>> GetParameterValidationErrorsAsync(string reportId, Dictionary<string, object> parameters);
    Task<Dictionary<string, object>> GetDefaultParameterValuesAsync(string reportId);

    // 输出管理
    Task<ReportOutputResult> GenerateReportOutputAsync(string executionId, OutputFormat format);
    Task<byte[]> GetReportFileAsync(string executionId, OutputFormat format);
    Task<List<string>> GetGeneratedFilesAsync(string executionId);
    Task<bool> DeleteOutputFileAsync(string filePath);
    Task<ReportOutputStatistics> GetOutputStatisticsAsync(string reportId);

    // 订阅管理
    Task<ReportSubscription> CreateSubscriptionAsync(ReportSubscription subscription);
    Task<ReportSubscription> UpdateSubscriptionAsync(ReportSubscription subscription);
    Task<bool> DeleteSubscriptionAsync(string subscriptionId);
    Task<List<ReportSubscription>> GetReportSubscriptionsAsync(string reportId);
    Task<List<ReportSubscription>> GetUserSubscriptionsAsync(string userId);
    Task<bool> EnableSubscriptionAsync(string subscriptionId);
    Task<bool> DisableSubscriptionAsync(string subscriptionId);

    // 分发管理
    Task<bool> SendReportAsync(string executionId, List<string> recipients, DeliveryFormat format);
    Task<bool> SendScheduledReportsAsync();
    Task<DeliveryResult> DeliverToSubscribersAsync(string reportId, string executionId);
    Task<List<DeliveryLog>> GetDeliveryHistoryAsync(string reportId, int skip = 0, int take = 20);
    Task<DeliveryStatistics> GetDeliveryStatisticsAsync(string reportId);

    // 监控和分析
    Task<ReportMetrics> GetReportMetricsAsync(string reportId);
    Task<List<ReportPerformanceData>> GetPerformanceDataAsync(string reportId, DateTime startDate, DateTime endDate);
    Task<ReportUsageStatistics> GetUsageStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ReportAlert>> GetReportAlertsAsync();
    Task<bool> MarkAlertAsReadAsync(string alertId, string userId);

    // 历史记录
    Task<List<ReportHistory>> GetReportHistoryAsync(string reportId, int skip = 0, int take = 20);
    Task<bool> CreateHistoryEntryAsync(string reportId, HistoryAction action, string performedBy, string description, object? oldValue = null, object? newValue = null);

    // 权限管理
    Task<bool> GrantReportAccessAsync(string reportId, string userId, ReportPermission permission);
    Task<bool> RevokeReportAccessAsync(string reportId, string userId);
    Task<List<ReportPermissionInfo>> GetReportPermissionsAsync(string reportId);
    Task<bool> HasReportPermissionAsync(string reportId, string userId, ReportPermission permission);

    // 导入导出
    Task<byte[]> ExportReportDefinitionAsync(string reportId, ExportFormat format);
    Task<ImportResult> ImportReportDefinitionAsync(byte[] data, ImportFormat format, ImportOptions options);
    Task<ImportValidationResult> ValidateImportDataAsync(byte[] data, ImportFormat format);
    Task<bool> ExportReportDataAsync(string reportId, Dictionary<string, object>? parameters, ExportDataOptions options);

    // 缓存管理
    Task<bool> ClearReportCacheAsync(string reportId);
    Task<bool> RefreshReportCacheAsync(string reportId);
    Task<CacheInfo> GetCacheInfoAsync(string reportId);
    Task<List<CacheEntry>> GetCacheEntriesAsync();
    Task<bool> ClearAllCacheAsync();

    // 系统管理
    Task<ReportSystemStatus> GetSystemStatusAsync();
    Task<List<ReportJob>> GetActiveJobsAsync();
    Task<bool> CancelJobAsync(string jobId);
    Task<ReportConfiguration> GetSystemConfigurationAsync();
    Task<bool> UpdateSystemConfigurationAsync(ReportConfiguration configuration);
}


// 支持类定义
public class ColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class DataPreviewResult
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public List<ColumnInfo> Columns { get; set; } = new();
    public int TotalRows { get; set; }
    public bool HasMoreData { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

public class ScheduleExecution
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public DateTime ScheduledTime { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public List<string> Recipients { get; set; } = new();
}

public class ReportOutputResult
{
    public bool Success { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public OutputFormat Format { get; set; }
    public TimeSpan GenerationTime { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ReportOutputStatistics
{
    public int TotalOutputs { get; set; }
    public Dictionary<OutputFormat, int> FormatDistribution { get; set; } = new();
    public long TotalSize { get; set; }
    public decimal AverageSize { get; set; }
    public TimeSpan AverageGenerationTime { get; set; }
    public int SuccessfulOutputs { get; set; }
    public int FailedOutputs { get; set; }
}

public class DeliveryResult
{
    public bool Success { get; set; }
    public int TotalRecipients { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public List<DeliveryError> Errors { get; set; } = new();
    public TimeSpan DeliveryTime { get; set; }
}

public class DeliveryError
{
    public string Recipient { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
}

public class DeliveryLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ReportId { get; set; } = string.Empty;
    public string ExecutionId { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public DeliveryFormat Format { get; set; }
    public DeliveryStatus Status { get; set; }
    public DateTime DeliveryTime { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class DeliveryStatistics
{
    public int TotalDeliveries { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public decimal SuccessRate { get; set; }
    public Dictionary<DeliveryFormat, int> FormatDistribution { get; set; } = new();
    public List<string> TopRecipients { get; set; } = new();
    public DateTime? LastDelivery { get; set; }
}

public class ReportPerformanceData
{
    public DateTime Timestamp { get; set; }
    public decimal ExecutionTime { get; set; }
    public long DataSize { get; set; }
    public int RecordCount { get; set; }
    public bool Success { get; set; }
}

public class ReportUsageStatistics
{
    public int TotalReports { get; set; }
    public int ActiveReports { get; set; }
    public int TotalExecutions { get; set; }
    public int ScheduledExecutions { get; set; }
    public int ManualExecutions { get; set; }
    public Dictionary<ReportType, int> TypeDistribution { get; set; } = new();
    public Dictionary<ReportCategory, int> CategoryDistribution { get; set; } = new();
    public List<TopReport> MostUsedReports { get; set; } = new();
    public List<string> ActiveUsers { get; set; } = new();
}

public class TopReport
{
    public string ReportId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ExecutionCount { get; set; }
    public DateTime LastExecution { get; set; }
}

public class ReportAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ReportId { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public string? ReadBy { get; set; }
    public DateTime? ReadAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ReportPermissionInfo
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public ReportPermission Permission { get; set; }
    public DateTime GrantedAt { get; set; }
    public string GrantedBy { get; set; } = string.Empty;
}

public class ImportResult
{
    public bool Success { get; set; }
    public string ReportId { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> Statistics { get; set; } = new();
}

public class ImportValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class ValidationWarning
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;
}

public class ExportDataOptions
{
    public OutputFormat Format { get; set; } = OutputFormat.Excel;
    public string FilePath { get; set; } = string.Empty;
    public bool IncludeHeaders { get; set; } = true;
    public int? MaxRows { get; set; }
    public Dictionary<string, object> FormatOptions { get; set; } = new();
}

public class CacheInfo
{
    public bool IsEnabled { get; set; }
    public DateTime? LastCacheTime { get; set; }
    public DateTime? CacheExpireTime { get; set; }
    public long CacheSize { get; set; }
    public int HitCount { get; set; }
    public int MissCount { get; set; }
    public decimal HitRate { get; set; }
}

public class CacheEntry
{
    public string Key { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public long Size { get; set; }
    public int AccessCount { get; set; }
    public DateTime LastAccessTime { get; set; }
}

public class ReportSystemStatus
{
    public bool IsHealthy { get; set; }
    public int ActiveExecutions { get; set; }
    public int QueuedJobs { get; set; }
    public int FailedJobs { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public DateTime LastChecked { get; set; }
    public List<SystemIssue> Issues { get; set; } = new();
}

public class SystemIssue
{
    public string Component { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
}

public class ReportJob
{
    public string Id { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public JobType Type { get; set; }
    public JobStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Progress { get; set; }
    public string? CurrentStep { get; set; }
}

public class ReportConfiguration
{
    public int MaxConcurrentExecutions { get; set; } = 5;
    public int DefaultTimeoutMinutes { get; set; } = 30;
    public int CacheExpirationMinutes { get; set; } = 60;
    public int MaxRetentionDays { get; set; } = 90;
    public long MaxOutputSizeMB { get; set; } = 100;
    public string DefaultEmailTemplate { get; set; } = string.Empty;
    public Dictionary<string, object> SystemSettings { get; set; } = new();
}

// 枚举定义
public enum ReportPermission
{
    View,
    Execute,
    Edit,
    Delete,
    Manage,
    Admin
}

public enum DeliveryStatus
{
    Pending,
    Sending,
    Delivered,
    Failed,
    Bounced
}

public enum IssueSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum JobType
{
    Execution,
    Schedule,
    Delivery,
    Cleanup,
    Maintenance
}

public enum JobStatus
{
    Queued,
    Running,
    Completed,
    Failed,
    Cancelled
}
#endif
