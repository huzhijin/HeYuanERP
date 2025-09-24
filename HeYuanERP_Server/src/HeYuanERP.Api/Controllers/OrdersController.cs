using System.IdentityModel.Tokens.Jwt;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Orders;
using HeYuanERP.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 订单（SalesOrders）接口：列表/详情/新增/编辑/删除/确认/反审；特价/项目申请（留桩）
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _svc;
    private readonly IOrderStateService _stateService;

    public OrdersController(IOrdersService svc, IOrderStateService stateService)
    {
        _svc = svc;
        _stateService = stateService;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // 列表
    [HttpGet]
    [RequirePermission("orders.read")]
    public async Task<IActionResult> QueryAsync([FromQuery] OrderListQueryDto req, CancellationToken ct)
    {
        var data = await _svc.QueryAsync(req, ct);
        return Ok(data);
    }

    // 详情
    [HttpGet("{id}")]
    [RequirePermission("orders.read")]
    public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken ct)
    {
        var data = await _svc.GetAsync(id, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    // 新建
    [HttpPost]
    [RequirePermission("orders.create")]
    public async Task<IActionResult> CreateAsync([FromBody] OrderCreateDto req, CancellationToken ct)
    {
        var data = await _svc.CreateAsync(req, CurrentUserId, ct);
        return Ok(data);
    }

    // 编辑
    [HttpPut("{id}")]
    [RequirePermission("orders.create")] // 暂复用 create 权限
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] OrderUpdateDto req, CancellationToken ct)
    {
        var data = await _svc.UpdateAsync(id, req, CurrentUserId, ct);
        return Ok(data);
    }

    // 删除（仅草稿）
    [HttpDelete("{id}")]
    [RequirePermission("orders.create")] // 暂复用 create 权限
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return Ok();
    }

    // 确认
    [HttpPost("{id}/confirm")]
    [RequirePermission("orders.confirm")]
    public async Task<IActionResult> ConfirmAsync([FromRoute] string id, CancellationToken ct)
    {
        await _svc.ConfirmAsync(id, CurrentUserId, ct);
        return Ok();
    }

    // 反审
    [HttpPost("{id}/reverse")]
    [RequirePermission("orders.reverse")]
    public async Task<IActionResult> ReverseAsync([FromRoute] string id, [FromBody] OrderReverseDto req, CancellationToken ct)
    {
        await _svc.ReverseAsync(id, CurrentUserId, req?.Reason, ct);
        return Ok();
    }

    // 特价/项目申请（留桩）：提交申请（不落库，返回占位申请号）
    [HttpPost("specials/apply")]
    [RequirePermission("orders.create")] // 复用下单权限
    public IActionResult ApplySpecial([FromBody] OrderSpecialApplyDto req)
    {
        var result = SpecialPriceStub.BuildSubmittedResult(req);
        return Ok(result);
    }

    // 特价/项目申请（留桩）：查询申请状态（固定 pending）
    [HttpGet("specials/{applyId}")]
    [RequirePermission("orders.read")] // 复用订单查看权限
    public IActionResult GetSpecialStatus([FromRoute] string applyId)
    {
        return Ok(new OrderSpecialApplyResultDto
        {
            ApplyId = applyId,
            Status = "pending",
            Message = "等待 OA/AI 审批流程（Mock）"
        });
    }

    // =========================== 新增：订单状态管理接口 ===========================

    /// <summary>
    /// 获取订单可流转的状态列表
    /// </summary>
    [HttpGet("{id}/transitions")]
    [RequirePermission("orders.read")]
    public async Task<IActionResult> GetAllowedTransitionsAsync([FromRoute] string id, CancellationToken ct)
    {
        var transitions = await _stateService.GetAllowedTransitionsAsync(id, CurrentUserId, ct);
        var result = transitions.Select(t => new
        {
            Status = t.ToString().ToLowerInvariant(),
            StatusName = t.GetDisplayName(),
            StyleClass = t.GetStyleClass()
        }).ToList();

        return Ok(result);
    }

    /// <summary>
    /// 执行订单状态流转
    /// </summary>
    [HttpPost("{id}/transition")]
    [RequirePermission("orders.read")] // 具体权限在服务层检查
    public async Task<IActionResult> TransitionStatusAsync([FromRoute] string id,
        [FromBody] OrderStatusTransitionDto req, CancellationToken ct)
    {
        if (!Enum.TryParse<OrderStatus>(req.TargetStatus, true, out var targetStatus))
        {
            return BadRequest("无效的目标状态");
        }

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        var result = await _stateService.TransitionAsync(id, targetStatus, req.Reason,
            CurrentUserId, userAgent, clientIp, ct);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new
        {
            Success = true,
            NewStatus = result.NewStatus?.ToString().ToLowerInvariant(),
            NewStatusName = result.NewStatus?.GetDisplayName(),
            LogId = result.LogId,
            Message = $"订单状态已更新为: {result.NewStatus?.GetDisplayName()}"
        });
    }

    /// <summary>
    /// 获取订单状态变更历史
    /// </summary>
    [HttpGet("{id}/status-history")]
    [RequirePermission("orders.read")]
    public async Task<IActionResult> GetStatusHistoryAsync([FromRoute] string id, CancellationToken ct)
    {
        var history = await _stateService.GetStatusHistoryAsync(id, ct);
        return Ok(history);
    }

    /// <summary>
    /// 快捷状态操作：提交订单
    /// </summary>
    [HttpPost("{id}/submit")]
    [RequirePermission("orders.submit")]
    public async Task<IActionResult> SubmitOrderAsync([FromRoute] string id, CancellationToken ct)
    {
        var result = await _stateService.TransitionAsync(id, OrderStatus.Submitted, "提交订单",
            CurrentUserId, Request.Headers["User-Agent"],
            HttpContext.Connection.RemoteIpAddress?.ToString(), ct);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { Success = true, Message = "订单已提交" });
    }

    /// <summary>
    /// 快捷状态操作：开始生产
    /// </summary>
    [HttpPost("{id}/start-production")]
    [RequirePermission("orders.production")]
    public async Task<IActionResult> StartProductionAsync([FromRoute] string id,
        [FromBody] OrderStatusReasonDto? req, CancellationToken ct)
    {
        var result = await _stateService.TransitionAsync(id, OrderStatus.InProduction,
            req?.Reason ?? "开始生产", CurrentUserId,
            Request.Headers["User-Agent"], HttpContext.Connection.RemoteIpAddress?.ToString(), ct);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { Success = true, Message = "订单已进入生产状态" });
    }

    /// <summary>
    /// 快捷状态操作：关闭订单
    /// </summary>
    [HttpPost("{id}/close")]
    [RequirePermission("orders.close")]
    public async Task<IActionResult> CloseOrderAsync([FromRoute] string id,
        [FromBody] OrderStatusReasonDto req, CancellationToken ct)
    {
        var result = await _stateService.TransitionAsync(id, OrderStatus.Closed,
            req.Reason, CurrentUserId,
            Request.Headers["User-Agent"], HttpContext.Connection.RemoteIpAddress?.ToString(), ct);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { Success = true, Message = "订单已关闭" });
    }

    /// <summary>
    /// 快捷状态操作：取消订单
    /// </summary>
    [HttpPost("{id}/cancel")]
    [RequirePermission("orders.cancel")]
    public async Task<IActionResult> CancelOrderAsync([FromRoute] string id,
        [FromBody] OrderStatusReasonDto req, CancellationToken ct)
    {
        var result = await _stateService.TransitionAsync(id, OrderStatus.Cancelled,
            req.Reason, CurrentUserId,
            Request.Headers["User-Agent"], HttpContext.Connection.RemoteIpAddress?.ToString(), ct);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { Success = true, Message = "订单已取消" });
    }
}
