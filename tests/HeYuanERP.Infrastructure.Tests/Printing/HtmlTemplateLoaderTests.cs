using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HeYuanERP.Domain.Printing;
using HeYuanERP.Domain.Printing.Exceptions;
using HeYuanERP.Infrastructure.Printing;
using Microsoft.Extensions.Options;
using Xunit;

namespace HeYuanERP.Infrastructure.Tests.Printing;

/// <summary>
/// HtmlTemplateLoader 测试：加载与包含解析。
/// </summary>
public class HtmlTemplateLoaderTests
{
    [Fact]
    public async Task Should_Load_Template_And_Resolve_Include()
    {
        var root = Path.Combine(Path.GetTempPath(), "heyuanerp-tests", Guid.NewGuid().ToString("N"));
        var shared = Path.Combine(root, "shared");
        var order = Path.Combine(root, "order");
        Directory.CreateDirectory(shared);
        Directory.CreateDirectory(order);
        try
        {
            await File.WriteAllTextAsync(Path.Combine(shared, "header.html"), "<div>HEADER</div>", Encoding.UTF8);
            var html = "<!doctype html><html><head><meta charset=\"utf-8\"></head><body><!--#include \"shared/header.html\"--><p>{{ text }}</p></body></html>";
            await File.WriteAllTextAsync(Path.Combine(order, "default.html"), html, Encoding.UTF8);

            var loader = new HtmlTemplateLoader(Options.Create(new PrintOptions { TemplatesRoot = root }));
            var result = await loader.LoadAsync("order", "default");

            Assert.Contains("HEADER", result);
        }
        finally
        {
            try { Directory.Delete(root, true); } catch { /* 忽略 */ }
        }
    }

    [Fact]
    public async Task Should_Throw_When_Template_NotFound()
    {
        var root = Path.Combine(Path.GetTempPath(), "heyuanerp-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var loader = new HtmlTemplateLoader(Options.Create(new PrintOptions { TemplatesRoot = root }));
            await Assert.ThrowsAsync<TemplateNotFoundException>(() => loader.LoadAsync("order", "missing"));
        }
        finally
        {
            try { Directory.Delete(root, true); } catch { /* 忽略 */ }
        }
    }
}

