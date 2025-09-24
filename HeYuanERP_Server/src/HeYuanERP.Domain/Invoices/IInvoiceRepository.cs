using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Domain.Invoices;

/// <summary>
/// 发票仓储接口（领域层依赖抽象，基础设施层实现）。
/// 说明：用于支撑从订单/交货生成发票、查询、分页等操作。
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>
    /// 根据主键获取发票（包含明细）。
    /// </summary>
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据系统内发票号获取发票（包含明细）。
    /// </summary>
    Task<Invoice?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增发票。
    /// </summary>
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新发票。
    /// </summary>
    void Update(Invoice invoice);

    /// <summary>
    /// 保存变更。
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件分页查询发票（不包含明细，列表性能优先）。
    /// </summary>
    Task<(IReadOnlyList<Invoice> Items, int TotalCount)> QueryAsync(
        InvoiceStatus? status,
        Guid? customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

