using System.ComponentModel.DataAnnotations;

namespace HeYuanERP.Infrastructure.Clients
{
    /// <summary>
    /// 外部 REST 客户端通用配置（Endpoint/超时/重试/熔断）。
    /// 所有配置建议通过环境变量注入（例如：Clients__OA__BaseUrl）。
    /// </summary>
    public abstract class ClientOptions
    {
        /// <summary>
        /// 服务基础地址（如：https://oa.example.com）。
        /// </summary>
        [Url]
        public string? BaseUrl { get; set; }

        /// <summary>
        /// HttpClient 级别超时时间（秒）。
        /// </summary>
        [Range(1, 600)]
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// 重试次数（仅对可瞬时错误生效：5xx/网络/408）。
        /// </summary>
        [Range(0, 10)]
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// 重试首次延迟（毫秒，指数退避基数）。
        /// </summary>
        [Range(0, 60000)]
        public int RetryBaseDelayMs { get; set; } = 200;

        /// <summary>
        /// 重试最大延迟上限（毫秒）。
        /// </summary>
        [Range(0, 120000)]
        public int RetryMaxDelayMs { get; set; } = 2000;

        /// <summary>
        /// 熔断失败阈值（比例 0-1，高级熔断使用）。
        /// </summary>
        [Range(0.01, 0.99)]
        public double CircuitBreakerFailureThreshold { get; set; } = 0.5;

        /// <summary>
        /// 熔断采样窗口（秒）。
        /// </summary>
        [Range(1, 600)]
        public int CircuitBreakerSamplingSeconds { get; set; } = 60;

        /// <summary>
        /// 熔断最小吞吐量（窗口内最少请求数）。
        /// </summary>
        [Range(2, 10000)]
        public int CircuitBreakerMinimumThroughput { get; set; } = 20;

        /// <summary>
        /// 熔断断开时长（秒）。
        /// </summary>
        [Range(1, 600)]
        public int CircuitBreakerBreakSeconds { get; set; } = 30;

        /// <summary>
        /// 是否使用 Mock（用于切换外部依赖，在不影响主链路的前提下可随时启用）。
        /// </summary>
        public bool UseMock { get; set; } = true;
    }

    /// <summary>
    /// OA 客户端配置。
    /// 环境变量示例：
    /// - Clients__OA__BaseUrl
    /// - Clients__OA__TimeoutSeconds
    /// - Clients__OA__RetryCount
    /// - Clients__OA__RetryBaseDelayMs / Clients__OA__RetryMaxDelayMs
    /// - Clients__OA__CircuitBreakerFailureThreshold / SamplingSeconds / MinimumThroughput / BreakSeconds
    /// - Clients__OA__UseMock
    /// </summary>
    public sealed class OaClientOptions : ClientOptions { }

    /// <summary>
    /// AI 客户端配置。
    /// 环境变量示例：
    /// - Clients__AI__BaseUrl
    /// - Clients__AI__TimeoutSeconds
    /// - Clients__AI__RetryCount
    /// - Clients__AI__RetryBaseDelayMs / Clients__AI__RetryMaxDelayMs
    /// - Clients__AI__CircuitBreakerFailureThreshold / SamplingSeconds / MinimumThroughput / BreakSeconds
    /// - Clients__AI__UseMock
    /// </summary>
    public sealed class AiClientOptions : ClientOptions { }
}

