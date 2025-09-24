using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HeYuanERP.Api.Services.Attachments;

/// <summary>
/// 统一附件管理服务实现（MVP版本）
/// </summary>
public class AttachmentService : IAttachmentService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AttachmentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _uploadPath;

    public AttachmentService(AppDbContext db, ILogger<AttachmentService> logger, IConfiguration configuration)
    {
        _db = db;
        _logger = logger;
        _configuration = configuration;
        _uploadPath = configuration.GetValue<string>("FileStorage:UploadPath") ?? "uploads";

        // 确保上传目录存在
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<AttachmentUploadResult> UploadAsync(
        IFormFile file,
        AttachmentBusinessType businessType,
        string? businessEntityId = null,
        string? businessEntityField = null,
        string? description = null,
        List<string>? tags = null,
        AttachmentAccessLevel accessLevel = AttachmentAccessLevel.Private,
        bool isTemporary = false,
        DateTime? temporaryExpireAt = null,
        string? userId = null,
        CancellationToken ct = default)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return AttachmentUploadResult.Failure("文件为空");
            }

            // 生成文件信息
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storageFileName = $"{Guid.NewGuid():N}{fileExtension}";
            var relativePath = Path.Combine(DateTime.UtcNow.ToString("yyyy/MM/dd"), storageFileName);
            var fullPath = Path.Combine(_uploadPath, relativePath);

            // 确保目录存在
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 计算文件哈希
            string fileHash;
            using (var stream = file.OpenReadStream())
            {
                using (var md5 = MD5.Create())
                {
                    var hashBytes = await md5.ComputeHashAsync(stream, ct);
                    fileHash = Convert.ToHexString(hashBytes).ToLowerInvariant();
                }
            }

            // 检查是否已存在相同文件
            var existingAttachment = await _db.Attachments.AsNoTracking()
                .FirstOrDefaultAsync(a => a.FileHash == fileHash && !a.IsDeleted, ct);

            if (existingAttachment != null)
            {
                _logger.LogInformation("File with hash {FileHash} already exists, attachment ID: {AttachmentId}",
                    fileHash, existingAttachment.Id);

                // 可以选择返回现有文件或创建新的引用
                return AttachmentUploadResult.Success(existingAttachment, GetAccessUrl(existingAttachment.Id));
            }

            // 保存文件
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream, ct);
            }

            // 创建附件记录
            var attachment = new Attachment
            {
                FileName = file.FileName,
                StorageFileName = storageFileName,
                FileExtension = fileExtension,
                ContentType = file.ContentType,
                FileSize = file.Length,
                FileHash = fileHash,
                StoragePath = relativePath,
                StorageType = AttachmentStorageType.Local,
                BusinessType = businessType,
                BusinessEntityId = businessEntityId,
                BusinessEntityField = businessEntityField,
                Category = DetermineCategory(fileExtension),
                Tags = tags ?? new List<string>(),
                Description = description,
                IsTemporary = isTemporary,
                TemporaryExpireAt = temporaryExpireAt,
                AccessLevel = accessLevel,
                Status = AttachmentStatus.Available,
                UploadedBy = userId
            };

            _db.Attachments.Add(attachment);

            // 记录访问日志
            await RecordAccessLogAsync(attachment.Id, AttachmentAccessType.View, userId, ct: ct);

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("File uploaded successfully: {FileName} -> {AttachmentId}", file.FileName, attachment.Id);

            return AttachmentUploadResult.Success(attachment, GetAccessUrl(attachment.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
            return AttachmentUploadResult.Failure($"上传文件时发生错误: {ex.Message}");
        }
    }

    public async Task<List<AttachmentUploadResult>> BatchUploadAsync(
        IFormFileCollection files,
        AttachmentBusinessType businessType,
        string? businessEntityId = null,
        string? businessEntityField = null,
        AttachmentAccessLevel accessLevel = AttachmentAccessLevel.Private,
        string? userId = null,
        CancellationToken ct = default)
    {
        var results = new List<AttachmentUploadResult>();

        foreach (var file in files)
        {
            var result = await UploadAsync(file, businessType, businessEntityId, businessEntityField,
                accessLevel: accessLevel, userId: userId, ct: ct);
            results.Add(result);
        }

        return results;
    }

    public async Task<AttachmentDownloadResult> DownloadAsync(string attachmentId, string? userId = null, CancellationToken ct = default)
    {
        try
        {
            var attachment = await _db.Attachments.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted, ct);

            if (attachment == null)
            {
                return new AttachmentDownloadResult
                {
                    IsSuccess = false,
                    ErrorMessage = "附件不存在"
                };
            }

            // 检查权限
            if (!await CheckAccessPermissionAsync(attachmentId, userId ?? string.Empty, AttachmentAccessType.Download, ct))
            {
                return new AttachmentDownloadResult
                {
                    IsSuccess = false,
                    ErrorMessage = "没有下载权限"
                };
            }

            var fullPath = Path.Combine(_uploadPath, attachment.StoragePath);
            if (!File.Exists(fullPath))
            {
                return new AttachmentDownloadResult
                {
                    IsSuccess = false,
                    ErrorMessage = "文件不存在"
                };
            }

            // 更新下载统计
            attachment.DownloadCount++;
            attachment.LastDownloadAt = DateTime.UtcNow;
            _db.Attachments.Update(attachment);

            // 记录访问日志
            await RecordAccessLogAsync(attachmentId, AttachmentAccessType.Download, userId, ct: ct);

            await _db.SaveChangesAsync(ct);

            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            return new AttachmentDownloadResult
            {
                IsSuccess = true,
                FileStream = fileStream,
                FileName = attachment.FileName,
                ContentType = attachment.ContentType,
                FileSize = attachment.FileSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading attachment {AttachmentId}", attachmentId);
            return new AttachmentDownloadResult
            {
                IsSuccess = false,
                ErrorMessage = $"下载文件时发生错误: {ex.Message}"
            };
        }
    }

    public async Task<string?> GetAccessUrlAsync(string attachmentId, TimeSpan? expiry = null, string? userId = null, CancellationToken ct = default)
    {
        var attachment = await _db.Attachments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted, ct);

        if (attachment == null)
            return null;

        // 检查权限
        if (!await CheckAccessPermissionAsync(attachmentId, userId ?? string.Empty, AttachmentAccessType.View, ct))
            return null;

        // 简单实现：返回下载API地址
        return GetAccessUrl(attachmentId);
    }

    public async Task<Attachment?> GetAttachmentAsync(string attachmentId, CancellationToken ct = default)
    {
        return await _db.Attachments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted, ct);
    }

    public async Task<(List<Attachment> attachments, int total)> QueryAttachmentsAsync(AttachmentQueryOptions options, CancellationToken ct = default)
    {
        var query = _db.Attachments.AsNoTracking().Where(a => !a.IsDeleted);

        if (options.BusinessType.HasValue)
            query = query.Where(a => a.BusinessType == options.BusinessType.Value);

        if (!string.IsNullOrEmpty(options.BusinessEntityId))
            query = query.Where(a => a.BusinessEntityId == options.BusinessEntityId);

        if (!string.IsNullOrEmpty(options.BusinessEntityField))
            query = query.Where(a => a.BusinessEntityField == options.BusinessEntityField);

        if (options.Category.HasValue)
            query = query.Where(a => a.Category == options.Category.Value);

        if (options.Status.HasValue)
            query = query.Where(a => a.Status == options.Status.Value);

        if (!string.IsNullOrEmpty(options.UploadedBy))
            query = query.Where(a => a.UploadedBy == options.UploadedBy);

        if (options.UploadedAfter.HasValue)
            query = query.Where(a => a.UploadedAt >= options.UploadedAfter.Value);

        if (options.UploadedBefore.HasValue)
            query = query.Where(a => a.UploadedAt <= options.UploadedBefore.Value);

        if (!string.IsNullOrEmpty(options.SearchKeyword))
        {
            query = query.Where(a => a.FileName.Contains(options.SearchKeyword) ||
                                   (a.Description != null && a.Description.Contains(options.SearchKeyword)));
        }

        var total = await query.CountAsync(ct);
        var attachments = await query
            .OrderByDescending(a => a.UploadedAt)
            .Skip((options.Page - 1) * options.Size)
            .Take(options.Size)
            .ToListAsync(ct);

        return (attachments, total);
    }

    public async Task<List<Attachment>> GetBusinessAttachmentsAsync(
        AttachmentBusinessType businessType,
        string businessEntityId,
        string? businessEntityField = null,
        CancellationToken ct = default)
    {
        var query = _db.Attachments.AsNoTracking()
            .Where(a => a.BusinessType == businessType &&
                       a.BusinessEntityId == businessEntityId &&
                       !a.IsDeleted);

        if (!string.IsNullOrEmpty(businessEntityField))
            query = query.Where(a => a.BusinessEntityField == businessEntityField);

        return await query.OrderByDescending(a => a.UploadedAt).ToListAsync(ct);
    }

    public async Task<bool> SoftDeleteAttachmentAsync(string attachmentId, string? userId = null, CancellationToken ct = default)
    {
        try
        {
            var attachment = await _db.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, ct);
            if (attachment == null) return false;

            attachment.IsDeleted = true;
            attachment.DeletedAt = DateTime.UtcNow;
            attachment.DeletedBy = userId;
            attachment.Status = AttachmentStatus.Deleted;

            await RecordAccessLogAsync(attachmentId, AttachmentAccessType.Delete, userId, ct: ct);

            await _db.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting attachment {AttachmentId}", attachmentId);
            return false;
        }
    }

    public async Task<int> CleanupTemporaryFilesAsync(CancellationToken ct = default)
    {
        try
        {
            var expiredFiles = await _db.Attachments
                .Where(a => a.IsTemporary &&
                           a.TemporaryExpireAt.HasValue &&
                           a.TemporaryExpireAt.Value < DateTime.UtcNow &&
                           !a.IsDeleted)
                .ToListAsync(ct);

            var deletedCount = 0;
            foreach (var file in expiredFiles)
            {
                if (await PermanentDeleteAttachmentAsync(file.Id, "SYSTEM", ct))
                {
                    deletedCount++;
                }
            }

            _logger.LogInformation("Cleaned up {Count} expired temporary files", deletedCount);
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temporary files");
            return 0;
        }
    }

    public async Task<bool> CheckAccessPermissionAsync(string attachmentId, string userId, AttachmentAccessType accessType, CancellationToken ct = default)
    {
        var attachment = await _db.Attachments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachmentId, ct);

        if (attachment == null) return false;

        // 简单权限检查逻辑
        switch (attachment.AccessLevel)
        {
            case AttachmentAccessLevel.Public:
                return true;
            case AttachmentAccessLevel.Private:
                return attachment.UploadedBy == userId;
            case AttachmentAccessLevel.Internal:
            case AttachmentAccessLevel.Protected:
                return !string.IsNullOrEmpty(userId); // 需要登录用户
            default:
                return false;
        }
    }

    public async Task RecordAccessLogAsync(
        string attachmentId,
        AttachmentAccessType accessType,
        string? userId = null,
        string? accessIp = null,
        string? userAgent = null,
        bool isSuccessful = true,
        string? errorMessage = null,
        CancellationToken ct = default)
    {
        try
        {
            var record = new AttachmentAccessRecord
            {
                AttachmentId = attachmentId,
                AccessType = accessType,
                AccessedBy = userId,
                AccessIp = accessIp,
                UserAgent = userAgent,
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };

            _db.AttachmentAccessRecords.Add(record);
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording access log for attachment {AttachmentId}", attachmentId);
        }
    }

    // 简化实现的其他方法
    public Task<AttachmentUploadResult> UploadFromUrlAsync(string url, AttachmentBusinessType businessType, string? businessEntityId = null, string? businessEntityField = null, string? description = null, List<string>? tags = null, string? userId = null, CancellationToken ct = default)
        => Task.FromResult(AttachmentUploadResult.Failure("暂不支持URL上传"));

    public Task<AttachmentUploadResult> UploadFromBase64Async(string base64Data, string fileName, AttachmentBusinessType businessType, string? businessEntityId = null, string? businessEntityField = null, string? description = null, List<string>? tags = null, string? userId = null, CancellationToken ct = default)
        => Task.FromResult(AttachmentUploadResult.Failure("暂不支持Base64上传"));

    public Task<string?> GetPreviewUrlAsync(string attachmentId, string? userId = null, CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public Task<string?> GetThumbnailUrlAsync(string attachmentId, string? userId = null, CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public async Task<bool> UpdateAttachmentAsync(string attachmentId, string? fileName = null, string? description = null, List<string>? tags = null, AttachmentAccessLevel? accessLevel = null, string? userId = null, CancellationToken ct = default)
    {
        var attachment = await _db.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, ct);
        if (attachment == null) return false;

        if (!string.IsNullOrEmpty(fileName)) attachment.FileName = fileName;
        if (!string.IsNullOrEmpty(description)) attachment.Description = description;
        if (tags != null) attachment.Tags = tags;
        if (accessLevel.HasValue) attachment.AccessLevel = accessLevel.Value;

        attachment.UpdatedAt = DateTime.UtcNow;
        attachment.UpdatedBy = userId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AssociateAttachmentAsync(string attachmentId, AttachmentBusinessType businessType, string businessEntityId, string? businessEntityField = null, string? userId = null, CancellationToken ct = default)
    {
        var attachment = await _db.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, ct);
        if (attachment == null) return false;

        attachment.BusinessType = businessType;
        attachment.BusinessEntityId = businessEntityId;
        attachment.BusinessEntityField = businessEntityField;
        attachment.UpdatedAt = DateTime.UtcNow;
        attachment.UpdatedBy = userId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DisassociateAttachmentAsync(string attachmentId, string? userId = null, CancellationToken ct = default)
    {
        var attachment = await _db.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, ct);
        if (attachment == null) return false;

        attachment.BusinessEntityId = null;
        attachment.BusinessEntityField = null;
        attachment.UpdatedAt = DateTime.UtcNow;
        attachment.UpdatedBy = userId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PermanentDeleteAttachmentAsync(string attachmentId, string? userId = null, CancellationToken ct = default)
    {
        try
        {
            var attachment = await _db.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, ct);
            if (attachment == null) return false;

            // 删除物理文件
            var fullPath = Path.Combine(_uploadPath, attachment.StoragePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 删除数据库记录
            _db.Attachments.Remove(attachment);
            await _db.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error permanently deleting attachment {AttachmentId}", attachmentId);
            return false;
        }
    }

    // 其他简化实现的方法...
    public Task<BatchOperationResult> BatchDeleteAttachmentsAsync(List<string> attachmentIds, bool permanent = false, string? userId = null, CancellationToken ct = default) => Task.FromResult(new BatchOperationResult());
    public Task<int> CleanupOrphanedFilesAsync(TimeSpan olderThan, CancellationToken ct = default) => Task.FromResult(0);
    public Task<AttachmentUploadResult> CreateVersionAsync(string attachmentId, IFormFile file, string? versionLabel = null, string? changeDescription = null, string? userId = null, CancellationToken ct = default) => Task.FromResult(AttachmentUploadResult.Failure("暂不支持版本管理"));
    public Task<List<AttachmentVersion>> GetVersionHistoryAsync(string attachmentId, CancellationToken ct = default) => Task.FromResult(new List<AttachmentVersion>());
    public Task<bool> RestoreVersionAsync(string attachmentId, string versionId, string? userId = null, CancellationToken ct = default) => Task.FromResult(false);
    public Task<object> GetAttachmentStatisticsAsync(AttachmentBusinessType? businessType = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) => Task.FromResult<object>(new { });
    public Task<object> GetStorageUsageAsync(AttachmentBusinessType? businessType = null, string? userId = null, CancellationToken ct = default) => Task.FromResult<object>(new { });
    public Task<List<Attachment>> GetPopularFilesAsync(int limit = 10, TimeSpan? period = null, CancellationToken ct = default) => Task.FromResult(new List<Attachment>());
    public Task<List<AttachmentAccessRecord>> GetAccessRecordsAsync(string attachmentId, int limit = 50, CancellationToken ct = default) => Task.FromResult(new List<AttachmentAccessRecord>());
    public Task<bool> GenerateThumbnailAsync(string attachmentId, int width = 200, int height = 200, CancellationToken ct = default) => Task.FromResult(false);
    public Task<bool> ValidateFileIntegrityAsync(string attachmentId, CancellationToken ct = default) => Task.FromResult(true);
    public Task<AttachmentUploadResult?> ConvertFileAsync(string attachmentId, string targetFormat, string? userId = null, CancellationToken ct = default) => Task.FromResult<AttachmentUploadResult?>(null);
    public Task<string> GenerateShareLinkAsync(string attachmentId, TimeSpan? expiry = null, string? password = null, int? downloadLimit = null, string? userId = null, CancellationToken ct = default) => Task.FromResult(string.Empty);
    public Task<bool> ValidateShareLinkAsync(string shareToken, string? password = null, CancellationToken ct = default) => Task.FromResult(false);
    public Task<AttachmentDownloadResult> DownloadByShareLinkAsync(string shareToken, string? password = null, CancellationToken ct = default) => Task.FromResult(new AttachmentDownloadResult { IsSuccess = false });

    #region Private Methods

    private static AttachmentCategory DetermineCategory(string fileExtension)
    {
        return fileExtension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => AttachmentCategory.Image,
            ".pdf" => AttachmentCategory.PDF,
            ".doc" or ".docx" or ".txt" or ".rtf" => AttachmentCategory.Document,
            ".xls" or ".xlsx" or ".csv" => AttachmentCategory.Spreadsheet,
            ".ppt" or ".pptx" => AttachmentCategory.Presentation,
            ".mp3" or ".wav" or ".flac" or ".aac" => AttachmentCategory.Audio,
            ".mp4" or ".avi" or ".mkv" or ".mov" => AttachmentCategory.Video,
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => AttachmentCategory.Archive,
            _ => AttachmentCategory.Other
        };
    }

    private string GetAccessUrl(string attachmentId)
    {
        return $"/api/attachments/{attachmentId}/download";
    }

    #endregion
}
