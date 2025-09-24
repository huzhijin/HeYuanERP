// 版权所有(c) HeYuanERP
// 说明：采购订单查询接口（应用层，中文注释）。

using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Queries;

/// <summary>
/// 采购订单查询接口：按条件查询并返回分页列表。
/// </summary>
public interface IPOQuery
{
    /// <summary>
    /// 执行采购订单查询（分页）。
    /// </summary>
    Task<PagedResult<POListItemDto>> QueryAsync(POQueryParamsDto args, CancellationToken ct = default);
}

