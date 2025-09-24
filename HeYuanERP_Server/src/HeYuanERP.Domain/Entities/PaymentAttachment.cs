using System;
using System.ComponentModel.DataAnnotations;

namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 收款附件实体，记录上传文件的元数据及存储键信息。
/// </summary>
public class PaymentAttachment
{
    /// <summary>
    /// 主键。
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 外键：所属收款记录 Id。
    /// </summary>
    public Guid PaymentId { get; set; }

    /// <summary>
    /// 原始文件名。
    /// </summary>
    [Required]
    [MaxLength(260)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME 类型（Content-Type）。
    /// </summary>
    [MaxLength(100)]
    public string? ContentType { get; set; }

    /// <summary>
    /// 文件大小（字节）。
    /// </summary>
    [Range(0, long.MaxValue)]
    public long Size { get; set; }

    /// <summary>
    /// 存储键或相对路径（不暴露物理绝对路径）。
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string StorageKey { get; set; } = string.Empty;

    /// <summary>
    /// 上传时间（UTC）。
    /// </summary>
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 导航属性：所属收款记录。
    /// </summary>
    public Payment? Payment { get; set; }
}

