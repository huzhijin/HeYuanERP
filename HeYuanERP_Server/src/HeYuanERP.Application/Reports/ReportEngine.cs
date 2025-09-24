// 版权所有(c) HeYuanERP
// 说明：报表引擎实现。负责参数白名单过滤（委托）、调用查询服务、以及为导出生成结构化数据（中文注释）。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Reports.Exporters;
using HeYuanERP.Application.Reports.Queries;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;

namespace HeYuanERP.Application.Reports;

/// <summary>
/// 报表引擎实现。
/// </summary>
public class ReportEngine : IReportEngine
{
    private readonly ISalesStatQuery _salesStat;
    private readonly IInvoiceStatQuery _invoiceStat;
    private readonly IPOQuery _poQuery;
    private readonly IInventoryQuery _inventoryQuery;
    private readonly IReportParameterWhitelist _whitelist;

    public ReportEngine(
        ISalesStatQuery salesStat,
        IInvoiceStatQuery invoiceStat,
        IPOQuery poQuery,
        IInventoryQuery inventoryQuery,
        IReportParameterWhitelist whitelist)
    {
        _salesStat = salesStat;
        _invoiceStat = invoiceStat;
        _poQuery = poQuery;
        _inventoryQuery = inventoryQuery;
        _whitelist = whitelist;
    }

    public Task<SalesStatSummaryDto> SalesStatAsync(SalesStatParamsDto args, CancellationToken ct = default)
        => _salesStat.QueryAsync(args, ct);

    public Task<InvoiceStatSummaryDto> InvoiceStatAsync(InvoiceStatParamsDto args, CancellationToken ct = default)
        => _invoiceStat.QueryAsync(args, ct);

    public Task<PagedResult<POListItemDto>> POQueryAsync(POQueryParamsDto args, CancellationToken ct = default)
        => _poQuery.QueryAsync(args, ct);

    public Task<IReadOnlyList<InventorySummaryDto>> InventorySummaryAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
        => _inventoryQuery.QuerySummaryAsync(args, ct);

    public Task<PagedResult<InventoryTxnDto>> InventoryTransactionsAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
        => _inventoryQuery.QueryTransactionsAsync(args, ct);

    /// <summary>
    /// 根据报表类型与参数，构建用于导出的表格/打印负载。
    /// </summary>
    public async Task<ExportPayload> BuildExportPayloadAsync(ReportType type, IDictionary<string, object?> parameters, ReportExportFormat format, CancellationToken ct = default)
    {
        // 1) 参数白名单过滤与规范化
        var safeParams = _whitelist.Filter(type, parameters);

        // 2) 根据类型查询数据并组装导出结构
        switch (type)
        {
            case ReportType.SalesStat:
            {
                var args = _whitelist.Bind<SalesStatParamsDto>(type, safeParams);
                var data = await SalesStatAsync(args, ct);
                var columns = new List<ExportColumn>
                {
                    new("key", "维度键"),
                    new("name", "名称"),
                    new("orderCount", "订单数"),
                    new("totalQty", "数量合计"),
                    new("subtotal", "不含税合计"),
                    new("tax", "税额合计"),
                    new("totalAmount", "含税合计")
                };
                var rows = data.Items.Select(x => new Dictionary<string, object?>
                {
                    ["key"] = x.Key,
                    ["name"] = x.Name,
                    ["orderCount"] = x.OrderCount,
                    ["totalQty"] = x.TotalQty,
                    ["subtotal"] = x.Subtotal,
                    ["tax"] = x.Tax,
                    ["totalAmount"] = x.TotalAmount
                }).ToList();
                return new ExportPayload
                {
                    Type = type,
                    Title = "销售统计",
                    Parameters = safeParams,
                    Columns = columns,
                    Rows = rows
                };
            }
            case ReportType.InvoiceStat:
            {
                var args = _whitelist.Bind<InvoiceStatParamsDto>(type, safeParams);
                var data = await InvoiceStatAsync(args, ct);
                var columns = new List<ExportColumn>
                {
                    new("key", "维度键"),
                    new("name", "名称"),
                    new("invoiceCount", "发票数"),
                    new("amount", "不含税合计"),
                    new("tax", "税额合计"),
                    new("amountWithTax", "含税合计")
                };
                var rows = data.Items.Select(x => new Dictionary<string, object?>
                {
                    ["key"] = x.Key,
                    ["name"] = x.Name,
                    ["invoiceCount"] = x.InvoiceCount,
                    ["amount"] = x.Amount,
                    ["tax"] = x.Tax,
                    ["amountWithTax"] = x.AmountWithTax
                }).ToList();
                return new ExportPayload
                {
                    Type = type,
                    Title = "发票统计",
                    Parameters = safeParams,
                    Columns = columns,
                    Rows = rows
                };
            }
            case ReportType.POQuery:
            {
                var args = _whitelist.Bind<POQueryParamsDto>(type, safeParams);
                var page = await POQueryAsync(args, ct);
                var columns = new List<ExportColumn>
                {
                    new("poNo", "采购单号"),
                    new("date", "业务日期"),
                    new("vendorId", "供应商"),
                    new("amount", "金额"),
                    new("status", "状态")
                };
                var rows = page.Items.Select(x => new Dictionary<string, object?>
                {
                    ["poNo"] = x.PoNo,
                    ["date"] = x.Date.ToString("yyyy-MM-dd"),
                    ["vendorId"] = x.VendorId,
                    ["amount"] = x.Amount,
                    ["status"] = x.Status
                }).ToList();
                return new ExportPayload
                {
                    Type = type,
                    Title = "采购订单查询",
                    Parameters = safeParams,
                    Columns = columns,
                    Rows = rows
                };
            }
            case ReportType.Inventory:
            default:
            {
                // 默认导出“库存汇总”
                var args = _whitelist.Bind<InventoryQueryParamsDto>(type, safeParams);
                var list = await InventorySummaryAsync(args, ct);
                var columns = new List<ExportColumn>
                {
                    new("productId", "产品"),
                    new("whse", "仓库"),
                    new("loc", "库位"),
                    new("onHand", "在手"),
                    new("reserved", "预留"),
                    new("available", "可用")
                };
                var rows = list.Select(x => new Dictionary<string, object?>
                {
                    ["productId"] = x.ProductId,
                    ["whse"] = x.Whse,
                    ["loc"] = x.Loc,
                    ["onHand"] = x.OnHand,
                    ["reserved"] = x.Reserved,
                    ["available"] = x.Available
                }).ToList();
                return new ExportPayload
                {
                    Type = ReportType.Inventory,
                    Title = "库存汇总",
                    Parameters = safeParams,
                    Columns = columns,
                    Rows = rows
                };
            }
        }
    }
}

