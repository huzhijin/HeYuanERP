using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Common.Models;
using HeYuanERP.Application.DTOs.Payments;

namespace HeYuanERP.Application.Interfaces;

/// <summary>
/// 收款应用服务接口。
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// 分页查询收款列表。
    /// </summary>
    Task<ApiResponse<PagedResult<PaymentListItemDto>>> ListAsync(PaymentListParams request, CancellationToken ct = default);

    /// <summary>
    /// 创建收款记录（含附件上传）。
    /// </summary>
    /// <param name="dto">创建 DTO</param>
    /// <param name="attachments">附件集合（流/文件名/类型/大小）</param>
    /// <param name="createdBy">创建人（可选）</param>
    /// <param name="orgId">组织标识（可选）</param>
    Task<ApiResponse<PaymentDetailDto>> CreateAsync(PaymentCreateDto dto, IEnumerable<AttachmentUpload>? attachments, string? createdBy, string? orgId, CancellationToken ct = default);

    /// <summary>
    /// 获取收款详情。
    /// </summary>
    Task<ApiResponse<PaymentDetailDto>> GetAsync(Guid id, CancellationToken ct = default);
}

/// <summary>
/// 收款列表查询参数（应用层）
/// </summary>
public class PaymentListParams : PagedRequest
{
    public string? Method { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public string? Keyword { get; set; }

    /// <summary>
    /// 规范化分页与筛选参数。
    /// </summary>
    public void NormalizeAll()
    {
        base.Normalize();
        if (MinAmount is { } && MinAmount < 0) MinAmount = 0;
        if (MaxAmount is { } && MaxAmount < 0) MaxAmount = 0;
        if (MinAmount.HasValue && MaxAmount.HasValue && MinAmount > MaxAmount)
            (MinAmount, MaxAmount) = (MaxAmount, MinAmount);
        if (DateFrom.HasValue && DateTo.HasValue && DateFrom > DateTo)
            (DateFrom, DateTo) = (DateTo, DateFrom);
    }
}

/// <summary>
/// 附件上传对象（应用层）。
/// </summary>
public record AttachmentUpload(Stream Content, string FileName, string ContentType, long Length);

