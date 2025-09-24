using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Api.Requests.Payments;
using HeYuanERP.Application.Common.Models;
using HeYuanERP.Application.DTOs.Payments;
using HeYuanERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 收款相关接口：列表、创建、详情。
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService service, ILogger<PaymentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 分页获取收款列表。
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentListItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentListItemDto>>>> ListAsync([FromQuery] PaymentListQuery query)
    {
        query.NormalizeFilters();
        var req = new PaymentListParams
        {
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = query.SortBy,
            SortOrder = query.SortOrder,
            Method = query.Method,
            MinAmount = query.MinAmount,
            MaxAmount = query.MaxAmount,
            DateFrom = query.DateFrom,
            DateTo = query.DateTo,
            Keyword = query.Keyword
        };

        var resp = await _service.ListAsync(req, HttpContext.RequestAborted);
        return Ok(resp);
    }

    /// <summary>
    /// 创建收款记录（multipart/form-data，支持多附件）。
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDetailDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentDetailDto>>> CreateAsync([FromForm] PaymentCreateForm form)
    {
        var ct = HttpContext.RequestAborted;
        var dto = form.ToDto();

        // 处理附件流并在调用后释放
        var openedStreams = new List<Stream>();
        var uploads = new List<AttachmentUpload>();
        foreach (var f in form.Attachments ?? new List<IFormFile>())
        {
            var s = f.OpenReadStream();
            openedStreams.Add(s);
            uploads.Add(new AttachmentUpload(s, f.FileName, f.ContentType ?? "application/octet-stream", f.Length));
        }

        var createdBy = User?.Identity?.Name;
        var orgId = Request.Headers.ContainsKey("X-Org-Id") ? Request.Headers["X-Org-Id"].ToString() : null;

        ApiResponse<PaymentDetailDto> resp;
        try
        {
            resp = await _service.CreateAsync(dto, uploads, createdBy, orgId, ct);
        }
        finally
        {
            foreach (var s in openedStreams) s.Dispose();
        }

        return Ok(resp);
    }

    /// <summary>
    /// 获取收款详情。
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDetailDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentDetailDto>>> GetAsync([FromRoute] Guid id)
    {
        var resp = await _service.GetAsync(id, HttpContext.RequestAborted);
        return Ok(resp);
    }
}

