using HeYuanERP.Application.AI;
using HeYuanERP.Application.OA;
using HeYuanERP.Infrastructure.Clients.Ai;
using HeYuanERP.Infrastructure.Clients.Http;
using HeYuanERP.Infrastructure.Clients.Oa;
using Microsoft.Extensions.DependencyInjection;

namespace HeYuanERP.Api.Extensions
{
    /// <summary>
    /// 外部客户端注册扩展（Mock/审计）。
    /// 说明：为命名 HttpClient（oa/ai）挂载审计处理器，并注册对应的 Mock 实现。
    /// </summary>
    public static class ServiceCollectionExtensionsExternalClients
    {
        /// <summary>
        /// 注册审计处理器（供 HttpClient 管道使用）。
        /// </summary>
        public static IServiceCollection AddExternalAudit(this IServiceCollection services)
        {
            services.AddSingleton<HeYuanERP.Infrastructure.Logging.Audit.IAuditLogger, HeYuanERP.Infrastructure.Logging.Audit.ConsoleAuditLogger>();
            services.AddTransient<AuditHttpMessageHandler>();
            return services;
        }

        /// <summary>
        /// 使用 Mock 注册 OA 客户端，并为命名 HttpClient("oa") 添加审计处理器。
        /// </summary>
        public static IServiceCollection AddOaClientMock(this IServiceCollection services)
        {
            // 为已注册的命名 HttpClient("oa") 增加审计处理器（不改变其它配置）。
            services.AddHttpClient("oa")
                .AddHttpMessageHandler<AuditHttpMessageHandler>();

            // 注册 Mock 实现（单例，纯内存，不影响主链路）。
            services.AddSingleton<IOaClient, OaClientMock>();
            return services;
        }

        /// <summary>
        /// 使用 Mock 注册 AI 客户端，并为命名 HttpClient("ai") 添加审计处理器。
        /// </summary>
        public static IServiceCollection AddAiClientMock(this IServiceCollection services)
        {
            services.AddHttpClient("ai")
                .AddHttpMessageHandler<AuditHttpMessageHandler>();

            services.AddSingleton<IAiClient, AiClientMock>();
            return services;
        }
    }
}

