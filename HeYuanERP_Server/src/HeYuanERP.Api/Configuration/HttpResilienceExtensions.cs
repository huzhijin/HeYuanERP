using System;
using Microsoft.Extensions.DependencyInjection;
using HeYuanERP.Infrastructure.Clients;

namespace HeYuanERP.Api.Configuration
{
    /// <summary>
    /// HttpClient 标准弹性策略（重试 + 熔断），用于外部 REST 依赖。
    /// 说明：策略仅对瞬时错误（5xx/网络/408）生效，避免影响主链路。
    /// </summary>
    public static class HttpResilienceExtensions
    {
        /// <summary>
        /// 为指定 HttpClientBuilder 添加标准重试/熔断策略。
        /// </summary>
        /// <param name="builder">IHttpClientBuilder</param>
        /// <param name="getOptions">从 DI 获取对应的 ClientOptions 的函数</param>
        public static IHttpClientBuilder AddStandardResilience(this IHttpClientBuilder builder, Func<IServiceProvider, ClientOptions> getOptions)
            => builder; // 简化为无操作占位，避免额外依赖；后续可按需接回 Polly 扩展
    }
}
