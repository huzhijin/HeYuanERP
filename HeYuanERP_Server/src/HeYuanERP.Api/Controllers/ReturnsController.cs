using System.IdentityModel.Tokens.Jwt;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Logistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.Requests.Logistics;
using HeYuanERP.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 退货单接口：创建与详情查询
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission")]
public class ReturnsController : ControllerBase
{
    private readonly IReturnsService _svc;
    private readonly AppDbContext _db;

    public ReturnsController(IReturnsService svc, AppDbContext db)
    {
        _svc = svc;
        _db = db;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    /// <summary>
    /// 创建退货单
    /// </summary>
    [HttpPost]
    [RequirePermission("returns.create")]
    public async Task<IActionResult> CreateAsync([FromBody] ReturnCreateDto req, CancellationToken ct)
    {
        var data = await _svc.CreateAsync(req, CurrentUserId, ct);
        return Ok(data);
    }

    /// <summary>
    /// 获取退货单详情
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("returns.read")]
    public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken ct)
    {
        var data = await _svc.GetAsync(id, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    /// <summary>
    /// 退货单列表（按单号/日期/状态筛选，分页）
    /// </summary>
    [HttpGet]
    [RequirePermission("returns.read")]
    public async Task<ActionResult<ApiResponse<Pagination<ReturnListItemDto>>>> ListAsync([FromQuery] ReturnListQuery query, CancellationToken ct)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var size = query.Size <= 0 ? 20 : query.Size;

        var q = _db.Returns.AsNoTracking().Include(r => r.Order).Include(r => r.SourceDelivery).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.ReturnNo))
            q = q.Where(r => r.ReturnNo.Contains(query.ReturnNo!));
        if (!string.IsNullOrWhiteSpace(query.Status))
            q = q.Where(r => r.Status == query.Status);
        if (query.From.HasValue)
            q = q.Where(r => r.ReturnDate >= query.From!.Value.Date);
        if (query.To.HasValue)
            q = q.Where(r => r.ReturnDate <= query.To!.Value.Date);
        if (!string.IsNullOrWhiteSpace(query.OrderNo))
            q = q.Where(r => r.Order != null && r.Order.OrderNo.Contains(query.OrderNo!));
        if (!string.IsNullOrWhiteSpace(query.SourceDeliveryNo))
            q = q.Where(r => r.SourceDelivery != null && r.SourceDelivery.DeliveryNo.Contains(query.SourceDeliveryNo!));

        var total = await q.LongCountAsync(ct);
        var items = await q.OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * size).Take(size)
            .Select(r => new ReturnListItemDto
            {
                Id = r.Id,
                ReturnNo = r.ReturnNo,
                OrderNo = r.Order != null ? r.Order.OrderNo : null,
                SourceDeliveryNo = r.SourceDelivery != null ? r.SourceDelivery.DeliveryNo : null,
                ReturnDate = r.ReturnDate,
                Status = r.Status
            }).ToListAsync(ct);

        var payload = new Pagination<ReturnListItemDto>
        {
            Items = items,
            Page = page,
            Size = size,
            Total = (int)total
        };
        return Ok(ApiResponse<Pagination<ReturnListItemDto>>.Ok(payload));
    }
}
