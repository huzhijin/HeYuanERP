using System;
using System.Collections.Generic;

namespace HeYuanERP.Application.Common.Models;

/// <summary>
/// 分页结果模型。
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 当前页数据项集合。
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// 总记录数。
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// 当前页码（1 开始）。
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页大小。
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数（向上取整）。
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否存在上一页。
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// 是否存在下一页。
    /// </summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// 创建一个分页结果。
    /// </summary>
    public static PagedResult<T> Create(IEnumerable<T> items, long totalCount, int page, int pageSize)
        => new()
        {
            Items = items is IReadOnlyList<T> ro ? ro : new List<T>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

    /// <summary>
    /// 空分页结果（通常用于无数据场景）。
    /// </summary>
    public static PagedResult<T> Empty(int page, int pageSize)
        => new() { Items = Array.Empty<T>(), TotalCount = 0, Page = page, PageSize = pageSize };
}

