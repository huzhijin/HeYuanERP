using System;
using HeYuanERP.Domain.Invoices;

namespace HeYuanERP.Application.Invoices.DTOs;

/// <summary>
/// 发票查询请求（分页）。
/// </summary>
public class QueryInvoiceRequest
{
    /// <summary>
    /// 发票状态（可选）。
    /// </summary>
    public InvoiceStatus? Status { get; set; }

    /// <summary>
    /// 客户 Id（可选）。
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// 页码（从 1 开始）。
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页条数（默认 20）。
    /// </summary>
    public int PageSize { get; set; } = 20;
}

