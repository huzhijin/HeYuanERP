using HeYuanERP.Api.Services.Attachments;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 统一附件管理接口
/// </summary>
[ApiController]
[Route("api/attachments")]
[Authorize(Policy = "Permission")]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly ILogger<AttachmentsController> _logger;

    public AttachmentsController(IAttachmentService attachmentService, ILogger<AttachmentsController> logger)
    {
        _attachmentService = attachmentService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // =================== 上传接口 ===================

    /// <summary>
    /// 上传单个附件
    /// </summary>
    [HttpPost("upload")]
    [RequirePermission("attachments.upload")]
    public async Task<IActionResult> UploadAsync(
        [FromForm] IFormFile file,
        [FromForm] AttachmentBusinessType businessType,
        [FromForm] string? businessEntityId = null,
        [FromForm] string? businessEntityField = null,
        [FromForm] string? description = null,
        [FromForm] string? tags = null,
        [FromForm] AttachmentAccessLevel accessLevel = AttachmentAccessLevel.Private,
        [FromForm] bool isTemporary = false,
        CancellationToken ct = default)
    {
        try
        {
            var tagList = string.IsNullOrEmpty(tags) ? null : tags.Split(',').Select(t => t.Trim()).ToList();

            var result = await _attachmentService.UploadAsync(
                file, businessType, businessEntityId, businessEntityField,
                description, tagList, accessLevel, isTemporary,
                isTemporary ? DateTime.UtcNow.AddHours(24) : null, CurrentUserId, ct);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        Id = result.Attachment!.Id,
                        FileName = result.Attachment.FileName,
                        FileSize = result.Attachment.FileSize,
                        ContentType = result.Attachment.ContentType,
                        AccessUrl = result.AccessUrl,
                        ThumbnailUrl = result.ThumbnailUrl
                    }
                });
            }
            else
            {
                return BadRequest(new { Success = false, Message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading attachment");
            return StatusCode(500, new { Success = false, Message = "上传附件时发生错误" });
        }
    }

    /// <summary>
    /// 批量上传附件
    /// </summary>
    [HttpPost("batch-upload")]
    [RequirePermission("attachments.upload")]
    public async Task<IActionResult> BatchUploadAsync(
        [FromForm] IFormFileCollection files,
        [FromForm] AttachmentBusinessType businessType,
        [FromForm] string? businessEntityId = null,
        [FromForm] string? businessEntityField = null,
        [FromForm] AttachmentAccessLevel accessLevel = AttachmentAccessLevel.Private,
        CancellationToken ct = default)
    {
        try
        {
            var results = await _attachmentService.BatchUploadAsync(
                files, businessType, businessEntityId, businessEntityField, accessLevel, CurrentUserId, ct);

            var successResults = results.Where(r => r.IsSuccess).Select(r => new
            {
                Id = r.Attachment!.Id,
                FileName = r.Attachment.FileName,
                FileSize = r.Attachment.FileSize,
                AccessUrl = r.AccessUrl
            }).ToList();

            var failures = results.Where(r => !r.IsSuccess).Select(r => r.ErrorMessage).ToList();

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    SuccessCount = successResults.Count,
                    FailureCount = failures.Count,
                    SuccessResults = successResults,
                    Failures = failures
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch uploading attachments");
            return StatusCode(500, new { Success = false, Message = "批量上传附件时发生错误" });
        }
    }

    // =================== 下载接口 ===================

    /// <summary>
    /// 下载附件
    /// </summary>
    [HttpGet("{id}/download")]
    [RequirePermission("attachments.download")]
    public async Task<IActionResult> DownloadAsync([FromRoute] string id, CancellationToken ct)
    {
        try
        {
            // 记录访问日志
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();
            await _attachmentService.RecordAccessLogAsync(id, AttachmentAccessType.Download, CurrentUserId, clientIp, userAgent, ct: ct);

            var result = await _attachmentService.DownloadAsync(id, CurrentUserId, ct);

            if (result.IsSuccess && result.FileStream != null)
            {
                return File(result.FileStream, result.ContentType ?? "application/octet-stream", result.FileName);
            }
            else
            {
                return NotFound(new { Success = false, Message = result.ErrorMessage ?? "文件不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading attachment {AttachmentId}", id);
            return StatusCode(500, new { Success = false, Message = "下载文件时发生错误" });
        }
    }

    /// <summary>
    /// 获取附件访问URL
    /// </summary>
    [HttpGet("{id}/access-url")]
    [RequirePermission("attachments.read")]
    public async Task<IActionResult> GetAccessUrlAsync([FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var url = await _attachmentService.GetAccessUrlAsync(id, TimeSpan.FromHours(1), CurrentUserId, ct);

            if (url != null)
            {
                return Ok(new { Success = true, Data = new { AccessUrl = url } });
            }
            else
            {
                return NotFound(new { Success = false, Message = "附件不存在或无访问权限" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting access URL for attachment {AttachmentId}", id);
            return StatusCode(500, new { Success = false, Message = "获取访问URL时发生错误" });
        }
    }

    // =================== 查询接口 ===================

    /// <summary>
    /// 获取附件详情
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("attachments.read")]
    public async Task<IActionResult> GetAttachmentAsync([FromRoute] string id, CancellationToken ct)
    {
        try
        {
            var attachment = await _attachmentService.GetAttachmentAsync(id, ct);

            if (attachment != null)
            {
                // 记录访问日志
                await _attachmentService.RecordAccessLogAsync(id, AttachmentAccessType.View, CurrentUserId, ct: ct);

                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        Id = attachment.Id,
                        FileName = attachment.FileName,
                        FileExtension = attachment.FileExtension,
                        ContentType = attachment.ContentType,
                        FileSize = attachment.FileSize,
                        Category = attachment.Category.ToString(),
                        BusinessType = attachment.BusinessType.ToString(),
                        BusinessEntityId = attachment.BusinessEntityId,
                        BusinessEntityField = attachment.BusinessEntityField,
                        Description = attachment.Description,
                        Tags = attachment.Tags,
                        AccessLevel = attachment.AccessLevel.ToString(),
                        Status = attachment.Status.ToString(),
                        DownloadCount = attachment.DownloadCount,
                        LastDownloadAt = attachment.LastDownloadAt,
                        UploadedBy = attachment.UploadedBy,
                        UploadedAt = attachment.UploadedAt
                    }
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "附件不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attachment {AttachmentId}", id);
            return StatusCode(500, new { Success = false, Message = "获取附件详情时发生错误" });
        }
    }

    /// <summary>
    /// 查询附件列表
    /// </summary>
    [HttpGet]
    [RequirePermission("attachments.read")]
    public async Task<IActionResult> QueryAttachmentsAsync(
        [FromQuery] AttachmentBusinessType? businessType = null,
        [FromQuery] string? businessEntityId = null,
        [FromQuery] string? businessEntityField = null,
        [FromQuery] AttachmentCategory? category = null,
        [FromQuery] AttachmentStatus? status = null,
        [FromQuery] string? uploadedBy = null,
        [FromQuery] DateTime? uploadedAfter = null,
        [FromQuery] DateTime? uploadedBefore = null,
        [FromQuery] string? searchKeyword = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        try
        {
            var options = new AttachmentQueryOptions
            {
                BusinessType = businessType,
                BusinessEntityId = businessEntityId,
                BusinessEntityField = businessEntityField,
                Category = category,
                Status = status,
                UploadedBy = uploadedBy,
                UploadedAfter = uploadedAfter,
                UploadedBefore = uploadedBefore,
                SearchKeyword = searchKeyword,
                Page = page,
                Size = size
            };

            var (attachments, total) = await _attachmentService.QueryAttachmentsAsync(options, ct);

            var attachmentList = attachments.Select(a => new
            {
                Id = a.Id,
                FileName = a.FileName,
                FileExtension = a.FileExtension,
                ContentType = a.ContentType,
                FileSize = a.FileSize,
                Category = a.Category.ToString(),
                BusinessType = a.BusinessType.ToString(),
                BusinessEntityId = a.BusinessEntityId,
                Description = a.Description,
                Tags = a.Tags,
                DownloadCount = a.DownloadCount,
                UploadedBy = a.UploadedBy,
                UploadedAt = a.UploadedAt
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Items = attachmentList,
                    Total = total,
                    Page = page,
                    Size = size,
                    TotalPages = (int)Math.Ceiling((double)total / size)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying attachments");
            return StatusCode(500, new { Success = false, Message = "查询附件列表时发生错误" });
        }
    }

    /// <summary>
    /// 获取业务实体的附件列表
    /// </summary>
    [HttpGet("business/{businessType}/{businessEntityId}")]
    [RequirePermission("attachments.read")]
    public async Task<IActionResult> GetBusinessAttachmentsAsync(
        [FromRoute] AttachmentBusinessType businessType,
        [FromRoute] string businessEntityId,
        [FromQuery] string? businessEntityField = null,
        CancellationToken ct = default)
    {
        try
        {
            var attachments = await _attachmentService.GetBusinessAttachmentsAsync(
                businessType, businessEntityId, businessEntityField, ct);

            var attachmentList = attachments.Select(a => new
            {
                Id = a.Id,
                FileName = a.FileName,
                FileExtension = a.FileExtension,
                FileSize = a.FileSize,
                Category = a.Category.ToString(),
                Description = a.Description,
                Tags = a.Tags,
                UploadedAt = a.UploadedAt
            }).ToList();

            return Ok(new { Success = true, Data = attachmentList });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting business attachments for {BusinessType}/{BusinessEntityId}",
                businessType, businessEntityId);
            return StatusCode(500, new { Success = false, Message = "获取业务附件时发生错误" });
        }
    }

    // =================== 管理接口 ===================

    /// <summary>
    /// 更新附件信息
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission("attachments.edit")]
    public async Task<IActionResult> UpdateAttachmentAsync(
        [FromRoute] string id,
        [FromBody] UpdateAttachmentRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _attachmentService.UpdateAttachmentAsync(
                id, request.FileName, request.Description, request.Tags, request.AccessLevel, CurrentUserId, ct);

            if (result)
            {
                return Ok(new { Success = true, Message = "附件更新成功" });
            }
            else
            {
                return NotFound(new { Success = false, Message = "附件不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating attachment {AttachmentId}", id);
            return StatusCode(500, new { Success = false, Message = "更新附件时发生错误" });
        }
    }

    /// <summary>
    /// 关联附件到业务实体
    /// </summary>
    [HttpPost("{id}/associate")]
    [RequirePermission("attachments.edit")]
    public async Task<IActionResult> AssociateAttachmentAsync(
        [FromRoute] string id,
        [FromBody] AssociateAttachmentRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _attachmentService.AssociateAttachmentAsync(
                id, request.BusinessType, request.BusinessEntityId, request.BusinessEntityField, CurrentUserId, ct);

            if (result)
            {
                return Ok(new { Success = true, Message = "附件关联成功" });
            }
            else
            {
                return NotFound(new { Success = false, Message = "附件不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error associating attachment {AttachmentId}", id);
            return StatusCode(500, new { Success = false, Message = "关联附件时发生错误" });
        }
    }

    /// <summary>
    /// 删除附件
    /// </summary>
    [HttpDelete("{id}")]
    [RequirePermission("attachments.delete")]
    public async Task<IActionResult> DeleteAttachmentAsync(
        [FromRoute] string id,
        [FromQuery] bool permanent = false,
        CancellationToken ct = default)
    {
        try
        {
            bool result;
            if (permanent)
            {
                result = await _attachmentService.PermanentDeleteAttachmentAsync(id, CurrentUserId, ct);
            }
            else
            {
                result = await _attachmentService.SoftDeleteAttachmentAsync(id, CurrentUserId, ct);
            }

            if (result)
            {
                return Ok(new { Success = true, Message = permanent ? "附件永久删除成功" : "附件删除成功" });
            }
            else
            {
                return NotFound(new { Success = false, Message = "附件不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attachment {AttachmentId}", id);
            return StatusCode(500, new { Success = false, Message = "删除附件时发生错误" });
        }
    }

    // =================== 系统管理 ===================

    /// <summary>
    /// 清理临时文件
    /// </summary>
    [HttpPost("cleanup-temporary")]
    [RequirePermission("attachments.admin")]
    public async Task<IActionResult> CleanupTemporaryFilesAsync(CancellationToken ct)
    {
        try
        {
            var deletedCount = await _attachmentService.CleanupTemporaryFilesAsync(ct);
            return Ok(new { Success = true, Message = $"清理了 {deletedCount} 个临时文件" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temporary files");
            return StatusCode(500, new { Success = false, Message = "清理临时文件时发生错误" });
        }
    }
}

/// <summary>
/// 更新附件请求
/// </summary>
public class UpdateAttachmentRequest
{
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public AttachmentAccessLevel? AccessLevel { get; set; }
}

/// <summary>
/// 关联附件请求
/// </summary>
public class AssociateAttachmentRequest
{
    public AttachmentBusinessType BusinessType { get; set; }
    public string BusinessEntityId { get; set; } = string.Empty;
    public string? BusinessEntityField { get; set; }
}