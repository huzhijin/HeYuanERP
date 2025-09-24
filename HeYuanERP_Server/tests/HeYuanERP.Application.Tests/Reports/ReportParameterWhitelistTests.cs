// 版权所有(c) HeYuanERP
// 说明：报表参数白名单与绑定测试（xUnit，中文注释）。

using System;
using System.Collections.Generic;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;
using Xunit;

namespace HeYuanERP.Application.Tests.Reports;

public class ReportParameterWhitelistTests
{
    [Fact]
    public void Filter_SalesStat_RemovesUnknown_And_BuildsRange()
    {
        var wl = new ReportParameterWhitelist();
        var input = new Dictionary<string, object?>
        {
            ["from"] = "2024-01-01",
            ["to"] = "2024-01-31",
            ["customerId"] = "C1",
            ["groupBy"] = "Month",
            ["hack"] = "1"
        };

        var safe = wl.Filter(ReportType.SalesStat, input);

        Assert.DoesNotContain("hack", safe.Keys, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("customerId", safe.Keys, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("groupBy", safe.Keys, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("range", safe.Keys, StringComparer.OrdinalIgnoreCase);

        var range = Assert.IsType<Dictionary<string, object?>>(safe["range"]!);
        var start = Assert.IsType<string>(range["startUtc"]!);
        var end = Assert.IsType<string>(range["endUtc"]!);
        Assert.StartsWith("2024-01-01", start, StringComparison.Ordinal);
        Assert.StartsWith("2024-01-31", end, StringComparison.Ordinal);
        Assert.Equal("month", safe["groupBy"]);
    }

    [Fact]
    public void Bind_SalesStat_NormalizesRange()
    {
        var wl = new ReportParameterWhitelist();
        var input = new Dictionary<string, object?>
        {
            ["from"] = "2024-02-10",
            ["to"] = "2024-02-01" // 颠倒，将被 Normalize()
        };

        var dto = wl.Bind<SalesStatParamsDto>(ReportType.SalesStat, input);
        Assert.NotNull(dto.Range);
        Assert.True(dto.Range!.IsValid);
    }
}

