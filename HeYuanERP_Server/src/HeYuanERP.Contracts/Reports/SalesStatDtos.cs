// 版权所有(c) HeYuanERP
// 说明：销售统计报表 DTO（中文注释）。

using System;
using System.Collections.Generic;
using HeYuanERP.Contracts.Common;

namespace HeYuanERP.Contracts.Reports;

/// <summary>
/// 销售统计报表查询参数。
/// </summary>
public sealed class SalesStatParamsDto
{
    /// <summary>时间范围（UTC，可选）。</summary>
    public DateRange? Range { get; init; }

    /// <summary>客户 Id（可选）。</summary>
    public string? CustomerId { get; init; }

    /// <summary>业务员 Id（可选）。</summary>
    public string? SalesmanId { get; init; }

    /// <summary>产品 Id（可选）。</summary>
    public string? ProductId { get; init; }

    /// <summary>币种（可选，如 CNY/USD）。</summary>
    public string? Currency { get; init; }

    /// <summary>
    /// 分组维度（可选）：day|month|product|salesman|customer。
    /// 若为空，默认按 day 聚合。
    /// </summary>
    public string? GroupBy { get; init; }
}

/// <summary>
/// 销售统计项（按维度聚合）。
/// </summary>
public sealed class SalesStatItemDto
{
    /// <summary>维度键：如日期(yyyy-MM-dd/yyyy-MM)、产品 Id、业务员 Id、客户 Id。</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>维度名称（可选，如产品名/业务员名/客户名）。</summary>
    public string? Name { get; init; }

    /// <summary>订单数。</summary>
    public int OrderCount { get; init; }

    /// <summary>销售数量合计。</summary>
    public decimal TotalQty { get; init; }

    /// <summary>不含税金额合计。</summary>
    public decimal Subtotal { get; init; }

    /// <summary>税额合计。</summary>
    public decimal Tax { get; init; }

    /// <summary>含税金额合计。</summary>
    public decimal TotalAmount { get; init; }
}

/// <summary>
/// 销售统计汇总。
/// </summary>
public sealed class SalesStatSummaryDto
{
    /// <summary>明细列表（可分页或一次性返回）。</summary>
    public IReadOnlyList<SalesStatItemDto> Items { get; init; } = Array.Empty<SalesStatItemDto>();

    /// <summary>数量总计。</summary>
    public decimal TotalQty { get; init; }

    /// <summary>不含税总计。</summary>
    public decimal Subtotal { get; init; }

    /// <summary>税额总计。</summary>
    public decimal Tax { get; init; }

    /// <summary>含税总计。</summary>
    public decimal TotalAmount { get; init; }
}

