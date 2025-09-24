// 版权所有(c) HeYuanERP
// 说明：报表导出控制器测试（xUnit，中文注释）。

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HeYuanERP.Api.Controllers.Reports;
using HeYuanERP.Application.Reports;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HeYuanERP.Api.Tests.Reports;

public class ReportExportControllerTests
{
    [Fact]
    public async Task Export_And_GetTask_Should_Work()
    {
        var repo = new InMemoryReportJobRepository();
        var service = new ReportExportService(new ReportParameterWhitelist(), repo, queue: null);
        IValidator<ReportExportRequestDto> validator = new ReportExportRequestValidator();
        var controller = new ReportExportController(service, validator)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = BuildHttpContext("user1")
            }
        };

        var body = new ReportExportRequestDto
        {
            Format = "csv",
            Params = new Dictionary<string, object?> { ["customerId"] = "C1" }
        };

        var accepted = await controller.ExportAsync("sales-stat", body, CancellationToken.None);
        var acceptedResult = Assert.IsType<ObjectResult>(accepted);
        Assert.Equal(StatusCodes.Status202Accepted, acceptedResult.StatusCode);

        // 从内存仓储获取任务 Id
        var jobId = repo.LastId;
        Assert.NotEqual(Guid.Empty, jobId);

        var ok = await controller.GetTaskAsync(jobId.ToString(), CancellationToken.None);
        var okResult = Assert.IsType<ObjectResult>(ok);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    private static DefaultHttpContext BuildHttpContext(string userId)
    {
        var ctx = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth");
        ctx.User = new ClaimsPrincipal(identity);
        return ctx;
    }

    private sealed class InMemoryReportJobRepository : IReportJobRepository
    {
        private readonly Dictionary<Guid, ReportJob> _store = new();
        public Guid LastId { get; private set; }

        public Task AddAsync(ReportJob job, CancellationToken ct = default)
        {
            _store[job.Id] = job;
            LastId = job.Id;
            return Task.CompletedTask;
        }

        public Task<ReportJob?> FindAsync(Guid id, CancellationToken ct = default)
        {
            _store.TryGetValue(id, out var job);
            return Task.FromResult(job);
        }

        public Task UpdateAsync(ReportJob job, CancellationToken ct = default)
        {
            _store[job.Id] = job;
            return Task.CompletedTask;
        }
    }
}

