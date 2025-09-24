// 版权所有(c) HeYuanERP
// 说明：采购订单查询报表 API 控制器（中文注释）。

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
/// 采购订单查询报表 API（分页）。
/// </summary>
[ApiController]
[Route("api/reports/po-query")]
[Authorize]
public class POQueryController : ControllerBase
{
    private readonly IReportEngine _engine;
    private readonly IReportParameterWhitelist _whitelist;
    private readonly IValidator<POQueryParamsDto> _validator;

    public POQueryController(IReportEngine engine, IReportParameterWhitelist whitelist, IValidator<POQueryParamsDto> validator)
    {
        _engine = engine;
        _whitelist = whitelist;
        _validator = validator;
    }

    /// <summary>
    /// 分页查询采购订单汇总（报表视图）。
    /// 支持 querystring 参数：from/to/vendorId/status/page/size。
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var args = _whitelist.Bind<POQueryParamsDto>(ReportType.POQuery, QueryToDictionary(HttpContext.Request.Query));
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

        var data = await _engine.POQueryAsync(args, ct);
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

