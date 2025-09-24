using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using HeYuanERP.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Api.Middleware;

/// <summary>
/// 统一响应中间件：
/// - 为所有响应注入 TraceId 头（X-Trace-Id）；
/// - 捕获未处理异常并输出统一错误结构（ApiResponse）。
/// 说明：成功响应建议由控制器直接返回 ApiResponse；此处不强行包裹成功结果，避免重复包裹及影响文件/流响应。
/// </summary>
public class UnifiedResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UnifiedResponseMiddleware> _logger;

    public UnifiedResponseMiddleware(RequestDelegate next, ILogger<UnifiedResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        context.Response.Headers["X-Trace-Id"] = traceId;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // 记录异常日志（后续会切换为 Serilog 丰富结构化字段）
            _logger.LogError(ex, "未处理异常：{Message}, TraceId={TraceId}", ex.Message, traceId);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("响应已开始写入，无法返回统一错误结构。TraceId={TraceId}", traceId);
                throw;
            }

            // 返回统一错误响应体
            context.Response.Clear();
            // 按统一响应约定：HTTP 200 + 业务 Code != 0（如需保留 500，可在此调整）
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = ApiResponse.Fail("服务器内部错误", -1, traceId);
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            await context.Response.WriteAsync(json);
        }
    }
}

