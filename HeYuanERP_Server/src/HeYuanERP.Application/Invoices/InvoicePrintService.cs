using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Printing;
using HeYuanERP.Domain.Invoices;

namespace HeYuanERP.Application.Invoices;

/// <summary>
/// 发票打印服务：构建模板数据并调用 P11 渲染 PDF。
/// </summary>
public class InvoicePrintService
{
    private readonly IInvoiceRepository _repo;
    private readonly IPrintClient _printClient;

    public const string DefaultTemplateCode = "INVOICE_STANDARD";

    public InvoicePrintService(IInvoiceRepository repo, IPrintClient printClient)
    {
        _repo = repo;
        _printClient = printClient;
    }

    /// <summary>
    /// 按发票 Id 渲染 PDF。
    /// </summary>
    /// <param name="invoiceId">发票 Id。</param>
    /// <param name="templateCode">模板代码（为空则使用默认）。</param>
    /// <param name="ct">取消令牌。</param>
    public async Task<byte[]> PrintInvoiceAsync(Guid invoiceId, string? templateCode = null, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(invoiceId, ct) ?? throw new InvalidOperationException("发票不存在");
        var model = BuildTemplateModel(entity);
        var code = string.IsNullOrWhiteSpace(templateCode) ? DefaultTemplateCode : templateCode!;
        return await _printClient.RenderPdfAsync(code, model, ct);
    }

    /// <summary>
    /// 构建模板数据模型（按 P11 约定的键名组织）。
    /// </summary>
    private static object BuildTemplateModel(Invoice entity)
    {
        var items = entity.Items
            .OrderBy(i => i.SortOrder)
            .Select(i => new
            {
                i.SortOrder,
                i.ProductCode,
                i.ProductName,
                i.Specification,
                i.Unit,
                i.Quantity,
                i.UnitPrice,
                i.TaxRate,
                i.AmountExcludingTax,
                i.TaxAmount,
                i.AmountIncludingTax,
                i.Remark
            }).ToList();

        var einv = entity.EInvoice == null ? null : new
        {
            entity.EInvoice.InvoiceCode,
            entity.EInvoice.InvoiceNumber,
            entity.EInvoice.CheckCode,
            entity.EInvoice.PdfUrl,
            entity.EInvoice.ViewUrl,
            entity.EInvoice.QrCodeUrl,
            entity.EInvoice.ElectronicIssuedAt,
            entity.EInvoice.BuyerTaxId,
            entity.EInvoice.SellerTaxId
        };

        // 模型结构：与打印模板字段对齐
        var model = new
        {
            invoice = new
            {
                entity.Id,
                entity.Number,
                entity.CustomerId,
                entity.CustomerName,
                entity.SourceType,
                entity.SourceId,
                entity.SourceNumber,
                entity.DefaultTaxRate,
                entity.SubtotalExcludingTax,
                entity.TotalTaxAmount,
                entity.GrandTotal,
                entity.Status,
                entity.IssuedAt,
                entity.IsElectronic,
                entity.Remark,
                entity.CreatedAt,
                entity.UpdatedAt
            },
            items,
            eInvoice = einv
        };

        return model;
    }
}

