// 版权所有(c) HeYuanERP
// 说明：发票统计报表 API 控制器（中文注释）。

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HeYuanERP.Application.Reports;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers.Reports;

/// <summary>
/// 发票统计报表 API。
/// </summary>
[ApiController]
[Route("api/reports/invoice-stat")]
[Authorize]
public class InvoiceStatController : ControllerBase
{
    private readonly IReportEngine _engine;
    private readonly IReportParameterWhitelist _whitelist;
    private readonly IValidator<InvoiceStatParamsDto> _validator;

    public InvoiceStatController(IReportEngine engine, IReportParameterWhitelist whitelist, IValidator<InvoiceStatParamsDto> validator)
    {
        _engine = engine;
        _whitelist = whitelist;
        _validator = validator;
    }

    /// <summary>
    /// 查询发票统计汇总。
    /// 支持 querystring 参数：from/to/accountId/status/currency/groupBy。
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var args = _whitelist.Bind<InvoiceStatParamsDto>(ReportType.InvoiceStat, QueryToDictionary(HttpContext.Request.Query));
        var result = _validator.Validate(args);
        if (!result.IsValid)
        {
            return BadRequest(new
            {
                code = "ERR_VALIDATION",
                message = string.Join("; ", result.Errors.Select(e => e.ErrorMessage)),
                traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        var data = await _engine.InvoiceStatAsync(args, ct);
        return Ok(new
        {
            code = "OK",
            message = "success",
            data,
            traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    private static IDictionary<string, object?> QueryToDictionary(IQueryCollection query)
        => query.ToDictionary(kv => kv.Key, kv => (object?)kv.Value.ToString(), StringComparer.OrdinalIgnoreCase);
}

