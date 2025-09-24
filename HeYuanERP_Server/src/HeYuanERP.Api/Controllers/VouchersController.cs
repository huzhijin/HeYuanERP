using System;
using System.Threading.Tasks;
using HeYuanERP.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 凭证接口桩（后续对接财务系统/总账模块）。
/// 现阶段返回占位响应，便于前端联调。
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VouchersController : ControllerBase
{
    private readonly ILogger<VouchersController> _logger;

    public VouchersController(ILogger<VouchersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 根据收款记录生成凭证（占位）。
    /// </summary>
    [HttpPost("generate-from-payment/{paymentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<object?>> GenerateFromPayment([FromRoute] Guid paymentId)
    {
        _logger.LogInformation("收到凭证生成功能调用（桩）：paymentId={PaymentId}", paymentId);
        var data = new
        {
            voucherId = Guid.NewGuid(),
            status = "pending",
            message = "凭证生成功能待对接财务系统（占位返回）"
        };
        return Ok(ApiResponse<object?>.Ok(data, "已接收凭证生成请求（桩）"));
    }

    /// <summary>
    /// 获取凭证详情（占位）。
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<object?>> GetVoucher([FromRoute] Guid id)
    {
        var data = new
        {
            id,
            status = "pending",
            message = "凭证查询功能待接入（占位返回）"
        };
        return Ok(ApiResponse<object?>.Ok(data, "查询成功（桩）"));
    }
}

