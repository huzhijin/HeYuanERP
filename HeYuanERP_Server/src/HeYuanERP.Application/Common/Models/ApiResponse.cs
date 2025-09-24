using System.Diagnostics;

namespace HeYuanERP.Application.Common.Models;

/// <summary>
/// 统一 API 响应模型（泛型）。
/// 约定：Code=0 表示成功；非 0 表示业务或系统错误。
/// </summary>
/// <typeparam name="T">数据负载类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码（0=成功，其他=失败）。
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 提示消息（成功/失败）。
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 数据负载。
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 分布式追踪标识（由中间件注入）。
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// 是否成功（Code==0）。
    /// </summary>
    public bool Success => Code == 0;

    /// <summary>
    /// 成功响应。
    /// </summary>
    public static ApiResponse<T> Ok(T? data, string message = "成功", string? traceId = null)
        => new() { Code = 0, Message = message, Data = data, TraceId = traceId ?? Activity.Current?.TraceId.ToString() };

    /// <summary>
    /// 失败响应。
    /// </summary>
    public static ApiResponse<T> Fail(string message, int code = -1, string? traceId = null, T? data = default)
        => new() { Code = code, Message = message, Data = data, TraceId = traceId ?? Activity.Current?.TraceId.ToString() };
}

/// <summary>
/// 统一 API 响应模型（非泛型便捷封装）。
/// </summary>
public static class ApiResponse
{
    /// <summary>
    /// 成功（无数据）。
    /// </summary>
    public static ApiResponse<object?> Ok(string message = "成功", string? traceId = null)
        => ApiResponse<object?>.Ok(null, message, traceId);

    /// <summary>
    /// 失败（无数据）。
    /// </summary>
    public static ApiResponse<object?> Fail(string message, int code = -1, string? traceId = null)
        => ApiResponse<object?>.Fail(message, code, traceId, null);
}

