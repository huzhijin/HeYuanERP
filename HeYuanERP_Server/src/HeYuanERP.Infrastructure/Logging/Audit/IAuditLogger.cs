using System;

namespace HeYuanERP.Infrastructure.Logging.Audit
{
    /// <summary>
    /// 审计日志接口：记录外部系统交互与替换说明。
    /// </summary>
    public interface IAuditLogger
    {
        /// <summary>
        /// 记录一次对外部系统的调用（请求/响应）。
        /// </summary>
        /// <param name="system">目标系统名（如：OA/AI）</param>
        /// <param name="action">动作名称（HTTP 方法或业务动作）</param>
        /// <param name="url">请求地址</param>
        /// <param name="statusCode">HTTP 状态码（异常时可为空）</param>
        /// <param name="duration">耗时</param>
        /// <param name="success">是否成功</param>
        /// <param name="requestBody">请求体（可截断）</param>
        /// <param name="responseBody">响应体（可截断）</param>
        /// <param name="traceId">链路追踪标识（OpenTelemetry）</param>
        void LogExternalCall(string system, string action, string url, int? statusCode, TimeSpan duration, bool success, string? requestBody = null, string? responseBody = null, string? traceId = null);

        /// <summary>
        /// 记录替换说明（如：某接口以 Mock 代替真实外部依赖）。
        /// </summary>
        /// <param name="component">被替换的契约/组件名（例如：IOaClient）</param>
        /// <param name="implementation">实际使用的实现（例如：OaClientMock）</param>
        /// <param name="targetSystem">目标外部系统（例如：OA/AI）</param>
        /// <param name="reason">替换原因</param>
        void LogReplacementNotice(string component, string implementation, string? targetSystem = null, string? reason = null);
    }
}

