using System;
using System.Net;
using System.Net.Http;
using HeYuanERP.Infrastructure.Clients;
// 使用 API 侧的 Resilience 扩展
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HeYuanERP.Api.Configuration
{
    /// <summary>
    /// OA / AI 客户端启动引导（注册 HttpClient、绑定配置）。
    /// 说明：仅做 HttpClient 与 Options 的装配，不直接绑定具体实现（Mock/真实），
    /// 以保证不影响主链路，实际实现的注册放在基础设施层的扩展中完成。
    /// </summary>
    public static class OaAiClientBootstrap
    {
        /// <summary>
        /// 注册 OA/AI 所需的 HttpClient 与 Options，配置来源优先使用环境变量。
        /// 支持配置项：Endpoint/超时/重试/熔断。
        /// </summary>
        public static IServiceCollection AddOaAiClients(this IServiceCollection services, IConfiguration configuration)
        {
            // 1) 绑定强类型配置（支持 env：Clients__OA__* / Clients__AI__*）
            services.AddOptions<OaClientOptions>().Bind(configuration.GetSection("Clients:OA"));
            services.AddOptions<AiClientOptions>().Bind(configuration.GetSection("Clients:AI"));

            // 2) 注册命名 HttpClient：OA
            services
                .AddHttpClient("oa", (sp, client) =>
                {
                    var opts = sp.GetRequiredService<IOptions<OaClientOptions>>().Value;
                    if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
                    {
                        client.BaseAddress = new Uri(opts.BaseUrl, UriKind.Absolute);
                    }
                    client.Timeout = TimeSpan.FromSeconds(Math.Max(1, opts.TimeoutSeconds));
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                })
                .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                    AllowAutoRedirect = false
                })
                .AddStandardResilience(sp => sp.GetRequiredService<IOptions<OaClientOptions>>().Value);

            // 3) 注册命名 HttpClient：AI
            services
                .AddHttpClient("ai", (sp, client) =>
                {
                    var opts = sp.GetRequiredService<IOptions<AiClientOptions>>().Value;
                    if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
                    {
                        client.BaseAddress = new Uri(opts.BaseUrl, UriKind.Absolute);
                    }
                    client.Timeout = TimeSpan.FromSeconds(Math.Max(1, opts.TimeoutSeconds));
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                })
                .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                    AllowAutoRedirect = false
                })
                .AddStandardResilience(sp => sp.GetRequiredService<IOptions<AiClientOptions>>().Value);

            // 提示：实际 IOaClient/IAiClient 的具体实现（Mock/真实）请在基础设施层扩展中注册，
            // 如：services.AddOaClientMock() / services.AddAiClientMock()（后续批次提供）。

            return services;
        }
    }
}
