using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.Services.Attachments;

/// <summary>
/// 附件上传结果
/// </summary>
public class AttachmentUploadResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 附件信息
    /// </summary>
    public Attachment? Attachment { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 文件访问URL
    /// </summary>
    public string? AccessUrl { get; set; }

    /// <summary>
    /// 缩略图URL
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static AttachmentUploadResult Success(Attachment attachment, string? accessUrl = null, string? thumbnailUrl = null)
    {
        return new AttachmentUploadResult
        {
            IsSuccess = true,
            Attachment = attachment,
            AccessUrl = accessUrl,
            ThumbnailUrl = thumbnailUrl
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static AttachmentUploadResult Failure(string errorMessage)
    {
        return new AttachmentUploadResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// 附件下载结果
/// </summary>
public class AttachmentDownloadResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 文件流
    /// </summary>
    public Stream? FileStream { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 内容类型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 附件查询条件
/// </summary>
public class AttachmentQueryOptions
{
    public AttachmentBusinessType? BusinessType { get; set; }
    public string? BusinessEntityId { get; set; }
    public string? BusinessEntityField { get; set; }
    public AttachmentCategory? Category { get; set; }
    public AttachmentStatus? Status { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime? UploadedAfter { get; set; }
    public DateTime? UploadedBefore { get; set; }
    public List<string>? Tags { get; set; }
    public string? SearchKeyword { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
}

/// <summary>
/// 批量操作结果
/// </summary>
public class BatchOperationResult
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> SuccessIds { get; set; } = new();
    public List<string> FailureIds { get; set; } = new();
}

/// <summary>
/// 统一附件管理服务接口
/// </summary>
public interface IAttachmentService
{
    // =================== 上传和存储 ===================

    /// <summary>
    /// 上传附件
    /// </summary>
    Task<AttachmentUploadResult> UploadAsync(
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
        CancellationToken ct = default);

    /// <summary>
    /// 批量上传附件
    /// </summary>
    Task<List<AttachmentUploadResult>> BatchUploadAsync(
        IFormFileCollection files,
        AttachmentBusinessType businessType,
        string? businessEntityId = null,
        string? businessEntityField = null,
        AttachmentAccessLevel accessLevel = AttachmentAccessLevel.Private,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 通过URL上传附件
    /// </summary>
    Task<AttachmentUploadResult> UploadFromUrlAsync(
        string url,
        AttachmentBusinessType businessType,
        string? businessEntityId = null,
        string? businessEntityField = null,
        string? description = null,
        List<string>? tags = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 通过Base64上传附件
    /// </summary>
    Task<AttachmentUploadResult> UploadFromBase64Async(
        string base64Data,
        string fileName,
        AttachmentBusinessType businessType,
        string? businessEntityId = null,
        string? businessEntityField = null,
        string? description = null,
        List<string>? tags = null,
        string? userId = null,
        CancellationToken ct = default);

    // =================== 下载和访问 ===================

    /// <summary>
    /// 下载附件
    /// </summary>
    Task<AttachmentDownloadResult> DownloadAsync(string attachmentId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取附件访问URL（支持临时URL）
    /// </summary>
    Task<string?> GetAccessUrlAsync(string attachmentId, TimeSpan? expiry = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取附件预览URL
    /// </summary>
    Task<string?> GetPreviewUrlAsync(string attachmentId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取缩略图URL
    /// </summary>
    Task<string?> GetThumbnailUrlAsync(string attachmentId, string? userId = null, CancellationToken ct = default);

    // =================== 查询和管理 ===================

    /// <summary>
    /// 获取附件详情
    /// </summary>
    Task<Attachment?> GetAttachmentAsync(string attachmentId, CancellationToken ct = default);

    /// <summary>
    /// 查询附件列表
    /// </summary>
    Task<(List<Attachment> attachments, int total)> QueryAttachmentsAsync(AttachmentQueryOptions options, CancellationToken ct = default);

    /// <summary>
    /// 获取业务实体的附件列表
    /// </summary>
    Task<List<Attachment>> GetBusinessAttachmentsAsync(
        AttachmentBusinessType businessType,
        string businessEntityId,
        string? businessEntityField = null,
        CancellationToken ct = default);

    /// <summary>
    /// 更新附件信息
    /// </summary>
    Task<bool> UpdateAttachmentAsync(
        string attachmentId,
        string? fileName = null,
        string? description = null,
        List<string>? tags = null,
        AttachmentAccessLevel? accessLevel = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 关联附件到业务实体
    /// </summary>
    Task<bool> AssociateAttachmentAsync(
        string attachmentId,
        AttachmentBusinessType businessType,
        string businessEntityId,
        string? businessEntityField = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 取消关联附件
    /// </summary>
    Task<bool> DisassociateAttachmentAsync(string attachmentId, string? userId = null, CancellationToken ct = default);

    // =================== 删除和清理 ===================

    /// <summary>
    /// 软删除附件
    /// </summary>
    Task<bool> SoftDeleteAttachmentAsync(string attachmentId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 永久删除附件
    /// </summary>
    Task<bool> PermanentDeleteAttachmentAsync(string attachmentId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 批量删除附件
    /// </summary>
    Task<BatchOperationResult> BatchDeleteAttachmentsAsync(List<string> attachmentIds, bool permanent = false, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 清理临时文件
    /// </summary>
    Task<int> CleanupTemporaryFilesAsync(CancellationToken ct = default);

    /// <summary>
    /// 清理孤立文件（没有关联业务实体的文件）
    /// </summary>
    Task<int> CleanupOrphanedFilesAsync(TimeSpan olderThan, CancellationToken ct = default);

    // =================== 版本管理 ===================

    /// <summary>
    /// 创建附件新版本
    /// </summary>
    Task<AttachmentUploadResult> CreateVersionAsync(
        string attachmentId,
        IFormFile file,
        string? versionLabel = null,
        string? changeDescription = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 获取附件版本历史
    /// </summary>
    Task<List<AttachmentVersion>> GetVersionHistoryAsync(string attachmentId, CancellationToken ct = default);

    /// <summary>
    /// 还原到指定版本
    /// </summary>
    Task<bool> RestoreVersionAsync(string attachmentId, string versionId, string? userId = null, CancellationToken ct = default);

    // =================== 统计和分析 ===================

    /// <summary>
    /// 获取附件统计信息
    /// </summary>
    Task<object> GetAttachmentStatisticsAsync(
        AttachmentBusinessType? businessType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken ct = default);

    /// <summary>
    /// 获取存储空间使用情况
    /// </summary>
    Task<object> GetStorageUsageAsync(
        AttachmentBusinessType? businessType = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 获取热门文件列表
    /// </summary>
    Task<List<Attachment>> GetPopularFilesAsync(int limit = 10, TimeSpan? period = null, CancellationToken ct = default);

    // =================== 安全和权限 ===================

    /// <summary>
    /// 检查用户是否有访问附件的权限
    /// </summary>
    Task<bool> CheckAccessPermissionAsync(string attachmentId, string userId, AttachmentAccessType accessType, CancellationToken ct = default);

    /// <summary>
    /// 记录附件访问日志
    /// </summary>
    Task RecordAccessLogAsync(
        string attachmentId,
        AttachmentAccessType accessType,
        string? userId = null,
        string? accessIp = null,
        string? userAgent = null,
        bool isSuccessful = true,
        string? errorMessage = null,
        CancellationToken ct = default);

    /// <summary>
    /// 获取附件访问记录
    /// </summary>
    Task<List<AttachmentAccessRecord>> GetAccessRecordsAsync(string attachmentId, int limit = 50, CancellationToken ct = default);

    // =================== 文件处理 ===================

    /// <summary>
    /// 生成图片缩略图
    /// </summary>
    Task<bool> GenerateThumbnailAsync(string attachmentId, int width = 200, int height = 200, CancellationToken ct = default);

    /// <summary>
    /// 验证文件完整性
    /// </summary>
    Task<bool> ValidateFileIntegrityAsync(string attachmentId, CancellationToken ct = default);

    /// <summary>
    /// 转换文件格式
    /// </summary>
    Task<AttachmentUploadResult?> ConvertFileAsync(
        string attachmentId,
        string targetFormat,
        string? userId = null,
        CancellationToken ct = default);

    // =================== 分享和协作 ===================

    /// <summary>
    /// 生成分享链接
    /// </summary>
    Task<string> GenerateShareLinkAsync(
        string attachmentId,
        TimeSpan? expiry = null,
        string? password = null,
        int? downloadLimit = null,
        string? userId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 验证分享链接
    /// </summary>
    Task<bool> ValidateShareLinkAsync(string shareToken, string? password = null, CancellationToken ct = default);

    /// <summary>
    /// 通过分享链接下载文件
    /// </summary>
    Task<AttachmentDownloadResult> DownloadByShareLinkAsync(string shareToken, string? password = null, CancellationToken ct = default);
}