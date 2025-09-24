using HeYuanERP.Api.Middleware;
using Microsoft.AspNetCore.Builder;

namespace HeYuanERP.Api.Extensions;

/// <summary>
/// 应用构建器扩展：统一响应中间件注册。
/// </summary>
public static class ApplicationBuilderExtensionsUnifiedResponse
{
    /// <summary>
    /// 启用统一响应与异常捕获中间件。
    /// </summary>
    public static IApplicationBuilder UseUnifiedResponse(this IApplicationBuilder app)
    {
        app.UseMiddleware<UnifiedResponseMiddleware>();
        return app;
    }
}

