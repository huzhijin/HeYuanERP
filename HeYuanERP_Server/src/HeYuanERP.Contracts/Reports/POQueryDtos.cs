// 版权所有(c) HeYuanERP
// 说明：采购订单查询报表 DTO（中文注释）。

using System;
using HeYuanERP.Contracts.Common;

namespace HeYuanERP.Contracts.Reports;

/// <summary>
/// 采购订单查询参数（与通用列表接口口径接近，用于报表导出）。
/// </summary>
public sealed class POQueryParamsDto
{
    /// <summary>时间范围（UTC，可选）。</summary>
    public DateRange? Range { get; init; }

    /// <summary>供应商 Id（可选）。</summary>
    public string? VendorId { get; init; }

    /// <summary>状态（可选）。</summary>
    public string? Status { get; init; }

    /// <summary>页码（用于列表/导出预览）。</summary>
    public int Page { get; init; } = 1;

    /// <summary>每页大小。</summary>
    public int Size { get; init; } = 20;
}

/// <summary>
/// 采购订单列表项（用于报表输出/预览）。
/// </summary>
public sealed class POListItemDto
{
    /// <summary>主键</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>采购单号</summary>
    public string PoNo { get; init; } = string.Empty;

    /// <summary>业务日期（本地日期）。</summary>
    public DateOnly Date { get; init; }

    /// <summary>供应商 Id</summary>
    public string VendorId { get; init; } = string.Empty;

    /// <summary>金额（含税或不含税视业务口径）。</summary>
    public decimal Amount { get; init; }

    /// <summary>状态</summary>
    public string Status { get; init; } = string.Empty;
}

