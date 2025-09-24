using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Services.CRM;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.Services.Authorization;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CRMController : ControllerBase
{
    private readonly ICRMService _crmService;

    public CRMController(ICRMService crmService)
    {
        _crmService = crmService;
    }

    // 销售机会管理
    [HttpPost("opportunities")]
    [RequirePermission("CRM.SalesOpportunity.Create")]
    public async Task<IActionResult> CreateSalesOpportunity([FromBody] SalesOpportunity opportunity)
    {
        var result = await _crmService.CreateSalesOpportunityAsync(opportunity);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("opportunities/{id}")]
    [RequirePermission("CRM.SalesOpportunity.Update")]
    public async Task<IActionResult> UpdateSalesOpportunity(int id, [FromBody] SalesOpportunity opportunity)
    {
        if (id != opportunity.Id)
            return BadRequest("ID不匹配");

        var result = await _crmService.UpdateSalesOpportunityAsync(opportunity);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("opportunities/{id}")]
    [RequirePermission("CRM.SalesOpportunity.Delete")]
    public async Task<IActionResult> DeleteSalesOpportunity(int id)
    {
        var result = await _crmService.DeleteSalesOpportunityAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("opportunities/{id}")]
    [RequirePermission("CRM.SalesOpportunity.View")]
    public async Task<IActionResult> GetSalesOpportunityById(int id)
    {
        var result = await _crmService.GetSalesOpportunityByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("opportunities/by-account/{accountId}")]
    [RequirePermission("CRM.SalesOpportunity.View")]
    public async Task<IActionResult> GetSalesOpportunitiesByAccount(int accountId)
    {
        var result = await _crmService.GetSalesOpportunitiesByAccountAsync(accountId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("opportunities/by-user/{userId}")]
    [RequirePermission("CRM.SalesOpportunity.View")]
    public async Task<IActionResult> GetSalesOpportunitiesByUser(int userId)
    {
        var result = await _crmService.GetSalesOpportunitiesByUserAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("opportunities/by-status/{status}")]
    [RequirePermission("CRM.SalesOpportunity.View")]
    public async Task<IActionResult> GetSalesOpportunitiesByStatus(string status)
    {
        var result = await _crmService.GetSalesOpportunitiesByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("opportunities/pipeline")]
    [RequirePermission("CRM.SalesOpportunity.View")]
    public async Task<IActionResult> GetSalesOpportunityPipeline([FromQuery] int? userId = null)
    {
        var result = await _crmService.GetSalesOpportunityPipelineAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("opportunities/value/{userId}")]
    [RequirePermission("CRM.SalesOpportunity.View")]
    public async Task<IActionResult> CalculateOpportunityValue(int userId,
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _crmService.CalculateOpportunityValueAsync(userId, startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 客户拜访管理
    [HttpPost("visits")]
    [RequirePermission("CRM.CustomerVisit.Create")]
    public async Task<IActionResult> CreateCustomerVisit([FromBody] CustomerVisit visit)
    {
        var result = await _crmService.CreateCustomerVisitAsync(visit);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("visits/{id}")]
    [RequirePermission("CRM.CustomerVisit.Update")]
    public async Task<IActionResult> UpdateCustomerVisit(int id, [FromBody] CustomerVisit visit)
    {
        if (id != visit.Id)
            return BadRequest("ID不匹配");

        var result = await _crmService.UpdateCustomerVisitAsync(visit);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("visits/{id}")]
    [RequirePermission("CRM.CustomerVisit.Delete")]
    public async Task<IActionResult> DeleteCustomerVisit(int id)
    {
        var result = await _crmService.DeleteCustomerVisitAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("visits/{id}")]
    [RequirePermission("CRM.CustomerVisit.View")]
    public async Task<IActionResult> GetCustomerVisitById(int id)
    {
        var result = await _crmService.GetCustomerVisitByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("visits/by-account/{accountId}")]
    [RequirePermission("CRM.CustomerVisit.View")]
    public async Task<IActionResult> GetCustomerVisitsByAccount(int accountId)
    {
        var result = await _crmService.GetCustomerVisitsByAccountAsync(accountId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("visits/by-user/{userId}")]
    [RequirePermission("CRM.CustomerVisit.View")]
    public async Task<IActionResult> GetCustomerVisitsByUser(int userId)
    {
        var result = await _crmService.GetCustomerVisitsByUserAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("visits/by-date")]
    [RequirePermission("CRM.CustomerVisit.View")]
    public async Task<IActionResult> GetCustomerVisitsByDateRange(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _crmService.GetCustomerVisitsByDateRangeAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("visits/upcoming/{userId}")]
    [RequirePermission("CRM.CustomerVisit.View")]
    public async Task<IActionResult> GetUpcomingVisits(int userId)
    {
        var result = await _crmService.GetUpcomingVisitsAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 销售目标管理
    [HttpPost("targets")]
    [RequirePermission("CRM.SalesTarget.Create")]
    public async Task<IActionResult> CreateSalesTarget([FromBody] SalesTarget target)
    {
        var result = await _crmService.CreateSalesTargetAsync(target);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("targets/{id}")]
    [RequirePermission("CRM.SalesTarget.Update")]
    public async Task<IActionResult> UpdateSalesTarget(int id, [FromBody] SalesTarget target)
    {
        if (id != target.Id)
            return BadRequest("ID不匹配");

        var result = await _crmService.UpdateSalesTargetAsync(target);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("targets/{id}")]
    [RequirePermission("CRM.SalesTarget.Delete")]
    public async Task<IActionResult> DeleteSalesTarget(int id)
    {
        var result = await _crmService.DeleteSalesTargetAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("targets/{id}")]
    [RequirePermission("CRM.SalesTarget.View")]
    public async Task<IActionResult> GetSalesTargetById(int id)
    {
        var result = await _crmService.GetSalesTargetByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("targets/by-user/{userId}")]
    [RequirePermission("CRM.SalesTarget.View")]
    public async Task<IActionResult> GetSalesTargetsByUser(int userId)
    {
        var result = await _crmService.GetSalesTargetsByUserAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("targets/by-period")]
    [RequirePermission("CRM.SalesTarget.View")]
    public async Task<IActionResult> GetSalesTargetsByPeriod([FromQuery] int year, [FromQuery] int month)
    {
        var result = await _crmService.GetSalesTargetsByPeriodAsync(year, month);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("targets/{id}/progress")]
    [RequirePermission("CRM.SalesTarget.Update")]
    public async Task<IActionResult> UpdateTargetProgress(int id, [FromBody] decimal actualValue)
    {
        var result = await _crmService.UpdateTargetProgressAsync(id, actualValue);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("targets/{id}/completion-rate")]
    [RequirePermission("CRM.SalesTarget.View")]
    public async Task<IActionResult> CalculateTargetCompletionRate(int id)
    {
        var result = await _crmService.CalculateTargetCompletionRateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 客户价值分析
    [HttpPost("value-analysis")]
    [RequirePermission("CRM.CustomerValueAnalysis.Create")]
    public async Task<IActionResult> CreateCustomerValueAnalysis([FromBody] CustomerValueAnalysis analysis)
    {
        var result = await _crmService.CreateCustomerValueAnalysisAsync(analysis);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("value-analysis/{id}")]
    [RequirePermission("CRM.CustomerValueAnalysis.Update")]
    public async Task<IActionResult> UpdateCustomerValueAnalysis(int id, [FromBody] CustomerValueAnalysis analysis)
    {
        if (id != analysis.Id)
            return BadRequest("ID不匹配");

        var result = await _crmService.UpdateCustomerValueAnalysisAsync(analysis);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("value-analysis/{id}")]
    [RequirePermission("CRM.CustomerValueAnalysis.Delete")]
    public async Task<IActionResult> DeleteCustomerValueAnalysis(int id)
    {
        var result = await _crmService.DeleteCustomerValueAnalysisAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("value-analysis/{id}")]
    [RequirePermission("CRM.CustomerValueAnalysis.View")]
    public async Task<IActionResult> GetCustomerValueAnalysisById(int id)
    {
        var result = await _crmService.GetCustomerValueAnalysisByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("value-analysis/latest/{accountId}")]
    [RequirePermission("CRM.CustomerValueAnalysis.View")]
    public async Task<IActionResult> GetLatestCustomerValueAnalysis(int accountId)
    {
        var result = await _crmService.GetLatestCustomerValueAnalysisAsync(accountId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("value-analysis/by-account/{accountId}")]
    [RequirePermission("CRM.CustomerValueAnalysis.View")]
    public async Task<IActionResult> GetCustomerValueAnalysesByAccount(int accountId)
    {
        var result = await _crmService.GetCustomerValueAnalysesByAccountAsync(accountId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("value-analysis/perform/{accountId}")]
    [RequirePermission("CRM.CustomerValueAnalysis.Create")]
    public async Task<IActionResult> PerformCustomerValueAnalysis(int accountId)
    {
        var result = await _crmService.PerformCustomerValueAnalysisAsync(accountId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // CRM 仪表板和统计
    [HttpGet("dashboard/{userId}")]
    [RequirePermission("CRM.Dashboard.View")]
    public async Task<IActionResult> GetCRMDashboardData(int userId)
    {
        var result = await _crmService.GetCRMDashboardDataAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("performance/{userId}")]
    [RequirePermission("CRM.Performance.View")]
    public async Task<IActionResult> GetSalesPerformanceMetrics(int userId,
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _crmService.GetSalesPerformanceMetricsAsync(userId, startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("insights")]
    [RequirePermission("CRM.Insights.View")]
    public async Task<IActionResult> GetCustomerInsights()
    {
        var result = await _crmService.GetCustomerInsightsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("forecast/{userId}")]
    [RequirePermission("CRM.Forecast.View")]
    public async Task<IActionResult> GetSalesForecast(int userId, [FromQuery] int months = 6)
    {
        var result = await _crmService.GetSalesForecastAsync(userId, months);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/opportunities")]
    [RequirePermission("CRM.Stats.View")]
    public async Task<IActionResult> GetOpportunityStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _crmService.GetOpportunityStatsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/visits")]
    [RequirePermission("CRM.Stats.View")]
    public async Task<IActionResult> GetVisitStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _crmService.GetVisitStatsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/targets/{year}")]
    [RequirePermission("CRM.Stats.View")]
    public async Task<IActionResult> GetTargetAchievementStats(int year)
    {
        var result = await _crmService.GetTargetAchievementStatsAsync(year);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("customers/top-performing")]
    [RequirePermission("CRM.Customers.View")]
    public async Task<IActionResult> GetTopPerformingCustomers([FromQuery] int topN = 10)
    {
        var result = await _crmService.GetTopPerformingCustomersAsync(topN);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("customers/churn-risk")]
    [RequirePermission("CRM.Customers.View")]
    public async Task<IActionResult> GetChurnRiskCustomers()
    {
        var result = await _crmService.GetChurnRiskCustomersAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
