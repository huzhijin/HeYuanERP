using System;
using System.Collections.Generic;

namespace HeYuanERP.Application.DTOs.Payments;

/// <summary>
/// 收款创建 DTO（应用层使用）。
/// 说明：附件在 API 层以表单文件上传并由服务保存，此处仅承载收款核心字段。
/// </summary>
public class PaymentCreateDto
{
    /// <summary>
    /// 收款方式（必填，如：现金、转账等）。
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 收款金额（大于 0）。
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 收款日期（业务日期）。
    /// </summary>
    public DateOnly PaymentDate { get; set; }

    /// <summary>
    /// 备注（可选，最长 500 字）。
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 收款附件 DTO（对外返回时使用）。
/// </summary>
public class PaymentAttachmentDto
{
    /// <summary>
    /// 附件 Id。
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 文件名。
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME 类型。
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// 大小（字节）。
    /// </summary>
    public long Size { get; set; }
}

/// <summary>
/// 收款列表项 DTO（列表场景）。
/// </summary>
public class PaymentListItemDto
{
    public Guid Id { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }
    public string? Remark { get; set; }
    public int AttachmentCount { get; set; }
}

/// <summary>
/// 收款详情 DTO（包含附件）。
/// </summary>
public class PaymentDetailDto
{
    public Guid Id { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }
    public string? Remark { get; set; }
    public List<PaymentAttachmentDto> Attachments { get; set; } = new();
}

