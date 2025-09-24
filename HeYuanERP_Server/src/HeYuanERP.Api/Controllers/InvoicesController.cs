using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Invoices;
using HeYuanERP.Application.Invoices.DTOs;
using HeYuanERP.Domain.Invoices;
using HeYuanERP.Api.Services.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 发票接口（P8）：从订单/交货生成，金额/税率/状态，电票字段可选。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly IInvoiceRepository _repo;
    private readonly IInvoiceBusinessRuleService _rules;

    public InvoicesController(IInvoiceService service, IInvoiceRepository repo, IInvoiceBusinessRuleService rules)
    {
        _service = service;
        _repo = repo;
        _rules = rules;
    }

    /// <summary>
    /// 创建发票（从订单/交货生成）。
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> CreateAsync([FromBody] CreateInvoiceRequest request, CancellationToken ct)
    {
        // P0: 发票不可超开 – 在创建前进行业务规则校验（总额约束）
        if (request.SourceId.HasValue)
        {
            var totalAmount = CalculateRequestedTotalAmount(request);

            if (request.SourceType == InvoiceSourceType.SalesOrder)
            {
                var validateResult = await _rules.ValidateInvoiceAmountAsync(request.SourceId!.Value.ToString("D"), totalAmount, ct);
                if (!validateResult.IsValid)
                {
                    return BadRequest(ApiResponse<InvoiceResponse>.Fail(string.Join("; ", validateResult.Errors)));
                }
            }
            else if (request.SourceType == InvoiceSourceType.Delivery)
            {
                var deliveryCheck = await _rules.CanInvoiceDeliveryAsync(request.SourceId!.Value.ToString("D"), totalAmount, ct);
                if (!deliveryCheck.IsValid)
                {
                    return BadRequest(ApiResponse<InvoiceResponse>.Fail(string.Join("; ", deliveryCheck.Errors)));
                }
            }
        }

        var created = await _service.CreateAsync(request, ct);
        return Ok(ApiResponse<InvoiceResponse>.Success(created));
    }

    /// <summary>
    /// 根据 Id 获取发票。
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var data = await _service.GetByIdAsync(id, ct);
        if (data == null) return NotFound(ApiResponse<InvoiceResponse>.Fail("发票不存在"));
        return Ok(ApiResponse<InvoiceResponse>.Success(data));
    }

    /// <summary>
    /// 根据系统内发票号获取发票。
    /// </summary>
    [HttpGet("by-number/{number}")]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> GetByNumberAsync([FromRoute] string number, CancellationToken ct)
    {
        var data = await _service.GetByNumberAsync(number, ct);
        if (data == null) return NotFound(ApiResponse<InvoiceResponse>.Fail("发票不存在"));
        return Ok(ApiResponse<InvoiceResponse>.Success(data));
    }

    /// <summary>
    /// 将发票置为已开具（Issued）。
    /// </summary>
    [HttpPost("{id:guid}/issue")]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> IssueAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var data = await _service.IssueAsync(id, ct);
        return Ok(ApiResponse<InvoiceResponse>.Success(data));
    }

    /// <summary>
    /// 作废发票。
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> CancelAsync([FromRoute] Guid id, [FromBody] CancelInvoiceRequest body, CancellationToken ct)
    {
        var data = await _service.CancelAsync(id, body?.Reason, ct);
        return Ok(ApiResponse<InvoiceResponse>.Success(data));
    }

    /// <summary>
    /// 分页查询发票（列表不包含明细）。
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceResponse>>>> QueryAsync([FromQuery] QueryInvoiceRequest query, CancellationToken ct)
    {
        var (items, total) = await _repo.QueryAsync(query.Status, query.CustomerId, query.Page, query.PageSize, ct);

        // 将领域实体映射为返回 DTO（列表场景不包含 Items，降低负载）
        var list = items.Select(i => new InvoiceResponse
        {
            Id = i.Id,
            Number = i.Number,
            CustomerId = i.CustomerId,
            CustomerName = i.CustomerName,
            SourceType = i.SourceType,
            SourceId = i.SourceId,
            SourceNumber = i.SourceNumber,
            DefaultTaxRate = i.DefaultTaxRate,
            SubtotalExcludingTax = i.SubtotalExcludingTax,
            TotalTaxAmount = i.TotalTaxAmount,
            GrandTotal = i.GrandTotal,
            Status = i.Status,
            IssuedAt = i.IssuedAt,
            IsElectronic = i.IsElectronic,
            Remark = i.Remark,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            EInvoice = i.EInvoice == null ? null : new InvoiceResponse.EInvoiceInfoDto
            {
                InvoiceCode = i.EInvoice.InvoiceCode,
                InvoiceNumber = i.EInvoice.InvoiceNumber,
                CheckCode = i.EInvoice.CheckCode,
                PdfUrl = i.EInvoice.PdfUrl,
                ViewUrl = i.EInvoice.ViewUrl,
                QrCodeUrl = i.EInvoice.QrCodeUrl,
                ElectronicIssuedAt = i.EInvoice.ElectronicIssuedAt,
                BuyerTaxId = i.EInvoice.BuyerTaxId,
                SellerTaxId = i.EInvoice.SellerTaxId
            }
        }).ToList();

        var page = new PagedResult<InvoiceResponse>
        {
            Items = list,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };

        return Ok(ApiResponse<PagedResult<InvoiceResponse>>.Success(page));
    }

    /// <summary>
    /// 取消作废请求体。
    /// </summary>
    public class CancelInvoiceRequest
    {
        /// <summary>
        /// 作废原因（可选）。
        /// </summary>
        public string? Reason { get; set; }
    }

    /// <summary>
    /// 统一响应包装。
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>状态码：0 表示成功，其它为失败。</summary>
        public int Code { get; set; }
        /// <summary>提示信息。</summary>
        public string Message { get; set; } = "OK";
        /// <summary>数据载荷。</summary>
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "OK") => new ApiResponse<T> { Code = 0, Message = message, Data = data };
        public static ApiResponse<T> Fail(string message, int code = 1) => new ApiResponse<T> { Code = code, Message = message, Data = default };
    }

    /// <summary>
    /// 统一分页结果。
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>数据列表。</summary>
        public List<T> Items { get; set; } = new();
        /// <summary>总记录数。</summary>
        public int Total { get; set; }
        /// <summary>当前页码。</summary>
        public int Page { get; set; }
        /// <summary>每页条数。</summary>
        public int PageSize { get; set; }
    }
    // 控制器内辅助：计算本次请求的含税合计（与领域计算口径一致：qty*price*(1+taxRate)）
    private static decimal CalculateRequestedTotalAmount(CreateInvoiceRequest req)
    {
        if (req == null || req.Items == null || req.Items.Count == 0) return 0m;
        decimal total = 0m;
        foreach (var i in req.Items)
        {
            var rate = i.TaxRate > 0 ? i.TaxRate : (req.DefaultTaxRate ?? 0);
            var amount = i.Quantity * i.UnitPrice * (1 + rate);
            total += Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }
        return total;
    }
}
