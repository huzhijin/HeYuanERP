#if false
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 工作流定义实体
/// </summary>
public class WorkflowDefinition
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public bool IsPublished { get; set; } = false;

    public WorkflowConfiguration Configuration { get; set; } = new();

    public List<WorkflowNode> Nodes { get; set; } = new();

    public List<WorkflowConnection> Connections { get; set; } = new();

    public WorkflowVariables Variables { get; set; } = new();

    public WorkflowPermissions Permissions { get; set; } = new();

    [Required]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    [MaxLength(50)]
    public string? PublishedBy { get; set; }

    public DateTime? PublishedAt { get; set; }

    [MaxLength(1000)]
    public string Tags { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; set; } = string.Empty;

    public Dictionary<string, object> ExtensionData { get; set; } = new();

    // 导航属性
    public List<WorkflowInstance> Instances { get; set; } = new();
    public List<WorkflowHistory> History { get; set; } = new();
}

/// <summary>
/// 工作流配置
/// </summary>
public class WorkflowConfiguration
{
    public bool AllowParallelExecution { get; set; } = false;

    public int MaxConcurrentInstances { get; set; } = 10;

    public int TimeoutMinutes { get; set; } = 1440; // 24小时

    public bool AutoStart { get; set; } = false;

    public List<string> StartTriggers { get; set; } = new();

    public WorkflowEscalation Escalation { get; set; } = new();

    public WorkflowNotification Notification { get; set; } = new();

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 工作流升级配置
/// </summary>
public class WorkflowEscalation
{
    public bool IsEnabled { get; set; } = false;

    public int EscalationTimeoutMinutes { get; set; } = 60;

    public List<EscalationRule> Rules { get; set; } = new();
}

/// <summary>
/// 升级规则
/// </summary>
public class EscalationRule
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public EscalationTrigger Trigger { get; set; } = EscalationTrigger.Timeout;

    public int TriggerValue { get; set; } = 60;

    [Required]
    public EscalationAction Action { get; set; } = EscalationAction.Notify;

    public List<string> Recipients { get; set; } = new();

    public string ActionData { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// 工作流通知配置
/// </summary>
public class WorkflowNotification
{
    public bool IsEnabled { get; set; } = true;

    public List<NotificationEvent> Events { get; set; } = new();

    public NotificationTemplate EmailTemplate { get; set; } = new();

    public NotificationTemplate SmsTemplate { get; set; } = new();

    public Dictionary<string, object> Settings { get; set; } = new();
}

/// <summary>
/// 通知事件
/// </summary>
public class NotificationEvent
{
    [Required]
    public WorkflowEventType EventType { get; set; } = WorkflowEventType.TaskAssigned;

    public bool SendEmail { get; set; } = true;

    public bool SendSms { get; set; } = false;

    public bool SendInApp { get; set; } = true;

    public List<string> Recipients { get; set; } = new();

    public string CustomTemplate { get; set; } = string.Empty;
}

/// <summary>
/// 通知模板
/// </summary>
public class NotificationTemplate
{
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; } = false;

    public Dictionary<string, string> Variables { get; set; } = new();
}

/// <summary>
/// 工作流节点
/// </summary>
public class WorkflowNode
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string WorkflowDefinitionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public WorkflowNodeType Type { get; set; } = WorkflowNodeType.Task;

    public NodePosition Position { get; set; } = new();

    public NodeConfiguration Configuration { get; set; } = new();

    public NodeAssignment Assignment { get; set; } = new();

    public NodeValidation Validation { get; set; } = new();

    public List<NodeAction> Actions { get; set; } = new();

    public List<NodeCondition> Conditions { get; set; } = new();

    public bool IsRequired { get; set; } = true;

    public bool CanSkip { get; set; } = false;

    public int TimeoutMinutes { get; set; } = 60;

    public Dictionary<string, object> Properties { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(WorkflowDefinitionId))]
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;
}

/// <summary>
/// 节点位置
/// </summary>
public class NodePosition
{
    public double X { get; set; } = 0;

    public double Y { get; set; } = 0;

    public double Width { get; set; } = 100;

    public double Height { get; set; } = 80;
}

/// <summary>
/// 节点配置
/// </summary>
public class NodeConfiguration
{
    public bool AllowDelegate { get; set; } = true;

    public bool AllowReject { get; set; } = true;

    public bool AllowWithdraw { get; set; } = false;

    public bool RequireComment { get; set; } = false;

    public bool AllowAttachment { get; set; } = true;

    public List<string> RequiredFields { get; set; } = new();

    public Dictionary<string, object> FormFields { get; set; } = new();

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 节点分配
/// </summary>
public class NodeAssignment
{
    [Required]
    public AssignmentType Type { get; set; } = AssignmentType.User;

    public List<string> Assignees { get; set; } = new();

    public AssignmentStrategy Strategy { get; set; } = AssignmentStrategy.All;

    public string AssignmentExpression { get; set; } = string.Empty;

    public bool AllowSelfAssign { get; set; } = false;

    public bool AllowReassign { get; set; } = true;

    public Dictionary<string, object> AssignmentRules { get; set; } = new();
}

/// <summary>
/// 节点验证
/// </summary>
public class NodeValidation
{
    public bool IsEnabled { get; set; } = false;

    public List<ValidationRule> Rules { get; set; } = new();

    public string ValidationExpression { get; set; } = string.Empty;

    public bool StopOnFirstError { get; set; } = true;

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
    public ValidationType Type { get; set; } = ValidationType.Required;

    [Required]
    public string Expression { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ErrorMessage { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 节点动作
/// </summary>
public class NodeAction
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ActionType Type { get; set; } = ActionType.Custom;

    [Required]
    public ActionTrigger Trigger { get; set; } = ActionTrigger.BeforeExecution;

    public string ActionExpression { get; set; } = string.Empty;

    public bool IsAsync { get; set; } = false;

    public bool ContinueOnError { get; set; } = true;

    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 节点条件
/// </summary>
public class NodeCondition
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ConditionType Type { get; set; } = ConditionType.Expression;

    [Required]
    public string Expression { get; set; } = string.Empty;

    public object? Value { get; set; }

    public LogicalOperator Operator { get; set; } = LogicalOperator.And;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// 工作流连接
/// </summary>
public class WorkflowConnection
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string WorkflowDefinitionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SourceNodeId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string TargetNodeId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ConnectionType Type { get; set; } = ConnectionType.Sequence;

    public string ConditionExpression { get; set; } = string.Empty;

    public int Priority { get; set; } = 0;

    public bool IsDefault { get; set; } = false;

    public ConnectionStyle Style { get; set; } = new();

    public Dictionary<string, object> Properties { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(WorkflowDefinitionId))]
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;
}

/// <summary>
/// 连接样式
/// </summary>
public class ConnectionStyle
{
    public string Color { get; set; } = "#000000";

    public int Width { get; set; } = 2;

    public string LineType { get; set; } = "solid";

    public bool ShowArrow { get; set; } = true;

    public Dictionary<string, object> CustomStyles { get; set; } = new();
}

/// <summary>
/// 工作流变量
/// </summary>
public class WorkflowVariables
{
    public List<WorkflowVariable> Variables { get; set; } = new();

    public Dictionary<string, object> DefaultValues { get; set; } = new();

    public Dictionary<string, object> GlobalSettings { get; set; } = new();
}

/// <summary>
/// 工作流变量定义
/// </summary>
public class WorkflowVariable
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public VariableType Type { get; set; } = VariableType.String;

    public object? DefaultValue { get; set; }

    public bool IsRequired { get; set; } = false;

    public bool IsReadOnly { get; set; } = false;

    public VariableScope Scope { get; set; } = VariableScope.Global;

    public VariableConstraints Constraints { get; set; } = new();
}

/// <summary>
/// 变量约束
/// </summary>
public class VariableConstraints
{
    public object? MinValue { get; set; }

    public object? MaxValue { get; set; }

    public int? MinLength { get; set; }

    public int? MaxLength { get; set; }

    public string? RegexPattern { get; set; }

    public List<object> AllowedValues { get; set; } = new();

    public string? ValidationExpression { get; set; }
}

/// <summary>
/// 工作流权限
/// </summary>
public class WorkflowPermissions
{
    public List<PermissionRule> Rules { get; set; } = new();

    public bool InheritFromParent { get; set; } = true;

    public Dictionary<string, List<string>> RolePermissions { get; set; } = new();

    public Dictionary<string, List<string>> UserPermissions { get; set; } = new();
}

/// <summary>
/// 权限规则
/// </summary>
public class PermissionRule
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public PermissionType Type { get; set; } = PermissionType.View;

    [Required]
    public PermissionTarget Target { get; set; } = PermissionTarget.User;

    public List<string> Principals { get; set; } = new();

    public bool Allow { get; set; } = true;

    public string ConditionExpression { get; set; } = string.Empty;

    public int Priority { get; set; } = 0;
}

/// <summary>
/// 工作流实例
/// </summary>
public class WorkflowInstance
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string WorkflowDefinitionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.Running;

    public WorkflowPriority Priority { get; set; } = WorkflowPriority.Normal;

    [Required]
    [MaxLength(50)]
    public string InitiatedBy { get; set; } = string.Empty;

    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public DateTime? DueDate { get; set; }

    [MaxLength(50)]
    public string? CurrentNodeId { get; set; }

    public Dictionary<string, object> Variables { get; set; } = new();

    public WorkflowContext Context { get; set; } = new();

    public WorkflowMetrics Metrics { get; set; } = new();

    [MaxLength(2000)]
    public string? CompletionReason { get; set; }

    [MaxLength(50)]
    public string? CompletedBy { get; set; }

    public Dictionary<string, object> ExtensionData { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(WorkflowDefinitionId))]
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;

    public List<WorkflowTask> Tasks { get; set; } = new();
    public List<WorkflowInstanceHistory> History { get; set; } = new();
    public List<WorkflowAttachment> Attachments { get; set; } = new();
}

/// <summary>
/// 工作流上下文
/// </summary>
public class WorkflowContext
{
    public string? BusinessKey { get; set; }

    public string? RelatedEntityType { get; set; }

    public string? RelatedEntityId { get; set; }

    public Dictionary<string, object> BusinessData { get; set; } = new();

    public Dictionary<string, object> RuntimeData { get; set; } = new();

    public List<string> ParticipantHistory { get; set; } = new();
}

/// <summary>
/// 工作流指标
/// </summary>
public class WorkflowMetrics
{
    public decimal TotalDurationMinutes { get; set; } = 0;

    public decimal ActiveDurationMinutes { get; set; } = 0;

    public int TaskCount { get; set; } = 0;

    public int CompletedTaskCount { get; set; } = 0;

    public int RejectedCount { get; set; } = 0;

    public int ReassignedCount { get; set; } = 0;

    public DateTime? FirstTaskStarted { get; set; }

    public DateTime? LastTaskCompleted { get; set; }

    public Dictionary<string, decimal> NodeDurations { get; set; } = new();
}

/// <summary>
/// 工作流任务
/// </summary>
public class WorkflowTask
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NodeId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    [Required]
    [MaxLength(50)]
    public string AssignedTo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? AssignedBy { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? DueDate { get; set; }

    [MaxLength(50)]
    public string? CompletedBy { get; set; }

    public TaskAction? Action { get; set; }

    [MaxLength(2000)]
    public string? Comments { get; set; }

    public Dictionary<string, object> FormData { get; set; } = new();

    public TaskConfiguration Configuration { get; set; } = new();

    public Dictionary<string, object> ExtensionData { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(WorkflowInstanceId))]
    public WorkflowInstance WorkflowInstance { get; set; } = null!;

    public List<TaskHistory> History { get; set; } = new();
    public List<TaskComment> TaskComments { get; set; } = new();
}

/// <summary>
/// 任务配置
/// </summary>
public class TaskConfiguration
{
    public bool AllowDelegate { get; set; } = true;

    public bool AllowReject { get; set; } = true;

    public bool RequireComment { get; set; } = false;

    public bool AllowAttachment { get; set; } = true;

    public List<string> AvailableActions { get; set; } = new();

    public Dictionary<string, object> FormSchema { get; set; } = new();

    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// 工作流实例历史
/// </summary>
public class WorkflowInstanceHistory
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    [Required]
    public WorkflowHistoryAction Action { get; set; } = WorkflowHistoryAction.Started;

    [Required]
    [MaxLength(50)]
    public string PerformedBy { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? NodeId { get; set; }

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public object? OldValue { get; set; }

    public object? NewValue { get; set; }

    [MaxLength(2000)]
    public string? Comments { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(WorkflowInstanceId))]
    public WorkflowInstance WorkflowInstance { get; set; } = null!;
}

/// <summary>
/// 任务历史
/// </summary>
public class TaskHistory
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string TaskId { get; set; } = string.Empty;

    [Required]
    public TaskHistoryAction Action { get; set; } = TaskHistoryAction.Assigned;

    [Required]
    [MaxLength(50)]
    public string PerformedBy { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public object? OldValue { get; set; }

    public object? NewValue { get; set; }

    [MaxLength(2000)]
    public string? Comments { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(TaskId))]
    public WorkflowTask Task { get; set; } = null!;
}

/// <summary>
/// 任务评论
/// </summary>
public class TaskComment
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string TaskId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Content { get; set; } = string.Empty;

    public CommentType Type { get; set; } = CommentType.General;

    public bool IsInternal { get; set; } = false;

    [MaxLength(50)]
    public string? ReplyToId { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(TaskId))]
    public WorkflowTask Task { get; set; } = null!;
}

/// <summary>
/// 工作流附件
/// </summary>
public class WorkflowAttachment
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TaskId { get; set; }

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

    [Required]
    [MaxLength(50)]
    public string UploadedBy { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Description { get; set; }

    public AttachmentType Type { get; set; } = AttachmentType.Document;

    public bool IsRequired { get; set; } = false;

    public Dictionary<string, object> Metadata { get; set; } = new();

    // 导航属性
    [ForeignKey(nameof(WorkflowInstanceId))]
    public WorkflowInstance WorkflowInstance { get; set; } = null!;
}

/// <summary>
/// 工作流历史记录
/// </summary>
public class WorkflowHistory
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [MaxLength(50)]
    public string WorkflowDefinitionId { get; set; } = string.Empty;

    [Required]
    public WorkflowHistoryAction Action { get; set; } = WorkflowHistoryAction.Created;

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
    [ForeignKey(nameof(WorkflowDefinitionId))]
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;
}
// Disabled non-P0 domain entities for minimal build
#endif
