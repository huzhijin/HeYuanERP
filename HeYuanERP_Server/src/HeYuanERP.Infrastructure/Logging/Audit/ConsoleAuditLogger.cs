using System;

namespace HeYuanERP.Infrastructure.Logging.Audit
{
    public class ConsoleAuditLogger : IAuditLogger
    {
        public void LogExternalCall(string system, string action, string url, int? statusCode, TimeSpan duration, bool success, string? requestBody = null, string? responseBody = null, string? traceId = null)
        {
            Console.WriteLine($"[Audit] {system} {action} {url} Status={statusCode} Duration={duration.TotalMilliseconds}ms Success={success}");
        }

        public void LogReplacementNotice(string component, string implementation, string? targetSystem = null, string? reason = null)
        {
            Console.WriteLine($"[Audit Notice] {component} using {implementation} (Target={targetSystem}, Reason={reason})");
        }
    }
}
