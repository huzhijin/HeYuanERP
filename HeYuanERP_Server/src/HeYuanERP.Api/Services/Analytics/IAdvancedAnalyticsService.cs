using HeYuanERP.Domain.Reports;
using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.Services.Analytics;

/// <summary>
/// 高级报表分析引擎服务接口
/// </summary>
public interface IAdvancedAnalyticsService
{
    // 管理驾驶舱
    Task<ApiResponse<ExecutiveDashboardData>> GetExecutiveDashboardAsync(DateRange dateRange);
    Task<ApiResponse<List<KPICard>>> GetExecutiveKPIsAsync(DateRange dateRange);
    Task<ApiResponse<ChartData>> GetBusinessOverviewChartAsync(string chartType, DateRange dateRange);

    // 销售分析
    Task<ApiResponse<SalesAnalysisData>> GetSalesAnalysisAsync(SalesAnalysisRequest request);
    Task<ApiResponse<List<KPICard>>> GetSalesKPIsAsync(SalesAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetSalesTrendAnalysisAsync(SalesAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetProductSalesAnalysisAsync(SalesAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetCustomerSalesAnalysisAsync(SalesAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetSalesRepPerformanceAsync(SalesAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetSalesFunnelAnalysisAsync(SalesAnalysisRequest request);

    // 采购分析
    Task<ApiResponse<PurchaseAnalysisData>> GetPurchaseAnalysisAsync(PurchaseAnalysisRequest request);
    Task<ApiResponse<List<KPICard>>> GetPurchaseKPIsAsync(PurchaseAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetPurchaseTrendAnalysisAsync(PurchaseAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetSupplierRankingAnalysisAsync(PurchaseAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetPurchaseCategoryAnalysisAsync(PurchaseAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetPurchaseCostAnalysisAsync(PurchaseAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetSupplierPerformanceRadarAsync(PurchaseAnalysisRequest request);

    // 财务分析
    Task<ApiResponse<FinanceAnalysisData>> GetFinanceAnalysisAsync(FinanceAnalysisRequest request);
    Task<ApiResponse<List<KPICard>>> GetFinanceKPIsAsync(FinanceAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetCashFlowAnalysisAsync(FinanceAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetProfitAnalysisAsync(FinanceAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetARAgingAnalysisAsync(FinanceAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetAPAgingAnalysisAsync(FinanceAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetCostStructureAnalysisAsync(FinanceAnalysisRequest request);

    // 库存分析
    Task<ApiResponse<InventoryAnalysisData>> GetInventoryAnalysisAsync(InventoryAnalysisRequest request);
    Task<ApiResponse<List<KPICard>>> GetInventoryKPIsAsync(InventoryAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetInventoryTurnoverAnalysisAsync(InventoryAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetInventoryDistributionAnalysisAsync(InventoryAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetSlowMovingAnalysisAsync(InventoryAnalysisRequest request);
    Task<ApiResponse<ChartData>> GetWarehouseUtilizationAnalysisAsync(InventoryAnalysisRequest request);

    // 钻取功能
    Task<ApiResponse<DrillDownResult>> DrillDownAsync(DrillDownRequest request);
    Task<ApiResponse<List<string>>> GetAvailableDrillPathsAsync(string reportType, string currentLevel);
    Task<ApiResponse<object>> GetDrillDownDetailAsync(DrillDownRequest request);

    // 数据立方体
    Task<ApiResponse<DataCube>> BuildDataCubeAsync(string cubeType, DateRange dateRange, List<Filter> filters);
    Task<ApiResponse<List<Dimension>>> GetAvailableDimensionsAsync(string cubeType);
    Task<ApiResponse<List<Measure>>> GetAvailableMeasuresAsync(string cubeType);

    // 自定义报表
    Task<ApiResponse<object>> CreateCustomReportAsync(CustomReportDefinition definition);
    Task<ApiResponse<object>> ExecuteCustomReportAsync(int reportId, Dictionary<string, object> parameters);
    Task<ApiResponse<List<CustomReportDefinition>>> GetCustomReportsAsync();
    Task<ApiResponse<bool>> SaveCustomReportAsync(CustomReportDefinition definition);
    Task<ApiResponse<bool>> DeleteCustomReportAsync(int reportId);

    // 报表缓存和性能
    Task<ApiResponse<bool>> RefreshReportCacheAsync(string reportType);
    Task<ApiResponse<object>> GetReportCacheStatusAsync();
    Task<ApiResponse<bool>> WarmUpReportsAsync(DateRange dateRange);

    // 数据导出
    Task<ApiResponse<byte[]>> ExportDashboardAsync(string dashboardType, DateRange dateRange, string format = "PDF");
    Task<ApiResponse<byte[]>> ExportChartAsync(string chartType, object chartData, string format = "PNG");
    Task<ApiResponse<byte[]>> ExportAnalysisAsync(string analysisType, object analysisData, string format = "Excel");

    // 报表订阅
    Task<ApiResponse<bool>> SubscribeToReportAsync(ReportSubscription subscription);
    Task<ApiResponse<bool>> UnsubscribeFromReportAsync(int subscriptionId);
    Task<ApiResponse<List<ReportSubscription>>> GetReportSubscriptionsAsync(int userId);
    Task<ApiResponse<bool>> SendScheduledReportsAsync();

    // 移动端支持
    Task<ApiResponse<object>> GetMobileDashboardAsync(string dashboardType, DateRange dateRange);
    Task<ApiResponse<List<KPICard>>> GetMobileKPIsAsync(DateRange dateRange);
    Task<ApiResponse<ChartData>> GetMobileChartAsync(string chartType, DateRange dateRange);

    // 实时数据和预警
    Task<ApiResponse<List<object>>> GetRealTimeAlertsAsync();
    Task<ApiResponse<object>> GetRealTimeKPIsAsync();
    Task<ApiResponse<bool>> SetupRealTimeMonitoringAsync(RealTimeMonitoringConfig config);

    // 数据质量和验证
    Task<ApiResponse<object>> ValidateReportDataAsync(string reportType, DateRange dateRange);
    Task<ApiResponse<object>> GetDataQualityReportAsync();
    Task<ApiResponse<bool>> RecalculateMetricsAsync(string metricType, DateRange dateRange);

    // 权限和安全
    Task<ApiResponse<List<string>>> GetUserReportPermissionsAsync(int userId);
    Task<ApiResponse<bool>> CheckReportAccessAsync(int userId, string reportType);
    Task<ApiResponse<object>> GetReportAuditLogAsync(string reportType, DateRange dateRange);
}

/// <summary>
/// 数据立方体服务接口
/// </summary>
public interface IDataCubeService
{
    Task<ApiResponse<DataCube>> BuildCubeAsync(string cubeType, List<string> dimensions, List<string> measures, List<Filter> filters);
    Task<ApiResponse<object>> QueryCubeAsync(DataCube cube, string query);
    Task<ApiResponse<DataCube>> SliceCubeAsync(DataCube cube, Dictionary<string, object> sliceParams);
    Task<ApiResponse<DataCube>> DiceCubeAsync(DataCube cube, Dictionary<string, List<object>> diceParams);
    Task<ApiResponse<DataCube>> PivotCubeAsync(DataCube cube, string newDimension);
}

/// <summary>
/// 钻取服务接口
/// </summary>
public interface IDrillDownService
{
    Task<ApiResponse<DrillDownResult>> ExecuteDrillDownAsync(DrillDownRequest request);
    Task<ApiResponse<List<string>>> GetDrillPathsAsync(string entityType, string currentLevel);
    Task<ApiResponse<object>> GetDrillTargetDataAsync(string targetLevel, Dictionary<string, object> context);
    Task<ApiResponse<DrillDownPath>> BuildDrillPathAsync(string startLevel, string targetLevel);
}

/// <summary>
/// 报表缓存服务接口
/// </summary>
public interface IReportCacheService
{
    Task<T?> GetCachedReportAsync<T>(string cacheKey) where T : class;
    Task SetCachedReportAsync<T>(string cacheKey, T data, TimeSpan expiration) where T : class;
    Task InvalidateCacheAsync(string cachePattern);
    Task<bool> IsCacheValidAsync(string cacheKey);
    Task<object> GetCacheStatsAsync();
}

// 自定义报表定义
public class CustomReportDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string DataSource { get; set; } = string.Empty;
    public List<string> Columns { get; set; } = new List<string>();
    public List<Filter> Filters { get; set; } = new List<Filter>();
    public string GroupBy { get; set; } = string.Empty;
    public string OrderBy { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; } = false;
}

// 报表订阅
public class ReportSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty; // Cron expression
    public string DeliveryMethod { get; set; } = "Email"; // Email, SMS, Push
    public string Format { get; set; } = "PDF";
    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSent { get; set; }
}

// 实时监控配置
public class RealTimeMonitoringConfig
{
    public string MetricName { get; set; } = string.Empty;
    public decimal ThresholdValue { get; set; }
    public string ThresholdOperator { get; set; } = ">"; // >, <, =, >=, <=
    public string AlertLevel { get; set; } = "Warning"; // Info, Warning, Error, Critical
    public List<string> NotificationChannels { get; set; } = new List<string>();
    public int CheckIntervalMinutes { get; set; } = 5;
    public bool IsEnabled { get; set; } = true;
}