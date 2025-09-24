// 版权所有(c) HeYuanERP
// 说明：报表快照查询 API 控制器（中文注释）。

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Reports.Snapshots;
using HeYuanERP.Domain.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers.Reports;

/// <summary>
/// 报表快照 API：用于按 Id 或参数哈希查询快照记录。
/// </summary>
[ApiController]
[Route("api/reports/snapshots")]
[Authorize]
public class SnapshotController : ControllerBase
{
    private readonly ReportSnapshotService _snapshots;

    public SnapshotController(ReportSnapshotService snapshots)
    {
        _snapshots = snapshots;
    }

    /// <summary>
    /// 通过快照 Id 获取快照。
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken ct)
    {
        if (!Guid.TryParse(id, out var gid))
        {
            return NotFound(new { code = "NOT_FOUND", message = "快照不存在", traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        var snapshot = await _snapshots.FindAsync(gid, ct);
        if (snapshot == null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "快照不存在", traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        return Ok(new
        {
            code = "OK",
            message = "success",
            data = snapshot,
            traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    /// <summary>
    /// 按类型与参数哈希查询最近一次快照。
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByHashAsync([FromQuery] string type, [FromQuery] string paramHash, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(paramHash))
        {
            return NotFound(new { code = "NOT_FOUND", message = "缺少必要参数 type 或 paramHash", traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        if (!TryParseType(type, out var rtype))
        {
            return NotFound(new { code = "NOT_FOUND", message = "不支持的报表类型", traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        var snapshot = await _snapshots.FindByHashAsync(rtype, paramHash, ct);
        if (snapshot == null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "快照不存在", traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        return Ok(new
        {
            code = "OK",
            message = "success",
            data = snapshot,
            traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    private static bool TryParseType(string name, out ReportType type)
    {
        var key = name.Trim().ToLowerInvariant();
        switch (key)
        {
            case "sales" or "salesstat" or "sales-stat":
                type = ReportType.SalesStat; return true;
            case "invoice" or "invoicestat" or "invoice-stat":
                type = ReportType.InvoiceStat; return true;
            case "po" or "poquery" or "po-query":
                type = ReportType.POQuery; return true;
            case "inventory":
                type = ReportType.Inventory; return true;
            default:
                type = default; return false;
        }
    }
}

