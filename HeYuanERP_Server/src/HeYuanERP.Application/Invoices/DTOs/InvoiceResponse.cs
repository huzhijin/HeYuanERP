using System;
using System.Collections.Generic;
using HeYuanERP.Domain.Invoices;

namespace HeYuanERP.Application.Invoices.DTOs;

/// <summary>
/// 发票返回 DTO。
/// </summary>
public class InvoiceResponse
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public InvoiceSourceType SourceType { get; set; }
    public Guid? SourceId { get; set; }
    public string? SourceNumber { get; set; }
    public decimal? DefaultTaxRate { get; set; }
    public decimal SubtotalExcludingTax { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
    public bool IsElectronic { get; set; }
    public EInvoiceInfoDto? EInvoice { get; set; }
    public string? Remark { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();

    /// <summary>
    /// 电子发票返回信息。
    /// </summary>
    public class EInvoiceInfoDto
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

