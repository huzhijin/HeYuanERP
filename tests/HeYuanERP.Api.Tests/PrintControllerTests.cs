using System.Net.Mime;
using System.Threading.Tasks;
using HeYuanERP.Api.Controllers;
using HeYuanERP.Api.Models;
using HeYuanERP.Application.Printing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HeYuanERP.Api.Tests;

/// <summary>
/// PrintController 单元测试：验证参数校验与返回类型。
/// 说明：使用简单桩实现替代 IPrintService，避免依赖实际浏览器。
/// </summary>
public class PrintControllerTests
{
    private sealed class StubPrintService : IPrintService
    {
        public Task<byte[]> GeneratePdfAsync(PrintRequest request, System.Threading.CancellationToken cancellationToken = default)
        {
            if (request.DocType == "order" && request.Id == "1")
            {
                return Task.FromResult(new byte[] { 1, 2, 3 });
            }
            return Task.FromResult(System.Array.Empty<byte>());
        }
    }

    [Fact]
    public async Task ReturnsBadRequest_WhenDocTypeEmpty()
    {
        var ctrl = new PrintController(new StubPrintService(), NullLogger<PrintController>.Instance);
        var result = await ctrl.GetPdf("", "1", new PrintRequestQuery { Template = "default" }, default);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ReturnsBadRequest_WhenIdEmpty()
    {
        var ctrl = new PrintController(new StubPrintService(), NullLogger<PrintController>.Instance);
        var result = await ctrl.GetPdf("order", "", new PrintRequestQuery { Template = "default" }, default);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ReturnsNotFound_WhenServiceReturnsEmpty()
    {
        var ctrl = new PrintController(new StubPrintService(), NullLogger<PrintController>.Instance);
        var result = await ctrl.GetPdf("order", "999", new PrintRequestQuery { Template = "default" }, default);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ReturnsPdfFile_WhenServiceReturnsBytes()
    {
        var ctrl = new PrintController(new StubPrintService(), NullLogger<PrintController>.Instance);
        var result = await ctrl.GetPdf("order", "1", new PrintRequestQuery { Template = "default" }, default);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal(MediaTypeNames.Application.Pdf, file.ContentType);
        Assert.NotEmpty(file.FileContents);
        Assert.Equal("order-1.pdf", file.FileDownloadName);
    }
}

