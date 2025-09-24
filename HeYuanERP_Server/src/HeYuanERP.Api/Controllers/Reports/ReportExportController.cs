// 版权所有(c) HeYuanERP
// 说明：报表导出与任务查询 API 控制器（中文注释）。

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HeYuanERP.Application.Reports;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers.Reports;

/// <summary>
/// 报表导出 API（异步）。
/// </summary>
[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportExportController : ControllerBase
{
    private readonly ReportExportService _service;
    private readonly IValidator<ReportExportRequestDto> _validator;

    public ReportExportController(ReportExportService service, IValidator<ReportExportRequestDto> validator)
    {
        _service = service;
        _validator = validator;
    }

    /// <summary>
    /// 新建导出任务（异步）。
    /// </summary>
    [HttpPost("{name}/export")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportAsync([FromRoute] string name, [FromBody] ReportExportRequestDto body, CancellationToken ct)
    {
        var result = _validator.Validate(body);
        if (!result.IsValid)
        {
            return BadRequest(new
            {
                code = "ERR_VALIDATION",
                message = string.Join("; ", result.Errors.Select(e => e.ErrorMessage)),
                traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        var createdBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var taskDto = await _service.EnqueueAsync(name, body, createdBy, clientIp, userAgent, ct);
        return Accepted(new
        {
            code = "OK",
            message = "accepted",
            data = taskDto,
            traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    /// <summary>
    /// 查询导出任务状态。
    /// </summary>
    [HttpGet("tasks/{taskId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskAsync([FromRoute] string taskId, CancellationToken ct)
    {
        if (!Guid.TryParse(taskId, out var id))
            return NotFound(new { code = "NOT_FOUND", message = "任务不存在", traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        var task = await _service.GetTaskAsync(id, ct);
        if (task == null)
            return NotFound(new { code = "NOT_FOUND", message = "任务不存在", traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        return Ok(new
        {
            code = "OK",
            message = "success",
            data = task,
            traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}

