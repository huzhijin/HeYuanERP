using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace HeYuanERP.Api.Tests;

// 登录接口的集成测试：验证 /api/auth/login 正常签发 JWT
public class AuthLoginTests : IClassFixture<AuthLoginTests.CustomFactory>
{
    private readonly CustomFactory _factory;

    public AuthLoginTests(CustomFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_Admin_Succeeds_ReturnsToken()
    {
        var client = _factory.CreateClient();
        var req = new { loginId = "admin", password = "admin123" };
        var resp = await client.PostAsJsonAsync("/api/auth/login", req);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var doc = await resp.Content.ReadFromJsonAsync<LoginDoc>();
        Assert.NotNull(doc);
        Assert.Equal("OK", doc!.code);
        Assert.False(string.IsNullOrWhiteSpace(doc.data?.token));
        Assert.Equal("admin", doc.data?.user?.loginId);
    }

    public class CustomFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
        }
    }

    private class LoginDoc
    {
        public string code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public LoginData? data { get; set; }
        public string? traceId { get; set; }
    }

    private class LoginData
    {
        public string token { get; set; } = string.Empty;
        public UserData? user { get; set; }
        public List<string>? roles { get; set; }
    }

    private class UserData
    {
        public string id { get; set; } = string.Empty;
        public string loginId { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public List<string>? permissions { get; set; }
    }
}
