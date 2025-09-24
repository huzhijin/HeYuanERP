// 版权所有(c) HeYuanERP
// 说明：库存报表 DTO（与 OpenAPI InventorySummary/InventoryTxn 对齐，中文注释）。

using System;
using HeYuanERP.Contracts.Common;

namespace HeYuanERP.Contracts.Reports;

/// <summary>
/// 库存汇总项（OpenAPI: InventorySummary）。
/// </summary>
public sealed class InventorySummaryDto
{
    /// <summary>产品 Id</summary>
    public string ProductId { get; init; } = string.Empty;

    /// <summary>仓库编码</summary>
    public string Whse { get; init; } = string.Empty;

    /// <summary>库位</summary>
    public string Loc { get; init; } = string.Empty;

    /// <summary>在手数量</summary>
    public decimal OnHand { get; init; }

    /// <summary>预留数量</summary>
    public decimal Reserved { get; init; }

    /// <summary>可用数量</summary>
    public decimal Available { get; init; }
}

/// <summary>
/// 库存交易明细（OpenAPI: InventoryTxn）。
/// </summary>
public sealed class InventoryTxnDto
{
    /// <summary>交易 Id</summary>
    public string TxnId { get; init; } = string.Empty;

    /// <summary>交易代码：IN|OUT|DELIVERY|RETURN|PORECEIVE|ADJ</summary>
    public string TxnCode { get; init; } = string.Empty;

    /// <summary>产品 Id</summary>
    public string ProductId { get; init; } = string.Empty;

    /// <summary>数量（正负）</summary>
    public decimal Qty { get; init; }

    /// <summary>仓库编码</summary>
    public string Whse { get; init; } = string.Empty;

    /// <summary>库位</summary>
    public string Loc { get; init; } = string.Empty;

    /// <summary>交易时间（UTC）</summary>
    public DateTimeOffset TxnDate { get; init; }

    /// <summary>来源类型（如: order/delivery/po/adjustment）</summary>
    public string? RefType { get; init; }

    /// <summary>来源 Id</summary>
    public string? RefId { get; init; }
}

/// <summary>
/// 库存查询参数。
/// </summary>
public sealed class InventoryQueryParamsDto
{
    /// <summary>产品 Id（可选）。</summary>
    public string? ProductId { get; init; }

    /// <summary>仓库编码（可选）。</summary>
    public string? Whse { get; init; }

    /// <summary>库位（可选）。</summary>
    public string? Loc { get; init; }

    /// <summary>时间范围（仅用于交易查询，可选）。</summary>
    public DateRange? Range { get; init; }

    /// <summary>页码（交易分页）。</summary>
    public int Page { get; init; } = 1;

    /// <summary>每页大小（交易分页）。</summary>
    public int Size { get; init; } = 20;
}

