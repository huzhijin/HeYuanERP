// 版权所有(c) HeYuanERP
// 说明：库存报表 API 控制器（汇总与交易，中文注释）。

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
/// 库存报表 API（汇总与交易）。
/// </summary>
[ApiController]
[Route("api/reports/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IReportEngine _engine;
    private readonly IReportParameterWhitelist _whitelist;
    private readonly IValidator<InventoryQueryParamsDto> _validator;

    public InventoryController(IReportEngine engine, IReportParameterWhitelist whitelist, IValidator<InventoryQueryParamsDto> validator)
    {
        _engine = engine;
        _whitelist = whitelist;
        _validator = validator;
    }

    /// <summary>
    /// 查询库存汇总（不分页）。
    /// 支持 querystring 参数：productId/whse/loc。
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SummaryAsync(CancellationToken ct)
    {
        var args = _whitelist.Bind<InventoryQueryParamsDto>(ReportType.Inventory, QueryToDictionary(HttpContext.Request.Query));
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

        var data = await _engine.InventorySummaryAsync(args, ct);
        return Ok(new
        {
            code = "OK",
            message = "success",
            data,
            traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    /// <summary>
    /// 查询库存交易明细（分页）。
    /// 支持 querystring 参数：productId/whse/loc/from/to/page/size。
    /// </summary>
    [HttpGet("transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TransactionsAsync(CancellationToken ct)
    {
        var args = _whitelist.Bind<InventoryQueryParamsDto>(ReportType.Inventory, QueryToDictionary(HttpContext.Request.Query));
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

        var data = await _engine.InventoryTransactionsAsync(args, ct);
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

