using System.IdentityModel.Tokens.Jwt;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 仓库（Warehouses）接口：列表/详情/新增/编辑/删除
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehousesService _svc;

    public WarehousesController(IWarehousesService svc)
    {
        _svc = svc;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // 列表
    [HttpGet]
    [RequirePermission("inventory.read")] // B5 将细分为 warehouses.read
    public async Task<IActionResult> QueryAsync([FromQuery] WarehouseListQueryDto req, CancellationToken ct)
    {
        var data = await _svc.QueryAsync(req, ct);
        return Ok(data);
    }

    // 详情
    [HttpGet("{id}")]
    [RequirePermission("inventory.read")] // B5 将细分为 warehouses.read
    public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken ct)
    {
        var data = await _svc.GetAsync(id, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    // 新建
    [HttpPost]
    [RequirePermission("inventory.read")] // B5 将细分并改为 warehouses.create
    public async Task<IActionResult> CreateAsync([FromBody] WarehouseCreateDto req, CancellationToken ct)
    {
        var data = await _svc.CreateAsync(req, CurrentUserId, ct);
        return Ok(data);
    }

    // 编辑
    [HttpPut("{id}")]
    [RequirePermission("inventory.read")] // B5 将细分并改为 warehouses.create
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] WarehouseUpdateDto req, CancellationToken ct)
    {
        var data = await _svc.UpdateAsync(id, req, CurrentUserId, ct);
        return Ok(data);
    }

    // 删除
    [HttpDelete("{id}")]
    [RequirePermission("inventory.read")] // B5 将细分并改为 warehouses.create
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return Ok();
    }
}

