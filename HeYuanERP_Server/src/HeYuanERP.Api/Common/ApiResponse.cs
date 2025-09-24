using System.Text.Json.Serialization;

namespace HeYuanERP.Api.Common;

// 统一响应 Envelope（与 OpenAPI 模板一致）
public class ApiResponse
{
    public string Code { get; set; } = "OK";
    public string Message { get; set; } = "success";
    public object? Data { get; set; }
    public string? TraceId { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public new T? Data
    {
        get => (T?)base.Data;
        set => base.Data = value;
    }

    public bool Success => string.Equals(Code, "OK", StringComparison.OrdinalIgnoreCase);

    public static ApiResponse<T> Ok(T data, string? message = "success")
        => new() { Code = "OK", Message = message ?? "success", Data = data };

    // Backward-compatible helpers used by services/controllers
    public static ApiResponse<T> Error(string message)
        => new() { Code = "ERR", Message = message, Data = default };
}

public class ErrorItem
{
    public string? Field { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ErrorResponse : ApiResponse
{
    public List<ErrorItem> Errors { get; set; } = new();
}

// 分页数据结构（与 OpenAPI 模板一致）
public class Pagination<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int Size { get; set; }
    public int Total { get; set; }
}
