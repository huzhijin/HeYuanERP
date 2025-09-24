// Disabled in minimal build: Reporting API is not enabled
#if false
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Api.Services.Reports;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;
using HeYuanERP.Api.Services.Authorization;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AutomatedReportController : ControllerBase
{
    private readonly IAutomatedReportService _reportService;
    private readonly ILogger<AutomatedReportController> _logger;

    public AutomatedReportController(
        IAutomatedReportService reportService,
        ILogger<AutomatedReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// 获取报表信息
    /// </summary>
    [HttpGet("{reportId}")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<AutomatedReport>> GetReport(string reportId)
    {
        try
        {
            var report = await _reportService.GetReportAsync(reportId);
            if (report == null)
                return NotFound($"未找到报表 {reportId}");

            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表信息失败: {ReportId}", reportId);
            return StatusCode(500, "获取报表信息失败");
        }
    }

    /// <summary>
    /// 创建报表
    /// </summary>
    [HttpPost]
    [RequirePermission("Report.Create")]
    public async Task<ActionResult<AutomatedReport>> CreateReport([FromBody] AutomatedReport report)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdReport = await _reportService.CreateReportAsync(report);
            return CreatedAtAction(nameof(GetReport), new { reportId = createdReport.Id }, createdReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建报表失败");
            return StatusCode(500, "创建报表失败");
        }
    }

    /// <summary>
    /// 更新报表
    /// </summary>
    [HttpPut("{reportId}")]
    [RequirePermission("Report.Update")]
    public async Task<ActionResult<AutomatedReport>> UpdateReport(string reportId, [FromBody] AutomatedReport report)
    {
        try
        {
            if (reportId != report.Id)
                return BadRequest("报表ID不匹配");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedReport = await _reportService.UpdateReportAsync(report);
            return Ok(updatedReport);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新报表失败: {ReportId}", reportId);
            return StatusCode(500, "更新报表失败");
        }
    }

    /// <summary>
    /// 删除报表
    /// </summary>
    [HttpDelete("{reportId}")]
    [RequirePermission("Report.Delete")]
    public async Task<ActionResult> DeleteReport(string reportId)
    {
        try
        {
            var result = await _reportService.DeleteReportAsync(reportId);
            if (!result)
                return NotFound($"未找到报表 {reportId}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除报表失败: {ReportId}", reportId);
            return StatusCode(500, "删除报表失败");
        }
    }

    /// <summary>
    /// 获取报表列表
    /// </summary>
    [HttpGet]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<AutomatedReport>>> GetReports(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var reports = await _reportService.GetReportsAsync(skip, take);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表列表失败");
            return StatusCode(500, "获取报表列表失败");
        }
    }

    /// <summary>
    /// 搜索报表
    /// </summary>
    [HttpGet("search")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<AutomatedReport>>> SearchReports(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var reports = await _reportService.SearchReportsAsync(searchTerm ?? "", skip, take);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索报表失败");
            return StatusCode(500, "搜索报表失败");
        }
    }

    /// <summary>
    /// 按类型获取报表
    /// </summary>
    [HttpGet("by-type/{type}")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<AutomatedReport>>> GetReportsByType(ReportType type)
    {
        try
        {
            var reports = await _reportService.GetReportsByTypeAsync(type);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "按类型获取报表失败: {Type}", type);
            return StatusCode(500, "获取报表失败");
        }
    }

    /// <summary>
    /// 按分类获取报表
    /// </summary>
    [HttpGet("by-category/{category}")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<AutomatedReport>>> GetReportsByCategory(ReportCategory category)
    {
        try
        {
            var reports = await _reportService.GetReportsByCategoryAsync(category);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "按分类获取报表失败: {Category}", category);
            return StatusCode(500, "获取报表失败");
        }
    }

    /// <summary>
    /// 获取公共报表
    /// </summary>
    [HttpGet("public")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<AutomatedReport>>> GetPublicReports()
    {
        try
        {
            var reports = await _reportService.GetPublicReportsAsync();
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取公共报表失败");
            return StatusCode(500, "获取公共报表失败");
        }
    }

    /// <summary>
    /// 获取我的报表
    /// </summary>
    [HttpGet("my-reports")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<AutomatedReport>>> GetMyReports()
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var reports = await _reportService.GetMyReportsAsync(userId);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取我的报表失败");
            return StatusCode(500, "获取我的报表失败");
        }
    }

    /// <summary>
    /// 更新报表模板
    /// </summary>
    [HttpPut("{reportId}/template")]
    [RequirePermission("Report.Update")]
    public async Task<ActionResult> UpdateReportTemplate(string reportId, [FromBody] ReportTemplate template)
    {
        try
        {
            var result = await _reportService.UpdateReportTemplateAsync(reportId, template);
            if (!result)
                return NotFound($"未找到报表 {reportId}");

            return Ok(new { message = "模板更新成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新报表模板失败: {ReportId}", reportId);
            return StatusCode(500, "更新报表模板失败");
        }
    }

    /// <summary>
    /// 获取报表模板
    /// </summary>
    [HttpGet("{reportId}/template")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<ReportTemplate>> GetReportTemplate(string reportId)
    {
        try
        {
            var template = await _reportService.GetReportTemplateAsync(reportId);
            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表模板失败: {ReportId}", reportId);
            return StatusCode(500, "获取报表模板失败");
        }
    }

    /// <summary>
    /// 获取模板库
    /// </summary>
    [HttpGet("templates")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<ReportTemplate>>> GetTemplateLibrary()
    {
        try
        {
            var templates = await _reportService.GetTemplateLibraryAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取模板库失败");
            return StatusCode(500, "获取模板库失败");
        }
    }

    /// <summary>
    /// 验证模板
    /// </summary>
    [HttpPost("templates/validate")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<ValidationResult>> ValidateTemplate([FromBody] ReportTemplate template)
    {
        try
        {
            var isValid = await _reportService.ValidateTemplateAsync(template);
            var errors = await _reportService.GetTemplateValidationErrorsAsync(template);

            return Ok(new ValidationResult
            {
                IsValid = isValid,
                Errors = errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证模板失败");
            return StatusCode(500, "验证模板失败");
        }
    }

    /// <summary>
    /// 更新数据源
    /// </summary>
    [HttpPut("{reportId}/datasource")]
    [RequirePermission("Report.Update")]
    public async Task<ActionResult> UpdateDataSource(string reportId, [FromBody] ReportDataSource dataSource)
    {
        try
        {
            var result = await _reportService.UpdateDataSourceAsync(reportId, dataSource);
            if (!result)
                return NotFound($"未找到报表 {reportId}");

            return Ok(new { message = "数据源更新成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新数据源失败: {ReportId}", reportId);
            return StatusCode(500, "更新数据源失败");
        }
    }

    /// <summary>
    /// 测试数据源连接
    /// </summary>
    [HttpPost("datasource/test")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<ConnectionTestResult>> TestDataSourceConnection([FromBody] ReportDataSource dataSource)
    {
        try
        {
            var success = await _reportService.TestDataSourceConnectionAsync(dataSource);
            return Ok(new ConnectionTestResult
            {
                Success = success,
                Message = success ? "连接成功" : "连接失败"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "测试数据源连接失败");
            return Ok(new ConnectionTestResult
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// 获取可用表列表
    /// </summary>
    [HttpPost("datasource/tables")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<string>>> GetAvailableTables([FromBody] ReportDataSource dataSource)
    {
        try
        {
            var tables = await _reportService.GetAvailableTablesAsync(dataSource);
            return Ok(tables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可用表失败");
            return StatusCode(500, "获取可用表失败");
        }
    }

    /// <summary>
    /// 获取表列信息
    /// </summary>
    [HttpPost("datasource/columns")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<ColumnInfo>>> GetTableColumns([FromBody] TableColumnsRequest request)
    {
        try
        {
            var columns = await _reportService.GetTableColumnsAsync(request.DataSource, request.TableName);
            return Ok(columns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取表列信息失败");
            return StatusCode(500, "获取表列信息失败");
        }
    }

    /// <summary>
    /// 预览数据
    /// </summary>
    [HttpGet("{reportId}/preview")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<DataPreviewResult>> PreviewData(string reportId, [FromQuery] int maxRows = 100)
    {
        try
        {
            var preview = await _reportService.PreviewDataAsync(reportId, maxRows);
            return Ok(preview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "预览数据失败: {ReportId}", reportId);
            return StatusCode(500, "预览数据失败");
        }
    }

    /// <summary>
    /// 执行报表
    /// </summary>
    [HttpPost("{reportId}/execute")]
    [RequirePermission("Report.Execute")]
    public async Task<ActionResult<ReportExecution>> ExecuteReport(
        string reportId,
        [FromBody] ExecuteReportRequest? request = null)
    {
        try
        {
            var userId = User.Identity?.Name ?? "unknown";
            var execution = await _reportService.ExecuteReportAsync(
                reportId,
                request?.Parameters,
                userId);

            return Accepted(new { executionId = execution.Id, status = execution.Status });
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行报表失败: {ReportId}", reportId);
            return StatusCode(500, "执行报表失败");
        }
    }

    /// <summary>
    /// 取消执行
    /// </summary>
    [HttpPost("executions/{executionId}/cancel")]
    [RequirePermission("Report.Execute")]
    public async Task<ActionResult> CancelExecution(string executionId)
    {
        try
        {
            var result = await _reportService.CancelExecutionAsync(executionId);
            if (!result)
                return NotFound($"未找到执行记录 {executionId} 或无法取消");

            return Ok(new { message = "取消成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消执行失败: {ExecutionId}", executionId);
            return StatusCode(500, "取消执行失败");
        }
    }

    /// <summary>
    /// 获取执行信息
    /// </summary>
    [HttpGet("executions/{executionId}")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<ReportExecution>> GetExecution(string executionId)
    {
        try
        {
            var execution = await _reportService.GetExecutionAsync(executionId);
            if (execution == null)
                return NotFound($"未找到执行记录 {executionId}");

            return Ok(execution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行信息失败: {ExecutionId}", executionId);
            return StatusCode(500, "获取执行信息失败");
        }
    }

    /// <summary>
    /// 获取报表执行历史
    /// </summary>
    [HttpGet("{reportId}/executions")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<ReportExecution>>> GetReportExecutions(
        string reportId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var executions = await _reportService.GetReportExecutionsAsync(reportId, skip, take);
            return Ok(executions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表执行历史失败: {ReportId}", reportId);
            return StatusCode(500, "获取报表执行历史失败");
        }
    }

    /// <summary>
    /// 获取正在运行的执行
    /// </summary>
    [HttpGet("executions/running")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<ReportExecution>>> GetRunningExecutions()
    {
        try
        {
            var executions = await _reportService.GetRunningExecutionsAsync();
            return Ok(executions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取正在运行的执行失败");
            return StatusCode(500, "获取正在运行的执行失败");
        }
    }

    /// <summary>
    /// 获取执行状态
    /// </summary>
    [HttpGet("executions/{executionId}/status")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<ExecutionStatusResponse>> GetExecutionStatus(string executionId)
    {
        try
        {
            var status = await _reportService.GetExecutionStatusAsync(executionId);
            return Ok(new ExecutionStatusResponse { Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行状态失败: {ExecutionId}", executionId);
            return StatusCode(500, "获取执行状态失败");
        }
    }

    /// <summary>
    /// 更新调度配置
    /// </summary>
    [HttpPut("{reportId}/schedule")]
    [RequirePermission("Report.Update")]
    public async Task<ActionResult> UpdateSchedule(string reportId, [FromBody] ReportSchedule schedule)
    {
        try
        {
            var result = await _reportService.UpdateScheduleAsync(reportId, schedule);
            if (!result)
                return NotFound($"未找到报表 {reportId}");

            return Ok(new { message = "调度配置更新成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新调度配置失败: {ReportId}", reportId);
            return StatusCode(500, "更新调度配置失败");
        }
    }

    /// <summary>
    /// 启用调度
    /// </summary>
    [HttpPost("{reportId}/schedule/enable")]
    [RequirePermission("Report.Update")]
    public async Task<ActionResult> EnableSchedule(string reportId)
    {
        try
        {
            var result = await _reportService.EnableScheduleAsync(reportId);
            if (!result)
                return NotFound($"未找到报表 {reportId}");

            return Ok(new { message = "调度已启用" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启用调度失败: {ReportId}", reportId);
            return StatusCode(500, "启用调度失败");
        }
    }

    /// <summary>
    /// 禁用调度
    /// </summary>
    [HttpPost("{reportId}/schedule/disable")]
    [RequirePermission("Report.Update")]
    public async Task<ActionResult> DisableSchedule(string reportId)
    {
        try
        {
            var result = await _reportService.DisableScheduleAsync(reportId);
            if (!result)
                return NotFound($"未找到报表 {reportId}");

            return Ok(new { message = "调度已禁用" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "禁用调度失败: {ReportId}", reportId);
            return StatusCode(500, "禁用调度失败");
        }
    }

    /// <summary>
    /// 获取计划执行列表
    /// </summary>
    [HttpGet("schedule/upcoming")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<ScheduleExecution>>> GetUpcomingExecutions(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var executions = await _reportService.GetUpcomingExecutionsAsync(startDate, endDate);
            return Ok(executions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取计划执行列表失败");
            return StatusCode(500, "获取计划执行列表失败");
        }
    }

    /// <summary>
    /// 触发调度执行
    /// </summary>
    [HttpPost("{reportId}/schedule/trigger")]
    [RequirePermission("Report.Execute")]
    public async Task<ActionResult> TriggerScheduledExecution(string reportId)
    {
        try
        {
            var result = await _reportService.TriggerScheduledExecutionAsync(reportId);
            if (!result)
                return NotFound($"未找到报表 {reportId}");

            return Ok(new { message = "调度执行已触发" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "触发调度执行失败: {ReportId}", reportId);
            return StatusCode(500, "触发调度执行失败");
        }
    }

    /// <summary>
    /// 获取报表指标
    /// </summary>
    [HttpGet("{reportId}/metrics")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<ReportMetrics>> GetReportMetrics(string reportId)
    {
        try
        {
            var metrics = await _reportService.GetReportMetricsAsync(reportId);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表指标失败: {ReportId}", reportId);
            return StatusCode(500, "获取报表指标失败");
        }
    }

    /// <summary>
    /// 获取使用统计
    /// </summary>
    [HttpGet("statistics/usage")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<ReportUsageStatistics>> GetUsageStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var statistics = await _reportService.GetUsageStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取使用统计失败");
            return StatusCode(500, "获取使用统计失败");
        }
    }

    /// <summary>
    /// 获取报表告警
    /// </summary>
    [HttpGet("alerts")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<ReportAlert>>> GetReportAlerts()
    {
        try
        {
            var alerts = await _reportService.GetReportAlertsAsync();
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表告警失败");
            return StatusCode(500, "获取报表告警失败");
        }
    }

    /// <summary>
    /// 获取报表历史记录
    /// </summary>
    [HttpGet("{reportId}/history")]
    [RequirePermission("Report.View")]
    public async Task<ActionResult<List<ReportHistory>>> GetReportHistory(
        string reportId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var history = await _reportService.GetReportHistoryAsync(reportId, skip, take);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表历史记录失败: {ReportId}", reportId);
            return StatusCode(500, "获取报表历史记录失败");
        }
    }

    /// <summary>
    /// 获取系统状态
    /// </summary>
    [HttpGet("system/status")]
    [RequirePermission("Report.Admin")]
    public async Task<ActionResult<ReportSystemStatus>> GetSystemStatus()
    {
        try
        {
            var status = await _reportService.GetSystemStatusAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统状态失败");
            return StatusCode(500, "获取系统状态失败");
        }
    }
}

// DTO 类
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ConnectionTestResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class TableColumnsRequest
{
    public ReportDataSource DataSource { get; set; } = new();
    public string TableName { get; set; } = string.Empty;
}

public class ExecuteReportRequest
{
    public Dictionary<string, object>? Parameters { get; set; }
}

public class ExecutionStatusResponse
{
    public ExecutionStatus Status { get; set; }
}
#endif
