// Disabled in minimal build: Business dashboard API not enabled
#if false
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Dashboard;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HeYuanERP.Api.Controllers;

/// <summary>
/// 业务仪表板管理接口
/// </summary>
[ApiController]
[Route("api/dashboard")]
[Authorize(Policy = "Permission")]
public class BusinessDashboardController : ControllerBase
{
    private readonly IBusinessDashboardService _dashboardService;
    private readonly ILogger<BusinessDashboardController> _logger;

    public BusinessDashboardController(
        IBusinessDashboardService dashboardService,
        ILogger<BusinessDashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // =================== 仪表板管理 ===================

    /// <summary>
    /// 创建仪表板
    /// </summary>
    [HttpPost]
    [RequirePermission("dashboard.create")]
    public async Task<IActionResult> CreateDashboardAsync(
        [FromBody] CreateDashboardRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var dashboard = await _dashboardService.CreateDashboardAsync(
                request.Name, request.Type, request.Category, request.Layout, CurrentUserId, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Id = dashboard.Id,
                    Name = dashboard.Name,
                    Type = dashboard.Type.ToString(),
                    Category = dashboard.Category.ToString()
                },
                Message = "仪表板创建成功"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dashboard");
            return StatusCode(500, new { Success = false, Message = "创建仪表板时发生错误" });
    }
}


    /// <summary>
    /// 更新仪表板
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission("dashboard.edit")]
    public async Task<IActionResult> UpdateDashboardAsync(
        [FromRoute] string id,
        [FromBody] UpdateDashboardRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _dashboardService.UpdateDashboardAsync(
                id, request.Name, request.Description, request.Layout, request.IsEnabled, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "仪表板更新成功"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "仪表板不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dashboard {DashboardId}", id);
            return StatusCode(500, new { Success = false, Message = "更新仪表板时发生错误" });
        }
    }

    /// <summary>
    /// 删除仪表板
    /// </summary>
    [HttpDelete("{id}")]
    [RequirePermission("dashboard.delete")]
    public async Task<IActionResult> DeleteDashboardAsync(
        [FromRoute] string id,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _dashboardService.DeleteDashboardAsync(id, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "仪表板删除成功"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "仪表板不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dashboard {DashboardId}", id);
            return StatusCode(500, new { Success = false, Message = "删除仪表板时发生错误" });
        }
    }

    /// <summary>
    /// 获取仪表板详情
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> GetDashboardAsync(
        [FromRoute] string id,
        CancellationToken ct = default)
    {
        try
        {
            // 记录访问日志
            await _dashboardService.LogDashboardAccessAsync(id, CurrentUserId, DashboardAccessType.View, ct: ct);

            var dashboard = await _dashboardService.GetDashboardAsync(id, ct);

            if (dashboard != null)
            {
                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        Id = dashboard.Id,
                        Name = dashboard.Name,
                        Description = dashboard.Description,
                        Type = dashboard.Type.ToString(),
                        Category = dashboard.Category.ToString(),
                        Layout = new
                        {
                            Type = dashboard.Layout.Type.ToString(),
                            Columns = dashboard.Layout.Columns,
                            RowHeight = dashboard.Layout.RowHeight,
                            Draggable = dashboard.Layout.Draggable,
                            Resizable = dashboard.Layout.Resizable,
                            Theme = dashboard.Layout.Theme
                        },
                        RefreshInterval = dashboard.RefreshInterval,
                        AutoRefresh = dashboard.AutoRefresh,
                        AccessLevel = dashboard.AccessLevel.ToString(),
                        IsDefault = dashboard.IsDefault,
                        IsEnabled = dashboard.IsEnabled,
                        Tags = dashboard.Tags,
                        Widgets = dashboard.Widgets.Where(w => w.IsEnabled).Select(w => new
                        {
                            Id = w.Id,
                            Title = w.Title,
                            Type = w.Type.ToString(),
                            Position = new
                            {
                                X = w.Position.X,
                                Y = w.Position.Y,
                                Width = w.Position.Width,
                                Height = w.Position.Height
                            },
                            Style = w.Style,
                            DisplayConfig = w.DisplayConfig,
                            DataSourceType = w.DataSourceType.ToString(),
                            CacheConfig = w.CacheConfig
                        }).OrderBy(w => w.Position.Y).ThenBy(w => w.Position.X).ToList(),
                        CreatedAt = dashboard.CreatedAt,
                        UpdatedAt = dashboard.UpdatedAt
                    }
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "仪表板不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard {DashboardId}", id);
            return StatusCode(500, new { Success = false, Message = "获取仪表板时发生错误" });
        }
    }

    /// <summary>
    /// 获取用户仪表板列表
    /// </summary>
    [HttpGet]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> GetUserDashboardsAsync(
        [FromQuery] DashboardType? type = null,
        [FromQuery] DashboardCategory? category = null,
        CancellationToken ct = default)
    {
        try
        {
            var dashboards = await _dashboardService.GetUserDashboardsAsync(CurrentUserId, type, category, ct);

            var dashboardList = dashboards.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Type = d.Type.ToString(),
                Category = d.Category.ToString(),
                IsDefault = d.IsDefault,
                WidgetCount = d.Widgets.Count(w => w.IsEnabled),
                LastUpdated = d.UpdatedAt ?? d.CreatedAt,
                Tags = d.Tags
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = dashboardList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user dashboards");
            return StatusCode(500, new { Success = false, Message = "获取仪表板列表时发生错误" });
        }
    }

    /// <summary>
    /// 搜索仪表板
    /// </summary>
    [HttpGet("search")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> SearchDashboardsAsync(
        [FromQuery] string? keyword = null,
        [FromQuery] DashboardType? type = null,
        [FromQuery] DashboardCategory? category = null,
        [FromQuery] bool? isEnabled = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        try
        {
            var (dashboards, total) = await _dashboardService.SearchDashboardsAsync(
                keyword, type, category, CurrentUserId, isEnabled, page, size, ct);

            var dashboardList = dashboards.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Type = d.Type.ToString(),
                Category = d.Category.ToString(),
                IsEnabled = d.IsEnabled,
                WidgetCount = d.Widgets.Count(w => w.IsEnabled),
                CreatedBy = d.CreatedBy,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Items = dashboardList,
                    Total = total,
                    Page = page,
                    Size = size,
                    TotalPages = (int)Math.Ceiling((double)total / size)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching dashboards");
            return StatusCode(500, new { Success = false, Message = "搜索仪表板时发生错误" });
        }
    }

    // =================== 组件管理 ===================

    /// <summary>
    /// 添加组件到仪表板
    /// </summary>
    [HttpPost("{dashboardId}/widgets")]
    [RequirePermission("dashboard.edit")]
    public async Task<IActionResult> AddWidgetAsync(
        [FromRoute] string dashboardId,
        [FromBody] AddWidgetRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var widget = await _dashboardService.AddWidgetAsync(
                dashboardId, request.Title, request.Type, request.DataSource, request.Position, CurrentUserId, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Id = widget.Id,
                    Title = widget.Title,
                    Type = widget.Type.ToString()
                },
                Message = "组件添加成功"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding widget to dashboard {DashboardId}", dashboardId);
            return StatusCode(500, new { Success = false, Message = "添加组件时发生错误" });
        }
    }

    /// <summary>
    /// 更新组件
    /// </summary>
    [HttpPut("widgets/{widgetId}")]
    [RequirePermission("dashboard.edit")]
    public async Task<IActionResult> UpdateWidgetAsync(
        [FromRoute] string widgetId,
        [FromBody] UpdateWidgetRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _dashboardService.UpdateWidgetAsync(
                widgetId, request.Title, request.DataSource, request.Position,
                request.Style, request.DisplayConfig, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "组件更新成功"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "组件不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating widget {WidgetId}", widgetId);
            return StatusCode(500, new { Success = false, Message = "更新组件时发生错误" });
        }
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    [HttpDelete("widgets/{widgetId}")]
    [RequirePermission("dashboard.edit")]
    public async Task<IActionResult> DeleteWidgetAsync(
        [FromRoute] string widgetId,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _dashboardService.DeleteWidgetAsync(widgetId, CurrentUserId, ct);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "组件删除成功"
                });
            }
            else
            {
                return NotFound(new { Success = false, Message = "组件不存在" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting widget {WidgetId}", widgetId);
            return StatusCode(500, new { Success = false, Message = "删除组件时发生错误" });
        }
    }

    /// <summary>
    /// 获取组件数据
    /// </summary>
    [HttpGet("widgets/{widgetId}/data")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> GetWidgetDataAsync(
        [FromRoute] string widgetId,
        [FromQuery] Dictionary<string, object>? parameters = null,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _dashboardService.GetWidgetDataAsync(widgetId, parameters, ct);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Meta = new
                    {
                        RowCount = result.RowCount,
                        DataSource = result.DataSource,
                        ExecutionTimeMs = result.ExecutionTimeMs,
                        FromCache = result.FromCache,
                        LastUpdated = result.LastUpdated
                    }
                });
            }
            else
            {
                return BadRequest(new { Success = false, Message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting widget data for {WidgetId}", widgetId);
            return StatusCode(500, new { Success = false, Message = "获取组件数据时发生错误" });
        }
    }

    /// <summary>
    /// 获取仪表板所有组件数据
    /// </summary>
    [HttpGet("{dashboardId}/data")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> GetDashboardDataAsync(
        [FromRoute] string dashboardId,
        [FromQuery] Dictionary<string, object>? parameters = null,
        CancellationToken ct = default)
    {
        try
        {
            var results = await _dashboardService.GetDashboardDataAsync(dashboardId, parameters, ct);

            var responseData = results.ToDictionary(
                kvp => kvp.Key,
                kvp => new
                {
                    Success = kvp.Value.IsSuccess,
                    Data = kvp.Value.Data,
                    RowCount = kvp.Value.RowCount,
                    ExecutionTimeMs = kvp.Value.ExecutionTimeMs,
                    FromCache = kvp.Value.FromCache,
                    ErrorMessage = kvp.Value.ErrorMessage,
                    LastUpdated = kvp.Value.LastUpdated
                });

            return Ok(new
            {
                Success = true,
                Data = responseData,
                Summary = new
                {
                    TotalWidgets = results.Count,
                    SuccessfulWidgets = results.Count(r => r.Value.IsSuccess),
                    FailedWidgets = results.Count(r => !r.Value.IsSuccess),
                    TotalExecutionTimeMs = results.Sum(r => r.Value.ExecutionTimeMs),
                    CachedWidgets = results.Count(r => r.Value.FromCache)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data for {DashboardId}", dashboardId);
            return StatusCode(500, new { Success = false, Message = "获取仪表板数据时发生错误" });
        }
    }

    // =================== KPI指标管理 ===================

    /// <summary>
    /// 获取KPI指标列表
    /// </summary>
    [HttpGet("kpi/metrics")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> GetKPIMetricsAsync(
        [FromQuery] KPICategory? category = null,
        CancellationToken ct = default)
    {
        try
        {
            var metrics = await _dashboardService.GetKPIMetricsAsync(category, true, ct);

            var metricList = metrics.Select(m => new
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                Category = m.Category.ToString(),
                Unit = m.Unit,
                Description = m.Description,
                Target = new
                {
                    Type = m.Target.Type.ToString(),
                    Value = m.Target.Value,
                    Period = m.Target.Period.ToString()
                },
                Thresholds = new
                {
                    ExcellentThreshold = m.Thresholds.ExcellentThreshold,
                    GoodThreshold = m.Thresholds.GoodThreshold,
                    WarningThreshold = m.Thresholds.WarningThreshold,
                    DangerThreshold = m.Thresholds.DangerThreshold,
                    ComparisonType = m.Thresholds.ComparisonType.ToString()
                },
                IsEnabled = m.IsEnabled
            }).ToList();

            return Ok(new
            {
                Success = true,
                Data = metricList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting KPI metrics");
            return StatusCode(500, new { Success = false, Message = "获取KPI指标时发生错误" });
        }
    }

    /// <summary>
    /// 计算KPI值
    /// </summary>
    [HttpPost("kpi/metrics/{metricId}/calculate")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> CalculateKPIValueAsync(
        [FromRoute] string metricId,
        [FromBody] CalculateKPIRequest? request = null,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _dashboardService.CalculateKPIValueAsync(
                metricId, request?.StartDate, request?.EndDate, request?.Parameters, ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    MetricId = result.MetricId,
                    CurrentValue = result.CurrentValue,
                    FormattedValue = result.FormattedValue,
                    TargetValue = result.TargetValue,
                    CompletionRate = result.CompletionRate,
                    YearOverYearValue = result.YearOverYearValue,
                    YearOverYearGrowthRate = result.YearOverYearGrowthRate,
                    PeriodOverPeriodValue = result.PeriodOverPeriodValue,
                    PeriodOverPeriodGrowthRate = result.PeriodOverPeriodGrowthRate,
                    TrendDirection = result.TrendDirection.ToString(),
                    StatusLevel = result.StatusLevel.ToString(),
                    StatusColor = result.StatusColor,
                    Insights = result.Insights,
                    HistoryData = result.HistoryData.Select(h => new
                    {
                        Date = h.Date,
                        Value = h.Value,
                        TargetValue = h.TargetValue
                    }).ToList(),
                    Unit = result.Unit,
                    CalculatedAt = result.CalculatedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating KPI value for metric {MetricId}", metricId);
            return StatusCode(500, new { Success = false, Message = "计算KPI值时发生错误" });
        }
    }

    // =================== 数据源管理 ===================

    /// <summary>
    /// 执行数据查询
    /// </summary>
    [HttpPost("data-source/execute")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> ExecuteDataQueryAsync(
        [FromBody] ExecuteQueryRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _dashboardService.ExecuteDataQueryAsync(request.DataSource, request.Parameters, ct);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    RowCount = result.RowCount,
                    ExecutionTimeMs = result.ExecutionTimeMs,
                    DataSource = result.DataSource,
                    LastUpdated = result.LastUpdated
                });
            }
            else
            {
                return BadRequest(new { Success = false, Message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing data query");
            return StatusCode(500, new { Success = false, Message = "执行数据查询时发生错误" });
        }
    }

    // =================== 统计分析 ===================

    /// <summary>
    /// 获取仪表板概览统计
    /// </summary>
    [HttpGet("overview")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> GetDashboardOverviewAsync(CancellationToken ct = default)
    {
        try
        {
            var overview = await _dashboardService.GetDashboardOverviewAsync(ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    TotalDashboards = overview.TotalDashboards,
                    ActiveDashboards = overview.ActiveDashboards,
                    TodayViews = overview.TodayViews,
                    ActiveUsers = overview.ActiveUsers,
                    TypeDistribution = overview.TypeDistribution.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value),
                    PopularDashboards = overview.PopularDashboards.Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Type = p.Type.ToString(),
                        ViewCount = p.ViewCount,
                        UniqueUsers = p.UniqueUsers,
                        AverageViewDuration = p.AverageViewDuration
                    }).ToList(),
                    AccessTrend = overview.AccessTrend.Select(t => new
                    {
                        Date = t.Date.ToString("yyyy-MM-dd"),
                        ViewCount = t.ViewCount,
                        UniqueUsers = t.UniqueUsers
                    }).ToList(),
                    AverageResponseTime = overview.AverageResponseTime,
                    SystemHealth = overview.SystemHealth.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard overview");
            return StatusCode(500, new { Success = false, Message = "获取仪表板概览时发生错误" });
        }
    }

    /// <summary>
    /// 获取实时监控数据
    /// </summary>
    [HttpGet("real-time")]
    [RequirePermission("dashboard.read")]
    public async Task<IActionResult> GetRealTimeDataAsync(CancellationToken ct = default)
    {
        try
        {
            var realTimeData = await _dashboardService.GetRealTimeDataAsync(ct);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Timestamp = realTimeData.Timestamp,
                    OnlineUsers = realTimeData.OnlineUsers,
                    ActiveSessions = realTimeData.ActiveSessions,
                    SystemMetrics = new
                    {
                        CpuUsage = realTimeData.CpuUsage,
                        MemoryUsage = realTimeData.MemoryUsage,
                        DatabaseConnections = realTimeData.DatabaseConnections
                    },
                    BusinessMetrics = realTimeData.BusinessMetrics,
                    Alerts = realTimeData.Alerts.Select(a => new
                    {
                        Id = a.Id,
                        Level = a.Level.ToString(),
                        Title = a.Title,
                        Message = a.Message,
                        CreatedAt = a.CreatedAt,
                        Source = a.Source
                    }).ToList()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting real-time data");
            return StatusCode(500, new { Success = false, Message = "获取实时数据时发生错误" });
        }
    }

    // =================== 系统管理 ===================

    /// <summary>
    /// 获取仪表板类型定义
    /// </summary>
    [HttpGet("types")]
    [RequirePermission("dashboard.read")]
    public IActionResult GetDashboardTypes()
    {
        var types = Enum.GetValues<DashboardType>().Select(t => new
        {
            Value = (int)t,
            Name = t.ToString(),
            DisplayName = t switch
            {
                DashboardType.Overview => "概览仪表板",
                DashboardType.Sales => "销售仪表板",
                DashboardType.Finance => "财务仪表板",
                DashboardType.Inventory => "库存仪表板",
                DashboardType.Customer => "客户仪表板",
                DashboardType.Supplier => "供应商仪表板",
                DashboardType.Operations => "运营仪表板",
                DashboardType.Quality => "质量仪表板",
                DashboardType.Custom => "自定义仪表板",
                _ => t.ToString()
            }
        }).ToList();

        return Ok(new { Success = true, Data = types });
    }

    /// <summary>
    /// 获取组件类型定义
    /// </summary>
    [HttpGet("widget-types")]
    [RequirePermission("dashboard.read")]
    public IActionResult GetWidgetTypes()
    {
        var types = Enum.GetValues<WidgetType>().Select(t => new
        {
            Value = (int)t,
            Name = t.ToString(),
            DisplayName = t switch
            {
                WidgetType.Card => "数据卡片",
                WidgetType.LineChart => "折线图",
                WidgetType.BarChart => "柱状图",
                WidgetType.PieChart => "饼图",
                WidgetType.DonutChart => "环形图",
                WidgetType.AreaChart => "面积图",
                WidgetType.ScatterChart => "散点图",
                WidgetType.GaugeChart => "仪表盘",
                WidgetType.RadarChart => "雷达图",
                WidgetType.HeatmapChart => "热力图",
                WidgetType.Table => "数据表格",
                WidgetType.ProgressBar => "进度条",
                WidgetType.Text => "文本显示",
                WidgetType.Image => "图片",
                WidgetType.KPI => "KPI指标",
                WidgetType.RealTimeMonitor => "实时监控",
                _ => t.ToString()
            },
            Category = t switch
            {
                WidgetType.Card or WidgetType.KPI or WidgetType.ProgressBar => "数据显示",
                WidgetType.LineChart or WidgetType.BarChart or WidgetType.PieChart or WidgetType.DonutChart or WidgetType.AreaChart => "基础图表",
                WidgetType.ScatterChart or WidgetType.GaugeChart or WidgetType.RadarChart or WidgetType.HeatmapChart => "高级图表",
                WidgetType.Table or WidgetType.Text or WidgetType.Image => "内容组件",
                WidgetType.RealTimeMonitor => "监控组件",
                _ => "其他"
            }
        }).ToList();

        return Ok(new { Success = true, Data = types });
    }
}

// =================== DTO类 ===================

/// <summary>
/// 创建仪表板请求
/// </summary>
public class CreateDashboardRequest
{
    public string Name { get; set; } = string.Empty;
    public DashboardType Type { get; set; }
    public DashboardCategory Category { get; set; }
    public DashboardLayout Layout { get; set; } = new();
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// 更新仪表板请求
/// </summary>
public class UpdateDashboardRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DashboardLayout? Layout { get; set; }
    public bool? IsEnabled { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// 添加组件请求
/// </summary>
public class AddWidgetRequest
{
    public string Title { get; set; } = string.Empty;
    public WidgetType Type { get; set; }
    public WidgetDataSource DataSource { get; set; } = new();
    public WidgetPosition Position { get; set; } = new();
    public WidgetStyle? Style { get; set; }
    public WidgetDisplayConfig? DisplayConfig { get; set; }
}

/// <summary>
/// 更新组件请求
/// </summary>
public class UpdateWidgetRequest
{
    public string? Title { get; set; }
    public WidgetDataSource? DataSource { get; set; }
    public WidgetPosition? Position { get; set; }
    public WidgetStyle? Style { get; set; }
    public WidgetDisplayConfig? DisplayConfig { get; set; }
}

/// <summary>
/// 计算KPI请求
/// </summary>
public class CalculateKPIRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// 执行查询请求
/// </summary>
public class ExecuteQueryRequest
{
    public WidgetDataSource DataSource { get; set; } = new();
    public Dictionary<string, object>? Parameters { get; set; }
}
#endif
