// 版权所有(c) HeYuanERP
// 说明：报表相关 Minimal API 路由扩展（可选），提供健康探针等（中文注释）。

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HeYuanERP.Api.Routes;

/// <summary>
/// 报表路由扩展：用于映射简单的 Minimal API 端点（不依赖控制器）。
/// </summary>
public static class ReportRoutes
{
    /// <summary>
    /// 映射报表健康探针与简易测试端点。
    /// </summary>
    public static IEndpointRouteBuilder MapReportRoutes(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/reports/_ping", () => new { code = "OK", message = "pong" });
        return endpoints;
    }
}

