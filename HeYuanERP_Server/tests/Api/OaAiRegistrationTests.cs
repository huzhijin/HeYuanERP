using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Api.Configuration;
using HeYuanERP.Infrastructure.Clients;
using HeYuanERP.Infrastructure.Logging.Audit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeYuanERP_Server.Tests.Api
{
    /// <summary>
    /// API 层 OA/AI 客户端注册测试（Options + 命名 HttpClient + Mock）。
    /// </summary>
    public class OaAiRegistrationTests
    {
        private sealed class FakeAuditLogger : IAuditLogger
        {
            public ConcurrentBag<(string System, string Action, string Url)> Entries { get; } = new();
            public void LogExternalCall(string system, string action, string url, int? statusCode, TimeSpan duration, bool success, string? requestBody = null, string? responseBody = null, string? traceId = null)
                => Entries.Add((system, action, url));
            public void LogReplacementNotice(string component, string implementation, string? targetSystem = null, string? reason = null) { }
        }

        private sealed class OkPrimaryHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") });
        }

        [Fact(DisplayName = "命名 HttpClient 注册与 Mock 挂载审计")]
        public async Task NamedClients_Registered_WithAudit()
        {
            var dict = new Dictionary<string, string?>
            {
                ["Clients:OA:BaseUrl"] = "https://oa.local/",
                ["Clients:OA:TimeoutSeconds"] = "15",
                ["Clients:AI:BaseUrl"] = "https://ai.local/",
                ["Clients:AI:TimeoutSeconds"] = "20",
                ["Clients:OA:UseMock"] = "true",
                ["Clients:AI:UseMock"] = "true",
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(dict!).Build();

            var services = new ServiceCollection();
            services.AddSingleton<IAuditLogger, FakeAuditLogger>();

            // 注册命名 HttpClient 与 Options
            services.AddOaAiClients(config);
            // 挂载审计与 Mock 客户端
            services.AddExternalAudit().AddOaClientMock().AddAiClientMock();

            // 使用测试 PrimaryHandler，避免实际网络请求
            services.AddHttpClient("oa").ConfigurePrimaryHttpMessageHandler(() => new OkPrimaryHandler());
            services.AddHttpClient("ai").ConfigurePrimaryHttpMessageHandler(() => new OkPrimaryHandler());

            var sp = services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var oa = factory.CreateClient("oa");
            var ai = factory.CreateClient("ai");

            Assert.Equal(TimeSpan.FromSeconds(15), oa.Timeout);
            Assert.Equal(new Uri("https://oa.local/"), oa.BaseAddress);
            Assert.Equal(TimeSpan.FromSeconds(20), ai.Timeout);

            // 发起请求，验证审计被触发（系统名由处理器推断）
            var resp1 = await oa.GetAsync("health");
            var resp2 = await ai.GetAsync("status");
            Assert.True(resp1.IsSuccessStatusCode);
            Assert.True(resp2.IsSuccessStatusCode);

            var audit = (FakeAuditLogger)sp.GetRequiredService<IAuditLogger>();
            Assert.NotEmpty(audit.Entries);
        }
    }
}

