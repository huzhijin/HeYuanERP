using System.ComponentModel.DataAnnotations;
using HeYuanERP.Application.Common.Models;
using System;

namespace HeYuanERP.Api.Requests.Payments;

/// <summary>
/// 收款列表查询参数（包含分页与可选筛选）。
/// </summary>
public class PaymentListQuery : PagedRequest
{
    /// <summary>
    /// 收款方式筛选（可选）。
    /// </summary>
    [MaxLength(50)]
    public string? Method { get; set; }

    /// <summary>
    /// 金额下限（可选）。
    /// </summary>
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// 金额上限（可选）。
    /// </summary>
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// 起始日期（可选，含）。
    /// </summary>
    public DateOnly? DateFrom { get; set; }

    /// <summary>
    /// 截止日期（可选，含）。
    /// </summary>
    public DateOnly? DateTo { get; set; }

    /// <summary>
    /// 关键字（可选，按备注等匹配）。
    /// </summary>
    [MaxLength(100)]
    public string? Keyword { get; set; }

    /// <summary>
    /// 规范化并修正区间参数。
    /// </summary>
    public void NormalizeFilters()
    {
        base.Normalize();
        if (MinAmount is { } min && MinAmount < 0) MinAmount = 0;
        if (MaxAmount is { } max && MaxAmount < 0) MaxAmount = 0;
        if (MinAmount.HasValue && MaxAmount.HasValue && MinAmount > MaxAmount)
        {
            (MinAmount, MaxAmount) = (MaxAmount, MinAmount);
        }

        if (DateFrom.HasValue && DateTo.HasValue && DateFrom > DateTo)
        {
            (DateFrom, DateTo) = (DateTo, DateFrom);
        }
    }
}

