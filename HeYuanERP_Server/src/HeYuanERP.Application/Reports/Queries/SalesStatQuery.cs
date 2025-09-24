// 版权所有(c) HeYuanERP
// 说明：销售统计查询实现（占位实现，后续由基础设施层使用 EF Core 完成数据访问）。

using System;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Queries;

/// <summary>
/// 销售统计查询实现。
/// 说明：本实现暂返回空结果，用于解耦应用层；数据访问将由基础设施层完善。
/// </summary>
public class SalesStatQuery : ISalesStatQuery
{
    public Task<SalesStatSummaryDto> QueryAsync(SalesStatParamsDto args, CancellationToken ct = default)
    {
        // 占位实现：返回空汇总。后续以 EF Core 聚合实现。
        var summary = new SalesStatSummaryDto
        {
            Items = Array.Empty<SalesStatItemDto>(),
            TotalQty = 0,
            Subtotal = 0,
            Tax = 0,
            TotalAmount = 0
        };
        return Task.FromResult(summary);
    }
}

