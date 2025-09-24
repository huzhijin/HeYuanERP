using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;
using HeYuanERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.CRM;

public class CRMService : ICRMService
{
    private readonly HeYuanERPDbContext _context;

    public CRMService(HeYuanERPDbContext context)
    {
        _context = context;
    }

    // 销售机会管理
    public async Task<ApiResponse<SalesOpportunity>> CreateSalesOpportunityAsync(SalesOpportunity opportunity)
    {
        try
        {
            _context.Set<SalesOpportunity>().Add(opportunity);
            await _context.SaveChangesAsync();
            return ApiResponse<SalesOpportunity>.Ok(opportunity);
        }
        catch (Exception ex)
        {
            return ApiResponse<SalesOpportunity>.Error($"创建销售机会失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SalesOpportunity>> UpdateSalesOpportunityAsync(SalesOpportunity opportunity)
    {
        try
        {
            opportunity.UpdatedAt = DateTime.UtcNow;
            _context.Set<SalesOpportunity>().Update(opportunity);
            await _context.SaveChangesAsync();
            return ApiResponse<SalesOpportunity>.Ok(opportunity);
        }
        catch (Exception ex)
        {
            return ApiResponse<SalesOpportunity>.Error($"更新销售机会失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSalesOpportunityAsync(int id)
    {
        try
        {
            var opportunity = await _context.Set<SalesOpportunity>().FindAsync(id);
            if (opportunity == null)
                return ApiResponse<bool>.Error("销售机会不存在");

            _context.Set<SalesOpportunity>().Remove(opportunity);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除销售机会失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SalesOpportunity?>> GetSalesOpportunityByIdAsync(int id)
    {
        try
        {
            var opportunity = await _context.Set<SalesOpportunity>()
                .Include(o => o.Account)
                .FirstOrDefaultAsync(o => o.Id == id);

            return ApiResponse<SalesOpportunity?>.Ok(opportunity);
        }
        catch (Exception ex)
        {
            return ApiResponse<SalesOpportunity?>.Error($"获取销售机会失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunitiesByAccountAsync(int accountId)
    {
        try
        {
            var opportunities = await _context.Set<SalesOpportunity>()
                .Where(o => o.AccountId == accountId)
                .Include(o => o.Account)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<SalesOpportunity>>.Ok(opportunities);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SalesOpportunity>>.Error($"获取客户销售机会失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunitiesByUserAsync(int userId)
    {
        try
        {
            var opportunities = await _context.Set<SalesOpportunity>()
                .Where(o => o.AssignedToUserId == userId)
                .Include(o => o.Account)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<SalesOpportunity>>.Ok(opportunities);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SalesOpportunity>>.Error($"获取用户销售机会失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunitiesByStatusAsync(string status)
    {
        try
        {
            var opportunities = await _context.Set<SalesOpportunity>()
                .Where(o => o.Status == status)
                .Include(o => o.Account)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<SalesOpportunity>>.Ok(opportunities);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SalesOpportunity>>.Error($"获取状态销售机会失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SalesOpportunity>>> GetSalesOpportunityPipelineAsync(int? userId = null)
    {
        try
        {
            var query = _context.Set<SalesOpportunity>()
                .Where(o => o.Status != "Closed-Won" && o.Status != "Closed-Lost");

            if (userId.HasValue)
                query = query.Where(o => o.AssignedToUserId == userId.Value);

            var opportunities = await query
                .Include(o => o.Account)
                .OrderBy(o => o.Stage)
                .ThenByDescending(o => o.EstimatedValue)
                .ToListAsync();

            return ApiResponse<List<SalesOpportunity>>.Ok(opportunities);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SalesOpportunity>>.Error($"获取销售渠道失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<decimal>> CalculateOpportunityValueAsync(int userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var totalValue = await _context.Set<SalesOpportunity>()
                .Where(o => o.AssignedToUserId == userId &&
                           o.CreatedAt >= startDate &&
                           o.CreatedAt <= endDate)
                .SumAsync(o => o.EstimatedValue * o.Probability / 100);

            return ApiResponse<decimal>.Ok(totalValue);
        }
        catch (Exception ex)
        {
            return ApiResponse<decimal>.Error($"计算机会价值失败: {ex.Message}");
        }
    }

    // 客户拜访管理
    public async Task<ApiResponse<CustomerVisit>> CreateCustomerVisitAsync(CustomerVisit visit)
    {
        try
        {
            _context.Set<CustomerVisit>().Add(visit);
            await _context.SaveChangesAsync();
            return ApiResponse<CustomerVisit>.Ok(visit);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerVisit>.Error($"创建客户拜访失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerVisit>> UpdateCustomerVisitAsync(CustomerVisit visit)
    {
        try
        {
            visit.UpdatedAt = DateTime.UtcNow;
            _context.Set<CustomerVisit>().Update(visit);
            await _context.SaveChangesAsync();
            return ApiResponse<CustomerVisit>.Ok(visit);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerVisit>.Error($"更新客户拜访失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCustomerVisitAsync(int id)
    {
        try
        {
            var visit = await _context.Set<CustomerVisit>().FindAsync(id);
            if (visit == null)
                return ApiResponse<bool>.Error("客户拜访记录不存在");

            _context.Set<CustomerVisit>().Remove(visit);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除客户拜访失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerVisit?>> GetCustomerVisitByIdAsync(int id)
    {
        try
        {
            var visit = await _context.Set<CustomerVisit>()
                .Include(v => v.Account)
                .Include(v => v.SalesOpportunity)
                .FirstOrDefaultAsync(v => v.Id == id);

            return ApiResponse<CustomerVisit?>.Ok(visit);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerVisit?>.Error($"获取客户拜访失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CustomerVisit>>> GetCustomerVisitsByAccountAsync(int accountId)
    {
        try
        {
            var visits = await _context.Set<CustomerVisit>()
                .Where(v => v.AccountId == accountId)
                .Include(v => v.Account)
                .Include(v => v.SalesOpportunity)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return ApiResponse<List<CustomerVisit>>.Ok(visits);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CustomerVisit>>.Error($"获取客户拜访记录失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CustomerVisit>>> GetCustomerVisitsByUserAsync(int userId)
    {
        try
        {
            var visits = await _context.Set<CustomerVisit>()
                .Where(v => v.VisitedByUserId == userId)
                .Include(v => v.Account)
                .Include(v => v.SalesOpportunity)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return ApiResponse<List<CustomerVisit>>.Ok(visits);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CustomerVisit>>.Error($"获取用户拜访记录失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CustomerVisit>>> GetCustomerVisitsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var visits = await _context.Set<CustomerVisit>()
                .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate)
                .Include(v => v.Account)
                .Include(v => v.SalesOpportunity)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return ApiResponse<List<CustomerVisit>>.Ok(visits);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CustomerVisit>>.Error($"获取时间范围拜访记录失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CustomerVisit>>> GetUpcomingVisitsAsync(int userId)
    {
        try
        {
            var upcomingVisits = await _context.Set<CustomerVisit>()
                .Where(v => v.VisitedByUserId == userId &&
                           v.NextVisitDate.HasValue &&
                           v.NextVisitDate.Value >= DateTime.UtcNow)
                .Include(v => v.Account)
                .Include(v => v.SalesOpportunity)
                .OrderBy(v => v.NextVisitDate)
                .ToListAsync();

            return ApiResponse<List<CustomerVisit>>.Ok(upcomingVisits);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CustomerVisit>>.Error($"获取即将拜访计划失败: {ex.Message}");
        }
    }

    // 销售目标管理 (继续其他方法...)
    public async Task<ApiResponse<SalesTarget>> CreateSalesTargetAsync(SalesTarget target)
    {
        try
        {
            _context.Set<SalesTarget>().Add(target);
            await _context.SaveChangesAsync();
            return ApiResponse<SalesTarget>.Ok(target);
        }
        catch (Exception ex)
        {
            return ApiResponse<SalesTarget>.Error($"创建销售目标失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SalesTarget>> UpdateSalesTargetAsync(SalesTarget target)
    {
        try
        {
            target.UpdatedAt = DateTime.UtcNow;
            _context.Set<SalesTarget>().Update(target);
            await _context.SaveChangesAsync();
            return ApiResponse<SalesTarget>.Ok(target);
        }
        catch (Exception ex)
        {
            return ApiResponse<SalesTarget>.Error($"更新销售目标失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSalesTargetAsync(int id)
    {
        try
        {
            var target = await _context.Set<SalesTarget>().FindAsync(id);
            if (target == null)
                return ApiResponse<bool>.Error("销售目标不存在");

            _context.Set<SalesTarget>().Remove(target);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除销售目标失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SalesTarget?>> GetSalesTargetByIdAsync(int id)
    {
        try
        {
            var target = await _context.Set<SalesTarget>().FindAsync(id);
            return ApiResponse<SalesTarget?>.Ok(target);
        }
        catch (Exception ex)
        {
            return ApiResponse<SalesTarget?>.Error($"获取销售目标失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SalesTarget>>> GetSalesTargetsByUserAsync(int userId)
    {
        try
        {
            var targets = await _context.Set<SalesTarget>()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Year)
                .ThenByDescending(t => t.Month)
                .ToListAsync();

            return ApiResponse<List<SalesTarget>>.Ok(targets);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SalesTarget>>.Error($"获取用户销售目标失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SalesTarget>>> GetSalesTargetsByPeriodAsync(int year, int month)
    {
        try
        {
            var targets = await _context.Set<SalesTarget>()
                .Where(t => t.Year == year && t.Month == month)
                .ToListAsync();

            return ApiResponse<List<SalesTarget>>.Ok(targets);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SalesTarget>>.Error($"获取期间销售目标失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> UpdateTargetProgressAsync(int targetId, decimal actualValue)
    {
        try
        {
            var target = await _context.Set<SalesTarget>().FindAsync(targetId);
            if (target == null)
                return ApiResponse<bool>.Error("销售目标不存在");

            target.ActualValue = actualValue;
            target.CompletionRate = target.TargetValue > 0 ? (actualValue / target.TargetValue) * 100 : 0;
            target.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"更新目标进度失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<decimal>> CalculateTargetCompletionRateAsync(int targetId)
    {
        try
        {
            var target = await _context.Set<SalesTarget>().FindAsync(targetId);
            if (target == null)
                return ApiResponse<decimal>.Error("销售目标不存在");

            var completionRate = target.TargetValue > 0 ? (target.ActualValue / target.TargetValue) * 100 : 0;
            return ApiResponse<decimal>.Ok(completionRate);
        }
        catch (Exception ex)
        {
            return ApiResponse<decimal>.Error($"计算目标完成率失败: {ex.Message}");
        }
    }

    // 客户价值分析 (方法实现将继续...)
    public async Task<ApiResponse<CustomerValueAnalysis>> CreateCustomerValueAnalysisAsync(CustomerValueAnalysis analysis)
    {
        try
        {
            _context.Set<CustomerValueAnalysis>().Add(analysis);
            await _context.SaveChangesAsync();
            return ApiResponse<CustomerValueAnalysis>.Ok(analysis);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerValueAnalysis>.Error($"创建客户价值分析失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerValueAnalysis>> UpdateCustomerValueAnalysisAsync(CustomerValueAnalysis analysis)
    {
        try
        {
            analysis.UpdatedAt = DateTime.UtcNow;
            _context.Set<CustomerValueAnalysis>().Update(analysis);
            await _context.SaveChangesAsync();
            return ApiResponse<CustomerValueAnalysis>.Ok(analysis);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerValueAnalysis>.Error($"更新客户价值分析失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCustomerValueAnalysisAsync(int id)
    {
        try
        {
            var analysis = await _context.Set<CustomerValueAnalysis>().FindAsync(id);
            if (analysis == null)
                return ApiResponse<bool>.Error("客户价值分析不存在");

            _context.Set<CustomerValueAnalysis>().Remove(analysis);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除客户价值分析失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerValueAnalysis?>> GetCustomerValueAnalysisByIdAsync(int id)
    {
        try
        {
            var analysis = await _context.Set<CustomerValueAnalysis>()
                .Include(a => a.Account)
                .FirstOrDefaultAsync(a => a.Id == id);

            return ApiResponse<CustomerValueAnalysis?>.Ok(analysis);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerValueAnalysis?>.Error($"获取客户价值分析失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerValueAnalysis?>> GetLatestCustomerValueAnalysisAsync(int accountId)
    {
        try
        {
            var analysis = await _context.Set<CustomerValueAnalysis>()
                .Where(a => a.AccountId == accountId)
                .Include(a => a.Account)
                .OrderByDescending(a => a.AnalysisDate)
                .FirstOrDefaultAsync();

            return ApiResponse<CustomerValueAnalysis?>.Ok(analysis);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerValueAnalysis?>.Error($"获取最新客户价值分析失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CustomerValueAnalysis>>> GetCustomerValueAnalysesByAccountAsync(int accountId)
    {
        try
        {
            var analyses = await _context.Set<CustomerValueAnalysis>()
                .Where(a => a.AccountId == accountId)
                .Include(a => a.Account)
                .OrderByDescending(a => a.AnalysisDate)
                .ToListAsync();

            return ApiResponse<List<CustomerValueAnalysis>>.Ok(analyses);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CustomerValueAnalysis>>.Error($"获取客户价值分析历史失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerValueAnalysis>> PerformCustomerValueAnalysisAsync(int accountId)
    {
        try
        {
            // 这里是一个简化的客户价值分析算法实现
            // 在实际应用中，这里会包含复杂的数据分析和机器学习算法

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
                return ApiResponse<CustomerValueAnalysis>.Error("客户不存在");

            var analysis = new CustomerValueAnalysis
            {
                AccountId = accountId,
                AnalysisDate = DateTime.UtcNow,
                AnalysisPeriod = "Yearly",
                AnalyzedBy = "System",
                CreatedAt = DateTime.UtcNow
            };

            // 计算历史交易价值 (简化逻辑)
            // 在实际实现中会查询销售订单等相关数据
            analysis.HistoricalTransactionValue = 100000m; // 示例值
            analysis.AverageOrderValue = 10000m; // 示例值
            analysis.PurchaseFrequency = 2.5m; // 示例值
            analysis.CustomerLifetimeValue = analysis.HistoricalTransactionValue * 3; // 简化算法

            // 设置其他分析指标 (简化逻辑)
            analysis.RetentionRate = 85.5m;
            analysis.ChurnRiskScore = 25;
            analysis.CrossSellPotential = 70;
            analysis.UpsellPotential = 60;
            analysis.SatisfactionScore = 80;
            analysis.ValueGrade = "B";
            analysis.CustomerSegment = "Standard";

            _context.Set<CustomerValueAnalysis>().Add(analysis);
            await _context.SaveChangesAsync();

            return ApiResponse<CustomerValueAnalysis>.Ok(analysis);
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerValueAnalysis>.Error($"执行客户价值分析失败: {ex.Message}");
        }
    }

    // CRM 仪表板和统计方法 (简化实现)
    public async Task<ApiResponse<object>> GetCRMDashboardDataAsync(int userId)
    {
        try
        {
            var opportunityCount = await _context.Set<SalesOpportunity>()
                .CountAsync(o => o.AssignedToUserId == userId);

            var totalOpportunityValue = await _context.Set<SalesOpportunity>()
                .Where(o => o.AssignedToUserId == userId)
                .SumAsync(o => o.EstimatedValue);

            var visitCount = await _context.Set<CustomerVisit>()
                .CountAsync(v => v.VisitedByUserId == userId &&
                               v.VisitDate >= DateTime.UtcNow.AddDays(-30));

            var dashboard = new
            {
                OpportunityCount = opportunityCount,
                TotalOpportunityValue = totalOpportunityValue,
                MonthlyVisitCount = visitCount,
                LastUpdated = DateTime.UtcNow
            };

            return ApiResponse<object>.Ok(dashboard);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取CRM仪表板数据失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetSalesPerformanceMetricsAsync(int userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var metrics = new
            {
                Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                OpportunityValue = await _context.Set<SalesOpportunity>()
                    .Where(o => o.AssignedToUserId == userId &&
                               o.CreatedAt >= startDate &&
                               o.CreatedAt <= endDate)
                    .SumAsync(o => o.EstimatedValue),
                VisitCount = await _context.Set<CustomerVisit>()
                    .CountAsync(v => v.VisitedByUserId == userId &&
                                v.VisitDate >= startDate &&
                                v.VisitDate <= endDate)
            };

            return ApiResponse<object>.Ok(metrics);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取销售绩效指标失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetCustomerInsightsAsync()
    {
        try
        {
            var insights = new
            {
                TotalCustomers = await _context.Accounts.CountAsync(),
                HighValueCustomers = await _context.Set<CustomerValueAnalysis>()
                    .CountAsync(a => a.ValueGrade == "A"),
                ChurnRiskCustomers = await _context.Set<CustomerValueAnalysis>()
                    .CountAsync(a => a.ChurnRiskScore > 70)
            };

            return ApiResponse<object>.Ok(insights);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取客户洞察失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetSalesForecastAsync(int userId, int months = 6)
    {
        try
        {
            // 简化的销售预测逻辑
            var forecast = new
            {
                ForecastPeriod = months,
                EstimatedRevenue = 500000m, // 示例值
                Confidence = 75 // 置信度百分比
            };

            return ApiResponse<object>.Ok(forecast);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取销售预测失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetOpportunityStatsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var stats = new
            {
                TotalOpportunities = await _context.Set<SalesOpportunity>()
                    .CountAsync(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate),
                WonOpportunities = await _context.Set<SalesOpportunity>()
                    .CountAsync(o => o.Status == "Closed-Won" &&
                                o.CreatedAt >= startDate && o.CreatedAt <= endDate),
                LostOpportunities = await _context.Set<SalesOpportunity>()
                    .CountAsync(o => o.Status == "Closed-Lost" &&
                                o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            };

            return ApiResponse<object>.Ok(stats);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取销售机会统计失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetVisitStatsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var stats = new
            {
                TotalVisits = await _context.Set<CustomerVisit>()
                    .CountAsync(v => v.VisitDate >= startDate && v.VisitDate <= endDate),
                SuccessfulVisits = await _context.Set<CustomerVisit>()
                    .CountAsync(v => v.VisitResult == "成功" &&
                                v.VisitDate >= startDate && v.VisitDate <= endDate)
            };

            return ApiResponse<object>.Ok(stats);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取拜访统计失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetTargetAchievementStatsAsync(int year)
    {
        try
        {
            var stats = new
            {
                Year = year,
                TotalTargets = await _context.Set<SalesTarget>()
                    .CountAsync(t => t.Year == year),
                AchievedTargets = await _context.Set<SalesTarget>()
                    .CountAsync(t => t.Year == year && t.CompletionRate >= 100)
            };

            return ApiResponse<object>.Ok(stats);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取目标完成统计失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<object>>> GetTopPerformingCustomersAsync(int topN = 10)
    {
        try
        {
            var topCustomers = await _context.Set<CustomerValueAnalysis>()
                .Include(a => a.Account)
                .OrderByDescending(a => a.CustomerLifetimeValue)
                .Take(topN)
                .Select(a => new
                {
                    a.AccountId,
                    CustomerName = a.Account.Name,
                    a.CustomerLifetimeValue,
                    a.ValueGrade
                })
                .ToListAsync();

            return ApiResponse<List<object>>.Ok(topCustomers.Cast<object>().ToList());
        }
        catch (Exception ex)
        {
            return ApiResponse<List<object>>.Error($"获取顶级客户失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<object>>> GetChurnRiskCustomersAsync()
    {
        try
        {
            var riskCustomers = await _context.Set<CustomerValueAnalysis>()
                .Include(a => a.Account)
                .Where(a => a.ChurnRiskScore > 70)
                .OrderByDescending(a => a.ChurnRiskScore)
                .Select(a => new
                {
                    a.AccountId,
                    CustomerName = a.Account.Name,
                    a.ChurnRiskScore,
                    a.CustomerLifetimeValue
                })
                .ToListAsync();

            return ApiResponse<List<object>>.Ok(riskCustomers.Cast<object>().ToList());
        }
        catch (Exception ex)
        {
            return ApiResponse<List<object>>.Error($"获取流失风险客户失败: {ex.Message}");
        }
    }
}
