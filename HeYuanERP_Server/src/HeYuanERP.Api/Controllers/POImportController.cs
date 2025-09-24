using System.IdentityModel.Tokens.Jwt;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 采购导入接口：Excel 导入预检与回执（当前支持 CSV 占位）
/// </summary>
[ApiController]
[Route("api/po/import")]
[Authorize(Policy = "Permission")]
public class POImportController : ControllerBase
{
    private readonly IPOImportService _svc;

    public POImportController(IPOImportService svc)
    {
        _svc = svc;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // 表单模型：用于处理 multipart/form-data（Swagger 要求以复合类型承载 IFormFile）
    public class POImportForm
    {
        public string VendorId { get; set; } = string.Empty;
        public IFormFile File { get; set; } = default!;
    }

    /// <summary>
    /// 预检：校验供应商/产品/数量单价，返回预览与错误列表
    /// </summary>
    [HttpPost("precheck")]
    [RequirePermission("po.create")] // 复用创建权限
    [RequestSizeLimit(20_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PrecheckAsync([FromForm] POImportForm form, CancellationToken ct)
    {
        var result = await _svc.PrecheckAsync(form.VendorId, form.File, ct);
        return Ok(result);
    }

    /// <summary>
    /// 提交导入：预检通过后创建采购单（单文件一单）
    /// </summary>
    [HttpPost("commit")]
    [RequirePermission("po.create")] // 复用创建权限
    [RequestSizeLimit(20_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CommitAsync([FromForm] POImportForm form, CancellationToken ct)
    {
        var result = await _svc.ImportAsync(form.VendorId, form.File, CurrentUserId, ct);
        return Ok(result);
    }
}
