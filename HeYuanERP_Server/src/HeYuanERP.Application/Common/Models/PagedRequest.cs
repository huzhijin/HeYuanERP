namespace HeYuanERP.Application.Common.Models;

/// <summary>
/// 分页请求模型。
/// 约定：页码从 1 开始；默认每页 20；最大每页 200。
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// 页码（1 开始）。
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页大小（默认 20，最大 200）。
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段（可选）。
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// 排序方向：asc/desc（默认 desc）。
    /// </summary>
    public string? SortOrder { get; set; } = "desc";

    /// <summary>
    /// 跳过条数（用于数据库分页 Skip）。
    /// </summary>
    public int Skip => Page <= 1 ? 0 : (Page - 1) * PageSize;

    /// <summary>
    /// 取数条数（用于数据库分页 Take）。
    /// </summary>
    public int Take => PageSize;

    /// <summary>
    /// 规范化分页参数（限制 Page 与 PageSize 合理范围）。
    /// </summary>
    /// <param name="maxPageSize">允许的最大每页大小（默认 200）</param>
    /// <param name="defaultPageSize">默认每页大小（默认 20）</param>
    public void Normalize(int maxPageSize = 200, int defaultPageSize = 20)
    {
        if (Page < 1) Page = 1;
        if (PageSize <= 0) PageSize = defaultPageSize;
        if (PageSize > maxPageSize) PageSize = maxPageSize;

        // 规范化排序方向
        if (!string.IsNullOrWhiteSpace(SortOrder))
        {
            var so = SortOrder.Trim().ToLowerInvariant();
            SortOrder = so == "asc" ? "asc" : "desc";
        }
        else
        {
            SortOrder = "desc";
        }
    }
}

