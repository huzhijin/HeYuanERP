// 版权所有(c) HeYuanERP
// 说明：发票统计查询接口（应用层，中文注释）。

using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Queries;

/// <summary>
/// 发票统计查询接口：按指定参数聚合发票数据并返回汇总。
/// </summary>
public interface IInvoiceStatQuery
{
    /// <summary>
    /// 执行发票统计查询。
    /// </summary>
    Task<InvoiceStatSummaryDto> QueryAsync(InvoiceStatParamsDto args, CancellationToken ct = default);
}

