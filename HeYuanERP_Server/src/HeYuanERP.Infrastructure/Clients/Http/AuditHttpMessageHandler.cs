using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Infrastructure.Logging.Audit;

namespace HeYuanERP.Infrastructure.Clients.Http
{
    /// <summary>
    /// 审计用 HttpMessageHandler：记录对外部系统的请求/响应。
    /// 注意：仅用于外部 REST 依赖；对内部服务调用不强制使用，以避免影响主链路。
    /// </summary>
    public class AuditHttpMessageHandler : DelegatingHandler
    {
        private readonly IAuditLogger _audit;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public AuditHttpMessageHandler(IAuditLogger audit)
        {
            _audit = audit;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            string? reqBody = null;

            if (request.Content != null)
            {
                var raw = await request.Content.ReadAsStringAsync(cancellationToken);
                reqBody = TrimForAudit(raw);
            }

            HttpResponseMessage response;
            try
            {
                response = await base.SendAsync(request, cancellationToken);
                sw.Stop();
            }
            catch (Exception ex)
            {
                sw.Stop();
                _audit.LogExternalCall(
                    system: ResolveSystemName(request.RequestUri),
                    action: request.Method.Method,
                    url: request.RequestUri?.ToString() ?? "",
                    statusCode: null,
                    duration: sw.Elapsed,
                    success: false,
                    requestBody: reqBody,
                    responseBody: $"EXCEPTION:{ex.GetType().Name}:{TrimForAudit(ex.Message)}",
                    traceId: Activity.Current?.TraceId.ToString());
                throw;
            }

            string? respBody = null;
            if (response.Content != null)
            {
                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                respBody = TrimForAudit(raw);
            }

            _audit.LogExternalCall(
                system: ResolveSystemName(request.RequestUri),
                action: request.Method.Method,
                url: request.RequestUri?.ToString() ?? "",
                statusCode: (int)response.StatusCode,
                duration: sw.Elapsed,
                success: response.IsSuccessStatusCode,
                requestBody: reqBody,
                responseBody: respBody,
                traceId: Activity.Current?.TraceId.ToString());

            return response;
        }

        /// <summary>
        /// 简单推断目标系统名（用于审计展示）。
        /// </summary>
        private static string ResolveSystemName(Uri? uri)
        {
            if (uri == null) return "External";
            var host = uri.Host?.ToLowerInvariant() ?? "external";
            if (host.Contains("oa")) return "OA";
            if (host.Contains("ai")) return "AI";
            return "External";
        }

        /// <summary>
        /// 审计内容长度限制，避免大报文影响性能与日志成本。
        /// </summary>
        private static string TrimForAudit(string? raw, int maxLen = 4096)
        {
            if (string.IsNullOrEmpty(raw)) return string.Empty;
            return raw.Length <= maxLen ? raw : raw.Substring(0, maxLen) + "…(truncated)";
        }
    }
}

