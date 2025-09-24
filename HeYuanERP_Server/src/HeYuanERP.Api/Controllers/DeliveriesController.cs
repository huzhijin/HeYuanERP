using System.IdentityModel.Tokens.Jwt;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.Requests.Logistics;
using HeYuanERP.Api.Data;
using Microsoft.EntityFrameworkCore;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Logistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 送货单接口：创建与详情查询（打印在下一批实现）
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveriesService _svc;
    private readonly AppDbContext _db;

    public DeliveriesController(IDeliveriesService svc, AppDbContext db)
    {
        _svc = svc;
        _db = db;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    /// <summary>
    /// 创建送货单
    /// </summary>
    [HttpPost]
    [RequirePermission("deliveries.create")]
    public async Task<IActionResult> CreateAsync([FromBody] DeliveryCreateDto req, CancellationToken ct)
    {
        var data = await _svc.CreateAsync(req, CurrentUserId, ct);
        return Ok(data);
    }

    /// <summary>
    /// 获取送货单详情
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("deliveries.read")]
    public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken ct)
    {
        var data = await _svc.GetAsync(id, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    /// <summary>
    /// 送货单列表（按单号/日期/状态筛选，分页）
    /// </summary>
    [HttpGet]
    [RequirePermission("deliveries.read")]
    public async Task<ActionResult<ApiResponse<Pagination<DeliveryListItemDto>>>> ListAsync([FromQuery] DeliveryListQuery query, CancellationToken ct)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var size = query.Size <= 0 ? 20 : query.Size;

        var q = _db.Deliveries.AsNoTracking().Include(d => d.Order).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.DeliveryNo))
            q = q.Where(d => d.DeliveryNo.Contains(query.DeliveryNo!));
        if (!string.IsNullOrWhiteSpace(query.Status))
            q = q.Where(d => d.Status == query.Status);
        if (query.From.HasValue)
            q = q.Where(d => d.DeliveryDate >= query.From!.Value.Date);
        if (query.To.HasValue)
            q = q.Where(d => d.DeliveryDate <= query.To!.Value.Date);
        if (!string.IsNullOrWhiteSpace(query.OrderNo))
            q = q.Where(d => d.Order != null && d.Order.OrderNo.Contains(query.OrderNo!));

        var total = await q.LongCountAsync(ct);
        var items = await q.OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * size).Take(size)
            .Select(d => new DeliveryListItemDto
            {
                Id = d.Id,
                DeliveryNo = d.DeliveryNo,
                OrderNo = d.Order != null ? d.Order.OrderNo : null,
                DeliveryDate = d.DeliveryDate,
                Status = d.Status
            }).ToListAsync(ct);

        var payload = new Pagination<DeliveryListItemDto>
        {
            Items = items,
            Page = page,
            Size = size,
            Total = (int)total
        };
        return Ok(ApiResponse<Pagination<DeliveryListItemDto>>.Ok(payload));
    }
}
