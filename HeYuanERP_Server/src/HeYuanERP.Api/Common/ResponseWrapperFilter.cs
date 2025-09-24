using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HeYuanERP.Api.Common;

// 统一响应过滤器：将 ActionResult 包装为 Envelope，附带 TraceId
public class ResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // 文件/二进制等直接返回（如 PDF、文件下载）
        if (context.Result is FileResult)
        {
            await next();
            return;
        }

        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        switch (context.Result)
        {
            case ObjectResult objResult:
                // 已经是 ApiResponse 则透传，仅补 TraceId
                if (objResult.Value is ApiResponse existing)
                {
                    existing.TraceId ??= traceId;
                    break;
                }

                // 2xx 才包装；其他状态码交由异常中间件或框架处理
                var status = objResult.StatusCode ?? 200;
                if (status >= 200 && status < 300)
                {
                    var wrapped = new ApiResponse
                    {
                        Code = "OK",
                        Message = "success",
                        Data = objResult.Value,
                        TraceId = traceId
                    };
                    context.Result = new ObjectResult(wrapped)
                    {
                        StatusCode = status
                    };
                }
                break;

            case EmptyResult:
                context.Result = new ObjectResult(new ApiResponse { TraceId = traceId })
                {
                    StatusCode = 200
                };
                break;

            default:
                // 其他类型（如 ContentResult），若状态为 2xx 则包装其内容
                if (context.Result is ContentResult content)
                {
                    context.Result = new ObjectResult(new ApiResponse
                    {
                        Code = "OK",
                        Message = content.Content ?? "success",
                        Data = null,
                        TraceId = traceId
                    }) { StatusCode = content.StatusCode ?? 200 };
                }
                break;
        }

        await next();
    }
}

