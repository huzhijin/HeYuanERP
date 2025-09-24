using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Services.Reconciliation;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.Services.Authorization;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReconciliationController : ControllerBase
{
    private readonly IReconciliationService _reconciliationService;

    public ReconciliationController(IReconciliationService reconciliationService)
    {
        _reconciliationService = reconciliationService;
    }

    // 对账记录管理
    [HttpPost("records")]
    [RequirePermission("Reconciliation.Record.Create")]
    public async Task<IActionResult> CreateReconciliationRecord([FromBody] ReconciliationRecord record)
    {
        var result = await _reconciliationService.CreateReconciliationRecordAsync(record);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("records/{id}")]
    [RequirePermission("Reconciliation.Record.Update")]
    public async Task<IActionResult> UpdateReconciliationRecord(int id, [FromBody] ReconciliationRecord record)
    {
        if (id != record.Id)
            return BadRequest("ID不匹配");

        var result = await _reconciliationService.UpdateReconciliationRecordAsync(record);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("records/{id}")]
    [RequirePermission("Reconciliation.Record.Delete")]
    public async Task<IActionResult> DeleteReconciliationRecord(int id)
    {
        var result = await _reconciliationService.DeleteReconciliationRecordAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("records/{id}")]
    [RequirePermission("Reconciliation.Record.View")]
    public async Task<IActionResult> GetReconciliationRecordById(int id)
    {
        var result = await _reconciliationService.GetReconciliationRecordByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("records")]
    [RequirePermission("Reconciliation.Record.View")]
    public async Task<IActionResult> GetReconciliationRecords([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _reconciliationService.GetReconciliationRecordsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("records/by-type/{type}")]
    [RequirePermission("Reconciliation.Record.View")]
    public async Task<IActionResult> GetReconciliationRecordsByType(string type)
    {
        var result = await _reconciliationService.GetReconciliationRecordsByTypeAsync(type);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("records/by-status/{status}")]
    [RequirePermission("Reconciliation.Record.View")]
    public async Task<IActionResult> GetReconciliationRecordsByStatus(string status)
    {
        var result = await _reconciliationService.GetReconciliationRecordsByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("records/{id}/complete")]
    [RequirePermission("Reconciliation.Record.Complete")]
    public async Task<IActionResult> CompleteReconciliation(int id)
    {
        var result = await _reconciliationService.CompleteReconciliationAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 对账项目管理
    [HttpPost("items")]
    [RequirePermission("Reconciliation.Item.Create")]
    public async Task<IActionResult> CreateReconciliationItem([FromBody] ReconciliationItem item)
    {
        var result = await _reconciliationService.CreateReconciliationItemAsync(item);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("items/{id}")]
    [RequirePermission("Reconciliation.Item.Update")]
    public async Task<IActionResult> UpdateReconciliationItem(int id, [FromBody] ReconciliationItem item)
    {
        if (id != item.Id)
            return BadRequest("ID不匹配");

        var result = await _reconciliationService.UpdateReconciliationItemAsync(item);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("items/{id}")]
    [RequirePermission("Reconciliation.Item.Delete")]
    public async Task<IActionResult> DeleteReconciliationItem(int id)
    {
        var result = await _reconciliationService.DeleteReconciliationItemAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("items/{id}")]
    [RequirePermission("Reconciliation.Item.View")]
    public async Task<IActionResult> GetReconciliationItemById(int id)
    {
        var result = await _reconciliationService.GetReconciliationItemByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("items/by-record/{recordId}")]
    [RequirePermission("Reconciliation.Item.View")]
    public async Task<IActionResult> GetReconciliationItemsByRecord(int recordId)
    {
        var result = await _reconciliationService.GetReconciliationItemsByRecordAsync(recordId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("items/unmatched/{recordId}")]
    [RequirePermission("Reconciliation.Item.View")]
    public async Task<IActionResult> GetUnmatchedReconciliationItems(int recordId)
    {
        var result = await _reconciliationService.GetUnmatchedReconciliationItemsAsync(recordId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("items/{id}/match")]
    [RequirePermission("Reconciliation.Item.Match")]
    public async Task<IActionResult> MatchReconciliationItem(int id, [FromBody] MatchRequest request)
    {
        var result = await _reconciliationService.MatchReconciliationItemAsync(id, request.MatchType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("items/bulk-match")]
    [RequirePermission("Reconciliation.Item.Match")]
    public async Task<IActionResult> BulkMatchReconciliationItems([FromBody] BulkMatchRequest request)
    {
        var result = await _reconciliationService.BulkMatchReconciliationItemsAsync(request.ItemIds, request.MatchType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 对账差异管理
    [HttpPost("differences")]
    [RequirePermission("Reconciliation.Difference.Create")]
    public async Task<IActionResult> CreateReconciliationDifference([FromBody] ReconciliationDifference difference)
    {
        var result = await _reconciliationService.CreateReconciliationDifferenceAsync(difference);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("differences/{id}")]
    [RequirePermission("Reconciliation.Difference.Update")]
    public async Task<IActionResult> UpdateReconciliationDifference(string id, [FromBody] ReconciliationDifference difference)
    {
        if (id != difference.Id)
            return BadRequest("ID不匹配");

        var result = await _reconciliationService.UpdateReconciliationDifferenceAsync(difference);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("differences/{id}")]
    [RequirePermission("Reconciliation.Difference.Delete")]
    public async Task<IActionResult> DeleteReconciliationDifference(string id)
    {
        var result = await _reconciliationService.DeleteReconciliationDifferenceAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("differences/{id}")]
    [RequirePermission("Reconciliation.Difference.View")]
    public async Task<IActionResult> GetReconciliationDifferenceById(string id)
    {
        var result = await _reconciliationService.GetReconciliationDifferenceByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("differences/by-record/{recordId}")]
    [RequirePermission("Reconciliation.Difference.View")]
    public async Task<IActionResult> GetReconciliationDifferencesByRecord(int recordId)
    {
        var result = await _reconciliationService.GetReconciliationDifferencesByRecordAsync(recordId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("differences/by-status/{status}")]
    [RequirePermission("Reconciliation.Difference.View")]
    public async Task<IActionResult> GetReconciliationDifferencesByStatus(ReconciliationStatus status)
    {
        var result = await _reconciliationService.GetReconciliationDifferencesByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("differences/{id}/resolve")]
    [RequirePermission("Reconciliation.Difference.Resolve")]
    public async Task<IActionResult> ResolveReconciliationDifference(string id, [FromBody] ResolveRequest request)
    {
        var result = await _reconciliationService.ResolveReconciliationDifferenceAsync(id, request.Resolution, request.HandledBy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("differences/unresolved")]
    [RequirePermission("Reconciliation.Difference.View")]
    public async Task<IActionResult> GetUnresolvedDifferences()
    {
        var result = await _reconciliationService.GetUnresolvedDifferencesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 调整分录管理
    [HttpPost("adjustments")]
    [RequirePermission("Reconciliation.Adjustment.Create")]
    public async Task<IActionResult> CreateAdjustmentEntry([FromBody] AdjustmentEntry entry)
    {
        var result = await _reconciliationService.CreateAdjustmentEntryAsync(entry);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// 请求模型类
public class MatchRequest
{
    public string MatchType { get; set; } = string.Empty;
}

public class BulkMatchRequest
{
    public List<int> ItemIds { get; set; } = new List<int>();
    public string MatchType { get; set; } = string.Empty;
}

public class ResolveRequest
{
    public string Resolution { get; set; } = string.Empty;
    public string HandledBy { get; set; } = string.Empty;
}
