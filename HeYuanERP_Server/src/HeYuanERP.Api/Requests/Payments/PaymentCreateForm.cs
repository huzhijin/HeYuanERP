using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using HeYuanERP.Application.DTOs.Payments;

namespace HeYuanERP.Api.Requests.Payments;

/// <summary>
/// 收款创建表单模型（用于 API 层绑定 multipart/form-data）。
/// 说明：附件通过表单文件上传，核心字段映射到应用层 DTO 后由服务处理。
/// </summary>
public class PaymentCreateForm
{
    /// <summary>
    /// 收款方式。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 收款金额。
    /// </summary>
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 收款日期（建议格式：yyyy-MM-dd）。
    /// </summary>
    [DataType(DataType.Date)]
    public DateOnly PaymentDate { get; set; }

    /// <summary>
    /// 备注（可选）。
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 附件（可选，多文件）。
    /// </summary>
    public List<IFormFile> Attachments { get; set; } = new();

    /// <summary>
    /// 映射为应用层的创建 DTO（不含附件流处理）。
    /// </summary>
    public PaymentCreateDto ToDto()
        => new()
        {
            Method = Method,
            Amount = Amount,
            PaymentDate = PaymentDate,
            Remark = Remark
        };
}

