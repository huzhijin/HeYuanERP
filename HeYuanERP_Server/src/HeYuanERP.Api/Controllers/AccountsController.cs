using System.IdentityModel.Tokens.Jwt;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Accounts;
using HeYuanERP.Api.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 客户（Accounts）接口：列表/详情/新建/编辑/共享/转移/拜访/附件列表
/// 注意：
/// - 统一响应由 ResponseWrapperFilter 处理，无需手动包装；
/// - 权限通过 Authorize(Policy="Permission") + RequirePermission 声明；
/// - 上传附件将在下一批提供（此处仅提供附件列表）。
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Permission")]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService _svc;

    public AccountsController(IAccountsService svc)
    {
        _svc = svc;
    }

    // 获取当前用户 Id（JWT 的 sub）
    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    /// <summary>
    /// 分页查询客户列表
    /// </summary>
    [HttpGet]
    [RequirePermission("accounts.read")]
    public async Task<IActionResult> QueryAsync([FromQuery] AccountListQueryDto req, CancellationToken ct)
    {
        var data = await _svc.QueryAsync(req, ct);
        return Ok(data);
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("accounts.read")]
    public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken ct)
    {
        var data = await _svc.GetAsync(id, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    /// <summary>
    /// 客户编码是否已存在（用于唯一性预校验）
    /// </summary>
    [HttpGet("exists-code")]
    [RequirePermission("accounts.read")]
    public async Task<IActionResult> ExistsCodeAsync([FromQuery] string code, [FromQuery] string? excludeId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code)) return BadRequest("code 不能为空");
        var exists = await _svc.ExistsCodeAsync(code, excludeId, ct);
        return Ok(new { exists });
    }

    /// <summary>
    /// 新建客户
    /// </summary>
    [HttpPost]
    [RequirePermission("accounts.create")]
    public async Task<IActionResult> CreateAsync([FromBody] AccountCreateDto req, CancellationToken ct)
    {
        var data = await _svc.CreateAsync(req, CurrentUserId, ct);
        return Ok(data);
    }

    /// <summary>
    /// 编辑客户
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission("accounts.create")] // 复用 create 权限；后续可细分为 accounts.update
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] AccountUpdateDto req, CancellationToken ct)
    {
        var data = await _svc.UpdateAsync(id, req, CurrentUserId, ct);
        return Ok(data);
    }

    /// <summary>
    /// 共享客户给指定用户（读/写权限，可设置过期时间）
    /// </summary>
    [HttpPost("{id}/share")]
    [RequirePermission("accounts.share")] // 默认仅 Admin 拥有（种子未分配给 Sales）
    public async Task<IActionResult> ShareAsync([FromRoute] string id, [FromBody] AccountShareRequestDto req, CancellationToken ct)
    {
        await _svc.ShareAsync(id, req, CurrentUserId, ct);
        return Ok();
    }

    /// <summary>
    /// 取消共享
    /// </summary>
    [HttpDelete("{id}/share/{targetUserId}")]
    [RequirePermission("accounts.share")]
    public async Task<IActionResult> UnshareAsync([FromRoute] string id, [FromRoute] string targetUserId, CancellationToken ct)
    {
        await _svc.UnshareAsync(id, targetUserId, ct);
        return Ok();
    }

    /// <summary>
    /// 转移客户负责人
    /// </summary>
    [HttpPost("{id}/transfer")]
    [RequirePermission("accounts.transfer")] // 默认仅 Admin 拥有
    public async Task<IActionResult> TransferAsync([FromRoute] string id, [FromBody] AccountTransferRequestDto req, CancellationToken ct)
    {
        await _svc.TransferAsync(id, req, CurrentUserId, ct);
        return Ok();
    }

    /// <summary>
    /// 列出客户的拜访记录（分页）
    /// </summary>
    [HttpGet("{id}/visits")]
    [RequirePermission("accounts.read")]
    public async Task<IActionResult> ListVisitsAsync([FromRoute] string id, [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var data = await _svc.ListVisitsAsync(id, page, size, ct);
        return Ok(data);
    }

    /// <summary>
    /// 新增拜访记录
    /// </summary>
    [HttpPost("{id}/visits")]
    [RequirePermission("accounts.create")] // 允许销售创建拜访
    public async Task<IActionResult> CreateVisitAsync([FromRoute] string id, [FromBody] AccountVisitCreateDto req, CancellationToken ct)
    {
        var data = await _svc.CreateVisitAsync(id, req, CurrentUserId, ct);
        return Ok(data);
    }

    /// <summary>
    /// 列出客户附件（仅明细，不含上传）
    /// </summary>
    [HttpGet("{id}/attachments")]
    [RequirePermission("accounts.read")]
    public async Task<IActionResult> ListAttachmentsAsync([FromRoute] string id, CancellationToken ct)
    {
        var data = await _svc.ListAttachmentsAsync(id, ct);
        return Ok(data);
    }
}

