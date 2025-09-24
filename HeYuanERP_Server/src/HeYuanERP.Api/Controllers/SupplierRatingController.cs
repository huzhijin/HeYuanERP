#if false
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Api.Services.Suppliers;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;
using HeYuanERP.Api.Attributes;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplierRatingController : ControllerBase
{
    private readonly ISupplierRatingService _supplierRatingService;
    private readonly ILogger<SupplierRatingController> _logger;

    public SupplierRatingController(
        ISupplierRatingService supplierRatingService,
        ILogger<SupplierRatingController> logger)
    {
        _supplierRatingService = supplierRatingService;
        _logger = logger;
    }

    /// <summary>
    /// 获取供应商评级信息
    /// </summary>
    [HttpGet("{supplierId}")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<SupplierRating>> GetSupplierRating(string supplierId)
    {
        try
        {
            var rating = await _supplierRatingService.GetSupplierRatingAsync(supplierId);
            if (rating == null)
                return NotFound($"未找到供应商 {supplierId} 的评级信息");

            return Ok(rating);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商评级信息失败: {SupplierId}", supplierId);
            return StatusCode(500, "获取供应商评级信息失败");
        }
    }

    /// <summary>
    /// 创建供应商评级
    /// </summary>
    [HttpPost]
    [RequirePermission("SupplierRating.Create")]
    public async Task<ActionResult<SupplierRating>> CreateSupplierRating([FromBody] SupplierRating rating)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationErrors = await _supplierRatingService.GetSupplierRatingValidationErrorsAsync(rating);
            if (validationErrors.Any())
                return BadRequest(string.Join("; ", validationErrors));

            var createdRating = await _supplierRatingService.CreateSupplierRatingAsync(rating);
            return CreatedAtAction(nameof(GetSupplierRating), new { supplierId = createdRating.SupplierId }, createdRating);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建供应商评级失败");
            return StatusCode(500, "创建供应商评级失败");
        }
    }

    /// <summary>
    /// 更新供应商评级
    /// </summary>
    [HttpPut("{ratingId}")]
    [RequirePermission("SupplierRating.Update")]
    public async Task<ActionResult<SupplierRating>> UpdateSupplierRating(string ratingId, [FromBody] SupplierRating rating)
    {
        try
        {
            if (ratingId != rating.Id)
                return BadRequest("评级ID不匹配");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationErrors = await _supplierRatingService.GetSupplierRatingValidationErrorsAsync(rating);
            if (validationErrors.Any())
                return BadRequest(string.Join("; ", validationErrors));

            var updatedRating = await _supplierRatingService.UpdateSupplierRatingAsync(rating);
            return Ok(updatedRating);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新供应商评级失败: {RatingId}", ratingId);
            return StatusCode(500, "更新供应商评级失败");
        }
    }

    /// <summary>
    /// 删除供应商评级
    /// </summary>
    [HttpDelete("{ratingId}")]
    [RequirePermission("SupplierRating.Delete")]
    public async Task<ActionResult> DeleteSupplierRating(string ratingId)
    {
        try
        {
            var result = await _supplierRatingService.DeleteSupplierRatingAsync(ratingId);
            if (!result)
                return NotFound($"未找到评级 {ratingId}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除供应商评级失败: {RatingId}", ratingId);
            return StatusCode(500, "删除供应商评级失败");
        }
    }

    /// <summary>
    /// 按级别获取供应商评级列表
    /// </summary>
    [HttpGet("by-level/{level}")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRating>>> GetSupplierRatingsByLevel(SupplierLevel level)
    {
        try
        {
            var ratings = await _supplierRatingService.GetSupplierRatingsByLevelAsync(level);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "按级别获取供应商评级失败: {Level}", level);
            return StatusCode(500, "获取供应商评级列表失败");
        }
    }

    /// <summary>
    /// 搜索供应商评级
    /// </summary>
    [HttpGet("search")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRating>>> SearchSupplierRatings(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var ratings = await _supplierRatingService.SearchSupplierRatingsAsync(searchTerm ?? "", skip, take);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索供应商评级失败");
            return StatusCode(500, "搜索供应商评级失败");
        }
    }

    /// <summary>
    /// 获取供应商级别分布统计
    /// </summary>
    [HttpGet("level-distribution")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<Dictionary<SupplierLevel, int>>> GetSupplierLevelDistribution()
    {
        try
        {
            var distribution = await _supplierRatingService.GetSupplierLevelDistributionAsync();
            return Ok(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商级别分布失败");
            return StatusCode(500, "获取供应商级别分布失败");
        }
    }

    /// <summary>
    /// 获取顶级供应商列表
    /// </summary>
    [HttpGet("top-suppliers")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRating>>> GetTopSuppliers([FromQuery] int count = 10)
    {
        try
        {
            var topSuppliers = await _supplierRatingService.GetTopSuppliersAsync(count);
            return Ok(topSuppliers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取顶级供应商失败");
            return StatusCode(500, "获取顶级供应商失败");
        }
    }

    /// <summary>
    /// 获取高风险供应商列表
    /// </summary>
    [HttpGet("high-risk")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRating>>> GetHighRiskSuppliers()
    {
        try
        {
            var highRiskSuppliers = await _supplierRatingService.GetHighRiskSuppliersAsync();
            return Ok(highRiskSuppliers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取高风险供应商失败");
            return StatusCode(500, "获取高风险供应商失败");
        }
    }

    /// <summary>
    /// 获取需要审查的供应商列表
    /// </summary>
    [HttpGet("requiring-review")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRating>>> GetSuppliersRequiringReview()
    {
        try
        {
            var suppliers = await _supplierRatingService.GetSuppliersRequiringReviewAsync();
            return Ok(suppliers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取需要审查的供应商失败");
            return StatusCode(500, "获取需要审查的供应商失败");
        }
    }

    /// <summary>
    /// 重新计算供应商评级
    /// </summary>
    [HttpPost("{supplierId}/recalculate")]
    [RequirePermission("SupplierRating.Calculate")]
    public async Task<ActionResult<SupplierRating>> RecalculateSupplierRating(string supplierId)
    {
        try
        {
            var rating = await _supplierRatingService.RecalculateSupplierRatingAsync(supplierId);
            return Ok(rating);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新计算供应商评级失败: {SupplierId}", supplierId);
            return StatusCode(500, "重新计算供应商评级失败");
        }
    }

    /// <summary>
    /// 批量重新计算供应商评级
    /// </summary>
    [HttpPost("batch-recalculate")]
    [RequirePermission("SupplierRating.Calculate")]
    public async Task<ActionResult<List<SupplierRating>>> BatchRecalculateRatings([FromBody] List<string> supplierIds)
    {
        try
        {
            var ratings = await _supplierRatingService.BatchRecalculateRatingsAsync(supplierIds);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量重新计算供应商评级失败");
            return StatusCode(500, "批量重新计算供应商评级失败");
        }
    }

    /// <summary>
    /// 更新供应商单项评分
    /// </summary>
    [HttpPut("{supplierId}/score")]
    [RequirePermission("SupplierRating.Update")]
    public async Task<ActionResult<SupplierRating>> UpdateSupplierScore(
        string supplierId,
        [FromBody] UpdateScoreRequest request)
    {
        try
        {
            var rating = await _supplierRatingService.UpdateSupplierScoreAsync(
                supplierId, request.Criterion, request.Score, request.Reason);
            return Ok(rating);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新供应商评分失败: {SupplierId}", supplierId);
            return StatusCode(500, "更新供应商评分失败");
        }
    }

    /// <summary>
    /// 审批供应商级别变更
    /// </summary>
    [HttpPost("{ratingId}/approve")]
    [RequirePermission("SupplierRating.Approve")]
    public async Task<ActionResult> ApproveSupplierLevelChange(
        string ratingId,
        [FromBody] ApprovalRequest request)
    {
        try
        {
            var result = await _supplierRatingService.ApproveSupplierLevelChangeAsync(
                ratingId, request.ApproverId, request.Comments);

            if (!result)
                return NotFound($"未找到评级 {ratingId}");

            return Ok(new { message = "审批成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审批供应商级别变更失败: {RatingId}", ratingId);
            return StatusCode(500, "审批供应商级别变更失败");
        }
    }

    /// <summary>
    /// 拒绝供应商级别变更
    /// </summary>
    [HttpPost("{ratingId}/reject")]
    [RequirePermission("SupplierRating.Approve")]
    public async Task<ActionResult> RejectSupplierLevelChange(
        string ratingId,
        [FromBody] RejectionRequest request)
    {
        try
        {
            var result = await _supplierRatingService.RejectSupplierLevelChangeAsync(
                ratingId, request.ApproverId, request.Reason);

            if (!result)
                return NotFound($"未找到评级 {ratingId}");

            return Ok(new { message = "拒绝成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "拒绝供应商级别变更失败: {RatingId}", ratingId);
            return StatusCode(500, "拒绝供应商级别变更失败");
        }
    }

    /// <summary>
    /// 创建供应商评估
    /// </summary>
    [HttpPost("{supplierId}/evaluations")]
    [RequirePermission("SupplierEvaluation.Create")]
    public async Task<ActionResult<SupplierEvaluation>> CreateSupplierEvaluation(
        string supplierId,
        [FromBody] SupplierEvaluation evaluation)
    {
        try
        {
            evaluation.SupplierId = supplierId;
            var createdEvaluation = await _supplierRatingService.CreateSupplierEvaluationAsync(evaluation);
            return CreatedAtAction(nameof(GetSupplierEvaluations),
                new { supplierId = supplierId }, createdEvaluation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建供应商评估失败: {SupplierId}", supplierId);
            return StatusCode(500, "创建供应商评估失败");
        }
    }

    /// <summary>
    /// 获取供应商评估列表
    /// </summary>
    [HttpGet("{supplierId}/evaluations")]
    [RequirePermission("SupplierEvaluation.View")]
    public async Task<ActionResult<List<SupplierEvaluation>>> GetSupplierEvaluations(
        string supplierId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var evaluations = await _supplierRatingService.GetSupplierEvaluationsAsync(
                supplierId, startDate, endDate);
            return Ok(evaluations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商评估失败: {SupplierId}", supplierId);
            return StatusCode(500, "获取供应商评估失败");
        }
    }

    /// <summary>
    /// 获取最新供应商评估
    /// </summary>
    [HttpGet("{supplierId}/evaluations/latest")]
    [RequirePermission("SupplierEvaluation.View")]
    public async Task<ActionResult<SupplierEvaluation>> GetLatestSupplierEvaluation(string supplierId)
    {
        try
        {
            var evaluation = await _supplierRatingService.GetLatestSupplierEvaluationAsync(supplierId);
            if (evaluation == null)
                return NotFound($"未找到供应商 {supplierId} 的评估记录");

            return Ok(evaluation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取最新供应商评估失败: {SupplierId}", supplierId);
            return StatusCode(500, "获取最新供应商评估失败");
        }
    }

    /// <summary>
    /// 获取待处理的评估列表
    /// </summary>
    [HttpGet("evaluations/pending")]
    [RequirePermission("SupplierEvaluation.View")]
    public async Task<ActionResult<List<SupplierEvaluation>>> GetPendingEvaluations()
    {
        try
        {
            var evaluations = await _supplierRatingService.GetPendingEvaluationsAsync();
            return Ok(evaluations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取待处理评估失败");
            return StatusCode(500, "获取待处理评估失败");
        }
    }

    /// <summary>
    /// 获取供应商评级历史记录
    /// </summary>
    [HttpGet("{supplierId}/history")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRatingHistory>>> GetSupplierRatingHistory(
        string supplierId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var history = await _supplierRatingService.GetSupplierRatingHistoryAsync(supplierId, skip, take);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商评级历史失败: {SupplierId}", supplierId);
            return StatusCode(500, "获取供应商评级历史失败");
        }
    }

    /// <summary>
    /// 获取评级统计信息
    /// </summary>
    [HttpGet("statistics")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<SupplierRatingStatistics>> GetRatingStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var statistics = await _supplierRatingService.GetRatingStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取评级统计信息失败");
            return StatusCode(500, "获取评级统计信息失败");
        }
    }

    /// <summary>
    /// 获取供应商评级告警
    /// </summary>
    [HttpGet("alerts")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRatingAlert>>> GetSupplierRatingAlerts()
    {
        try
        {
            var alerts = await _supplierRatingService.GetSupplierRatingAlertsAsync();
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商评级告警失败");
            return StatusCode(500, "获取供应商评级告警失败");
        }
    }

    /// <summary>
    /// 获取供应商风险档案
    /// </summary>
    [HttpGet("{supplierId}/risk-profile")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<SupplierRiskProfile>> GetSupplierRiskProfile(string supplierId)
    {
        try
        {
            var riskProfile = await _supplierRatingService.GetSupplierRiskProfileAsync(supplierId);
            return Ok(riskProfile);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商风险档案失败: {SupplierId}", supplierId);
            return StatusCode(500, "获取供应商风险档案失败");
        }
    }

    /// <summary>
    /// 按风险等级获取供应商
    /// </summary>
    [HttpGet("by-risk-level/{riskLevel}")]
    [RequirePermission("SupplierRating.View")]
    public async Task<ActionResult<List<SupplierRating>>> GetSuppliersByRiskLevel(RiskLevel riskLevel)
    {
        try
        {
            var suppliers = await _supplierRatingService.GetSuppliersByRiskLevelAsync(riskLevel);
            return Ok(suppliers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "按风险等级获取供应商失败: {RiskLevel}", riskLevel);
            return StatusCode(500, "按风险等级获取供应商失败");
        }
    }
}
#endif

// DTO 类（禁用以避免与其它控制器命名冲突）
#if false
public class UpdateScoreRequest
{
    public string Criterion { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ApprovalRequest
{
    public string ApproverId { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}

public class RejectionRequest
{
    public string ApproverId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
#endif
