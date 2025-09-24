using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 收款记录实体（方式/金额/日期/备注/附件）。
/// 说明：仅包含核心业务字段与基础审计字段，数据库具体映射在 Infrastructure 层配置。
/// </summary>
public class Payment
{
    /// <summary>
    /// 主键。
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 收款方式，如：现金、银行转账、支付宝、微信等。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 收款金额（保留两位小数）。
    /// </summary>
    [Range(typeof(decimal), "0.00", "79228162514264337593543950335")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 收款日期（业务日期，默认为当前 UTC 日期）。
    /// </summary>
    public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// 备注（可选）。
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 附件集合。
    /// </summary>
    public List<PaymentAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// 创建时间（UTC）。
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人（用户名/员工号等，可选）。
    /// </summary>
    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 租户/公司标识（可选）。
    /// </summary>
    [MaxLength(64)]
    public string? OrgId { get; set; }

    /// <summary>
    /// 并发控制时间戳（RowVersion）。
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}

