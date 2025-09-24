using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Invoices.DTOs;
using HeYuanERP.Domain.Invoices;

namespace HeYuanERP.Application.Invoices;

/// <summary>
/// 发票应用服务实现。
/// 负责：
/// - 从订单/交货创建发票，计算金额与税额；
/// - 电子发票信息可选；
/// - 状态流转（待开具、已开具、作废）。
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repo;

    public InvoiceService(IInvoiceRepository repo)
    {
        _repo = repo;
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (request.Items == null || request.Items.Count == 0)
            throw new ArgumentException("发票明细不能为空", nameof(request.Items));

        var invoice = new Invoice
        {
            Number = string.IsNullOrWhiteSpace(request.Number) ? GenerateInvoiceNumber() : request.Number!,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            SourceType = request.SourceType,
            SourceId = request.SourceId,
            SourceNumber = request.SourceNumber,
            DefaultTaxRate = request.DefaultTaxRate,
            IsElectronic = request.IsElectronic,
            Remark = null
        };

        if (request.IsElectronic && request.EInvoice != null)
        {
            invoice.EInvoice = new EInvoiceInfo
            {
                InvoiceCode = request.EInvoice.InvoiceCode,
                InvoiceNumber = request.EInvoice.InvoiceNumber,
                CheckCode = request.EInvoice.CheckCode,
                PdfUrl = request.EInvoice.PdfUrl,
                ViewUrl = request.EInvoice.ViewUrl,
                QrCodeUrl = request.EInvoice.QrCodeUrl,
                ElectronicIssuedAt = request.EInvoice.ElectronicIssuedAt,
                BuyerTaxId = request.EInvoice.BuyerTaxId,
                SellerTaxId = request.EInvoice.SellerTaxId
            };
        }

        var items = new List<InvoiceItem>(request.Items.Count);
        foreach (var dto in request.Items.OrderBy(i => i.SortOrder))
        {
            var item = new InvoiceItem
            {
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                Specification = dto.Specification,
                Unit = dto.Unit,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TaxRate = dto.TaxRate,
                SortOrder = dto.SortOrder,
                Remark = dto.Remark
            };
            items.Add(item);
        }

        invoice.ReplaceItems(items);

        // 新生成的发票缺省置为“待开具”。
        invoice.MarkPending();

        await _repo.AddAsync(invoice, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return ToResponse(invoice);
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        return entity == null ? null : ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByNumberAsync(number, cancellationToken);
        return entity == null ? null : ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse> IssueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("发票不存在");
        entity.Issue();
        _repo.Update(entity);
        await _repo.SaveChangesAsync(cancellationToken);
        return ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse> CancelAsync(Guid id, string? reason = null, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("发票不存在");
        entity.Cancel(reason);
        _repo.Update(entity);
        await _repo.SaveChangesAsync(cancellationToken);
        return ToResponse(entity);
    }

    /// <summary>
    /// 将领域模型映射为返回 DTO。
    /// </summary>
    private static InvoiceResponse ToResponse(Invoice entity)
    {
        var resp = new InvoiceResponse
        {
            Id = entity.Id,
            Number = entity.Number,
            CustomerId = entity.CustomerId,
            CustomerName = entity.CustomerName,
            SourceType = entity.SourceType,
            SourceId = entity.SourceId,
            SourceNumber = entity.SourceNumber,
            DefaultTaxRate = entity.DefaultTaxRate,
            SubtotalExcludingTax = entity.SubtotalExcludingTax,
            TotalTaxAmount = entity.TotalTaxAmount,
            GrandTotal = entity.GrandTotal,
            Status = entity.Status,
            IssuedAt = entity.IssuedAt,
            IsElectronic = entity.IsElectronic,
            Remark = entity.Remark,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        if (entity.EInvoice != null)
        {
            resp.EInvoice = new InvoiceResponse.EInvoiceInfoDto
            {
                InvoiceCode = entity.EInvoice.InvoiceCode,
                InvoiceNumber = entity.EInvoice.InvoiceNumber,
                CheckCode = entity.EInvoice.CheckCode,
                PdfUrl = entity.EInvoice.PdfUrl,
                ViewUrl = entity.EInvoice.ViewUrl,
                QrCodeUrl = entity.EInvoice.QrCodeUrl,
                ElectronicIssuedAt = entity.EInvoice.ElectronicIssuedAt,
                BuyerTaxId = entity.EInvoice.BuyerTaxId,
                SellerTaxId = entity.EInvoice.SellerTaxId
            };
        }

        foreach (var it in entity.Items.OrderBy(i => i.SortOrder))
        {
            resp.Items.Add(new InvoiceItemDto
            {
                Id = it.Id,
                ProductCode = it.ProductCode,
                ProductName = it.ProductName,
                Specification = it.Specification,
                Unit = it.Unit,
                Quantity = it.Quantity,
                UnitPrice = it.UnitPrice,
                TaxRate = it.TaxRate,
                SortOrder = it.SortOrder,
                Remark = it.Remark,
                AmountExcludingTax = it.AmountExcludingTax,
                TaxAmount = it.TaxAmount,
                AmountIncludingTax = it.AmountIncludingTax
            });
        }

        return resp;
    }

    /// <summary>
    /// 生成系统内发票号（可替换为领域服务/编号生成器）。
    /// </summary>
    private static string GenerateInvoiceNumber() => $"INV{DateTime.UtcNow:yyyyMMddHHmmssfff}";
}

