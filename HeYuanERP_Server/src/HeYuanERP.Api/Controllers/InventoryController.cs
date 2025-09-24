using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 库存接口：库存汇总与库存事务查询
/// </summary>
[ApiController]
[Route("api/inventory")]
[Authorize(Policy = "Permission")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _svc;

    public InventoryController(IInventoryService svc)
    {
        _svc = svc;
    }

    // 库存汇总：
    // GET /api/inventory/summary
    [HttpGet("summary")]
    [RequirePermission("inventory.read")]
    public async Task<IActionResult> SummaryAsync([FromQuery] InventorySummaryQueryDto req, CancellationToken ct)
    {
        var data = await _svc.QuerySummaryAsync(req, ct);
        return Ok(data);
    }

    // 库存事务：
    // GET /api/inventory/transactions
    [HttpGet("transactions")]
    [RequirePermission("inventory.read")]
    public async Task<IActionResult> TransactionsAsync([FromQuery] InventoryTxnQueryDto req, CancellationToken ct)
    {
        var data = await _svc.QueryTransactionsAsync(req, ct);
        return Ok(data);
    }
}

