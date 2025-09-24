using System;

namespace HeYuanERP.Application.Invoices.DTOs;

/// <summary>
/// 发票明细 DTO（用于创建与返回）。
/// 注意：创建时仅需提供商品信息、数量、单价、税率等；
/// 金额字段在返回时由后端计算回填。
/// </summary>
public class InvoiceItemDto
{
    /// <summary>
    /// 明细 Id（返回时提供）。
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 商品编码。
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称。
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 规格型号（可选）。
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// 单位（可选）。
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 数量（>=0）。
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// 单价。
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 税率（0~1）。
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 排序号（行号）。
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注（可选）。
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 不含税金额（返回时回填）。
    /// </summary>
    public decimal? AmountExcludingTax { get; set; }

    /// <summary>
    /// 税额（返回时回填）。
    /// </summary>
    public decimal? TaxAmount { get; set; }

    /// <summary>
    /// 含税金额（返回时回填）。
    /// </summary>
    public decimal? AmountIncludingTax { get; set; }
}

