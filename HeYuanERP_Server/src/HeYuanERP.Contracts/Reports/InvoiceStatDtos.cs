// 版权所有(c) HeYuanERP
// 说明：发票统计报表 DTO（中文注释）。

using System;
using System.Collections.Generic;
using HeYuanERP.Contracts.Common;

namespace HeYuanERP.Contracts.Reports;

/// <summary>
/// 发票统计报表查询参数。
/// </summary>
public sealed class InvoiceStatParamsDto
{
    /// <summary>时间范围（UTC，可选）。</summary>
    public DateRange? Range { get; init; }

    /// <summary>往来户 Id（可选）。</summary>
    public string? AccountId { get; init; }

    /// <summary>发票状态（可选）。</summary>
    public string? Status { get; init; }

    /// <summary>币种（可选，如 CNY/USD）。</summary>
    public string? Currency { get; init; }

    /// <summary>
    /// 分组维度（可选）：day|month|account。
    /// 若为空，默认按 day 聚合。
    /// </summary>
    public string? GroupBy { get; init; }
}

/// <summary>
/// 发票统计项（按维度聚合）。
/// </summary>
public sealed class InvoiceStatItemDto
{
    /// <summary>维度键：如日期(yyyy-MM-dd/yyyy-MM)或往来户 Id。</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>维度名称（可选，如客户名）。</summary>
    public string? Name { get; init; }

    /// <summary>发票数量。</summary>
    public int InvoiceCount { get; init; }

    /// <summary>不含税金额合计。</summary>
    public decimal Amount { get; init; }

    /// <summary>税额合计。</summary>
    public decimal Tax { get; init; }

    /// <summary>含税金额合计。</summary>
    public decimal AmountWithTax { get; init; }
}

/// <summary>
/// 发票统计汇总。
/// </summary>
public sealed class InvoiceStatSummaryDto
{
    /// <summary>明细列表。</summary>
    public IReadOnlyList<InvoiceStatItemDto> Items { get; init; } = Array.Empty<InvoiceStatItemDto>();

    /// <summary>不含税总计。</summary>
    public decimal Amount { get; init; }

    /// <summary>税额总计。</summary>
    public decimal Tax { get; init; }

    /// <summary>含税总计。</summary>
    public decimal AmountWithTax { get; init; }
}

