using System;
using System.Collections.Generic;
using System.Linq;

namespace HeYuanERP.Domain.Invoices;

/// <summary>
/// 发票聚合根（领域模型）。
/// - 支持从销售订单/交货生成；
/// - 包含金额/税率/状态等核心字段；
/// - 电子发票字段可选（见 <see cref="EInvoiceInfo"/>）。
/// </summary>
public class Invoice
{
    /// <summary>
    /// 发票主键。
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 发票号（系统内编号，非税票号码）。
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// 客户 Id。
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 客户名称（可冗余保存以便打印）。
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 业务来源类型（订单/交货）。
    /// </summary>
    public InvoiceSourceType SourceType { get; set; }

    /// <summary>
    /// 来源单据 Id（例如订单或交货单）。
    /// </summary>
    public Guid? SourceId { get; set; }

    /// <summary>
    /// 来源单据编号。
    /// </summary>
    public string? SourceNumber { get; set; }

    /// <summary>
    /// 默认税率（0~1），可被明细覆盖。
    /// </summary>
    public decimal? DefaultTaxRate { get; set; }

    /// <summary>
    /// 不含税合计金额（自动计算）。
    /// </summary>
    public decimal SubtotalExcludingTax { get; private set; }

    /// <summary>
    /// 税额合计（自动计算）。
    /// </summary>
    public decimal TotalTaxAmount { get; private set; }

    /// <summary>
    /// 含税合计金额（自动计算）。
    /// </summary>
    public decimal GrandTotal { get; private set; }

    /// <summary>
    /// 发票状态。
    /// </summary>
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;

    /// <summary>
    /// 开票日期（实际开具时写入）。
    /// </summary>
    public DateTimeOffset? IssuedAt { get; private set; }

    /// <summary>
    /// 是否电子发票。
    /// </summary>
    public bool IsElectronic { get; set; }

    /// <summary>
    /// 电子发票信息（可选）。
    /// </summary>
    public EInvoiceInfo? EInvoice { get; set; }

    /// <summary>
    /// 备注。
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间。
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 最后更新时间。
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; private set; }

    /// <summary>
    /// 发票明细集合。
    /// </summary>
    public List<InvoiceItem> Items { get; private set; } = new();

    /// <summary>
    /// 添加明细并按需套用默认税率。
    /// </summary>
    public void AddItem(InvoiceItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        if (DefaultTaxRate.HasValue && item.TaxRate <= 0)
        {
            item.TaxRate = DefaultTaxRate.Value;
        }
        item.InvoiceId = Id;
        item.Recalculate();
        Items.Add(item);
        RecalculateTotals();
        Touch();
    }

    /// <summary>
    /// 批量替换明细（用于从订单/交货生成）。
    /// </summary>
    public void ReplaceItems(IEnumerable<InvoiceItem> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        Items.Clear();
        foreach (var it in items)
        {
            var item = it;
            if (DefaultTaxRate.HasValue && item.TaxRate <= 0)
            {
                item.TaxRate = DefaultTaxRate.Value;
            }
            item.InvoiceId = Id;
            item.Recalculate();
            Items.Add(item);
        }
        RecalculateTotals();
        Touch();
    }

    /// <summary>
    /// 重算整单金额合计（不含税、税额、含税）。
    /// </summary>
    public void RecalculateTotals()
    {
        foreach (var item in Items)
        {
            item.Recalculate();
        }

        SubtotalExcludingTax = Math.Round(Items.Sum(i => i.AmountExcludingTax), 2, MidpointRounding.AwayFromZero);
        TotalTaxAmount = Math.Round(Items.Sum(i => i.TaxAmount), 2, MidpointRounding.AwayFromZero);
        GrandTotal = Math.Round(SubtotalExcludingTax + TotalTaxAmount, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 将发票状态置为“待开具”。
    /// </summary>
    public void MarkPending()
    {
        Status = InvoiceStatus.Pending;
        Touch();
    }

    /// <summary>
    /// 开具发票（状态改为 Issued 并记录时间）。
    /// </summary>
    public void Issue()
    {
        if (Status == InvoiceStatus.Canceled)
            throw new InvalidOperationException("作废单据不可再次开具");
        Status = InvoiceStatus.Issued;
        IssuedAt = DateTimeOffset.UtcNow;
        Touch();
    }

    /// <summary>
    /// 作废发票（仅允许已开具或待开具的单据被作废）。
    /// </summary>
    public void Cancel(string? reason = null)
    {
        if (Status == InvoiceStatus.Draft)
            throw new InvalidOperationException("草稿不可直接作废，请先取消或删除");
        Status = InvoiceStatus.Canceled;
        Remark = string.IsNullOrWhiteSpace(reason) ? Remark : $"{Remark}\n作废原因：{reason}".Trim();
        Touch();
    }

    /// <summary>
    /// 轻量更新乐观字段。
    /// </summary>
    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

