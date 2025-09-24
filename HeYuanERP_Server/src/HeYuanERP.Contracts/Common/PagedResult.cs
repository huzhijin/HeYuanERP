// 版权所有(c) HeYuanERP
// 说明：统一分页返回模型（中文注释）。

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeYuanERP.Contracts.Common;

/// <summary>
/// 统一分页结果模型（与 OpenAPI 分页口径对齐）。
/// </summary>
/// <typeparam name="T">列表项数据类型</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// 当前页码（从 1 开始）。
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// 每页大小。
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// 总记录数。
    /// </summary>
    public long Total { get; init; }

    /// <summary>
    /// 总页数（根据 Total 与 PageSize 计算）。
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// 是否有上一页。
    /// </summary>
    public bool HasPrevious { get; init; }

    /// <summary>
    /// 是否有下一页。
    /// </summary>
    public bool HasNext { get; init; }

    /// <summary>
    /// 数据列表。
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    private PagedResult() { }

    /// <summary>
    /// 构建分页结果。
    /// </summary>
    public static PagedResult<T> Create(IEnumerable<T> source, int page, int pageSize, long total)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "页码必须从 1 开始。");
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "每页大小必须大于 0。");

        var items = source?.ToList() ?? new List<T>();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PagedResult<T>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            TotalPages = totalPages,
            HasPrevious = page > 1,
            HasNext = page < totalPages,
            Items = items
        };
    }
}

