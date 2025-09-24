using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Serilog;

namespace HeYuanERP.Api.Common;

// 全局异常处理中间件：捕获未处理异常，统一返回 ErrorEnvelope，并记录日志
public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var (statusCode, code, message, errors) = MapException(ex);

        Log.ForContext("TraceId", traceId)
           .ForContext("Path", context.Request.Path)
           .ForContext("Method", context.Request.Method)
           .Error(ex, "请求发生未处理异常");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var resp = new ErrorResponse
        {
            Code = code,
            Message = message,
            Errors = errors,
            TraceId = traceId
        };

        var json = JsonSerializer.Serialize(resp, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }

    // 将异常映射为业务错误码与 HTTP 状态码
    private static (HttpStatusCode status, string code, string message, List<ErrorItem> errors) MapException(Exception ex)
    {
        switch (ex)
        {
            case ValidationException vex:
                return (HttpStatusCode.BadRequest,
                    "ERR_VALIDATION",
                    "请求参数校验失败",
                    vex.Errors.Select(e => new ErrorItem { Field = e.PropertyName, Message = e.ErrorMessage }).ToList());
            case KeyNotFoundException:
                return (HttpStatusCode.NotFound, "ERR_NOT_FOUND", ex.Message, new());
            case UnauthorizedAccessException:
                return (HttpStatusCode.Forbidden, "ERR_FORBIDDEN", "无权限执行该操作", new());
            case ApplicationException:
                return (HttpStatusCode.BadRequest, "ERR_BUSINESS", ex.Message, new());
            default:
                return (HttpStatusCode.InternalServerError, "ERR_UNHANDLED", "服务器内部错误", new());
        }
    }
}

