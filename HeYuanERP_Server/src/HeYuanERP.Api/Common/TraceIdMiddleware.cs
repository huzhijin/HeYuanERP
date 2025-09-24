using System.Diagnostics;

namespace HeYuanERP.Api.Common;

// TraceId 中间件：统一请求相关的 TraceId/X-Request-ID，并透传到响应头
public class TraceIdMiddleware
{
    private const string HeaderName = "X-Request-ID";
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // 读取来访的 X-Request-ID，或使用 Activity/TraceIdentifier 生成
        if (!context.Request.Headers.TryGetValue(HeaderName, out var reqId) || string.IsNullOrWhiteSpace(reqId))
        {
            reqId = Activity.Current?.Id ?? context.TraceIdentifier;
        }

        // 设置 HttpContext 的 TraceIdentifier 与响应头
        context.TraceIdentifier = reqId!;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = reqId!;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}

