using System;
using Serilog;

namespace HeYuanERP.Infrastructure.Logging.Audit
{
    /// <summary>
    /// 基于 Serilog 的审计日志实现。
    /// 说明：所有字段以结构化日志输出，便于后续检索与审计报表。
    /// </summary>
    public class SerilogAuditLogger : IAuditLogger
    {
        private readonly ILogger _logger;

        public SerilogAuditLogger(ILogger logger)
        {
            _logger = logger.ForContext("SourceContext", "Audit");
        }

        /// <inheritdoc />
        public void LogExternalCall(string system, string action, string url, int? statusCode, TimeSpan duration, bool success, string? requestBody = null, string? responseBody = null, string? traceId = null)
        {
            var evt = success ? "audit.external_call.ok" : "audit.external_call.fail";
            _logger
                .ForContext("event", evt)
                .ForContext("system", system)
                .ForContext("action", action)
                .ForContext("url", url)
                .ForContext("status", statusCode)
                .ForContext("duration_ms", (long)duration.TotalMilliseconds)
                .ForContext("success", success)
                .ForContext("trace_id", traceId)
                .ForContext("req", requestBody)
                .ForContext("resp", responseBody)
                .Information("外部调用：{System} {Action} {Url} 状态={Status} 耗时={Duration}ms 成功={Success}",
                    system, action, url, statusCode, (long)duration.TotalMilliseconds, success);
        }

        /// <inheritdoc />
        public void LogReplacementNotice(string component, string implementation, string? targetSystem = null, string? reason = null)
        {
            _logger
                .ForContext("event", "audit.replacement")
                .ForContext("component", component)
                .ForContext("impl", implementation)
                .ForContext("target", targetSystem)
                .ForContext("reason", reason)
                .Warning("替换说明：{Component} 使用 {Implementation} （目标系统={Target}，原因={Reason}）",
                    component, implementation, targetSystem, reason);
        }
    }
}

