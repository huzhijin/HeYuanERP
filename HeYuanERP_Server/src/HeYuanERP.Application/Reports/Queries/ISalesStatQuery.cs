// 版权所有(c) HeYuanERP
// 说明：销售统计查询接口（应用层，中文注释）。

using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Queries;

/// <summary>
/// 销售统计查询接口：按指定参数聚合销售数据并返回汇总。
/// </summary>
public interface ISalesStatQuery
{
    /// <summary>
    /// 执行销售统计查询。
    /// </summary>
    Task<SalesStatSummaryDto> QueryAsync(SalesStatParamsDto args, CancellationToken ct = default);
}

