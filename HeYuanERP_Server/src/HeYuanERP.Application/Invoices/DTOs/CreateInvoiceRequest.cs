using System;
using System.Collections.Generic;
using HeYuanERP.Domain.Invoices;

namespace HeYuanERP.Application.Invoices.DTOs;

/// <summary>
/// 创建发票请求 DTO（从订单/交货生成）。
/// 说明：金额由服务端根据明细数量/单价/税率自动计算。
/// 电子发票字段可选，若提供将一并保存。
/// </summary>
public class CreateInvoiceRequest
{
    /// <summary>
    /// 系统内发票号（可选；为空则由系统自动生成）。
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// 客户 Id。
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 客户名称。
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 来源类型：订单或交货。
    /// </summary>
    public InvoiceSourceType SourceType { get; set; }

    /// <summary>
    /// 来源单据 Id（可选）。
    /// </summary>
    public Guid? SourceId { get; set; }

    /// <summary>
    /// 来源单据编号（可选）。
    /// </summary>
    public string? SourceNumber { get; set; }

    /// <summary>
    /// 默认税率（0~1）；若明细税率缺省则使用此值。
    /// </summary>
    public decimal? DefaultTaxRate { get; set; }

    /// <summary>
    /// 是否电子发票。
    /// </summary>
    public bool IsElectronic { get; set; }

    /// <summary>
    /// 电子发票信息（可选）。
    /// </summary>
    public EInvoiceInfoInput? EInvoice { get; set; }

    /// <summary>
    /// 发票明细列表。
    /// </summary>
    public List<InvoiceItemDto> Items { get; set; } = new();

    /// <summary>
    /// 电子发票信息（输入模型）。
    /// </summary>
    public class EInvoiceInfoInput
    {
        public string? InvoiceCode { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CheckCode { get; set; }
        public string? PdfUrl { get; set; }
        public string? ViewUrl { get; set; }
        public string? QrCodeUrl { get; set; }
        public DateTimeOffset? ElectronicIssuedAt { get; set; }
        public string? BuyerTaxId { get; set; }
        public string? SellerTaxId { get; set; }
    }
}

