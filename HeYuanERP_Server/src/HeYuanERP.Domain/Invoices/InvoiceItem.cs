using System;

namespace HeYuanERP.Domain.Invoices;

/// <summary>
/// 发票明细项（领域模型）。
/// </summary>
public class InvoiceItem
{
    /// <summary>
    /// 明细主键。
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 所属发票 Id（聚合关系）。
    /// </summary>
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// 物料/商品编码。
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// 物料/商品名称。
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 规格型号（可选）。
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// 单位（如：件、箱）。
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 数量（>=0）。
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// 含税单价或不含税单价（按业务口径确定）。
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 不含税金额（自动计算）。
    /// </summary>
    public decimal AmountExcludingTax { get; private set; }

    /// <summary>
    /// 税率（0~1，例如 0.13 表示 13%）。
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// 税额（自动计算）。
    /// </summary>
    public decimal TaxAmount { get; private set; }

    /// <summary>
    /// 含税金额（自动计算）。
    /// </summary>
    public decimal AmountIncludingTax { get; private set; }

    /// <summary>
    /// 排序号（行号）。
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注（可选）。
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 根据数量/单价/税率，重算金额字段。
    /// 说明：默认按不含税口径计算，税额 = 不含税金额 * 税率。
    /// </summary>
    public void Recalculate()
    {
        if (Quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(Quantity), "数量不能为负数");
        if (TaxRate < 0)
            throw new ArgumentOutOfRangeException(nameof(TaxRate), "税率不能为负数");

        var excluding = Quantity * UnitPrice;
        excluding = Math.Round(excluding, 2, MidpointRounding.AwayFromZero);

        var tax = excluding * TaxRate;
        tax = Math.Round(tax, 2, MidpointRounding.AwayFromZero);

        var including = excluding + tax;
        including = Math.Round(including, 2, MidpointRounding.AwayFromZero);

        AmountExcludingTax = excluding;
        TaxAmount = tax;
        AmountIncludingTax = including;
    }
}

