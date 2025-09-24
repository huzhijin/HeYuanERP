// Disabled in minimal build: Workflow API is not enabled
#if false
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Api.Services.Workflow;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;
using HeYuanERP.Api.Services.Authorization;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkflowEngineController : ControllerBase
{
    private readonly IWorkflowEngineService _workflowService;
    private readonly ILogger<WorkflowEngineController> _logger;

    public WorkflowEngineController(
        IWorkflowEngineService workflowService,
        ILogger<WorkflowEngineController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    /// <summary>
    /// 获取工作流定义
    /// </summary>
    [HttpGet("definitions/{definitionId}")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<WorkflowDefinition>> GetWorkflowDefinition(string definitionId)
    {
        try
        {
            var definition = await _workflowService.GetWorkflowDefinitionAsync(definitionId);
            if (definition == null)
                return NotFound($"未找到工作流定义 {definitionId}");

            return Ok(definition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工作流定义失败: {DefinitionId}", definitionId);
            return StatusCode(500, "获取工作流定义失败");
        }
    }

    /// <summary>
    /// 创建工作流定义
    /// </summary>
    [HttpPost("definitions")]
    [RequirePermission("Workflow.Create")]
    public async Task<ActionResult<WorkflowDefinition>> CreateWorkflowDefinition([FromBody] WorkflowDefinition definition)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.Identity?.Name ?? "unknown";
            definition.CreatedBy = userId;

            var createdDefinition = await _workflowService.CreateWorkflowDefinitionAsync(definition);
            return CreatedAtAction(nameof(GetWorkflowDefinition),
                new { definitionId = createdDefinition.Id }, createdDefinition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建工作流定义失败");
            return StatusCode(500, "创建工作流定义失败");
        }
    }

    /// <summary>
    /// 更新工作流定义
    /// </summary>
    [HttpPut("definitions/{definitionId}")]
    [RequirePermission("Workflow.Update")]
    public async Task<ActionResult<WorkflowDefinition>> UpdateWorkflowDefinition(string definitionId, [FromBody] WorkflowDefinition definition)
    {
        try
        {
            if (definitionId != definition.Id)
                return BadRequest("工作流定义ID不匹配");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.Identity?.Name ?? "unknown";
            definition.LastModifiedBy = userId;

            var updatedDefinition = await _workflowService.UpdateWorkflowDefinitionAsync(definition);
            return Ok(updatedDefinition);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新工作流定义失败: {DefinitionId}", definitionId);
            return StatusCode(500, "更新工作流定义失败");
        }
    }

    /// <summary>
    /// 删除工作流定义
    /// </summary>
    [HttpDelete("definitions/{definitionId}")]
    [RequirePermission("Workflow.Delete")]
    public async Task<ActionResult> DeleteWorkflowDefinition(string definitionId)
    {
        try
        {
            var result = await _workflowService.DeleteWorkflowDefinitionAsync(definitionId);
            if (!result)
                return NotFound($"未找到工作流定义 {definitionId}");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除工作流定义失败: {DefinitionId}", definitionId);
            return StatusCode(500, "删除工作流定义失败");
        }
    }

    /// <summary>
    /// 获取工作流定义列表
    /// </summary>
    [HttpGet("definitions")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowDefinition>>> GetWorkflowDefinitions(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var definitions = await _workflowService.GetWorkflowDefinitionsAsync(skip, take);
            return Ok(definitions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工作流定义列表失败");
            return StatusCode(500, "获取工作流定义列表失败");
        }
    }

    /// <summary>
    /// 搜索工作流定义
    /// </summary>
    [HttpGet("definitions/search")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowDefinition>>> SearchWorkflowDefinitions(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var definitions = await _workflowService.SearchWorkflowDefinitionsAsync(searchTerm ?? "", skip, take);
            return Ok(definitions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索工作流定义失败");
            return StatusCode(500, "搜索工作流定义失败");
        }
    }

    /// <summary>
    /// 按分类获取工作流定义
    /// </summary>
    [HttpGet("definitions/category/{category}")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowDefinition>>> GetWorkflowDefinitionsByCategory(string category)
    {
        try
        {
            var definitions = await _workflowService.GetWorkflowDefinitionsByCategoryAsync(category);
            return Ok(definitions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "按分类获取工作流定义失败: {Category}", category);
            return StatusCode(500, "获取工作流定义失败");
        }
    }

    /// <summary>
    /// 获取已发布的工作流定义
    /// </summary>
    [HttpGet("definitions/published")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowDefinition>>> GetPublishedWorkflowDefinitions()
    {
        try
        {
            var definitions = await _workflowService.GetPublishedWorkflowDefinitionsAsync();
            return Ok(definitions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取已发布工作流定义失败");
            return StatusCode(500, "获取已发布工作流定义失败");
        }
    }

    /// <summary>
    /// 获取我的工作流定义
    /// </summary>
    [HttpGet("definitions/my")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowDefinition>>> GetMyWorkflowDefinitions()
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var definitions = await _workflowService.GetMyWorkflowDefinitionsAsync(userId);
            return Ok(definitions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取我的工作流定义失败");
            return StatusCode(500, "获取我的工作流定义失败");
        }
    }

    /// <summary>
    /// 发布工作流
    /// </summary>
    [HttpPost("definitions/{definitionId}/publish")]
    [RequirePermission("Workflow.Publish")]
    public async Task<ActionResult> PublishWorkflow(string definitionId)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.PublishWorkflowAsync(definitionId, userId);

            if (!result)
                return NotFound($"未找到工作流定义 {definitionId}");

            return Ok(new { message = "工作流已发布" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布工作流失败: {DefinitionId}", definitionId);
            return StatusCode(500, "发布工作流失败");
        }
    }

    /// <summary>
    /// 取消发布工作流
    /// </summary>
    [HttpPost("definitions/{definitionId}/unpublish")]
    [RequirePermission("Workflow.Publish")]
    public async Task<ActionResult> UnpublishWorkflow(string definitionId)
    {
        try
        {
            var result = await _workflowService.UnpublishWorkflowAsync(definitionId);

            if (!result)
                return NotFound($"未找到工作流定义 {definitionId}");

            return Ok(new { message = "工作流已取消发布" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消发布工作流失败: {DefinitionId}", definitionId);
            return StatusCode(500, "取消发布工作流失败");
        }
    }

    /// <summary>
    /// 验证工作流定义
    /// </summary>
    [HttpPost("definitions/{definitionId}/validate")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<WorkflowValidationResult>> ValidateWorkflowDefinition(string definitionId)
    {
        try
        {
            var isValid = await _workflowService.ValidateWorkflowDefinitionAsync(definitionId);
            var errors = await _workflowService.GetWorkflowValidationErrorsAsync(definitionId);

            return Ok(new WorkflowValidationResult
            {
                IsValid = isValid,
                Errors = errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证工作流定义失败: {DefinitionId}", definitionId);
            return StatusCode(500, "验证工作流定义失败");
        }
    }

    /// <summary>
    /// 启动工作流实例
    /// </summary>
    [HttpPost("definitions/{definitionId}/start")]
    [RequirePermission("Workflow.Execute")]
    public async Task<ActionResult<WorkflowInstance>> StartWorkflow(
        string definitionId,
        [FromBody] StartWorkflowRequest request)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var instance = await _workflowService.StartWorkflowAsync(
                definitionId,
                request.Title,
                userId,
                request.Variables);

            return CreatedAtAction(nameof(GetWorkflowInstance),
                new { instanceId = instance.Id }, instance);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动工作流失败: {DefinitionId}", definitionId);
            return StatusCode(500, "启动工作流失败");
        }
    }

    /// <summary>
    /// 获取工作流实例
    /// </summary>
    [HttpGet("instances/{instanceId}")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<WorkflowInstance>> GetWorkflowInstance(string instanceId)
    {
        try
        {
            var instance = await _workflowService.GetWorkflowInstanceAsync(instanceId);
            if (instance == null)
                return NotFound($"未找到工作流实例 {instanceId}");

            return Ok(instance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工作流实例失败: {InstanceId}", instanceId);
            return StatusCode(500, "获取工作流实例失败");
        }
    }

    /// <summary>
    /// 获取工作流实例列表
    /// </summary>
    [HttpGet("definitions/{definitionId}/instances")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowInstance>>> GetWorkflowInstances(
        string definitionId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var instances = await _workflowService.GetWorkflowInstancesAsync(definitionId, skip, take);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工作流实例列表失败: {DefinitionId}", definitionId);
            return StatusCode(500, "获取工作流实例列表失败");
        }
    }

    /// <summary>
    /// 获取我的工作流实例
    /// </summary>
    [HttpGet("instances/my")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowInstance>>> GetMyWorkflowInstances(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var instances = await _workflowService.GetMyWorkflowInstancesAsync(userId, skip, take);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取我的工作流实例失败");
            return StatusCode(500, "获取我的工作流实例失败");
        }
    }

    /// <summary>
    /// 获取运行中的工作流实例
    /// </summary>
    [HttpGet("instances/running")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowInstance>>> GetRunningWorkflowInstances()
    {
        try
        {
            var instances = await _workflowService.GetRunningWorkflowInstancesAsync();
            return Ok(instances);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取运行中工作流实例失败");
            return StatusCode(500, "获取运行中工作流实例失败");
        }
    }

    /// <summary>
    /// 取消工作流实例
    /// </summary>
    [HttpPost("instances/{instanceId}/cancel")]
    [RequirePermission("Workflow.Execute")]
    public async Task<ActionResult> CancelWorkflowInstance(string instanceId, [FromBody] CancelWorkflowRequest request)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.CancelWorkflowInstanceAsync(instanceId, userId, request.Reason);

            if (!result)
                return NotFound($"未找到工作流实例 {instanceId} 或状态不允许取消");

            return Ok(new { message = "工作流实例已取消" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消工作流实例失败: {InstanceId}", instanceId);
            return StatusCode(500, "取消工作流实例失败");
        }
    }

    /// <summary>
    /// 暂停工作流实例
    /// </summary>
    [HttpPost("instances/{instanceId}/suspend")]
    [RequirePermission("Workflow.Execute")]
    public async Task<ActionResult> SuspendWorkflowInstance(string instanceId, [FromBody] SuspendWorkflowRequest request)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.SuspendWorkflowInstanceAsync(instanceId, userId, request.Reason);

            if (!result)
                return NotFound($"未找到工作流实例 {instanceId} 或状态不允许暂停");

            return Ok(new { message = "工作流实例已暂停" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "暂停工作流实例失败: {InstanceId}", instanceId);
            return StatusCode(500, "暂停工作流实例失败");
        }
    }

    /// <summary>
    /// 恢复工作流实例
    /// </summary>
    [HttpPost("instances/{instanceId}/resume")]
    [RequirePermission("Workflow.Execute")]
    public async Task<ActionResult> ResumeWorkflowInstance(string instanceId)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.ResumeWorkflowInstanceAsync(instanceId, userId);

            if (!result)
                return NotFound($"未找到工作流实例 {instanceId} 或状态不允许恢复");

            return Ok(new { message = "工作流实例已恢复" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "恢复工作流实例失败: {InstanceId}", instanceId);
            return StatusCode(500, "恢复工作流实例失败");
        }
    }

    /// <summary>
    /// 获取任务信息
    /// </summary>
    [HttpGet("tasks/{taskId}")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<WorkflowTask>> GetTask(string taskId)
    {
        try
        {
            var task = await _workflowService.GetTaskAsync(taskId);
            if (task == null)
                return NotFound($"未找到任务 {taskId}");

            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务失败: {TaskId}", taskId);
            return StatusCode(500, "获取任务失败");
        }
    }

    /// <summary>
    /// 获取我的任务
    /// </summary>
    [HttpGet("tasks/my")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowTask>>> GetMyTasks(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var tasks = await _workflowService.GetMyTasksAsync(userId, skip, take);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取我的任务失败");
            return StatusCode(500, "获取我的任务失败");
        }
    }

    /// <summary>
    /// 获取待处理任务
    /// </summary>
    [HttpGet("tasks/pending")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowTask>>> GetPendingTasks()
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var tasks = await _workflowService.GetPendingTasksAsync(userId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取待处理任务失败");
            return StatusCode(500, "获取待处理任务失败");
        }
    }

    /// <summary>
    /// 开始任务
    /// </summary>
    [HttpPost("tasks/{taskId}/start")]
    [RequirePermission("Workflow.Execute")]
    public async Task<ActionResult> StartTask(string taskId)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.StartTaskAsync(taskId, userId);

            if (!result)
                return BadRequest("无法开始任务，请检查任务状态和权限");

            return Ok(new { message = "任务已开始" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "开始任务失败: {TaskId}", taskId);
            return StatusCode(500, "开始任务失败");
        }
    }

    /// <summary>
    /// 完成任务
    /// </summary>
    [HttpPost("tasks/{taskId}/complete")]
    [RequirePermission("Workflow.Execute")]
    public async Task<ActionResult> CompleteTask(string taskId, [FromBody] CompleteTaskRequest request)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.CompleteTaskAsync(
                taskId,
                userId,
                request.Action,
                request.FormData,
                request.Comments);

            if (!result)
                return BadRequest("无法完成任务，请检查任务状态和权限");

            return Ok(new { message = "任务已完成" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "完成任务失败: {TaskId}", taskId);
            return StatusCode(500, "完成任务失败");
        }
    }

    /// <summary>
    /// 分配任务
    /// </summary>
    [HttpPost("tasks/{taskId}/assign")]
    [RequirePermission("Workflow.Assign")]
    public async Task<ActionResult> AssignTask(string taskId, [FromBody] AssignTaskRequest request)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.AssignTaskAsync(taskId, request.AssignedTo, userId);

            if (!result)
                return NotFound($"未找到任务 {taskId}");

            return Ok(new { message = "任务已分配" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分配任务失败: {TaskId}", taskId);
            return StatusCode(500, "分配任务失败");
        }
    }

    /// <summary>
    /// 认领任务
    /// </summary>
    [HttpPost("tasks/{taskId}/claim")]
    [RequirePermission("Workflow.Execute")]
    public async Task<ActionResult> ClaimTask(string taskId)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.ClaimTaskAsync(taskId, userId);

            if (!result)
                return BadRequest("无法认领任务，请检查任务状态");

            return Ok(new { message = "任务已认领" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "认领任务失败: {TaskId}", taskId);
            return StatusCode(500, "认领任务失败");
        }
    }

    /// <summary>
    /// 添加任务评论
    /// </summary>
    [HttpPost("tasks/{taskId}/comments")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<TaskComment>> AddTaskComment(string taskId, [FromBody] AddTaskCommentRequest request)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var comment = await _workflowService.AddTaskCommentAsync(taskId, request.Content, userId, request.Type);

            return CreatedAtAction(nameof(GetTaskComments),
                new { taskId = taskId }, comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加任务评论失败: {TaskId}", taskId);
            return StatusCode(500, "添加任务评论失败");
        }
    }

    /// <summary>
    /// 获取任务评论
    /// </summary>
    [HttpGet("tasks/{taskId}/comments")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<TaskComment>>> GetTaskComments(string taskId)
    {
        try
        {
            var comments = await _workflowService.GetTaskCommentsAsync(taskId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务评论失败: {TaskId}", taskId);
            return StatusCode(500, "获取任务评论失败");
        }
    }

    /// <summary>
    /// 获取工作流变量
    /// </summary>
    [HttpGet("instances/{instanceId}/variables")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<Dictionary<string, object>>> GetWorkflowVariables(string instanceId)
    {
        try
        {
            var variables = await _workflowService.GetWorkflowVariablesAsync(instanceId);
            return Ok(variables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工作流变量失败: {InstanceId}", instanceId);
            return StatusCode(500, "获取工作流变量失败");
        }
    }

    /// <summary>
    /// 设置工作流变量
    /// </summary>
    [HttpPut("instances/{instanceId}/variables")]
    [RequirePermission("Workflow.Update")]
    public async Task<ActionResult> SetWorkflowVariables(string instanceId, [FromBody] Dictionary<string, object> variables)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var result = await _workflowService.SetWorkflowVariablesAsync(instanceId, variables, userId);

            if (!result)
                return NotFound($"未找到工作流实例 {instanceId}");

            return Ok(new { message = "工作流变量已更新" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置工作流变量失败: {InstanceId}", instanceId);
            return StatusCode(500, "设置工作流变量失败");
        }
    }

    /// <summary>
    /// 获取工作流统计信息
    /// </summary>
    [HttpGet("statistics")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<WorkflowStatistics>> GetWorkflowStatistics(
        [FromQuery] string? definitionId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var statistics = await _workflowService.GetWorkflowStatisticsAsync(definitionId, startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工作流统计信息失败");
            return StatusCode(500, "获取工作流统计信息失败");
        }
    }

    /// <summary>
    /// 获取实例历史记录
    /// </summary>
    [HttpGet("instances/{instanceId}/history")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<WorkflowInstanceHistory>>> GetInstanceHistory(
        string instanceId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var history = await _workflowService.GetInstanceHistoryAsync(instanceId, skip, take);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取实例历史记录失败: {InstanceId}", instanceId);
            return StatusCode(500, "获取实例历史记录失败");
        }
    }

    /// <summary>
    /// 获取任务历史记录
    /// </summary>
    [HttpGet("tasks/{taskId}/history")]
    [RequirePermission("Workflow.View")]
    public async Task<ActionResult<List<TaskHistory>>> GetTaskHistory(
        string taskId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var history = await _workflowService.GetTaskHistoryAsync(taskId, skip, take);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务历史记录失败: {TaskId}", taskId);
            return StatusCode(500, "获取任务历史记录失败");
        }
    }
}
// previously stray #endif removed to keep entire file disabled

// DTO 类
public class WorkflowValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class StartWorkflowRequest
{
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, object>? Variables { get; set; }
}

public class CancelWorkflowRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class SuspendWorkflowRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class CompleteTaskRequest
{
    public TaskAction Action { get; set; } = TaskAction.Approve;
    public Dictionary<string, object>? FormData { get; set; }
    public string? Comments { get; set; }
}

public class AssignTaskRequest
{
    public string AssignedTo { get; set; } = string.Empty;
}

public class AddTaskCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public CommentType Type { get; set; } = CommentType.General;
}
// Disabled in minimal build
#endif
