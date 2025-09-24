using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Http.Json;

namespace HeYuanERP.Api.Tests;

// 健康检查接口的集成测试：验证 /healthz 返回 200 且包含 Envelope 字段
public class HealthzTests : IClassFixture<HealthzTests.CustomFactory>
{
    private readonly CustomFactory _factory;

    public HealthzTests(CustomFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Healthz_ReturnsOk_WithEnvelope()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/healthz");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var doc = await resp.Content.ReadFromJsonAsync<JsonDoc>();
        Assert.NotNull(doc);
        Assert.Equal("OK", doc!.code);
        Assert.Equal("success", doc.message);
        Assert.NotNull(doc.traceId);
    }

    // 自定义工厂：将环境设为 Test，避免开发期数据播种触发数据库连接
    public class CustomFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
        }
    }

    private class JsonDoc
    {
        public string code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public object? data { get; set; }
        public string? traceId { get; set; }
    }
}
