using HeYuanERP.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeYuanERP.Api.Configuration
{
    /// <summary>
    /// Mock 开关（从环境变量/配置读取）。
    /// 用于按需启用 OA/AI 的 Mock 实现，便于灰度与回滚，不影响主链路。
    /// </summary>
    public static class MockFlags
    {
        private const string OaKey = "Clients:OA:UseMock";
        private const string AiKey = "Clients:AI:UseMock";

        /// <summary>
        /// 是否启用 OA Mock（默认 true）。
        /// </summary>
        public static bool OaMockEnabled(IConfiguration configuration) => configuration.GetValue<bool>(OaKey, true);

        /// <summary>
        /// 是否启用 AI Mock（默认 true）。
        /// </summary>
        public static bool AiMockEnabled(IConfiguration configuration) => configuration.GetValue<bool>(AiKey, true);

        /// <summary>
        /// 根据配置开关注册 Mock 客户端（需提前调用 AddOaAiClients 注册命名 HttpClient）。
        /// </summary>
        public static IServiceCollection AddMockClientsIfEnabled(this IServiceCollection services, IConfiguration configuration)
        {
            if (OaMockEnabled(configuration))
            {
                services.AddOaClientMock();
            }

            if (AiMockEnabled(configuration))
            {
                services.AddAiClientMock();
            }

            return services;
        }
    }
}
