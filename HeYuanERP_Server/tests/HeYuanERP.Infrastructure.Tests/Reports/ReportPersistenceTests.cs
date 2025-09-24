// 版权所有(c) HeYuanERP
// 说明：报表持久化相关基础模型测试（xUnit，中文注释）。

using System;
using HeYuanERP.Domain.Reports;
using Xunit;

namespace HeYuanERP.Infrastructure.Tests.Reports;

public class ReportPersistenceTests
{
    [Fact]
    public void ReportJob_Defaults_Should_Be_Set()
    {
        var job = new ReportJob
        {
            Type = ReportType.SalesStat,
            Format = ReportExportFormat.Csv,
            ParametersJson = "{}"
        };

        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.Equal(ReportJobStatus.Pending, job.Status);
        Assert.True(job.CreatedAtUtc <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void ReportSnapshot_Defaults_Should_Be_Set()
    {
        var snap = new ReportSnapshot
        {
            Type = ReportType.Inventory,
            ParametersJson = "{}",
            FileUri = "file:///tmp/x.pdf"
        };

        Assert.NotEqual(Guid.Empty, snap.Id);
        Assert.True(snap.CreatedAtUtc <= DateTimeOffset.UtcNow);
        Assert.Null(snap.RowVersion); // EF 写入后才会赋值
    }
}

