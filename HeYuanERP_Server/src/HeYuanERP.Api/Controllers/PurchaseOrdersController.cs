using System.IdentityModel.Tokens.Jwt;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 采购（PurchaseOrders）接口：列表/详情/新增/编辑/删除/确认/收货
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrdersService _svc;
    private readonly IPurchaseExceptionService _exceptionSvc;

    public PurchaseOrdersController(IPurchaseOrdersService svc, IPurchaseExceptionService exceptionSvc)
    {
        _svc = svc;
        _exceptionSvc = exceptionSvc;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // 列表
    [HttpGet]
    [RequirePermission("po.read")]
    public async Task<IActionResult> QueryAsync([FromQuery] POListQueryDto req, CancellationToken ct)
    {
        var data = await _svc.QueryAsync(req, ct);
        return Ok(data);
    }

    // 详情
    [HttpGet("{id}")]
    [RequirePermission("po.read")]
    public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken ct)
    {
        var data = await _svc.GetAsync(id, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    // 新建
    [HttpPost]
    [RequirePermission("po.create")]
    public async Task<IActionResult> CreateAsync([FromBody] POCreateDto req, CancellationToken ct)
    {
        var data = await _svc.CreateAsync(req, CurrentUserId, ct);
        return Ok(data);
    }

    // 编辑
    [HttpPut("{id}")]
    [RequirePermission("po.create")] // 暂复用 create 权限
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] POUpdateDto req, CancellationToken ct)
    {
        var data = await _svc.UpdateAsync(id, req, CurrentUserId, ct);
        return Ok(data);
    }

    // 删除（仅草稿）
    [HttpDelete("{id}")]
    [RequirePermission("po.create")] // 暂复用 create 权限
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return Ok();
    }

    // 确认
    [HttpPost("{id}/confirm")]
    [RequirePermission("po.create")] // 暂复用 create 权限
    public async Task<IActionResult> ConfirmAsync([FromRoute] string id, CancellationToken ct)
    {
        await _svc.ConfirmAsync(id, CurrentUserId, ct);
        return Ok();
    }

    // 收货入库：产生收货单 + 库存事务
    [HttpPost("{id}/receive")]
    [RequirePermission("po.create")] // 暂复用 create 权限
    public async Task<IActionResult> ReceiveAsync([FromRoute] string id, [FromBody] POReceiveCreateDto req, CancellationToken ct)
    {
        var data = await _svc.ReceiveAsync(id, req, CurrentUserId, ct);
        // 异步触发：收货完成后自动检测采购异常（不阻断主流程）
        if (data != null && !string.IsNullOrWhiteSpace(data.Id))
        {
            _ = Task.Run(async () =>
            {
                try { await _exceptionSvc.AutoDetectExceptionsAsync(data.Id!, ct); } catch { }
            }, ct);
        }
        return Ok(data);
    }
}
