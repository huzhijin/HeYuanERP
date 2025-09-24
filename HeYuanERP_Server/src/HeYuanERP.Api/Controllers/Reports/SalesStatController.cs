// 版权所有(c) HeYuanERP
// 说明：销售统计报表 API 控制器（中文注释）。

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
/// 销售统计报表 API。
/// </summary>
[ApiController]
[Route("api/reports/sales-stat")]
[Authorize]
public class SalesStatController : ControllerBase
{
    private readonly IReportEngine _engine;
    private readonly IReportParameterWhitelist _whitelist;
    private readonly IValidator<SalesStatParamsDto> _validator;

    public SalesStatController(IReportEngine engine, IReportParameterWhitelist whitelist, IValidator<SalesStatParamsDto> validator)
    {
        _engine = engine;
        _whitelist = whitelist;
        _validator = validator;
    }

    /// <summary>
    /// 查询销售统计汇总。
    /// 支持 querystring 参数：from/to/customerId/salesmanId/productId/currency/groupBy。
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var args = _whitelist.Bind<SalesStatParamsDto>(ReportType.SalesStat, QueryToDictionary(HttpContext.Request.Query));
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

        var data = await _engine.SalesStatAsync(args, ct);
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

