using System;
using System.Threading.Tasks;
using FluentValidation;
using HeYuanERP.Application.Printing;
using HeYuanERP.Application.Printing.Validation;
using HeYuanERP.Domain.Printing;
using HeYuanERP.Infrastructure.Printing;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace HeYuanERP.Infrastructure.Tests.Printing;

/// <summary>
/// PlaywrightPrintService 测试：仅验证参数非法时抛出异常，避免真实启动浏览器。
/// </summary>
public class PlaywrightPrintServiceTests
{
    private sealed class DummyRenderer : IPrintTemplateRenderer
    {
        public Task<string> RenderHtmlAsync(string docType, string templateName, object viewModel, System.Threading.CancellationToken cancellationToken = default)
            => Task.FromResult("<html><body>OK</body></html>");
    }

    private sealed class DummySnapshotStore : IPrintSnapshotStore
    {
        public Task SaveAsync(Domain.Printing.Models.PrintSnapshot snapshot, System.Threading.CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<Domain.Printing.Models.PrintSnapshot?> TryGetAsync(string docType, string id, string templateName, System.Threading.CancellationToken cancellationToken = default)
            => Task.FromResult<Domain.Printing.Models.PrintSnapshot?>(null);
    }

    [Fact]
    public async Task GeneratePdfAsync_ShouldThrow_WhenRequestInvalid()
    {
        // 使用正式的 Validator，传入未知 docType 触发校验失败
        IValidator<PrintRequest> validator = new PrintRequestValidator();
        var service = new PlaywrightPrintService(
            new DummyRenderer(),
            new DummySnapshotStore(),
            validator,
            Options.Create(new PrintOptions { DefaultTemplate = "default" }),
            NullLogger<PlaywrightPrintService>.Instance);

        var invalid = new PrintRequest { DocType = "unknown", Id = "1", Template = "default" };

        await Assert.ThrowsAsync<ArgumentException>(() => service.GeneratePdfAsync(invalid));
    }
}

