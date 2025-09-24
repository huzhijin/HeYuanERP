// 版权所有(c) HeYuanERP
// 说明：报表通用参数模型（可复用的简单参数类型，中文注释）。

namespace HeYuanERP.Application.Reports.Parameters;

/// <summary>
/// 分页参数（可用于内部方法或扩展，不直接对外暴露）。
/// </summary>
public sealed class PagingParams
{
    /// <summary>页码（≥1）</summary>
    public int Page { get; init; } = 1;

    /// <summary>每页大小（1~200）</summary>
    public int Size { get; init; } = 20;
}

/// <summary>
/// 排序参数（简单键+方向）。
/// </summary>
public sealed class SortParam
{
    /// <summary>排序字段（如 date/amount 等）。</summary>
    public string Field { get; init; } = string.Empty;

    /// <summary>排序方向：asc/desc。</summary>
    public string Direction { get; init; } = "asc";
}

