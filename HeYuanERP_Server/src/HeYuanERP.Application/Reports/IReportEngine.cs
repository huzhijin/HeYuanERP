// 版权所有(c) HeYuanERP
// 说明：报表引擎接口定义。负责汇总报表查询与导出内容构建（中文注释）。

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;
using HeYuanERP.Application.Reports.Exporters;

namespace HeYuanERP.Application.Reports;

/// <summary>
/// 报表引擎接口：统一对外提供报表查询与导出内容构建能力。
/// </summary>
public interface IReportEngine
{
    /// <summary>销售统计（聚合）。</summary>
    Task<SalesStatSummaryDto> SalesStatAsync(SalesStatParamsDto args, CancellationToken ct = default);

    /// <summary>发票统计（聚合）。</summary>
    Task<InvoiceStatSummaryDto> InvoiceStatAsync(InvoiceStatParamsDto args, CancellationToken ct = default);

    /// <summary>采购订单查询（分页列表）。</summary>
    Task<PagedResult<POListItemDto>> POQueryAsync(POQueryParamsDto args, CancellationToken ct = default);

    /// <summary>库存汇总查询。</summary>
    Task<IReadOnlyList<InventorySummaryDto>> InventorySummaryAsync(InventoryQueryParamsDto args, CancellationToken ct = default);

    /// <summary>库存交易明细（分页）。</summary>
    Task<PagedResult<InventoryTxnDto>> InventoryTransactionsAsync(InventoryQueryParamsDto args, CancellationToken ct = default);

    /// <summary>
    /// 构建导出负载（用于 PDF/CSV 导出）。
    /// </summary>
    Task<ExportPayload> BuildExportPayloadAsync(ReportType type, IDictionary<string, object?> parameters, ReportExportFormat format, CancellationToken ct = default);
}

