// Disabled in minimal build: Workflow service not enabled
#if false
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Api.Services.Workflow;

public interface IWorkflowEngineService
{
    // 工作流定义管理
    Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId);
    Task<WorkflowDefinition> CreateWorkflowDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition> UpdateWorkflowDefinitionAsync(WorkflowDefinition definition);
    Task<bool> DeleteWorkflowDefinitionAsync(string definitionId);
    Task<List<WorkflowDefinition>> GetWorkflowDefinitionsAsync(int skip = 0, int take = 20);
    Task<List<WorkflowDefinition>> SearchWorkflowDefinitionsAsync(string searchTerm, int skip = 0, int take = 20);
    Task<List<WorkflowDefinition>> GetWorkflowDefinitionsByCategoryAsync(string category);
    Task<List<WorkflowDefinition>> GetPublishedWorkflowDefinitionsAsync();
    Task<List<WorkflowDefinition>> GetMyWorkflowDefinitionsAsync(string userId);

    // 工作流版本管理
    Task<WorkflowDefinition> CreateVersionAsync(string definitionId, string createdBy);
    Task<List<WorkflowDefinition>> GetVersionsAsync(string definitionId);
    Task<WorkflowDefinition?> GetLatestVersionAsync(string definitionId);
    Task<bool> SetActiveVersionAsync(string definitionId, int version);
    Task<bool> PublishWorkflowAsync(string definitionId, string publishedBy);
    Task<bool> UnpublishWorkflowAsync(string definitionId);

    // 工作流设计器
    Task<bool> UpdateWorkflowNodesAsync(string definitionId, List<WorkflowNode> nodes);
    Task<bool> UpdateWorkflowConnectionsAsync(string definitionId, List<WorkflowConnection> connections);
    Task<WorkflowNode> AddNodeAsync(string definitionId, WorkflowNode node);
    Task<bool> RemoveNodeAsync(string definitionId, string nodeId);
    Task<WorkflowConnection> AddConnectionAsync(string definitionId, WorkflowConnection connection);
    Task<bool> RemoveConnectionAsync(string definitionId, string connectionId);
    Task<bool> ValidateWorkflowDefinitionAsync(string definitionId);
    Task<List<string>> GetWorkflowValidationErrorsAsync(string definitionId);

    // 工作流实例管理
    Task<WorkflowInstance> StartWorkflowAsync(string definitionId, string title, string initiatedBy, Dictionary<string, object>? variables = null);
    Task<WorkflowInstance> StartWorkflowWithContextAsync(string definitionId, string title, string initiatedBy, WorkflowContext context, Dictionary<string, object>? variables = null);
    Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId);
    Task<List<WorkflowInstance>> GetWorkflowInstancesAsync(string definitionId, int skip = 0, int take = 20);
    Task<List<WorkflowInstance>> GetMyWorkflowInstancesAsync(string userId, int skip = 0, int take = 20);
    Task<List<WorkflowInstance>> GetRunningWorkflowInstancesAsync();
    Task<bool> CancelWorkflowInstanceAsync(string instanceId, string cancelledBy, string reason);
    Task<bool> SuspendWorkflowInstanceAsync(string instanceId, string suspendedBy, string reason);
    Task<bool> ResumeWorkflowInstanceAsync(string instanceId, string resumedBy);

    // 任务管理
    Task<WorkflowTask?> GetTaskAsync(string taskId);
    Task<List<WorkflowTask>> GetTasksAsync(string instanceId);
    Task<List<WorkflowTask>> GetMyTasksAsync(string userId, int skip = 0, int take = 20);
    Task<List<WorkflowTask>> GetPendingTasksAsync(string userId);
    Task<List<WorkflowTask>> GetTasksByStatusAsync(TaskStatus status, int skip = 0, int take = 20);
    Task<bool> AssignTaskAsync(string taskId, string assignedTo, string assignedBy);
    Task<bool> ReassignTaskAsync(string taskId, string newAssignee, string reassignedBy, string reason);
    Task<bool> DelegateTaskAsync(string taskId, string delegateTo, string delegatedBy, string reason);

    // 任务执行
    Task<bool> StartTaskAsync(string taskId, string userId);
    Task<bool> CompleteTaskAsync(string taskId, string userId, TaskAction action, Dictionary<string, object>? formData = null, string? comments = null);
    Task<bool> RejectTaskAsync(string taskId, string userId, string reason);
    Task<bool> ClaimTaskAsync(string taskId, string userId);
    Task<bool> ReleaseTaskAsync(string taskId, string userId);
    Task<bool> WithdrawTaskAsync(string taskId, string userId, string reason);

    // 任务评论
    Task<TaskComment> AddTaskCommentAsync(string taskId, string content, string createdBy, CommentType type = CommentType.General);
    Task<List<TaskComment>> GetTaskCommentsAsync(string taskId);
    Task<bool> UpdateTaskCommentAsync(string commentId, string content);
    Task<bool> DeleteTaskCommentAsync(string commentId, string deletedBy);

    // 附件管理
    Task<WorkflowAttachment> AddAttachmentAsync(string instanceId, string fileName, string filePath, string uploadedBy, string? taskId = null);
    Task<List<WorkflowAttachment>> GetAttachmentsAsync(string instanceId);
    Task<List<WorkflowAttachment>> GetTaskAttachmentsAsync(string taskId);
    Task<bool> RemoveAttachmentAsync(string attachmentId, string removedBy);
    Task<byte[]> GetAttachmentContentAsync(string attachmentId);

    // 变量管理
    Task<Dictionary<string, object>> GetWorkflowVariablesAsync(string instanceId);
    Task<bool> SetWorkflowVariableAsync(string instanceId, string variableName, object value, string setBy);
    Task<bool> SetWorkflowVariablesAsync(string instanceId, Dictionary<string, object> variables, string setBy);
    Task<object?> GetWorkflowVariableAsync(string instanceId, string variableName);
    Task<bool> RemoveWorkflowVariableAsync(string instanceId, string variableName, string removedBy);

    // 权限管理
    Task<bool> HasPermissionAsync(string definitionId, string userId, PermissionType permission);
    Task<bool> GrantPermissionAsync(string definitionId, string userId, PermissionType permission, string grantedBy);
    Task<bool> RevokePermissionAsync(string definitionId, string userId, PermissionType permission, string revokedBy);
    Task<List<WorkflowPermissionInfo>> GetWorkflowPermissionsAsync(string definitionId);
    Task<List<string>> GetUserWorkflowPermissionsAsync(string definitionId, string userId);

    // 历史记录
    Task<List<WorkflowInstanceHistory>> GetInstanceHistoryAsync(string instanceId, int skip = 0, int take = 20);
    Task<List<TaskHistory>> GetTaskHistoryAsync(string taskId, int skip = 0, int take = 20);
    Task<List<WorkflowHistory>> GetDefinitionHistoryAsync(string definitionId, int skip = 0, int take = 20);
    Task<bool> AddInstanceHistoryAsync(string instanceId, WorkflowHistoryAction action, string performedBy, string description, object? oldValue = null, object? newValue = null);
    Task<bool> AddTaskHistoryAsync(string taskId, TaskHistoryAction action, string performedBy, string description, object? oldValue = null, object? newValue = null);

    // 监控和报告
    Task<WorkflowStatistics> GetWorkflowStatisticsAsync(string? definitionId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<WorkflowPerformanceData>> GetPerformanceDataAsync(string definitionId, DateTime startDate, DateTime endDate);
    Task<WorkflowInstanceMetrics> GetInstanceMetricsAsync(string instanceId);
    Task<List<WorkflowBottleneck>> GetWorkflowBottlenecksAsync(string definitionId);
    Task<WorkflowHealthCheck> GetWorkflowHealthAsync(string definitionId);

    // 通知管理
    Task<bool> SendNotificationAsync(string instanceId, string taskId, WorkflowEventType eventType, List<string> recipients);
    Task<List<WorkflowNotificationLog>> GetNotificationLogsAsync(string instanceId, int skip = 0, int take = 20);
    Task<bool> UpdateNotificationSettingsAsync(string definitionId, WorkflowNotification settings);
    Task<WorkflowNotification> GetNotificationSettingsAsync(string definitionId);

    // 升级管理
    Task<bool> EscalateTaskAsync(string taskId, string escalatedBy, string reason);
    Task<List<EscalationCandidate>> GetEscalationCandidatesAsync();
    Task<bool> ProcessEscalationsAsync();
    Task<bool> UpdateEscalationSettingsAsync(string definitionId, WorkflowEscalation settings);

    // 表单管理
    Task<Dictionary<string, object>> GetTaskFormSchemaAsync(string taskId);
    Task<Dictionary<string, object>> GetTaskFormDataAsync(string taskId);
    Task<bool> SaveTaskFormDataAsync(string taskId, Dictionary<string, object> formData, string savedBy);
    Task<bool> ValidateTaskFormDataAsync(string taskId, Dictionary<string, object> formData);
    Task<List<string>> GetFormValidationErrorsAsync(string taskId, Dictionary<string, object> formData);

    // 条件评估
    Task<bool> EvaluateConditionAsync(string expression, Dictionary<string, object> context);
    Task<object?> EvaluateExpressionAsync(string expression, Dictionary<string, object> context);
    Task<List<string>> GetAvailableVariablesAsync(string instanceId);
    Task<List<FunctionInfo>> GetAvailableFunctionsAsync();

    // 模板管理
    Task<List<WorkflowTemplate>> GetWorkflowTemplatesAsync();
    Task<WorkflowTemplate?> GetWorkflowTemplateAsync(string templateId);
    Task<WorkflowDefinition> CreateFromTemplateAsync(string templateId, string name, string createdBy);
    Task<WorkflowTemplate> SaveAsTemplateAsync(string definitionId, string templateName, string createdBy);

    // 导入导出
    Task<byte[]> ExportWorkflowDefinitionAsync(string definitionId, ExportFormat format);
    Task<ImportResult> ImportWorkflowDefinitionAsync(byte[] data, ImportFormat format, ImportOptions options);
    Task<ImportValidationResult> ValidateImportDataAsync(byte[] data, ImportFormat format);
    Task<byte[]> ExportWorkflowInstanceDataAsync(string instanceId, ExportFormat format);

    // 系统管理
    Task<WorkflowEngineStatus> GetEngineStatusAsync();
    Task<List<WorkflowJob>> GetActiveJobsAsync();
    Task<bool> StartJobAsync(string jobType, Dictionary<string, object> parameters);
    Task<bool> StopJobAsync(string jobId);
    Task<WorkflowConfiguration> GetEngineConfigurationAsync();
    Task<bool> UpdateEngineConfigurationAsync(WorkflowConfiguration configuration);

    // 缓存管理
    Task<bool> ClearDefinitionCacheAsync(string definitionId);
    Task<bool> RefreshDefinitionCacheAsync(string definitionId);
    Task<bool> ClearAllCacheAsync();
    Task<CacheStatistics> GetCacheStatisticsAsync();

    // 搜索和查询
    Task<List<WorkflowInstance>> SearchInstancesAsync(WorkflowSearchCriteria criteria);
    Task<List<WorkflowTask>> SearchTasksAsync(TaskSearchCriteria criteria);
    Task<List<WorkflowDefinition>> SearchDefinitionsAsync(DefinitionSearchCriteria criteria);
    Task<WorkflowSearchResult> AdvancedSearchAsync(AdvancedSearchCriteria criteria);
}

// 支持类定义
public class WorkflowPermissionInfo
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public PermissionType Permission { get; set; }
    public DateTime GrantedAt { get; set; }
    public string GrantedBy { get; set; } = string.Empty;
}

public class WorkflowStatistics
{
    public int TotalDefinitions { get; set; }
    public int ActiveDefinitions { get; set; }
    public int TotalInstances { get; set; }
    public int RunningInstances { get; set; }
    public int CompletedInstances { get; set; }
    public int CancelledInstances { get; set; }
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal AverageCompletionTimeHours { get; set; }
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    public List<TopWorkflow> MostUsedWorkflows { get; set; } = new();
}

public class TopWorkflow
{
    public string DefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int InstanceCount { get; set; }
    public DateTime LastUsed { get; set; }
}

public class WorkflowPerformanceData
{
    public DateTime Date { get; set; }
    public int InstancesStarted { get; set; }
    public int InstancesCompleted { get; set; }
    public decimal AverageCompletionTime { get; set; }
    public int TasksCompleted { get; set; }
    public decimal TaskCompletionRate { get; set; }
}

public class WorkflowInstanceMetrics
{
    public string InstanceId { get; set; } = string.Empty;
    public decimal TotalDurationHours { get; set; }
    public decimal ActiveDurationHours { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public decimal CompletionRate { get; set; }
    public List<NodeMetrics> NodeMetrics { get; set; } = new();
    public List<BottleneckInfo> Bottlenecks { get; set; } = new();
}

public class NodeMetrics
{
    public string NodeId { get; set; } = string.Empty;
    public string NodeName { get; set; } = string.Empty;
    public decimal AverageDurationMinutes { get; set; }
    public int CompletedCount { get; set; }
    public int PendingCount { get; set; }
    public int RejectedCount { get; set; }
}

public class BottleneckInfo
{
    public string NodeId { get; set; } = string.Empty;
    public string NodeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BottleneckSeverity Severity { get; set; }
    public decimal ImpactScore { get; set; }
}

public class WorkflowBottleneck
{
    public string DefinitionId { get; set; } = string.Empty;
    public string DefinitionName { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string NodeName { get; set; } = string.Empty;
    public int QueuedTasks { get; set; }
    public decimal AverageWaitTime { get; set; }
    public BottleneckSeverity Severity { get; set; }
    public List<string> Recommendations { get; set; } = new();
}

public class WorkflowHealthCheck
{
    public string DefinitionId { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public decimal HealthScore { get; set; }
    public List<HealthIssue> Issues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public DateTime LastChecked { get; set; }
}

public class HealthIssue
{
    public string Component { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Resolution { get; set; } = string.Empty;
}

public class WorkflowNotificationLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string InstanceId { get; set; } = string.Empty;
    public string? TaskId { get; set; }
    public WorkflowEventType EventType { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class EscalationCandidate
{
    public string TaskId { get; set; } = string.Empty;
    public string InstanceId { get; set; } = string.Empty;
    public string TaskTitle { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public int OverdueMinutes { get; set; }
    public EscalationReason Reason { get; set; }
    public List<string> EscalationTargets { get; set; } = new();
}

public class FunctionInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
    public string ReturnType { get; set; } = string.Empty;
    public List<string> Examples { get; set; } = new();
}

public class ParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string Description { get; set; } = string.Empty;
    public object? DefaultValue { get; set; }
}

public class WorkflowTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public WorkflowDefinition Definition { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; } = false;
    public int UsageCount { get; set; } = 0;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class WorkflowEngineStatus
{
    public bool IsRunning { get; set; }
    public int ActiveInstances { get; set; }
    public int QueuedTasks { get; set; }
    public int ProcessingTasks { get; set; }
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public int ThreadCount { get; set; }
    public DateTime LastActivity { get; set; }
    public List<EngineIssue> Issues { get; set; } = new();
}

public class EngineIssue
{
    public string Component { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
}

public class WorkflowJob
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Progress { get; set; }
    public string? CurrentStep { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class CacheStatistics
{
    public int TotalEntries { get; set; }
    public long TotalSize { get; set; }
    public int HitCount { get; set; }
    public int MissCount { get; set; }
    public decimal HitRate { get; set; }
    public DateTime LastCleared { get; set; }
    public List<CacheEntry> TopEntries { get; set; } = new();
}

public class CacheEntry
{
    public string Key { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastAccessed { get; set; }
    public int AccessCount { get; set; }
}

// 搜索条件类
public class WorkflowSearchCriteria
{
    public string? DefinitionId { get; set; }
    public List<WorkflowInstanceStatus>? Statuses { get; set; }
    public string? InitiatedBy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}

public class TaskSearchCriteria
{
    public string? InstanceId { get; set; }
    public string? AssignedTo { get; set; }
    public List<TaskStatus>? Statuses { get; set; }
    public List<TaskPriority>? Priorities { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public bool? IsOverdue { get; set; }
    public string? SearchTerm { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}

public class DefinitionSearchCriteria
{
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsPublished { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string? SearchTerm { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}

public class AdvancedSearchCriteria
{
    public WorkflowSearchCriteria? InstanceCriteria { get; set; }
    public TaskSearchCriteria? TaskCriteria { get; set; }
    public DefinitionSearchCriteria? DefinitionCriteria { get; set; }
    public bool IncludeInstances { get; set; } = true;
    public bool IncludeTasks { get; set; } = true;
    public bool IncludeDefinitions { get; set; } = true;
}

public class WorkflowSearchResult
{
    public List<WorkflowInstance> Instances { get; set; } = new();
    public List<WorkflowTask> Tasks { get; set; } = new();
    public List<WorkflowDefinition> Definitions { get; set; } = new();
    public int TotalInstanceCount { get; set; }
    public int TotalTaskCount { get; set; }
    public int TotalDefinitionCount { get; set; }
}

// 枚举定义
public enum BottleneckSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum NotificationChannel
{
    Email,
    Sms,
    InApp,
    WebPush
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Delivered,
    Bounced
}

public enum EscalationReason
{
    Timeout,
    Overdue,
    StuckInQueue,
    ManualEscalation,
    SystemRule
}
#endif
