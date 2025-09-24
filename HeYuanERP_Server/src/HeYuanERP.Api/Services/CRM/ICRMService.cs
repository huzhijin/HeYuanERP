using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.Services.CRM;

public interface ICRMService
{
    // 销售机会管理
    Task<ApiResponse<SalesOpportunity>> CreateSalesOpportunityAsync(SalesOpportunity opportunity);
    Task<ApiResponse<SalesOpportunity>> UpdateSalesOpportunityAsync(SalesOpportunity opportunity);
    Task<ApiResponse<bool>> DeleteSalesOpportunityAsync(int id);
    Task<ApiResponse<SalesOpportunity?>> GetSalesOpportunityByIdAsync(int id);
    Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunitiesByAccountAsync(int accountId);
    Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunitiesByUserAsync(int userId);
    Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunitiesByStatusAsync(string status);
    Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunityPipelineAsync(int? userId = null);
    Task<ApiResponse<decimal>> CalculateOpportunityValueAsync(int userId, DateTime startDate, DateTime endDate);

    // 客户拜访管理
    Task<ApiResponse<CustomerVisit>> CreateCustomerVisitAsync(CustomerVisit visit);
    Task<ApiResponse<CustomerVisit>> UpdateCustomerVisitAsync(CustomerVisit visit);
    Task<ApiResponse<bool>> DeleteCustomerVisitAsync(int id);
    Task<ApiResponse<CustomerVisit?>> GetCustomerVisitByIdAsync(int id);
    Task<ApiResponse<List<CustomerVisit>>> GetCustomerVisitsByAccountAsync(int accountId);
    Task<ApiResponse<List<CustomerVisit>>> GetCustomerVisitsByUserAsync(int userId);
    Task<ApiResponse<List<CustomerVisit>>> GetCustomerVisitsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<CustomerVisit>>> GetUpcomingVisitsAsync(int userId);

    // 销售目标管理
    Task<ApiResponse<SalesTarget>> CreateSalesTargetAsync(SalesTarget target);
    Task<ApiResponse<SalesTarget>> UpdateSalesTargetAsync(SalesTarget target);
    Task<ApiResponse<bool>> DeleteSalesTargetAsync(int id);
    Task<ApiResponse<SalesTarget?>> GetSalesTargetByIdAsync(int id);
    Task<ApiResponse<List<SalesTarget>>> GetSalesTargetsByUserAsync(int userId);
    Task<ApiResponse<List<SalesTarget>>> GetSalesTargetsByPeriodAsync(int year, int month);
    Task<ApiResponse<bool>> UpdateTargetProgressAsync(int targetId, decimal actualValue);
    Task<ApiResponse<decimal>> CalculateTargetCompletionRateAsync(int targetId);

    // 客户价值分析
    Task<ApiResponse<CustomerValueAnalysis>> CreateCustomerValueAnalysisAsync(CustomerValueAnalysis analysis);
    Task<ApiResponse<CustomerValueAnalysis>> UpdateCustomerValueAnalysisAsync(CustomerValueAnalysis analysis);
    Task<ApiResponse<bool>> DeleteCustomerValueAnalysisAsync(int id);
    Task<ApiResponse<CustomerValueAnalysis?>> GetCustomerValueAnalysisByIdAsync(int id);
    Task<ApiResponse<CustomerValueAnalysis?>> GetLatestCustomerValueAnalysisAsync(int accountId);
    Task<ApiResponse<List<CustomerValueAnalysis>>> GetCustomerValueAnalysesByAccountAsync(int accountId);
    Task<ApiResponse<CustomerValueAnalysis>> PerformCustomerValueAnalysisAsync(int accountId);

    // CRM 仪表板数据
    Task<ApiResponse<object>> GetCRMDashboardDataAsync(int userId);
    Task<ApiResponse<object>> GetSalesPerformanceMetricsAsync(int userId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetCustomerInsightsAsync();
    Task<ApiResponse<object>> GetSalesForecastAsync(int userId, int months = 6);

    // 统计和报告
    Task<ApiResponse<object>> GetOpportunityStatsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetVisitStatsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetTargetAchievementStatsAsync(int year);
    Task<ApiResponse<List<object>>> GetTopPerformingCustomersAsync(int topN = 10);
    Task<ApiResponse<List<object>>> GetChurnRiskCustomersAsync();
}