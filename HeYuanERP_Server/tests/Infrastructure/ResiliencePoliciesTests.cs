using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Infrastructure.Clients;
using HeYuanERP.Infrastructure.Clients.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeYuanERP_Server.Tests.Infrastructure
{
    /// <summary>
    /// Http 重试/熔断策略基础测试（不访问外网）。
    /// </summary>
    public class ResiliencePoliciesTests
    {
        private sealed class FlakyPrimaryHandler : HttpMessageHandler
        {
            private readonly int _failuresBeforeSuccess;
            private int _attempts;

            public int Attempts => _attempts;

            public FlakyPrimaryHandler(int failuresBeforeSuccess)
            {
                _failuresBeforeSuccess = failuresBeforeSuccess;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _attempts);
                if (_attempts <= _failuresBeforeSuccess)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        RequestMessage = request,
                        Content = new StringContent("fail")
                    });
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = request,
                    Content = new StringContent("ok")
                });
            }
        }

        [Fact(DisplayName = "重试策略：失败后重试至成功")]
        public async Task Retry_UntilSuccess_WithinLimit()
        {
            var services = new ServiceCollection();

            var opts = new OaClientOptions
            {
                RetryCount = 3,
                RetryBaseDelayMs = 1,
                RetryMaxDelayMs = 2,
                TimeoutSeconds = 5
            };

            var flaky = new FlakyPrimaryHandler(failuresBeforeSuccess: 2);

            services.AddHttpClient("test", client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
                })
                .ConfigurePrimaryHttpMessageHandler(() => flaky)
                .AddStandardResilience(_ => opts);

            var sp = services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("test");

            var response = await client.GetAsync("http://unit.test/");

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(3, flaky.Attempts); // 2失败 + 1成功
        }

        [Fact(DisplayName = "重试策略：超过次数仍失败")]
        public async Task Retry_ExceedLimit_Fails()
        {
            var services = new ServiceCollection();

            var opts = new OaClientOptions
            {
                RetryCount = 2,
                RetryBaseDelayMs = 1,
                RetryMaxDelayMs = 2,
                TimeoutSeconds = 5
            };

            var flaky = new FlakyPrimaryHandler(failuresBeforeSuccess: 5);

            services.AddHttpClient("test2")
                .ConfigurePrimaryHttpMessageHandler(() => flaky)
                .AddStandardResilience(_ => opts);

            var sp = services.BuildServiceProvider();
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("test2");

            var response = await client.GetAsync("http://unit.test/");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(3, flaky.Attempts); // 1次初始 + 2次重试
        }
    }
}

