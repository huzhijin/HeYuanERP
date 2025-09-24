// 版权所有(c) HeYuanERP
// 说明：库存查询实现（占位实现，后续由基础设施层使用 EF Core 完成数据访问）。

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Queries;

/// <summary>
/// 库存查询实现。
/// 说明：本实现暂返回空结果，用于解耦应用层；数据访问将由基础设施层完善。
/// </summary>
public class InventoryQuery : IInventoryQuery
{
    public Task<IReadOnlyList<InventorySummaryDto>> QuerySummaryAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
    {
        // 占位实现：返回空列表
        IReadOnlyList<InventorySummaryDto> list = Array.Empty<InventorySummaryDto>();
        return Task.FromResult(list);
    }

    public Task<PagedResult<InventoryTxnDto>> QueryTransactionsAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
    {
        // 占位实现：返回空分页
        var page = args?.Page > 0 ? args.Page : 1;
        var size = args?.Size > 0 ? args.Size : 20;
        var result = PagedResult<InventoryTxnDto>.Create(Array.Empty<InventoryTxnDto>(), page, size, total: 0);
        return Task.FromResult(result);
    }
}

