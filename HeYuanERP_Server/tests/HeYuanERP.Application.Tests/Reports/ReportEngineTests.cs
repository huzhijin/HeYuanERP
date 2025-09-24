// 版权所有(c) HeYuanERP
// 说明：报表引擎核心逻辑测试（xUnit，中文注释）。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Reports;
using HeYuanERP.Application.Reports.Exporters;
using HeYuanERP.Application.Reports.Queries;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;
using Xunit;

namespace HeYuanERP.Application.Tests.Reports;

public class ReportEngineTests
{
    [Fact]
    public async Task BuildExportPayload_SalesStat_Should_Build_Table()
    {
        var engine = CreateEngine();
        var whitelist = new ReportParameterWhitelist();
        var safe = whitelist.Filter(ReportType.SalesStat, new Dictionary<string, object?> { ["groupBy"] = "day" });
        var payload = await engine.BuildExportPayloadAsync(ReportType.SalesStat, safe, ReportExportFormat.Csv);

        Assert.Equal(ReportType.SalesStat, payload.Type);
        Assert.Equal("销售统计", payload.Title);
        Assert.True(payload.Rows.Count > 0);
        Assert.Contains(payload.Columns, c => c.Key == "orderCount");
    }

    [Fact]
    public async Task BuildExportPayload_POQuery_Should_Format_DateOnly()
    {
        var engine = CreateEngine();
        var whitelist = new ReportParameterWhitelist();
        var safe = whitelist.Filter(ReportType.POQuery, new Dictionary<string, object?> { ["page"] = 1, ["size"] = 10 });
        var payload = await engine.BuildExportPayloadAsync(ReportType.POQuery, safe, ReportExportFormat.Csv);
        Assert.Equal("采购订单查询", payload.Title);
        var row = payload.Rows.First();
        Assert.Matches("^\\d{4}-\\d{2}-\\d{2}$", row["date"]?.ToString() ?? "");
    }

    private static IReportEngine CreateEngine()
    {
        var sales = new FakeSalesStatQuery();
        var invoice = new FakeInvoiceStatQuery();
        var po = new FakePOQuery();
        var inv = new FakeInventoryQuery();
        var wl = new ReportParameterWhitelist();
        return new ReportEngine(sales, invoice, po, inv, wl);
    }

    private sealed class FakeSalesStatQuery : ISalesStatQuery
    {
        public Task<SalesStatSummaryDto> QueryAsync(SalesStatParamsDto args, CancellationToken ct = default)
        {
            var list = new[]
            {
                new SalesStatItemDto{ Key = "2024-01", Name = "2024-01", OrderCount = 3, TotalQty = 10, Subtotal = 100, Tax = 5, TotalAmount = 105 }
            };
            return Task.FromResult(new SalesStatSummaryDto
            {
                Items = list,
                TotalQty = list.Sum(x => x.TotalQty),
                Subtotal = list.Sum(x => x.Subtotal),
                Tax = list.Sum(x => x.Tax),
                TotalAmount = list.Sum(x => x.TotalAmount)
            });
        }
    }

    private sealed class FakeInvoiceStatQuery : IInvoiceStatQuery
    {
        public Task<InvoiceStatSummaryDto> QueryAsync(InvoiceStatParamsDto args, CancellationToken ct = default)
        {
            var list = new[]
            {
                new InvoiceStatItemDto{ Key = "2024-01", Name = "2024-01", InvoiceCount = 2, Amount = 200, Tax = 10, AmountWithTax = 210 }
            };
            return Task.FromResult(new InvoiceStatSummaryDto
            {
                Items = list,
                Amount = list.Sum(x => x.Amount),
                Tax = list.Sum(x => x.Tax),
                AmountWithTax = list.Sum(x => x.AmountWithTax)
            });
        }
    }

    private sealed class FakePOQuery : IPOQuery
    {
        public Task<PagedResult<POListItemDto>> QueryAsync(POQueryParamsDto args, CancellationToken ct = default)
        {
            var list = new[]
            {
                new POListItemDto{ Id = "1", PoNo = "PO001", Date = new DateOnly(2024, 1, 1), VendorId = "V1", Amount = 123.45m, Status = "Open" }
            };
            return Task.FromResult(PagedResult<POListItemDto>.Create(list, args.Page, args.Size, 1));
        }
    }

    private sealed class FakeInventoryQuery : IInventoryQuery
    {
        public Task<IReadOnlyList<InventorySummaryDto>> QuerySummaryAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
        {
            IReadOnlyList<InventorySummaryDto> list = new[]
            {
                new InventorySummaryDto{ ProductId = "P1", Whse = "A", Loc = "A-01", OnHand = 100, Reserved = 10, Available = 90 }
            };
            return Task.FromResult(list);
        }

        public Task<PagedResult<InventoryTxnDto>> QueryTransactionsAsync(InventoryQueryParamsDto args, CancellationToken ct = default)
        {
            var list = new[]
            {
                new InventoryTxnDto{ TxnId = "T1", TxnCode = "IN", ProductId = "P1", Qty = 100, Whse = "A", Loc = "A-01", TxnDate = DateTimeOffset.UtcNow }
            };
            return Task.FromResult(PagedResult<InventoryTxnDto>.Create(list, args.Page, args.Size, 1));
        }
    }
}

