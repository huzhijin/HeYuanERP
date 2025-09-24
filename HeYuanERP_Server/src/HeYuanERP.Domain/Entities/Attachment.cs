namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 附件管理 - 统一的文件附件管理实体
/// </summary>
public class Attachment
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 文件名（原始文件名）
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件存储名（实际存储的文件名）
    /// </summary>
    public string StorageFileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// 文件MIME类型
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件MD5哈希值（用于去重和完整性校验）
    /// </summary>
    public string FileHash { get; set; } = string.Empty;

    /// <summary>
    /// 存储路径
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// 存储类型
    /// </summary>
    public AttachmentStorageType StorageType { get; set; } = AttachmentStorageType.Local;

    /// <summary>
    /// 业务类型（关联的业务模块）
    /// </summary>
    public AttachmentBusinessType BusinessType { get; set; }

    /// <summary>
    /// 关联业务实体ID
    /// </summary>
    public string? BusinessEntityId { get; set; }

    /// <summary>
    /// 关联业务实体字段（可选，用于区分同一实体的不同附件字段）
    /// </summary>
    public string? BusinessEntityField { get; set; }

    /// <summary>
    /// 文件分类
    /// </summary>
    public AttachmentCategory Category { get; set; } = AttachmentCategory.Document;

    /// <summary>
    /// 文件标签（JSON格式，支持多标签）
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// 文件描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否为临时文件
    /// </summary>
    public bool IsTemporary { get; set; } = false;

    /// <summary>
    /// 临时文件过期时间
    /// </summary>
    public DateTime? TemporaryExpireAt { get; set; }

    /// <summary>
    /// 是否公开访问
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// 访问权限控制
    /// </summary>
    public AttachmentAccessLevel AccessLevel { get; set; } = AttachmentAccessLevel.Private;

    /// <summary>
    /// 文件状态
    /// </summary>
    public AttachmentStatus Status { get; set; } = AttachmentStatus.Uploading;

    /// <summary>
    /// 缩略图路径（如果是图片）
    /// </summary>
    public string? ThumbnailPath { get; set; }

    /// <summary>
    /// 预览URL（用于在线预览）
    /// </summary>
    public string? PreviewUrl { get; set; }

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// 最后下载时间
    /// </summary>
    public DateTime? LastDownloadAt { get; set; }

    /// <summary>
    /// 扩展属性（JSON格式）
    /// </summary>
    public Dictionary<string, object> ExtendedProperties { get; set; } = new();

    /// <summary>
    /// 上传者
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// 上传时间
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 软删除标记
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// 导航属性：附件访问记录
    /// </summary>
    public List<AttachmentAccessRecord> AccessRecords { get; set; } = new();

    /// <summary>
    /// 导航属性：附件版本历史
    /// </summary>
    public List<AttachmentVersion> Versions { get; set; } = new();
}

/// <summary>
/// 附件访问记录
/// </summary>
public class AttachmentAccessRecord
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 附件ID
    /// </summary>
    public string AttachmentId { get; set; } = string.Empty;

    /// <summary>
    /// 访问类型
    /// </summary>
    public AttachmentAccessType AccessType { get; set; }

    /// <summary>
    /// 访问者
    /// </summary>
    public string? AccessedBy { get; set; }

    /// <summary>
    /// 访问IP
    /// </summary>
    public string? AccessIp { get; set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 访问时间
    /// </summary>
    public DateTime AccessedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 访问结果
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// 错误信息（如果访问失败）
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 扩展信息
    /// </summary>
    public Dictionary<string, object> ExtendedInfo { get; set; } = new();

    /// <summary>
    /// 导航属性：关联附件
    /// </summary>
    public Attachment? Attachment { get; set; }
}

/// <summary>
/// 附件版本历史
/// </summary>
public class AttachmentVersion
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 附件ID
    /// </summary>
    public string AttachmentId { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public int VersionNumber { get; set; }

    /// <summary>
    /// 版本标签
    /// </summary>
    public string? VersionLabel { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 存储路径
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件哈希
    /// </summary>
    public string FileHash { get; set; } = string.Empty;

    /// <summary>
    /// 变更说明
    /// </summary>
    public string? ChangeDescription { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 导航属性：关联附件
    /// </summary>
    public Attachment? Attachment { get; set; }
}

/// <summary>
/// 附件存储类型
/// </summary>
public enum AttachmentStorageType
{
    /// <summary>
    /// 本地存储
    /// </summary>
    Local = 1,

    /// <summary>
    /// 阿里云OSS
    /// </summary>
    AliyunOSS = 2,

    /// <summary>
    /// 腾讯云COS
    /// </summary>
    TencentCOS = 3,

    /// <summary>
    /// AWS S3
    /// </summary>
    AWSS3 = 4,

    /// <summary>
    /// 七牛云
    /// </summary>
    Qiniu = 5,

    /// <summary>
    /// 又拍云
    /// </summary>
    Upyun = 6
}

/// <summary>
/// 附件业务类型
/// </summary>
public enum AttachmentBusinessType
{
    /// <summary>
    /// 销售订单
    /// </summary>
    SalesOrder = 1,

    /// <summary>
    /// 采购订单
    /// </summary>
    PurchaseOrder = 2,

    /// <summary>
    /// 发货单
    /// </summary>
    Delivery = 3,

    /// <summary>
    /// 收货单
    /// </summary>
    Receipt = 4,

    /// <summary>
    /// 发票
    /// </summary>
    Invoice = 5,

    /// <summary>
    /// 收款单
    /// </summary>
    Payment = 6,

    /// <summary>
    /// 产品信息
    /// </summary>
    Product = 7,

    /// <summary>
    /// 客户信息
    /// </summary>
    Customer = 8,

    /// <summary>
    /// 供应商信息
    /// </summary>
    Supplier = 9,

    /// <summary>
    /// 库存盘点
    /// </summary>
    InventoryCheck = 10,

    /// <summary>
    /// 质量检验
    /// </summary>
    QualityInspection = 11,

    /// <summary>
    /// 采购异常
    /// </summary>
    PurchaseException = 12,

    /// <summary>
    /// 对账差异
    /// </summary>
    ReconciliationDifference = 13,

    /// <summary>
    /// 库存预警
    /// </summary>
    InventoryAlert = 14,

    /// <summary>
    /// 系统配置
    /// </summary>
    SystemConfig = 15,

    /// <summary>
    /// 用户资料
    /// </summary>
    UserProfile = 16,

    /// <summary>
    /// 其他
    /// </summary>
    Other = 99
}

/// <summary>
/// 附件分类
/// </summary>
public enum AttachmentCategory
{
    /// <summary>
    /// 文档
    /// </summary>
    Document = 1,

    /// <summary>
    /// 图片
    /// </summary>
    Image = 2,

    /// <summary>
    /// 音频
    /// </summary>
    Audio = 3,

    /// <summary>
    /// 视频
    /// </summary>
    Video = 4,

    /// <summary>
    /// 压缩包
    /// </summary>
    Archive = 5,

    /// <summary>
    /// 表格
    /// </summary>
    Spreadsheet = 6,

    /// <summary>
    /// 演示文稿
    /// </summary>
    Presentation = 7,

    /// <summary>
    /// PDF
    /// </summary>
    PDF = 8,

    /// <summary>
    /// 其他
    /// </summary>
    Other = 99
}

/// <summary>
/// 附件访问级别
/// </summary>
public enum AttachmentAccessLevel
{
    /// <summary>
    /// 私有（仅上传者可访问）
    /// </summary>
    Private = 1,

    /// <summary>
    /// 内部（公司内部可访问）
    /// </summary>
    Internal = 2,

    /// <summary>
    /// 受保护（需要权限验证）
    /// </summary>
    Protected = 3,

    /// <summary>
    /// 公开（任何人可访问）
    /// </summary>
    Public = 4
}

/// <summary>
/// 附件状态
/// </summary>
public enum AttachmentStatus
{
    /// <summary>
    /// 上传中
    /// </summary>
    Uploading = 1,

    /// <summary>
    /// 上传完成
    /// </summary>
    Uploaded = 2,

    /// <summary>
    /// 处理中（如图片压缩、格式转换等）
    /// </summary>
    Processing = 3,

    /// <summary>
    /// 可用
    /// </summary>
    Available = 4,

    /// <summary>
    /// 已归档
    /// </summary>
    Archived = 5,

    /// <summary>
    /// 损坏
    /// </summary>
    Corrupted = 6,

    /// <summary>
    /// 已删除
    /// </summary>
    Deleted = 7
}

/// <summary>
/// 附件访问类型
/// </summary>
public enum AttachmentAccessType
{
    /// <summary>
    /// 下载
    /// </summary>
    Download = 1,

    /// <summary>
    /// 预览
    /// </summary>
    Preview = 2,

    /// <summary>
    /// 查看信息
    /// </summary>
    View = 3,

    /// <summary>
    /// 分享
    /// </summary>
    Share = 4,

    /// <summary>
    /// 编辑
    /// </summary>
    Edit = 5,

    /// <summary>
    /// 删除
    /// </summary>
    Delete = 6
}

