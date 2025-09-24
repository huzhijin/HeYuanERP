// 版权所有(c) HeYuanERP
// 说明：库存查询接口（应用层，中文注释）。

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Queries;

/// <summary>
/// 库存查询接口：汇总与交易明细。
/// </summary>
public interface IInventoryQuery
{
    /// <summary>
    /// 库存汇总查询（不分页）。
    /// </summary>
    Task<IReadOnlyList<InventorySummaryDto>> QuerySummaryAsync(InventoryQueryParamsDto args, CancellationToken ct = default);

    /// <summary>
    /// 库存交易明细（分页）。
    /// </summary>
    Task<PagedResult<InventoryTxnDto>> QueryTransactionsAsync(InventoryQueryParamsDto args, CancellationToken ct = default);
}

