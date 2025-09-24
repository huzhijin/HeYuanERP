// Disabled in minimal build: Workflow service not enabled
#if false
using Microsoft.EntityFrameworkCore;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;
using HeYuanERP.Infrastructure.Data;
using System.Text.Json;

namespace HeYuanERP.Api.Services.Workflow;

public class WorkflowEngineService : IWorkflowEngineService
{
    private readonly HeYuanERPDbContext _context;
    private readonly ILogger<WorkflowEngineService> _logger;

    public WorkflowEngineService(HeYuanERPDbContext context, ILogger<WorkflowEngineService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 工作流定义管理
    public async Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId)
    {
        return await _context.WorkflowDefinitions
            .Include(d => d.Nodes)
            .Include(d => d.Connections)
            .Include(d => d.Instances.Take(10))
            .Include(d => d.History.Take(20))
            .FirstOrDefaultAsync(d => d.Id == definitionId);
    }

    public async Task<WorkflowDefinition> CreateWorkflowDefinitionAsync(WorkflowDefinition definition)
    {
        definition.CreatedAt = DateTime.UtcNow;
        definition.Version = 1;
        definition.IsActive = true;
        definition.IsPublished = false;

        _context.WorkflowDefinitions.Add(definition);
        await _context.SaveChangesAsync();

        await AddDefinitionHistoryAsync(definition.Id, WorkflowHistoryAction.Created, definition.CreatedBy, "工作流定义创建");

        _logger.LogInformation("Created workflow definition {DefinitionId} by {CreatedBy}", definition.Id, definition.CreatedBy);
        return definition;
    }

    public async Task<WorkflowDefinition> UpdateWorkflowDefinitionAsync(WorkflowDefinition definition)
    {
        var existing = await _context.WorkflowDefinitions.FindAsync(definition.Id);
        if (existing == null)
            throw new ArgumentException($"Workflow definition {definition.Id} not found");

        var oldData = JsonSerializer.Serialize(existing);

        _context.Entry(existing).CurrentValues.SetValues(definition);
        existing.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await AddDefinitionHistoryAsync(definition.Id, WorkflowHistoryAction.Modified, definition.LastModifiedBy ?? "System",
            "工作流定义更新", oldData, JsonSerializer.Serialize(definition));

        _logger.LogInformation("Updated workflow definition {DefinitionId}", definition.Id);
        return existing;
    }

    public async Task<bool> DeleteWorkflowDefinitionAsync(string definitionId)
    {
        var definition = await _context.WorkflowDefinitions.FindAsync(definitionId);
        if (definition == null) return false;

        // 检查是否有运行中的实例
        var runningInstances = await _context.WorkflowInstances
            .AnyAsync(i => i.WorkflowDefinitionId == definitionId && i.Status == WorkflowInstanceStatus.Running);

        if (runningInstances)
            throw new InvalidOperationException("无法删除有运行中实例的工作流定义");

        _context.WorkflowDefinitions.Remove(definition);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted workflow definition {DefinitionId}", definitionId);
        return true;
    }

    public async Task<List<WorkflowDefinition>> GetWorkflowDefinitionsAsync(int skip = 0, int take = 20)
    {
        return await _context.WorkflowDefinitions
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<WorkflowDefinition>> SearchWorkflowDefinitionsAsync(string searchTerm, int skip = 0, int take = 20)
    {
        var query = _context.WorkflowDefinitions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d =>
                d.Name.Contains(searchTerm) ||
                d.Description.Contains(searchTerm) ||
                d.Category.Contains(searchTerm) ||
                d.Tags.Contains(searchTerm));
        }

        return await query
            .OrderByDescending(d => d.LastModifiedAt ?? d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<WorkflowDefinition>> GetWorkflowDefinitionsByCategoryAsync(string category)
    {
        return await _context.WorkflowDefinitions
            .Where(d => d.Category == category && d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<List<WorkflowDefinition>> GetPublishedWorkflowDefinitionsAsync()
    {
        return await _context.WorkflowDefinitions
            .Where(d => d.IsPublished && d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<List<WorkflowDefinition>> GetMyWorkflowDefinitionsAsync(string userId)
    {
        return await _context.WorkflowDefinitions
            .Where(d => d.CreatedBy == userId && d.IsActive)
            .OrderByDescending(d => d.LastModifiedAt ?? d.CreatedAt)
            .ToListAsync();
    }

    // 版本管理
    public async Task<WorkflowDefinition> CreateVersionAsync(string definitionId, string createdBy)
    {
        var original = await GetWorkflowDefinitionAsync(definitionId);
        if (original == null)
            throw new ArgumentException($"Workflow definition {definitionId} not found");

        var newVersion = JsonSerializer.Deserialize<WorkflowDefinition>(JsonSerializer.Serialize(original))!;
        newVersion.Id = Guid.NewGuid().ToString("N");
        newVersion.Version = original.Version + 1;
        newVersion.CreatedBy = createdBy;
        newVersion.CreatedAt = DateTime.UtcNow;
        newVersion.LastModifiedBy = null;
        newVersion.LastModifiedAt = null;
        newVersion.IsPublished = false;

        _context.WorkflowDefinitions.Add(newVersion);
        await _context.SaveChangesAsync();

        return newVersion;
    }

    public async Task<List<WorkflowDefinition>> GetVersionsAsync(string definitionId)
    {
        var definition = await _context.WorkflowDefinitions.FindAsync(definitionId);
        if (definition == null) return new List<WorkflowDefinition>();

        return await _context.WorkflowDefinitions
            .Where(d => d.Name == definition.Name && d.Category == definition.Category)
            .OrderByDescending(d => d.Version)
            .ToListAsync();
    }

    public async Task<WorkflowDefinition?> GetLatestVersionAsync(string definitionId)
    {
        var definition = await _context.WorkflowDefinitions.FindAsync(definitionId);
        if (definition == null) return null;

        return await _context.WorkflowDefinitions
            .Where(d => d.Name == definition.Name && d.Category == definition.Category)
            .OrderByDescending(d => d.Version)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SetActiveVersionAsync(string definitionId, int version)
    {
        var definition = await _context.WorkflowDefinitions.FindAsync(definitionId);
        if (definition == null) return false;

        // 停用其他版本
        var allVersions = await _context.WorkflowDefinitions
            .Where(d => d.Name == definition.Name && d.Category == definition.Category)
            .ToListAsync();

        foreach (var ver in allVersions)
        {
            ver.IsActive = ver.Version == version;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishWorkflowAsync(string definitionId, string publishedBy)
    {
        var definition = await _context.WorkflowDefinitions.FindAsync(definitionId);
        if (definition == null) return false;

        var validationErrors = await GetWorkflowValidationErrorsAsync(definitionId);
        if (validationErrors.Any())
            throw new InvalidOperationException($"工作流验证失败: {string.Join(", ", validationErrors)}");

        definition.IsPublished = true;
        definition.PublishedBy = publishedBy;
        definition.PublishedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await AddDefinitionHistoryAsync(definitionId, WorkflowHistoryAction.Published, publishedBy, "工作流已发布");
        return true;
    }

    public async Task<bool> UnpublishWorkflowAsync(string definitionId)
    {
        var definition = await _context.WorkflowDefinitions.FindAsync(definitionId);
        if (definition == null) return false;

        definition.IsPublished = false;
        await _context.SaveChangesAsync();

        await AddDefinitionHistoryAsync(definitionId, WorkflowHistoryAction.Unpublished, "System", "工作流已取消发布");
        return true;
    }

    // 工作流设计器
    public async Task<bool> UpdateWorkflowNodesAsync(string definitionId, List<WorkflowNode> nodes)
    {
        var definition = await _context.WorkflowDefinitions
            .Include(d => d.Nodes)
            .FirstOrDefaultAsync(d => d.Id == definitionId);

        if (definition == null) return false;

        definition.Nodes.Clear();
        definition.Nodes.AddRange(nodes);
        definition.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateWorkflowConnectionsAsync(string definitionId, List<WorkflowConnection> connections)
    {
        var definition = await _context.WorkflowDefinitions
            .Include(d => d.Connections)
            .FirstOrDefaultAsync(d => d.Id == definitionId);

        if (definition == null) return false;

        definition.Connections.Clear();
        definition.Connections.AddRange(connections);
        definition.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WorkflowNode> AddNodeAsync(string definitionId, WorkflowNode node)
    {
        node.WorkflowDefinitionId = definitionId;
        _context.WorkflowNodes.Add(node);
        await _context.SaveChangesAsync();

        return node;
    }

    public async Task<bool> RemoveNodeAsync(string definitionId, string nodeId)
    {
        var node = await _context.WorkflowNodes
            .FirstOrDefaultAsync(n => n.Id == nodeId && n.WorkflowDefinitionId == definitionId);

        if (node == null) return false;

        _context.WorkflowNodes.Remove(node);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WorkflowConnection> AddConnectionAsync(string definitionId, WorkflowConnection connection)
    {
        connection.WorkflowDefinitionId = definitionId;
        _context.WorkflowConnections.Add(connection);
        await _context.SaveChangesAsync();

        return connection;
    }

    public async Task<bool> RemoveConnectionAsync(string definitionId, string connectionId)
    {
        var connection = await _context.WorkflowConnections
            .FirstOrDefaultAsync(c => c.Id == connectionId && c.WorkflowDefinitionId == definitionId);

        if (connection == null) return false;

        _context.WorkflowConnections.Remove(connection);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateWorkflowDefinitionAsync(string definitionId)
    {
        var errors = await GetWorkflowValidationErrorsAsync(definitionId);
        return errors.Count == 0;
    }

    public async Task<List<string>> GetWorkflowValidationErrorsAsync(string definitionId)
    {
        var errors = new List<string>();
        var definition = await GetWorkflowDefinitionAsync(definitionId);

        if (definition == null)
        {
            errors.Add("工作流定义不存在");
            return errors;
        }

        if (definition.Nodes.Count == 0)
            errors.Add("工作流必须包含至少一个节点");

        var startNodes = definition.Nodes.Where(n => n.Type == WorkflowNodeType.Start).ToList();
        if (startNodes.Count == 0)
            errors.Add("工作流必须包含开始节点");
        else if (startNodes.Count > 1)
            errors.Add("工作流只能包含一个开始节点");

        var endNodes = definition.Nodes.Where(n => n.Type == WorkflowNodeType.End).ToList();
        if (endNodes.Count == 0)
            errors.Add("工作流必须包含结束节点");

        // 检查节点连接
        foreach (var node in definition.Nodes)
        {
            if (node.Type != WorkflowNodeType.End)
            {
                var outgoingConnections = definition.Connections.Where(c => c.SourceNodeId == node.Id).ToList();
                if (outgoingConnections.Count == 0)
                {
                    errors.Add($"节点 {node.Name} 缺少输出连接");
                }
            }
        }

        return errors;
    }

    // 工作流实例管理
    public async Task<WorkflowInstance> StartWorkflowAsync(string definitionId, string title, string initiatedBy, Dictionary<string, object>? variables = null)
    {
        var context = new WorkflowContext();
        return await StartWorkflowWithContextAsync(definitionId, title, initiatedBy, context, variables);
    }

    public async Task<WorkflowInstance> StartWorkflowWithContextAsync(string definitionId, string title, string initiatedBy, WorkflowContext context, Dictionary<string, object>? variables = null)
    {
        var definition = await GetWorkflowDefinitionAsync(definitionId);
        if (definition == null)
            throw new ArgumentException($"Workflow definition {definitionId} not found");

        if (!definition.IsPublished)
            throw new InvalidOperationException("只能启动已发布的工作流");

        var startNode = definition.Nodes.FirstOrDefault(n => n.Type == WorkflowNodeType.Start);
        if (startNode == null)
            throw new InvalidOperationException("工作流缺少开始节点");

        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = definitionId,
            Title = title,
            Status = WorkflowInstanceStatus.Running,
            InitiatedBy = initiatedBy,
            InitiatedAt = DateTime.UtcNow,
            CurrentNodeId = startNode.Id,
            Variables = variables ?? new(),
            Context = context
        };

        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync();

        await AddInstanceHistoryAsync(instance.Id, WorkflowHistoryAction.Started, initiatedBy, "工作流实例启动");

        // 创建第一个任务
        await ProcessNodeAsync(instance.Id, startNode.Id);

        _logger.LogInformation("Started workflow instance {InstanceId} from definition {DefinitionId}", instance.Id, definitionId);
        return instance;
    }

    public async Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId)
    {
        return await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .Include(i => i.Tasks.OrderByDescending(t => t.AssignedAt).Take(10))
            .Include(i => i.History.OrderByDescending(h => h.Timestamp).Take(20))
            .Include(i => i.Attachments)
            .FirstOrDefaultAsync(i => i.Id == instanceId);
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstancesAsync(string definitionId, int skip = 0, int take = 20)
    {
        return await _context.WorkflowInstances
            .Where(i => i.WorkflowDefinitionId == definitionId)
            .OrderByDescending(i => i.InitiatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<WorkflowInstance>> GetMyWorkflowInstancesAsync(string userId, int skip = 0, int take = 20)
    {
        return await _context.WorkflowInstances
            .Where(i => i.InitiatedBy == userId)
            .OrderByDescending(i => i.InitiatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<WorkflowInstance>> GetRunningWorkflowInstancesAsync()
    {
        return await _context.WorkflowInstances
            .Where(i => i.Status == WorkflowInstanceStatus.Running)
            .OrderBy(i => i.InitiatedAt)
            .ToListAsync();
    }

    public async Task<bool> CancelWorkflowInstanceAsync(string instanceId, string cancelledBy, string reason)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance == null || instance.Status != WorkflowInstanceStatus.Running)
            return false;

        instance.Status = WorkflowInstanceStatus.Cancelled;
        instance.CompletedAt = DateTime.UtcNow;
        instance.CompletedBy = cancelledBy;
        instance.CompletionReason = reason;

        // 取消所有待处理的任务
        var pendingTasks = await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId && t.Status == TaskStatus.Pending)
            .ToListAsync();

        foreach (var task in pendingTasks)
        {
            task.Status = TaskStatus.Cancelled;
        }

        await _context.SaveChangesAsync();

        await AddInstanceHistoryAsync(instanceId, WorkflowHistoryAction.Cancelled, cancelledBy, $"工作流实例已取消: {reason}");
        return true;
    }

    public async Task<bool> SuspendWorkflowInstanceAsync(string instanceId, string suspendedBy, string reason)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance == null || instance.Status != WorkflowInstanceStatus.Running)
            return false;

        instance.Status = WorkflowInstanceStatus.Suspended;

        await _context.SaveChangesAsync();

        await AddInstanceHistoryAsync(instanceId, WorkflowHistoryAction.Suspended, suspendedBy, $"工作流实例已暂停: {reason}");
        return true;
    }

    public async Task<bool> ResumeWorkflowInstanceAsync(string instanceId, string resumedBy)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance == null || instance.Status != WorkflowInstanceStatus.Suspended)
            return false;

        instance.Status = WorkflowInstanceStatus.Running;

        await _context.SaveChangesAsync();

        await AddInstanceHistoryAsync(instanceId, WorkflowHistoryAction.Resumed, resumedBy, "工作流实例已恢复");
        return true;
    }

    // 任务管理
    public async Task<WorkflowTask?> GetTaskAsync(string taskId)
    {
        return await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .Include(t => t.History.OrderByDescending(h => h.Timestamp))
            .Include(t => t.TaskComments.OrderByDescending(c => c.CreatedAt))
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task<List<WorkflowTask>> GetTasksAsync(string instanceId)
    {
        return await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId)
            .OrderByDescending(t => t.AssignedAt)
            .ToListAsync();
    }

    public async Task<List<WorkflowTask>> GetMyTasksAsync(string userId, int skip = 0, int take = 20)
    {
        return await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .Where(t => t.AssignedTo == userId)
            .OrderByDescending(t => t.AssignedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<WorkflowTask>> GetPendingTasksAsync(string userId)
    {
        return await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .Where(t => t.AssignedTo == userId && t.Status == TaskStatus.Pending)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<List<WorkflowTask>> GetTasksByStatusAsync(TaskStatus status, int skip = 0, int take = 20)
    {
        return await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.AssignedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<bool> AssignTaskAsync(string taskId, string assignedTo, string assignedBy)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null) return false;

        var oldAssignee = task.AssignedTo;
        task.AssignedTo = assignedTo;
        task.AssignedBy = assignedBy;
        task.AssignedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await AddTaskHistoryAsync(taskId, TaskHistoryAction.Assigned, assignedBy,
            $"任务分配从 {oldAssignee} 变更为 {assignedTo}");

        return true;
    }

    public async Task<bool> ReassignTaskAsync(string taskId, string newAssignee, string reassignedBy, string reason)
    {
        return await AssignTaskAsync(taskId, newAssignee, reassignedBy);
    }

    public async Task<bool> DelegateTaskAsync(string taskId, string delegateTo, string delegatedBy, string reason)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null) return false;

        task.AssignedTo = delegateTo;
        task.AssignedBy = delegatedBy;
        task.AssignedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await AddTaskHistoryAsync(taskId, TaskHistoryAction.Delegated, delegatedBy,
            $"任务委托给 {delegateTo}: {reason}");

        return true;
    }

    // 任务执行
    public async Task<bool> StartTaskAsync(string taskId, string userId)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null || task.AssignedTo != userId || task.Status != TaskStatus.Pending)
            return false;

        task.Status = TaskStatus.InProgress;
        task.StartedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await AddTaskHistoryAsync(taskId, TaskHistoryAction.Started, userId, "任务开始执行");
        return true;
    }

    public async Task<bool> CompleteTaskAsync(string taskId, string userId, TaskAction action, Dictionary<string, object>? formData = null, string? comments = null)
    {
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .ThenInclude(i => i.WorkflowDefinition)
            .ThenInclude(d => d.Nodes)
            .ThenInclude(n => n)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null || task.AssignedTo != userId)
            return false;

        task.Status = action == TaskAction.Approve ? TaskStatus.Completed : TaskStatus.Rejected;
        task.Action = action;
        task.CompletedAt = DateTime.UtcNow;
        task.CompletedBy = userId;
        task.Comments = comments;
        task.FormData = formData ?? new();

        await _context.SaveChangesAsync();

        await AddTaskHistoryAsync(taskId, TaskHistoryAction.Completed, userId,
            $"任务完成，动作: {action}");

        // 处理下一个节点
        await ProcessNextNodeAsync(task.WorkflowInstanceId, task.NodeId, action);

        return true;
    }

    public async Task<bool> RejectTaskAsync(string taskId, string userId, string reason)
    {
        return await CompleteTaskAsync(taskId, userId, TaskAction.Reject, null, reason);
    }

    public async Task<bool> ClaimTaskAsync(string taskId, string userId)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null || task.Status != TaskStatus.Pending)
            return false;

        task.AssignedTo = userId;
        task.AssignedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await AddTaskHistoryAsync(taskId, TaskHistoryAction.Claimed, userId, "任务被认领");
        return true;
    }

    public async Task<bool> ReleaseTaskAsync(string taskId, string userId)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null || task.AssignedTo != userId)
            return false;

        task.AssignedTo = string.Empty;
        task.Status = TaskStatus.Pending;

        await _context.SaveChangesAsync();

        await AddTaskHistoryAsync(taskId, TaskHistoryAction.Released, userId, "任务被释放");
        return true;
    }

    public async Task<bool> WithdrawTaskAsync(string taskId, string userId, string reason)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null) return false;

        task.Status = TaskStatus.Withdrawn;
        task.CompletedAt = DateTime.UtcNow;
        task.Comments = reason;

        await _context.SaveChangesAsync();

        await AddTaskHistoryAsync(taskId, TaskHistoryAction.Withdrawn, userId, $"任务撤回: {reason}");
        return true;
    }

    // 任务评论
    public async Task<TaskComment> AddTaskCommentAsync(string taskId, string content, string createdBy, CommentType type = CommentType.General)
    {
        var comment = new TaskComment
        {
            TaskId = taskId,
            Content = content,
            CreatedBy = createdBy,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskComments.Add(comment);
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<List<TaskComment>> GetTaskCommentsAsync(string taskId)
    {
        return await _context.TaskComments
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateTaskCommentAsync(string commentId, string content)
    {
        var comment = await _context.TaskComments.FindAsync(commentId);
        if (comment == null) return false;

        comment.Content = content;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaskCommentAsync(string commentId, string deletedBy)
    {
        var comment = await _context.TaskComments.FindAsync(commentId);
        if (comment == null) return false;

        _context.TaskComments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }

    // 其他方法的简化实现（由于篇幅限制，这里提供核心方法的实现）
    public async Task<WorkflowAttachment> AddAttachmentAsync(string instanceId, string fileName, string filePath, string uploadedBy, string? taskId = null)
    {
        var attachment = new WorkflowAttachment
        {
            WorkflowInstanceId = instanceId,
            TaskId = taskId,
            FileName = fileName,
            FilePath = filePath,
            UploadedBy = uploadedBy,
            UploadedAt = DateTime.UtcNow
        };

        _context.WorkflowAttachments.Add(attachment);
        await _context.SaveChangesAsync();

        return attachment;
    }

    public async Task<List<WorkflowAttachment>> GetAttachmentsAsync(string instanceId)
    {
        return await _context.WorkflowAttachments
            .Where(a => a.WorkflowInstanceId == instanceId)
            .OrderByDescending(a => a.UploadedAt)
            .ToListAsync();
    }

    public async Task<List<WorkflowAttachment>> GetTaskAttachmentsAsync(string taskId)
    {
        return await _context.WorkflowAttachments
            .Where(a => a.TaskId == taskId)
            .OrderByDescending(a => a.UploadedAt)
            .ToListAsync();
    }

    public async Task<bool> RemoveAttachmentAsync(string attachmentId, string removedBy)
    {
        var attachment = await _context.WorkflowAttachments.FindAsync(attachmentId);
        if (attachment == null) return false;

        _context.WorkflowAttachments.Remove(attachment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<byte[]> GetAttachmentContentAsync(string attachmentId)
    {
        throw new NotImplementedException("File content retrieval will be implemented in future version");
    }

    // 变量管理
    public async Task<Dictionary<string, object>> GetWorkflowVariablesAsync(string instanceId)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        return instance?.Variables ?? new Dictionary<string, object>();
    }

    public async Task<bool> SetWorkflowVariableAsync(string instanceId, string variableName, object value, string setBy)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance == null) return false;

        instance.Variables[variableName] = value;
        await _context.SaveChangesAsync();

        await AddInstanceHistoryAsync(instanceId, WorkflowHistoryAction.VariableChanged, setBy,
            $"变量 {variableName} 已更新");

        return true;
    }

    public async Task<bool> SetWorkflowVariablesAsync(string instanceId, Dictionary<string, object> variables, string setBy)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance == null) return false;

        foreach (var kvp in variables)
        {
            instance.Variables[kvp.Key] = kvp.Value;
        }

        await _context.SaveChangesAsync();

        await AddInstanceHistoryAsync(instanceId, WorkflowHistoryAction.VariableChanged, setBy,
            $"批量更新了 {variables.Count} 个变量");

        return true;
    }

    public async Task<object?> GetWorkflowVariableAsync(string instanceId, string variableName)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        return instance?.Variables.GetValueOrDefault(variableName);
    }

    public async Task<bool> RemoveWorkflowVariableAsync(string instanceId, string variableName, string removedBy)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance == null) return false;

        instance.Variables.Remove(variableName);
        await _context.SaveChangesAsync();

        await AddInstanceHistoryAsync(instanceId, WorkflowHistoryAction.VariableChanged, removedBy,
            $"变量 {variableName} 已删除");

        return true;
    }

    // 简化实现的其他方法
    public async Task<bool> HasPermissionAsync(string definitionId, string userId, PermissionType permission)
    {
        // MVP实现：简单权限检查
        return true;
    }

    public async Task<bool> GrantPermissionAsync(string definitionId, string userId, PermissionType permission, string grantedBy)
    {
        throw new NotImplementedException("Permission management will be implemented in future version");
    }

    public async Task<bool> RevokePermissionAsync(string definitionId, string userId, PermissionType permission, string revokedBy)
    {
        throw new NotImplementedException("Permission management will be implemented in future version");
    }

    public async Task<List<WorkflowPermissionInfo>> GetWorkflowPermissionsAsync(string definitionId)
    {
        throw new NotImplementedException("Permission management will be implemented in future version");
    }

    public async Task<List<string>> GetUserWorkflowPermissionsAsync(string definitionId, string userId)
    {
        throw new NotImplementedException("Permission management will be implemented in future version");
    }

    // 历史记录
    public async Task<List<WorkflowInstanceHistory>> GetInstanceHistoryAsync(string instanceId, int skip = 0, int take = 20)
    {
        return await _context.WorkflowInstanceHistories
            .Where(h => h.WorkflowInstanceId == instanceId)
            .OrderByDescending(h => h.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<TaskHistory>> GetTaskHistoryAsync(string taskId, int skip = 0, int take = 20)
    {
        return await _context.TaskHistories
            .Where(h => h.TaskId == taskId)
            .OrderByDescending(h => h.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<WorkflowHistory>> GetDefinitionHistoryAsync(string definitionId, int skip = 0, int take = 20)
    {
        return await _context.WorkflowHistories
            .Where(h => h.WorkflowDefinitionId == definitionId)
            .OrderByDescending(h => h.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<bool> AddInstanceHistoryAsync(string instanceId, WorkflowHistoryAction action, string performedBy, string description, object? oldValue = null, object? newValue = null)
    {
        var history = new WorkflowInstanceHistory
        {
            WorkflowInstanceId = instanceId,
            Action = action,
            PerformedBy = performedBy,
            Description = description,
            OldValue = oldValue,
            NewValue = newValue,
            Timestamp = DateTime.UtcNow
        };

        _context.WorkflowInstanceHistories.Add(history);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTaskHistoryAsync(string taskId, TaskHistoryAction action, string performedBy, string description, object? oldValue = null, object? newValue = null)
    {
        var history = new TaskHistory
        {
            TaskId = taskId,
            Action = action,
            PerformedBy = performedBy,
            Description = description,
            OldValue = oldValue,
            NewValue = newValue,
            Timestamp = DateTime.UtcNow
        };

        _context.TaskHistories.Add(history);
        await _context.SaveChangesAsync();
        return true;
    }

    // 监控和统计
    public async Task<WorkflowStatistics> GetWorkflowStatisticsAsync(string? definitionId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var totalDefinitions = await _context.WorkflowDefinitions.CountAsync();
        var activeDefinitions = await _context.WorkflowDefinitions.CountAsync(d => d.IsActive);
        var totalInstances = await _context.WorkflowInstances.CountAsync();
        var runningInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == WorkflowInstanceStatus.Running);

        return new WorkflowStatistics
        {
            TotalDefinitions = totalDefinitions,
            ActiveDefinitions = activeDefinitions,
            TotalInstances = totalInstances,
            RunningInstances = runningInstances,
            CompletedInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == WorkflowInstanceStatus.Completed),
            CancelledInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == WorkflowInstanceStatus.Cancelled),
            TotalTasks = await _context.WorkflowTasks.CountAsync(),
            PendingTasks = await _context.WorkflowTasks.CountAsync(t => t.Status == TaskStatus.Pending),
            OverdueTasks = await _context.WorkflowTasks.CountAsync(t => t.DueDate < DateTime.UtcNow && t.Status == TaskStatus.Pending)
        };
    }

    // 私有辅助方法
    private async Task ProcessNodeAsync(string instanceId, string nodeId)
    {
        var instance = await GetWorkflowInstanceAsync(instanceId);
        var definition = instance?.WorkflowDefinition;
        var node = definition?.Nodes.FirstOrDefault(n => n.Id == nodeId);

        if (node == null) return;

        if (node.Type == WorkflowNodeType.Task)
        {
            // 创建任务
            var task = new WorkflowTask
            {
                WorkflowInstanceId = instanceId,
                NodeId = nodeId,
                Title = node.Name,
                Description = node.Description,
                Status = TaskStatus.Pending,
                AssignedTo = node.Assignment.Assignees.FirstOrDefault() ?? "admin",
                AssignedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddMinutes(node.TimeoutMinutes)
            };

            _context.WorkflowTasks.Add(task);
            await _context.SaveChangesAsync();
        }
        else if (node.Type == WorkflowNodeType.End)
        {
            // 完成工作流
            if (instance != null)
            {
                instance.Status = WorkflowInstanceStatus.Completed;
                instance.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await AddInstanceHistoryAsync(instanceId, WorkflowHistoryAction.Completed, "System", "工作流实例完成");
            }
        }
    }

    private async Task ProcessNextNodeAsync(string instanceId, string currentNodeId, TaskAction action)
    {
        var instance = await GetWorkflowInstanceAsync(instanceId);
        var definition = instance?.WorkflowDefinition;

        if (definition == null) return;

        var nextConnections = definition.Connections
            .Where(c => c.SourceNodeId == currentNodeId)
            .ToList();

        if (action == TaskAction.Reject)
        {
            // 处理拒绝逻辑
            var rejectConnection = nextConnections.FirstOrDefault(c => c.Name?.Contains("reject") == true || c.Name?.Contains("拒绝") == true);
            if (rejectConnection != null)
            {
                await ProcessNodeAsync(instanceId, rejectConnection.TargetNodeId);
                return;
            }
        }

        // 处理默认流程
        var defaultConnection = nextConnections.FirstOrDefault(c => c.IsDefault) ?? nextConnections.FirstOrDefault();
        if (defaultConnection != null)
        {
            await ProcessNodeAsync(instanceId, defaultConnection.TargetNodeId);
        }
    }

    private async Task AddDefinitionHistoryAsync(string definitionId, WorkflowHistoryAction action, string performedBy, string description, object? oldValue = null, object? newValue = null)
    {
        var history = new WorkflowHistory
        {
            WorkflowDefinitionId = definitionId,
            Action = action,
            PerformedBy = performedBy,
            Description = description,
            OldValue = oldValue,
            NewValue = newValue,
            Timestamp = DateTime.UtcNow
        };

        _context.WorkflowHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    // 其他接口方法的占位符实现
    public async Task<List<WorkflowPerformanceData>> GetPerformanceDataAsync(string definitionId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException("Performance analytics will be implemented in future version");
    }

    public async Task<WorkflowInstanceMetrics> GetInstanceMetricsAsync(string instanceId)
    {
        throw new NotImplementedException("Instance metrics will be implemented in future version");
    }

    public async Task<List<WorkflowBottleneck>> GetWorkflowBottlenecksAsync(string definitionId)
    {
        throw new NotImplementedException("Bottleneck analysis will be implemented in future version");
    }

    public async Task<WorkflowHealthCheck> GetWorkflowHealthAsync(string definitionId)
    {
        throw new NotImplementedException("Health check will be implemented in future version");
    }

    public async Task<bool> SendNotificationAsync(string instanceId, string taskId, WorkflowEventType eventType, List<string> recipients)
    {
        throw new NotImplementedException("Notification system will be implemented in future version");
    }

    public async Task<List<WorkflowNotificationLog>> GetNotificationLogsAsync(string instanceId, int skip = 0, int take = 20)
    {
        throw new NotImplementedException("Notification logging will be implemented in future version");
    }

    public async Task<bool> UpdateNotificationSettingsAsync(string definitionId, WorkflowNotification settings)
    {
        throw new NotImplementedException("Notification settings will be implemented in future version");
    }

    public async Task<WorkflowNotification> GetNotificationSettingsAsync(string definitionId)
    {
        throw new NotImplementedException("Notification settings will be implemented in future version");
    }

    public async Task<bool> EscalateTaskAsync(string taskId, string escalatedBy, string reason)
    {
        throw new NotImplementedException("Task escalation will be implemented in future version");
    }

    public async Task<List<EscalationCandidate>> GetEscalationCandidatesAsync()
    {
        throw new NotImplementedException("Escalation management will be implemented in future version");
    }

    public async Task<bool> ProcessEscalationsAsync()
    {
        throw new NotImplementedException("Escalation processing will be implemented in future version");
    }

    public async Task<bool> UpdateEscalationSettingsAsync(string definitionId, WorkflowEscalation settings)
    {
        throw new NotImplementedException("Escalation settings will be implemented in future version");
    }

    public async Task<Dictionary<string, object>> GetTaskFormSchemaAsync(string taskId)
    {
        throw new NotImplementedException("Form management will be implemented in future version");
    }

    public async Task<Dictionary<string, object>> GetTaskFormDataAsync(string taskId)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        return task?.FormData ?? new Dictionary<string, object>();
    }

    public async Task<bool> SaveTaskFormDataAsync(string taskId, Dictionary<string, object> formData, string savedBy)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null) return false;

        task.FormData = formData;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateTaskFormDataAsync(string taskId, Dictionary<string, object> formData)
    {
        throw new NotImplementedException("Form validation will be implemented in future version");
    }

    public async Task<List<string>> GetFormValidationErrorsAsync(string taskId, Dictionary<string, object> formData)
    {
        throw new NotImplementedException("Form validation will be implemented in future version");
    }

    public async Task<bool> EvaluateConditionAsync(string expression, Dictionary<string, object> context)
    {
        throw new NotImplementedException("Expression evaluation will be implemented in future version");
    }

    public async Task<object?> EvaluateExpressionAsync(string expression, Dictionary<string, object> context)
    {
        throw new NotImplementedException("Expression evaluation will be implemented in future version");
    }

    public async Task<List<string>> GetAvailableVariablesAsync(string instanceId)
    {
        var variables = await GetWorkflowVariablesAsync(instanceId);
        return variables.Keys.ToList();
    }

    public async Task<List<FunctionInfo>> GetAvailableFunctionsAsync()
    {
        throw new NotImplementedException("Function library will be implemented in future version");
    }

    public async Task<List<WorkflowTemplate>> GetWorkflowTemplatesAsync()
    {
        throw new NotImplementedException("Template management will be implemented in future version");
    }

    public async Task<WorkflowTemplate?> GetWorkflowTemplateAsync(string templateId)
    {
        throw new NotImplementedException("Template management will be implemented in future version");
    }

    public async Task<WorkflowDefinition> CreateFromTemplateAsync(string templateId, string name, string createdBy)
    {
        throw new NotImplementedException("Template instantiation will be implemented in future version");
    }

    public async Task<WorkflowTemplate> SaveAsTemplateAsync(string definitionId, string templateName, string createdBy)
    {
        throw new NotImplementedException("Template creation will be implemented in future version");
    }

    public async Task<byte[]> ExportWorkflowDefinitionAsync(string definitionId, ExportFormat format)
    {
        throw new NotImplementedException("Export functionality will be implemented in future version");
    }

    public async Task<ImportResult> ImportWorkflowDefinitionAsync(byte[] data, ImportFormat format, ImportOptions options)
    {
        throw new NotImplementedException("Import functionality will be implemented in future version");
    }

    public async Task<ImportValidationResult> ValidateImportDataAsync(byte[] data, ImportFormat format)
    {
        throw new NotImplementedException("Import validation will be implemented in future version");
    }

    public async Task<byte[]> ExportWorkflowInstanceDataAsync(string instanceId, ExportFormat format)
    {
        throw new NotImplementedException("Instance export will be implemented in future version");
    }

    public async Task<WorkflowEngineStatus> GetEngineStatusAsync()
    {
        throw new NotImplementedException("Engine monitoring will be implemented in future version");
    }

    public async Task<List<WorkflowJob>> GetActiveJobsAsync()
    {
        throw new NotImplementedException("Job management will be implemented in future version");
    }

    public async Task<bool> StartJobAsync(string jobType, Dictionary<string, object> parameters)
    {
        throw new NotImplementedException("Job management will be implemented in future version");
    }

    public async Task<bool> StopJobAsync(string jobId)
    {
        throw new NotImplementedException("Job management will be implemented in future version");
    }

    public async Task<WorkflowConfiguration> GetEngineConfigurationAsync()
    {
        throw new NotImplementedException("Engine configuration will be implemented in future version");
    }

    public async Task<bool> UpdateEngineConfigurationAsync(WorkflowConfiguration configuration)
    {
        throw new NotImplementedException("Engine configuration will be implemented in future version");
    }

    public async Task<bool> ClearDefinitionCacheAsync(string definitionId)
    {
        throw new NotImplementedException("Cache management will be implemented in future version");
    }

    public async Task<bool> RefreshDefinitionCacheAsync(string definitionId)
    {
        throw new NotImplementedException("Cache management will be implemented in future version");
    }

    public async Task<bool> ClearAllCacheAsync()
    {
        throw new NotImplementedException("Cache management will be implemented in future version");
    }

    public async Task<CacheStatistics> GetCacheStatisticsAsync()
    {
        throw new NotImplementedException("Cache statistics will be implemented in future version");
    }

    public async Task<List<WorkflowInstance>> SearchInstancesAsync(WorkflowSearchCriteria criteria)
    {
        throw new NotImplementedException("Advanced search will be implemented in future version");
    }

    public async Task<List<WorkflowTask>> SearchTasksAsync(TaskSearchCriteria criteria)
    {
        throw new NotImplementedException("Advanced search will be implemented in future version");
    }

    public async Task<List<WorkflowDefinition>> SearchDefinitionsAsync(DefinitionSearchCriteria criteria)
    {
        throw new NotImplementedException("Advanced search will be implemented in future version");
    }

    public async Task<WorkflowSearchResult> AdvancedSearchAsync(AdvancedSearchCriteria criteria)
    {
        throw new NotImplementedException("Advanced search will be implemented in future version");
    }
}
#endif
