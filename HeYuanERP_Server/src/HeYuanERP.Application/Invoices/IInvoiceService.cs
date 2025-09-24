using System;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Invoices.DTOs;

namespace HeYuanERP.Application.Invoices;

/// <summary>
/// 发票应用服务接口。
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// 创建发票（从订单/交货生成）。
    /// </summary>
    Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 Id 获取发票。
    /// </summary>
    Task<InvoiceResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据系统内发票号获取发票。
    /// </summary>
    Task<InvoiceResponse?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 开具发票（状态置为 Issued）。
    /// </summary>
    Task<InvoiceResponse> IssueAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 作废发票。
    /// </summary>
    Task<InvoiceResponse> CancelAsync(Guid id, string? reason = null, CancellationToken cancellationToken = default);
}

