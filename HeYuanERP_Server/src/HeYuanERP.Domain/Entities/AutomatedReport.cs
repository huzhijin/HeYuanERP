// Disabled non-P0 domain entities for minimal build
#if false
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 自动化报表实体
/// </summary>
public class AutomatedReport
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public ReportType Type { get; set; } = ReportType.DataReport;

    [Required]
    public ReportCategory Category { get; set; } = ReportCategory.Financial;

    public ReportTemplate Template { get; set; } = new();

    public ReportDataSource DataSource { get; set; } = new();

    public ReportSchedule Schedule { get; set; } = new();

    public ReportOutput Output { get; set; } = new();

    public ReportParameters Parameters { get; set; } = new();

    public ReportStatus Status { get; set; } = ReportStatus.Draft;

    public bool IsActive { get; set; } = true;

    public bool IsPublic { get; set; } = false;

    [Required]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    [MaxLength(50)]
    public string? LastExecutedBy { get; set; }

    public DateTime? LastExecutedAt { get; set; }

    public DateTime? NextExecutionTime { get; set; }

    public int ExecutionCount { get; set; } = 0;

    public ReportMetrics Metrics { get; set; } = new();

    [MaxLength(1000)]
    public string Tags { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; set; } = string.Empty;

    public Dictionary<string, object> ExtensionData { get; set; } = new();

    // 导航属性
    public List<ReportExecution> Executions { get; set; } = new();
    public List<ReportSubscription> Subscriptions { get; set; } = new();
    public List<ReportHistory> History { get; set; } = new();
}
#endif

// Advanced Automated Reporting model (disabled in minimal build)
#if false
/// <summary>
/// 报表模板配置
/// </summary>
public class ReportTemplate
{
    [Required]
    public TemplateFormat Format { get; set; } = TemplateFormat.Standard;

    [Required]
    [MaxLength(100)]
    public string Layout { get; set; } = "DefaultLayout";

    public ReportHeader Header { get; set; } = new();

    public ReportFooter Footer { get; set; } = new();

    public List<ReportSection> Sections { get; set; } = new();

    public ReportStyling Styling { get; set; } = new();

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 报表头部配置
/// </summary>
public class ReportHeader
{
    public bool ShowLogo { get; set; } = true;

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Subtitle { get; set; } = string.Empty;

    public bool ShowGenerationTime { get; set; } = true;

    public bool ShowPageNumbers { get; set; } = true;

    public List<HeaderElement> CustomElements { get; set; } = new();
}

/// <summary>
/// 报表尾部配置
/// </summary>
public class ReportFooter
{
    public bool ShowCompanyInfo { get; set; } = true;

    [MaxLength(500)]
    public string CustomText { get; set; } = string.Empty;

    public bool ShowDisclaimer { get; set; } = false;

    [MaxLength(1000)]
    public string DisclaimerText { get; set; } = string.Empty;

    public List<FooterElement> CustomElements { get; set; } = new();
}

/// <summary>
/// 报表章节
/// </summary>
public class ReportSection
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public SectionType Type { get; set; } = SectionType.DataTable;

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public int Order { get; set; } = 0;

    public bool IsVisible { get; set; } = true;

    public SectionConfiguration Configuration { get; set; } = new();

    public List<SectionComponent> Components { get; set; } = new();
}

/// <summary>
/// 章节配置
/// </summary>
public class SectionConfiguration
{
    public int Columns { get; set; } = 1;

    public bool ShowBorder { get; set; } = true;

    public bool ShowHeader { get; set; } = true;

    public string BackgroundColor { get; set; } = "#FFFFFF";

    public Dictionary<string, object> CustomProperties { get; set; } = new();
}

/// <summary>
/// 章节组件
/// </summary>
public class SectionComponent
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ComponentType Type { get; set; } = ComponentType.Table;

    [Required]
    [MaxLength(200)]
    public string DataBinding { get; set; } = string.Empty;

    public ComponentFormatting Formatting { get; set; } = new();

    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// 组件格式化
/// </summary>
public class ComponentFormatting
{
    public string Width { get; set; } = "auto";

    public string Height { get; set; } = "auto";

    public string Alignment { get; set; } = "left";

    public string FontFamily { get; set; } = "Arial";

    public int FontSize { get; set; } = 12;

    public bool IsBold { get; set; } = false;

    public string Color { get; set; } = "#000000";

    public Dictionary<string, object> AdditionalStyles { get; set; } = new();
}

/// <summary>
/// 报表样式配置
/// </summary>
public class ReportStyling
{
    [MaxLength(50)]
    public string Theme { get; set; } = "Default";

    [MaxLength(50)]
    public string FontFamily { get; set; } = "Arial";

    public int FontSize { get; set; } = 12;

    public string PrimaryColor { get; set; } = "#007ACC";

    public string SecondaryColor { get; set; } = "#F5F5F5";

    public string AccentColor { get; set; } = "#FF6B35";

    public bool UseCompanyBranding { get; set; } = true;

    public Dictionary<string, object> CustomStyles { get; set; } = new();
}

/// <summary>
/// 报表数据源配置
/// </summary>
public class ReportDataSource
{
    [Required]
    public DataSourceType Type { get; set; } = DataSourceType.Database;

    [Required]
    [MaxLength(200)]
    public string ConnectionString { get; set; } = string.Empty;

    public List<DataQuery> Queries { get; set; } = new();

    public DataTransformation Transformation { get; set; } = new();

    public DataCache Cache { get; set; } = new();

    public Dictionary<string, object> Configuration { get; set; } = new();
}

/// <summary>
/// 数据查询配置
/// </summary>
public class DataQuery
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public QueryType Type { get; set; } = QueryType.SQL;

    [Required]
    public string Query { get; set; } = string.Empty;

    public List<QueryParameter> Parameters { get; set; } = new();

    public QueryOptions Options { get; set; } = new();

    public bool IsMainQuery { get; set; } = false;

    public int TimeoutSeconds { get; set; } = 300;
}

/// <summary>
/// 查询参数
/// </summary>
public class QueryParameter
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ParameterType Type { get; set; } = ParameterType.String;

    public object? DefaultValue { get; set; }

    public bool IsRequired { get; set; } = false;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public List<ParameterOption> Options { get; set; } = new();
}

/// <summary>
/// 参数选项
/// </summary>
public class ParameterOption
{
    [Required]
    public string Label { get; set; } = string.Empty;

    [Required]
    public object Value { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
}

/// <summary>
/// 查询选项
/// </summary>
public class QueryOptions
{
    public int? MaxRows { get; set; }

    public bool EnablePaging { get; set; } = false;

    public int PageSize { get; set; } = 100;

    public bool EnableSorting { get; set; } = true;

    public bool EnableFiltering { get; set; } = true;

    public Dictionary<string, object> AdditionalOptions { get; set; } = new();
}

/// <summary>
/// 数据转换配置
/// </summary>
public class DataTransformation
{
    public List<TransformationRule> Rules { get; set; } = new();

    public List<CalculatedField> CalculatedFields { get; set; } = new();

    public List<DataAggregation> Aggregations { get; set; } = new();

    public Dictionary<string, object> Settings { get; set; } = new();
}

/// <summary>
/// 转换规则
/// </summary>
public class TransformationRule
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TransformationType Type { get; set; } = TransformationType.FieldMapping;

    [MaxLength(100)]
    public string SourceField { get; set; } = string.Empty;

    [MaxLength(100)]
    public string TargetField { get; set; } = string.Empty;

    public string Expression { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 计算字段
/// </summary>
public class CalculatedField
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string Expression { get; set; } = string.Empty;

    [Required]
    public FieldDataType DataType { get; set; } = FieldDataType.String;

    public string Format { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// 数据聚合
/// </summary>
public class DataAggregation
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Field { get; set; } = string.Empty;

    [Required]
    public AggregationType Type { get; set; } = AggregationType.Sum;

    public List<string> GroupByFields { get; set; } = new();

    public string FilterExpression { get; set; } = string.Empty;
}

/// <summary>
/// 数据缓存配置
/// </summary>
public class DataCache
{
    public bool IsEnabled { get; set; } = true;

    public int CacheDurationMinutes { get; set; } = 60;

    public CacheStrategy Strategy { get; set; } = CacheStrategy.TimeBase;

    public List<string> InvalidationTriggers { get; set; } = new();

    public Dictionary<string, object> Settings { get; set; } = new();
}

/// <summary>
/// 报表调度配置
/// </summary>
public class ReportSchedule
{
    public bool IsEnabled { get; set; } = false;

    [Required]
    public ScheduleType Type { get; set; } = ScheduleType.Manual;

    public ScheduleFrequency Frequency { get; set; } = new();

    public List<ScheduleRecipient> Recipients { get; set; } = new();

    public ScheduleOptions Options { get; set; } = new();

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Local;
}

/// <summary>
/// 调度频率
/// </summary>
public class ScheduleFrequency
{
    [Required]
    public FrequencyType Type { get; set; } = FrequencyType.Daily;

    public int Interval { get; set; } = 1;

    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    public List<int> DaysOfMonth { get; set; } = new();

    public TimeSpan ExecutionTime { get; set; } = new(9, 0, 0);

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 调度收件人
/// </summary>
public class ScheduleRecipient
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public RecipientType Type { get; set; } = RecipientType.User;

    public bool IsActive { get; set; } = true;

    public List<DeliveryFormat> Formats { get; set; } = new() { DeliveryFormat.PDF };
}

/// <summary>
/// 调度选项
/// </summary>
public class ScheduleOptions
{
    public bool SendOnlyIfDataExists { get; set; } = true;

    public bool CompressLargeFiles { get; set; } = true;

    public bool IncludeAttachments { get; set; } = true;

    [MaxLength(500)]
    public string EmailSubject { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string EmailBody { get; set; } = string.Empty;

    public int MaxRetryAttempts { get; set; } = 3;

    public Dictionary<string, object> AdditionalSettings { get; set; } = new();
}

/// <summary>
/// 报表输出配置
/// </summary>
public class ReportOutput
{
    public List<OutputFormat> SupportedFormats { get; set; } = new() { OutputFormat.PDF, OutputFormat.Excel };

    [Required]
    public OutputFormat DefaultFormat { get; set; } = OutputFormat.PDF;

    public OutputSettings PdfSettings { get; set; } = new();

    public OutputSettings ExcelSettings { get; set; } = new();

    public OutputSettings CsvSettings { get; set; } = new();

    public OutputLocation Location { get; set; } = new();

    public FileNaming Naming { get; set; } = new();
}

/// <summary>
/// 输出设置
/// </summary>
public class OutputSettings
{
    public string PageSize { get; set; } = "A4";

    public string Orientation { get; set; } = "Portrait";

    public double MarginTop { get; set; } = 2.0;

    public double MarginBottom { get; set; } = 2.0;

    public double MarginLeft { get; set; } = 2.0;

    public double MarginRight { get; set; } = 2.0;

    public int Quality { get; set; } = 100;

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 输出位置
/// </summary>
public class OutputLocation
{
    [Required]
    public LocationType Type { get; set; } = LocationType.FileSystem;

    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;

    public bool CreateDateFolders { get; set; } = true;

    public int RetentionDays { get; set; } = 90;

    public Dictionary<string, object> Configuration { get; set; } = new();
}

/// <summary>
/// 文件命名规则
/// </summary>
public class FileNaming
{
    [MaxLength(100)]
    public string Pattern { get; set; } = "{ReportName}_{Date:yyyyMMdd}_{Time:HHmmss}";

    public bool IncludeTimestamp { get; set; } = true;

    public bool IncludeUserName { get; set; } = false;

    public string DateFormat { get; set; } = "yyyyMMdd";

    public string TimeFormat { get; set; } = "HHmmss";

    public Dictionary<string, string> CustomTokens { get; set; } = new();
}

/// <summary>
/// 报表参数配置
/// </summary>
public class ReportParameters
{
    public List<ReportParameter> Parameters { get; set; } = new();

    public ParameterValidation Validation { get; set; } = new();

    public Dictionary<string, object> DefaultValues { get; set; } = new();
}

/// <summary>
/// 报表参数
/// </summary>
public class ReportParameter
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public ParameterType Type { get; set; } = ParameterType.String;

    public object? DefaultValue { get; set; }

    public bool IsRequired { get; set; } = false;

    public bool IsVisible { get; set; } = true;

    public int Order { get; set; } = 0;

    public ParameterControl ControlType { get; set; } = ParameterControl.TextBox;

    public List<ParameterOption> Options { get; set; } = new();

    public ParameterConstraints Constraints { get; set; } = new();
}

/// <summary>
/// 参数约束
/// </summary>
public class ParameterConstraints
{
    public object? MinValue { get; set; }

    public object? MaxValue { get; set; }

    public int? MinLength { get; set; }

    public int? MaxLength { get; set; }

    public string? RegexPattern { get; set; }

    public List<object> AllowedValues { get; set; } = new();

    public string CustomValidation { get; set; } = string.Empty;
}

/// <summary>
/// 参数验证配置
/// </summary>
public class ParameterValidation
{
    public bool EnableValidation { get; set; } = true;

    public bool StrictMode { get; set; } = false;

    public List<ValidationRule> CustomRules { get; set; } = new();

    public Dictionary<string, string> ErrorMessages { get; set; } = new();
}

/// <summary>
/// 验证规则
/// </summary>
public class ValidationRule
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Expression { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ErrorMessage { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// 报表指标
/// </summary>
public class ReportMetrics
{
    public decimal AverageExecutionTimeSeconds { get; set; } = 0;

    public decimal MaxExecutionTimeSeconds { get; set; } = 0;

    public int SuccessfulExecutions { get; set; } = 0;

    public int FailedExecutions { get; set; } = 0;

    public decimal SuccessRate { get; set; } = 0;

    public long AverageDataSize { get; set; } = 0;

    public long MaxDataSize { get; set; } = 0;

    public DateTime? LastSuccessfulExecution { get; set; }

    public DateTime? LastFailedExecution { get; set; }

    public Dictionary<string, object> CustomMetrics { get; set; } = new();
}

/// <summary>
/// 报表执行记录
/// </summary>
public class ReportExecution
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string ReportId { get; set; } = string.Empty;

    [Required]
    public ExecutionTrigger Trigger { get; set; } = ExecutionTrigger.Manual;

    [Required]
    public ExecutionStatus Status { get; set; } = ExecutionStatus.Running;

    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    public DateTime? EndTime { get; set; }

    public decimal ExecutionTimeSeconds { get; set; } = 0;

    [MaxLength(50)]
    public string? ExecutedBy { get; set; }

    public Dictionary<string, object> ParameterValues { get; set; } = new();

    public ExecutionResult Result { get; set; } = new();

    public List<ExecutionLog> Logs { get; set; } = new();

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(ReportId))]
    public AutomatedReport Report { get; set; } = null!;
}

/// <summary>
/// 执行结果
/// </summary>
public class ExecutionResult
{
    public long RecordCount { get; set; } = 0;

    public long OutputSize { get; set; } = 0;

    public List<string> GeneratedFiles { get; set; } = new();

    public List<string> SentEmails { get; set; } = new();

    public Dictionary<string, object> Statistics { get; set; } = new();

    public List<ExecutionWarning> Warnings { get; set; } = new();
}

/// <summary>
/// 执行警告
/// </summary>
public class ExecutionWarning
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public WarningSeverity Severity { get; set; } = WarningSeverity.Low;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 执行日志
/// </summary>
public class ExecutionLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string ExecutionId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required]
    public LogLevel Level { get; set; } = LogLevel.Information;

    [Required]
    [MaxLength(200)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? Exception { get; set; }

    public Dictionary<string, object> Properties { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(ExecutionId))]
    public ReportExecution Execution { get; set; } = null!;
}

/// <summary>
/// 报表订阅
/// </summary>
public class ReportSubscription
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string ReportId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public List<DeliveryFormat> Formats { get; set; } = new() { DeliveryFormat.PDF };

    public SubscriptionSchedule Schedule { get; set; } = new();

    public Dictionary<string, object> Preferences { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastDeliveryAt { get; set; }

    public int DeliveryCount { get; set; } = 0;

    // 导航属性
    [ForeignKey(nameof(ReportId))]
    public AutomatedReport Report { get; set; } = null!;
}

/// <summary>
/// 订阅调度
/// </summary>
public class SubscriptionSchedule
{
    [Required]
    public SubscriptionFrequency Frequency { get; set; } = SubscriptionFrequency.Weekly;

    public List<DayOfWeek> DaysOfWeek { get; set; } = new() { DayOfWeek.Monday };

    public TimeSpan DeliveryTime { get; set; } = new(9, 0, 0);

    public bool OnlyOnDataChange { get; set; } = false;

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 报表历史记录
/// </summary>
public class ReportHistory
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string ReportId { get; set; } = string.Empty;

    [Required]
    public HistoryAction Action { get; set; } = HistoryAction.Created;

    [Required]
    [MaxLength(50)]
    public string PerformedBy { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public object? OldValue { get; set; }

    public object? NewValue { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(ReportId))]
    public AutomatedReport Report { get; set; } = null!;
}

// 辅助类
public class HeaderElement
{
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class FooterElement
{
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
}
#endif
