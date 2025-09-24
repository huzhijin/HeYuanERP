// Disabled in minimal build: Dashboard service not enabled
#if false
using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.Services.Dashboard;

/// <summary>
/// 仪表板数据结果
/// </summary>
public class DashboardDataResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// 数据行数
    /// </summary>
    public int RowCount { get; set; }

    /// <summary>
    /// 数据来源
    /// </summary>
    public string? DataSource { get; set; }

    /// <summary>
    /// 查询耗时（毫秒）
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// 是否来自缓存
    /// </summary>
    public bool FromCache { get; set; }

    /// <summary>
    /// 缓存过期时间
    /// </summary>
    public DateTime? CacheExpiration { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 扩展信息
    /// </summary>
    public Dictionary<string, object> ExtensionInfo { get; set; } = new();

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static DashboardDataResult Success(object data, int rowCount = 0, bool fromCache = false)
    {
        return new DashboardDataResult
        {
            IsSuccess = true,
            Data = data,
            RowCount = rowCount,
            FromCache = fromCache
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static DashboardDataResult Failure(string errorMessage)
    {
        return new DashboardDataResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// KPI数值结果
/// </summary>
public class KPIValueResult
{
    /// <summary>
    /// KPI指标ID
    /// </summary>
    public string MetricId { get; set; } = string.Empty;

    /// <summary>
    /// 当前值
    /// </summary>
    public decimal CurrentValue { get; set; }

    /// <summary>
    /// 目标值
    /// </summary>
    public decimal? TargetValue { get; set; }

    /// <summary>
    /// 完成率
    /// </summary>
    public decimal? CompletionRate => TargetValue.HasValue && TargetValue > 0
        ? (CurrentValue / TargetValue.Value) * 100 : null;

    /// <summary>
    /// 同比值
    /// </summary>
    public decimal? YearOverYearValue { get; set; }

    /// <summary>
    /// 同比增长率
    /// </summary>
    public decimal? YearOverYearGrowthRate => YearOverYearValue.HasValue && YearOverYearValue > 0
        ? ((CurrentValue - YearOverYearValue.Value) / YearOverYearValue.Value) * 100 : null;

    /// <summary>
    /// 环比值
    /// </summary>
    public decimal? PeriodOverPeriodValue { get; set; }

    /// <summary>
    /// 环比增长率
    /// </summary>
    public decimal? PeriodOverPeriodGrowthRate => PeriodOverPeriodValue.HasValue && PeriodOverPeriodValue > 0
        ? ((CurrentValue - PeriodOverPeriodValue.Value) / PeriodOverPeriodValue.Value) * 100 : null;

    /// <summary>
    /// 趋势方向
    /// </summary>
    public TrendDirection TrendDirection { get; set; } = TrendDirection.Stable;

    /// <summary>
    /// 状态等级
    /// </summary>
    public KPIStatusLevel StatusLevel { get; set; } = KPIStatusLevel.Normal;

    /// <summary>
    /// 状态颜色
    /// </summary>
    public string StatusColor => StatusLevel switch
    {
        KPIStatusLevel.Excellent => "#52c41a",
        KPIStatusLevel.Good => "#1890ff",
        KPIStatusLevel.Normal => "#faad14",
        KPIStatusLevel.Warning => "#fa8c16",
        KPIStatusLevel.Danger => "#f5222d",
        _ => "#d9d9d9"
    };

    /// <summary>
    /// 历史数据
    /// </summary>
    public List<KPIHistoryPoint> HistoryData { get; set; } = new();

    /// <summary>
    /// 预测数据
    /// </summary>
    public List<KPIForecastPoint> ForecastData { get; set; } = new();

    /// <summary>
    /// 计算时间
    /// </summary>
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 格式化显示值
    /// </summary>
    public string FormattedValue { get; set; } = string.Empty;

    /// <summary>
    /// 分析洞察
    /// </summary>
    public List<string> Insights { get; set; } = new();
}

/// <summary>
/// KPI历史数据点
/// </summary>
public class KPIHistoryPoint
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public decimal? TargetValue { get; set; }
}

/// <summary>
/// KPI预测数据点
/// </summary>
public class KPIForecastPoint
{
    public DateTime Date { get; set; }
    public decimal PredictedValue { get; set; }
    public decimal? ConfidenceInterval { get; set; }
}

/// <summary>
/// 仪表板概览统计
/// </summary>
public class DashboardOverviewStats
{
    /// <summary>
    /// 总仪表板数
    /// </summary>
    public int TotalDashboards { get; set; }

    /// <summary>
    /// 活跃仪表板数
    /// </summary>
    public int ActiveDashboards { get; set; }

    /// <summary>
    /// 今日访问次数
    /// </summary>
    public int TodayViews { get; set; }

    /// <summary>
    /// 活跃用户数
    /// </summary>
    public int ActiveUsers { get; set; }

    /// <summary>
    /// 各类型仪表板分布
    /// </summary>
    public Dictionary<DashboardType, int> TypeDistribution { get; set; } = new();

    /// <summary>
    /// 热门仪表板TOP5
    /// </summary>
    public List<PopularDashboard> PopularDashboards { get; set; } = new();

    /// <summary>
    /// 访问趋势数据
    /// </summary>
    public List<AccessTrendPoint> AccessTrend { get; set; } = new();

    /// <summary>
    /// 平均响应时间
    /// </summary>
    public double AverageResponseTime { get; set; }

    /// <summary>
    /// 系统状态
    /// </summary>
    public SystemHealthStatus SystemHealth { get; set; }
}

/// <summary>
/// 热门仪表板
/// </summary>
public class PopularDashboard
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DashboardType Type { get; set; }
    public int ViewCount { get; set; }
    public int UniqueUsers { get; set; }
    public double AverageViewDuration { get; set; }
}

/// <summary>
/// 访问趋势数据点
/// </summary>
public class AccessTrendPoint
{
    public DateTime Date { get; set; }
    public int ViewCount { get; set; }
    public int UniqueUsers { get; set; }
}

/// <summary>
/// 实时监控数据
/// </summary>
public class RealTimeMonitorData
{
    /// <summary>
    /// 监控时间
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 在线用户数
    /// </summary>
    public int OnlineUsers { get; set; }

    /// <summary>
    /// 当前活跃会话数
    /// </summary>
    public int ActiveSessions { get; set; }

    /// <summary>
    /// 系统CPU使用率
    /// </summary>
    public double CpuUsage { get; set; }

    /// <summary>
    /// 系统内存使用率
    /// </summary>
    public double MemoryUsage { get; set; }

    /// <summary>
    /// 数据库连接数
    /// </summary>
    public int DatabaseConnections { get; set; }

    /// <summary>
    /// 实时业务指标
    /// </summary>
    public Dictionary<string, decimal> BusinessMetrics { get; set; } = new();

    /// <summary>
    /// 异常告警
    /// </summary>
    public List<SystemAlert> Alerts { get; set; } = new();
}

/// <summary>
/// 系统告警
/// </summary>
public class SystemAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public AlertLevel Level { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Source { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// 仪表板导出选项
/// </summary>
public class DashboardExportOptions
{
    /// <summary>
    /// 导出格式
    /// </summary>
    public ExportFormat Format { get; set; } = ExportFormat.PDF;

    /// <summary>
    /// 页面布局
    /// </summary>
    public PageLayout Layout { get; set; } = PageLayout.Landscape;

    /// <summary>
    /// 是否包含数据
    /// </summary>
    public bool IncludeData { get; set; } = false;

    /// <summary>
    /// 是否包含图表
    /// </summary>
    public bool IncludeCharts { get; set; } = true;

    /// <summary>
    /// 质量设置
    /// </summary>
    public ExportQuality Quality { get; set; } = ExportQuality.High;

    /// <summary>
    /// 水印文本
    /// </summary>
    public string? Watermark { get; set; }

    /// <summary>
    /// 自定义标题
    /// </summary>
    public string? CustomTitle { get; set; }

    /// <summary>
    /// 时间范围
    /// </summary>
    public DateTimeRange? TimeRange { get; set; }
}

/// <summary>
/// 时间范围
/// </summary>
public class DateTimeRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// 业务仪表板服务接口
/// </summary>
public interface IBusinessDashboardService
{
    // =================== 仪表板管理 ===================

    /// <summary>
    /// 创建仪表板
    /// </summary>
    Task<BusinessDashboard> CreateDashboardAsync(string name, DashboardType type, DashboardCategory category, DashboardLayout layout, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 更新仪表板
    /// </summary>
    Task<bool> UpdateDashboardAsync(string dashboardId, string? name = null, string? description = null, DashboardLayout? layout = null, bool? isEnabled = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 删除仪表板
    /// </summary>
    Task<bool> DeleteDashboardAsync(string dashboardId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取仪表板详情
    /// </summary>
    Task<BusinessDashboard?> GetDashboardAsync(string dashboardId, CancellationToken ct = default);

    /// <summary>
    /// 获取用户可访问的仪表板列表
    /// </summary>
    Task<List<BusinessDashboard>> GetUserDashboardsAsync(string userId, DashboardType? type = null, DashboardCategory? category = null, CancellationToken ct = default);

    /// <summary>
    /// 搜索仪表板
    /// </summary>
    Task<(List<BusinessDashboard> dashboards, int total)> SearchDashboardsAsync(string? keyword = null, DashboardType? type = null, DashboardCategory? category = null, string? userId = null, bool? isEnabled = null, int page = 1, int size = 20, CancellationToken ct = default);

    /// <summary>
    /// 复制仪表板
    /// </summary>
    Task<BusinessDashboard> CloneDashboardAsync(string sourceDashboardId, string newName, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 设置默认仪表板
    /// </summary>
    Task<bool> SetDefaultDashboardAsync(string dashboardId, string userId, CancellationToken ct = default);

    // =================== 组件管理 ===================

    /// <summary>
    /// 添加组件到仪表板
    /// </summary>
    Task<DashboardWidget> AddWidgetAsync(string dashboardId, string title, WidgetType type, WidgetDataSource dataSource, WidgetPosition position, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 更新组件
    /// </summary>
    Task<bool> UpdateWidgetAsync(string widgetId, string? title = null, WidgetDataSource? dataSource = null, WidgetPosition? position = null, WidgetStyle? style = null, WidgetDisplayConfig? displayConfig = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 删除组件
    /// </summary>
    Task<bool> DeleteWidgetAsync(string widgetId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 批量更新组件位置
    /// </summary>
    Task<bool> BatchUpdateWidgetPositionsAsync(Dictionary<string, WidgetPosition> widgetPositions, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取组件数据
    /// </summary>
    Task<DashboardDataResult> GetWidgetDataAsync(string widgetId, Dictionary<string, object>? parameters = null, CancellationToken ct = default);

    /// <summary>
    /// 刷新组件数据
    /// </summary>
    Task<DashboardDataResult> RefreshWidgetDataAsync(string widgetId, bool bypassCache = false, CancellationToken ct = default);

    /// <summary>
    /// 批量获取仪表板所有组件数据
    /// </summary>
    Task<Dictionary<string, DashboardDataResult>> GetDashboardDataAsync(string dashboardId, Dictionary<string, object>? globalParameters = null, CancellationToken ct = default);

    // =================== KPI指标管理 ===================

    /// <summary>
    /// 创建KPI指标
    /// </summary>
    Task<KPIMetric> CreateKPIMetricAsync(string name, string code, KPICategory category, string formula, WidgetDataSource dataSource, KPITarget target, KPIThresholds thresholds, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 更新KPI指标
    /// </summary>
    Task<bool> UpdateKPIMetricAsync(string metricId, string? name = null, string? formula = null, WidgetDataSource? dataSource = null, KPITarget? target = null, KPIThresholds? thresholds = null, bool? isEnabled = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 删除KPI指标
    /// </summary>
    Task<bool> DeleteKPIMetricAsync(string metricId, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取KPI指标列表
    /// </summary>
    Task<List<KPIMetric>> GetKPIMetricsAsync(KPICategory? category = null, bool enabledOnly = true, CancellationToken ct = default);

    /// <summary>
    /// 计算KPI值
    /// </summary>
    Task<KPIValueResult> CalculateKPIValueAsync(string metricId, DateTime? startDate = null, DateTime? endDate = null, Dictionary<string, object>? parameters = null, CancellationToken ct = default);

    /// <summary>
    /// 批量计算KPI值
    /// </summary>
    Task<Dictionary<string, KPIValueResult>> BatchCalculateKPIValuesAsync(List<string> metricIds, DateTime? startDate = null, DateTime? endDate = null, Dictionary<string, object>? parameters = null, CancellationToken ct = default);

    /// <summary>
    /// 获取KPI趋势数据
    /// </summary>
    Task<List<KPIHistoryPoint>> GetKPITrendDataAsync(string metricId, TrendPeriod period, CancellationToken ct = default);

    /// <summary>
    /// 预测KPI值
    /// </summary>
    Task<List<KPIForecastPoint>> ForecastKPIValueAsync(string metricId, int forecastDays = 30, CancellationToken ct = default);

    // =================== 数据源管理 ===================

    /// <summary>
    /// 测试数据源连接
    /// </summary>
    Task<(bool IsConnected, string? ErrorMessage, long ResponseTimeMs)> TestDataSourceAsync(WidgetDataSource dataSource, CancellationToken ct = default);

    /// <summary>
    /// 执行数据查询
    /// </summary>
    Task<DashboardDataResult> ExecuteDataQueryAsync(WidgetDataSource dataSource, Dictionary<string, object>? parameters = null, CancellationToken ct = default);

    /// <summary>
    /// 验证查询语法
    /// </summary>
    Task<(bool IsValid, string? ErrorMessage, List<string> Suggestions)> ValidateQuerySyntaxAsync(string query, DataSourceType dataSourceType, CancellationToken ct = default);

    /// <summary>
    /// 获取数据源模式信息
    /// </summary>
    Task<object> GetDataSourceSchemaAsync(string dataSourceId, CancellationToken ct = default);

    // =================== 权限管理 ===================

    /// <summary>
    /// 检查用户仪表板访问权限
    /// </summary>
    Task<bool> CheckDashboardAccessAsync(string dashboardId, string userId, DashboardAccessType accessType = DashboardAccessType.View, CancellationToken ct = default);

    /// <summary>
    /// 设置仪表板权限
    /// </summary>
    Task<bool> SetDashboardPermissionsAsync(string dashboardId, DashboardAccessLevel accessLevel, List<string>? allowedRoles = null, List<string>? allowedUsers = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 分享仪表板
    /// </summary>
    Task<string> ShareDashboardAsync(string dashboardId, List<string> targetUsers, DateTime? expiryDate = null, bool allowEdit = false, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 取消分享仪表板
    /// </summary>
    Task<bool> UnshareDashboardAsync(string shareId, string? userId = null, CancellationToken ct = default);

    // =================== 缓存管理 ===================

    /// <summary>
    /// 清除组件缓存
    /// </summary>
    Task<bool> ClearWidgetCacheAsync(string widgetId, CancellationToken ct = default);

    /// <summary>
    /// 清除仪表板缓存
    /// </summary>
    Task<bool> ClearDashboardCacheAsync(string dashboardId, CancellationToken ct = default);

    /// <summary>
    /// 预热缓存
    /// </summary>
    Task<int> WarmupCacheAsync(List<string>? dashboardIds = null, CancellationToken ct = default);

    /// <summary>
    /// 获取缓存统计
    /// </summary>
    Task<object> GetCacheStatisticsAsync(CancellationToken ct = default);

    // =================== 实时监控 ===================

    /// <summary>
    /// 获取实时监控数据
    /// </summary>
    Task<RealTimeMonitorData> GetRealTimeDataAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取系统性能指标
    /// </summary>
    Task<object> GetSystemPerformanceMetricsAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取活跃用户统计
    /// </summary>
    Task<object> GetActiveUserStatisticsAsync(CancellationToken ct = default);

    // =================== 统计分析 ===================

    /// <summary>
    /// 获取仪表板概览统计
    /// </summary>
    Task<DashboardOverviewStats> GetDashboardOverviewAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取仪表板使用分析
    /// </summary>
    Task<object> GetDashboardUsageAnalysisAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);

    /// <summary>
    /// 获取用户行为分析
    /// </summary>
    Task<object> GetUserBehaviorAnalysisAsync(string? userId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);

    /// <summary>
    /// 获取热门内容分析
    /// </summary>
    Task<object> GetPopularContentAnalysisAsync(int topN = 10, CancellationToken ct = default);

    // =================== 导入导出 ===================

    /// <summary>
    /// 导出仪表板配置
    /// </summary>
    Task<byte[]> ExportDashboardConfigAsync(string dashboardId, bool includeData = false, CancellationToken ct = default);

    /// <summary>
    /// 导入仪表板配置
    /// </summary>
    Task<BusinessDashboard> ImportDashboardConfigAsync(byte[] configData, string? newName = null, string? userId = null, CancellationToken ct = default);

    /// <summary>
    /// 导出仪表板为图片/PDF
    /// </summary>
    Task<byte[]> ExportDashboardAsync(string dashboardId, DashboardExportOptions options, CancellationToken ct = default);

    /// <summary>
    /// 生成仪表板报告
    /// </summary>
    Task<byte[]> GenerateDashboardReportAsync(string dashboardId, string templateId, Dictionary<string, object>? parameters = null, CancellationToken ct = default);

    // =================== 访问日志 ===================

    /// <summary>
    /// 记录仪表板访问
    /// </summary>
    Task<bool> LogDashboardAccessAsync(string dashboardId, string? userId, DashboardAccessType accessType, string? sourceIP = null, string? userAgent = null, CancellationToken ct = default);

    /// <summary>
    /// 获取访问日志
    /// </summary>
    Task<List<DashboardAccessLog>> GetAccessLogsAsync(string? dashboardId = null, string? userId = null, DateTime? fromDate = null, DateTime? toDate = null, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// 清理过期访问日志
    /// </summary>
    Task<int> CleanupExpiredAccessLogsAsync(int retentionDays = 90, CancellationToken ct = default);

    // =================== 数据刷新调度 ===================

    /// <summary>
    /// 启动数据刷新调度
    /// </summary>
    Task<bool> StartDataRefreshSchedulerAsync(CancellationToken ct = default);

    /// <summary>
    /// 停止数据刷新调度
    /// </summary>
    Task<bool> StopDataRefreshSchedulerAsync(CancellationToken ct = default);

    /// <summary>
    /// 手动触发数据刷新
    /// </summary>
    Task<bool> TriggerDataRefreshAsync(string? dashboardId = null, string? widgetId = null, CancellationToken ct = default);

    /// <summary>
    /// 获取刷新任务状态
    /// </summary>
    Task<object> GetRefreshTaskStatusAsync(CancellationToken ct = default);
}

// =================== 辅助枚举 ===================

/// <summary>
/// 趋势方向
/// </summary>
public enum TrendDirection
{
    Up = 1,
    Down = 2,
    Stable = 3
}

/// <summary>
/// KPI状态等级
/// </summary>
public enum KPIStatusLevel
{
    Excellent = 1,
    Good = 2,
    Normal = 3,
    Warning = 4,
    Danger = 5
}

/// <summary>
/// 系统健康状态
/// </summary>
public enum SystemHealthStatus
{
    Healthy = 1,
    Warning = 2,
    Critical = 3,
    Unknown = 4
}

/// <summary>
/// 告警级别
/// </summary>
public enum AlertLevel
{
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

/// <summary>
/// 导出格式
/// </summary>
public enum ExportFormat
{
    PDF = 1,
    PNG = 2,
    JPEG = 3,
    Excel = 4,
    CSV = 5,
    JSON = 6
}

/// <summary>
/// 页面布局
/// </summary>
public enum PageLayout
{
    Portrait = 1,
    Landscape = 2
}

/// <summary>
/// 导出质量
/// </summary>
public enum ExportQuality
{
    Low = 1,
    Medium = 2,
    High = 3,
    Ultra = 4
}
#endif
