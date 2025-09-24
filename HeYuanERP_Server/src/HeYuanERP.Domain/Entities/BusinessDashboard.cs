#if false
namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 业务仪表板配置 - 定义仪表板的布局和组件
/// </summary>
public class BusinessDashboard
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 仪表板名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 仪表板描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 仪表板类型
    /// </summary>
    public DashboardType Type { get; set; } = DashboardType.Overview;

    /// <summary>
    /// 仪表板分类
    /// </summary>
    public DashboardCategory Category { get; set; } = DashboardCategory.Management;

    /// <summary>
    /// 布局配置（JSON格式）
    /// </summary>
    public DashboardLayout Layout { get; set; } = new();

    /// <summary>
    /// 刷新频率（秒）
    /// </summary>
    public int RefreshInterval { get; set; } = 300; // 5分钟

    /// <summary>
    /// 是否启用自动刷新
    /// </summary>
    public bool AutoRefresh { get; set; } = true;

    /// <summary>
    /// 访问权限级别
    /// </summary>
    public DashboardAccessLevel AccessLevel { get; set; } = DashboardAccessLevel.Department;

    /// <summary>
    /// 允许访问的角色列表
    /// </summary>
    public List<string> AllowedRoles { get; set; } = new();

    /// <summary>
    /// 允许访问的用户ID列表
    /// </summary>
    public List<string> AllowedUsers { get; set; } = new();

    /// <summary>
    /// 是否为默认仪表板
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int SortOrder { get; set; } = 100;

    /// <summary>
    /// 标签
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// 扩展配置
    /// </summary>
    public Dictionary<string, object> ExtensionConfig { get; set; } = new();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 导航属性：仪表板组件
    /// </summary>
    public List<DashboardWidget> Widgets { get; set; } = new();

    /// <summary>
    /// 导航属性：访问日志
    /// </summary>
    public List<DashboardAccessLog> AccessLogs { get; set; } = new();
}

/// <summary>
/// 仪表板布局配置
/// </summary>
public class DashboardLayout
{
    /// <summary>
    /// 布局类型
    /// </summary>
    public LayoutType Type { get; set; } = LayoutType.Grid;

    /// <summary>
    /// 网格列数
    /// </summary>
    public int Columns { get; set; } = 12;

    /// <summary>
    /// 网格行高
    /// </summary>
    public int RowHeight { get; set; } = 80;

    /// <summary>
    /// 是否可拖拽调整
    /// </summary>
    public bool Draggable { get; set; } = true;

    /// <summary>
    /// 是否可调整大小
    /// </summary>
    public bool Resizable { get; set; } = true;

    /// <summary>
    /// 边距配置
    /// </summary>
    public LayoutMargin Margin { get; set; } = new();

    /// <summary>
    /// 主题配置
    /// </summary>
    public DashboardTheme Theme { get; set; } = new();
}

/// <summary>
/// 布局边距配置
/// </summary>
public class LayoutMargin
{
    public int Top { get; set; } = 10;
    public int Right { get; set; } = 10;
    public int Bottom { get; set; } = 10;
    public int Left { get; set; } = 10;
}

/// <summary>
/// 仪表板主题配置
/// </summary>
public class DashboardTheme
{
    /// <summary>
    /// 主题名称
    /// </summary>
    public string Name { get; set; } = "default";

    /// <summary>
    /// 主色调
    /// </summary>
    public string PrimaryColor { get; set; } = "#1890ff";

    /// <summary>
    /// 背景色
    /// </summary>
    public string BackgroundColor { get; set; } = "#f0f2f5";

    /// <summary>
    /// 卡片背景色
    /// </summary>
    public string CardBackgroundColor { get; set; } = "#ffffff";

    /// <summary>
    /// 文字颜色
    /// </summary>
    public string TextColor { get; set; } = "#000000";

    /// <summary>
    /// 边框颜色
    /// </summary>
    public string BorderColor { get; set; } = "#d9d9d9";
}

/// <summary>
/// 仪表板组件
/// </summary>
public class DashboardWidget
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 仪表板ID
    /// </summary>
    public string DashboardId { get; set; } = string.Empty;

    /// <summary>
    /// 组件标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType Type { get; set; } = WidgetType.Card;

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType DataSourceType { get; set; } = DataSourceType.SQL;

    /// <summary>
    /// 数据源配置
    /// </summary>
    public WidgetDataSource DataSource { get; set; } = new();

    /// <summary>
    /// 位置配置
    /// </summary>
    public WidgetPosition Position { get; set; } = new();

    /// <summary>
    /// 样式配置
    /// </summary>
    public WidgetStyle Style { get; set; } = new();

    /// <summary>
    /// 显示配置
    /// </summary>
    public WidgetDisplayConfig DisplayConfig { get; set; } = new();

    /// <summary>
    /// 交互配置
    /// </summary>
    public WidgetInteractionConfig InteractionConfig { get; set; } = new();

    /// <summary>
    /// 缓存配置
    /// </summary>
    public WidgetCacheConfig CacheConfig { get; set; } = new();

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int SortOrder { get; set; } = 100;

    /// <summary>
    /// 扩展配置
    /// </summary>
    public Dictionary<string, object> ExtensionConfig { get; set; } = new();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 导航属性：关联仪表板
    /// </summary>
    public BusinessDashboard? Dashboard { get; set; }
}

/// <summary>
/// 组件数据源配置
/// </summary>
public class WidgetDataSource
{
    /// <summary>
    /// 数据源ID
    /// </summary>
    public string? SourceId { get; set; }

    /// <summary>
    /// 查询语句或API地址
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// 查询参数
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// 数据转换配置
    /// </summary>
    public DataTransformConfig TransformConfig { get; set; } = new();

    /// <summary>
    /// 刷新频率（秒）
    /// </summary>
    public int RefreshInterval { get; set; } = 300;

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// 数据转换配置
/// </summary>
public class DataTransformConfig
{
    /// <summary>
    /// 字段映射
    /// </summary>
    public Dictionary<string, string> FieldMapping { get; set; } = new();

    /// <summary>
    /// 数据过滤条件
    /// </summary>
    public List<DataFilter> Filters { get; set; } = new();

    /// <summary>
    /// 聚合配置
    /// </summary>
    public List<DataAggregation> Aggregations { get; set; } = new();

    /// <summary>
    /// 排序配置
    /// </summary>
    public List<DataSort> Sorts { get; set; } = new();
}

/// <summary>
/// 数据过滤器
/// </summary>
public class DataFilter
{
    public string Field { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; } = FilterOperator.Equals;
    public object Value { get; set; } = string.Empty;
}

/// <summary>
/// 数据聚合配置
/// </summary>
public class DataAggregation
{
    public string Field { get; set; } = string.Empty;
    public AggregationFunction Function { get; set; } = AggregationFunction.Sum;
    public string? Alias { get; set; }
    public string? GroupBy { get; set; }
}

/// <summary>
/// 数据排序配置
/// </summary>
public class DataSort
{
    public string Field { get; set; } = string.Empty;
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}

// (disabled entire file in minimal build)

/// <summary>
/// 组件位置配置
/// </summary>
public class WidgetPosition
{
    /// <summary>
    /// X坐标（网格列）
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y坐标（网格行）
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// 宽度（网格列数）
    /// </summary>
    public int Width { get; set; } = 4;

    /// <summary>
    /// 高度（网格行数）
    /// </summary>
    public int Height { get; set; } = 2;

    /// <summary>
    /// 最小宽度
    /// </summary>
    public int MinWidth { get; set; } = 2;

    /// <summary>
    /// 最小高度
    /// </summary>
    public int MinHeight { get; set; } = 1;

    /// <summary>
    /// 最大宽度
    /// </summary>
    public int? MaxWidth { get; set; }

    /// <summary>
    /// 最大高度
    /// </summary>
    public int? MaxHeight { get; set; }
}

/// <summary>
/// 组件样式配置
/// </summary>
public class WidgetStyle
{
    /// <summary>
    /// 背景色
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// 边框颜色
    /// </summary>
    public string? BorderColor { get; set; }

    /// <summary>
    /// 边框宽度
    /// </summary>
    public int BorderWidth { get; set; } = 1;

    /// <summary>
    /// 圆角半径
    /// </summary>
    public int BorderRadius { get; set; } = 4;

    /// <summary>
    /// 内边距
    /// </summary>
    public LayoutMargin Padding { get; set; } = new();

    /// <summary>
    /// 外边距
    /// </summary>
    public LayoutMargin Margin { get; set; } = new();

    /// <summary>
    /// 阴影配置
    /// </summary>
    public string? BoxShadow { get; set; }

    /// <summary>
    /// 字体配置
    /// </summary>
    public FontConfig Font { get; set; } = new();
}

/// <summary>
/// 字体配置
/// </summary>
public class FontConfig
{
    public string Family { get; set; } = "Arial, sans-serif";
    public int Size { get; set; } = 14;
    public string Weight { get; set; } = "normal";
    public string Color { get; set; } = "#000000";
}

// (region continues until end for minimal build)

/// <summary>
/// 组件显示配置
/// </summary>
public class WidgetDisplayConfig
{
    /// <summary>
    /// 是否显示标题
    /// </summary>
    public bool ShowTitle { get; set; } = true;

    /// <summary>
    /// 是否显示边框
    /// </summary>
    public bool ShowBorder { get; set; } = true;

    /// <summary>
    /// 是否显示加载指示器
    /// </summary>
    public bool ShowLoading { get; set; } = true;

    /// <summary>
    /// 是否显示错误信息
    /// </summary>
    public bool ShowError { get; set; } = true;

    /// <summary>
    /// 是否显示工具栏
    /// </summary>
    public bool ShowToolbar { get; set; } = false;

    /// <summary>
    /// 数字格式化配置
    /// </summary>
    public NumberFormatConfig NumberFormat { get; set; } = new();

    /// <summary>
    /// 日期格式化配置
    /// </summary>
    public DateFormatConfig DateFormat { get; set; } = new();

    /// <summary>
    /// 颜色配置
    /// </summary>
    public List<string> ColorPalette { get; set; } = new() { "#1890ff", "#52c41a", "#faad14", "#f5222d", "#722ed1" };
}

/// <summary>
/// 数字格式化配置
/// </summary>
public class NumberFormatConfig
{
    public int DecimalPlaces { get; set; } = 2;
    public bool UseThousandSeparator { get; set; } = true;
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public string? Unit { get; set; }
}

/// <summary>
/// 日期格式化配置
/// </summary>
public class DateFormatConfig
{
    public string Format { get; set; } = "yyyy-MM-dd HH:mm:ss";
    public string? Timezone { get; set; }
}

/// <summary>
/// 组件交互配置
/// </summary>
public class WidgetInteractionConfig
{
    /// <summary>
    /// 是否可点击
    /// </summary>
    public bool Clickable { get; set; } = false;

    /// <summary>
    /// 点击跳转URL
    /// </summary>
    public string? ClickUrl { get; set; }

    /// <summary>
    /// 是否支持钻取
    /// </summary>
    public bool SupportDrillDown { get; set; } = false;

    /// <summary>
    /// 钻取配置
    /// </summary>
    public List<DrillDownConfig> DrillDownConfigs { get; set; } = new();

    /// <summary>
    /// 是否支持筛选
    /// </summary>
    public bool SupportFilter { get; set; } = false;

    /// <summary>
    /// 筛选字段配置
    /// </summary>
    public List<FilterField> FilterFields { get; set; } = new();
}

/// <summary>
/// 钻取配置
/// </summary>
public class DrillDownConfig
{
    public string Field { get; set; } = string.Empty;
    public string TargetDashboardId { get; set; } = string.Empty;
    public string TargetWidgetId { get; set; } = string.Empty;
    public Dictionary<string, string> ParameterMapping { get; set; } = new();
}

/// <summary>
/// 筛选字段配置
/// </summary>
public class FilterField
{
    public string Field { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public FilterFieldType Type { get; set; } = FilterFieldType.Text;
    public List<FilterOption> Options { get; set; } = new();
}

/// <summary>
/// 筛选选项
/// </summary>
public class FilterOption
{
    public string Label { get; set; } = string.Empty;
    public object Value { get; set; } = string.Empty;
}

/// <summary>
/// 组件缓存配置
/// </summary>
public class WidgetCacheConfig
{
    /// <summary>
    /// 是否启用缓存
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 缓存TTL（秒）
    /// </summary>
    public int TTLSeconds { get; set; } = 300;

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// 是否按用户缓存
    /// </summary>
    public bool CacheByUser { get; set; } = false;

    /// <summary>
    /// 缓存标签
    /// </summary>
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// 仪表板访问日志
/// </summary>
public class DashboardAccessLog
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 仪表板ID
    /// </summary>
    public string DashboardId { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 访问时间
    /// </summary>
    public DateTime AccessTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 访问类型
    /// </summary>
    public DashboardAccessType AccessType { get; set; }

    /// <summary>
    /// 访问来源IP
    /// </summary>
    public string? SourceIP { get; set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// 停留时间（秒）
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// 访问结果
    /// </summary>
    public AccessResult Result { get; set; } = AccessResult.Success;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, object> ExtensionData { get; set; } = new();

    /// <summary>
    /// 导航属性：关联仪表板
    /// </summary>
    public BusinessDashboard? Dashboard { get; set; }
}

/// <summary>
/// KPI指标定义
/// </summary>
public class KPIMetric
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 指标名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 指标代码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 指标描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 指标分类
    /// </summary>
    public KPICategory Category { get; set; } = KPICategory.Sales;

    /// <summary>
    /// 指标单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 计算公式
    /// </summary>
    public string Formula { get; set; } = string.Empty;

    /// <summary>
    /// 数据源配置
    /// </summary>
    public WidgetDataSource DataSource { get; set; } = new();

    /// <summary>
    /// 目标值配置
    /// </summary>
    public KPITarget Target { get; set; } = new();

    /// <summary>
    /// 预警阈值配置
    /// </summary>
    public KPIThresholds Thresholds { get; set; } = new();

    /// <summary>
    /// 趋势配置
    /// </summary>
    public KPITrendConfig TrendConfig { get; set; } = new();

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int SortOrder { get; set; } = 100;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// KPI目标配置
/// </summary>
public class KPITarget
{
    /// <summary>
    /// 目标值类型
    /// </summary>
    public TargetType Type { get; set; } = TargetType.Fixed;

    /// <summary>
    /// 目标值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 目标期间类型
    /// </summary>
    public TargetPeriod Period { get; set; } = TargetPeriod.Monthly;

    /// <summary>
    /// 自定义期间（天数）
    /// </summary>
    public int? CustomPeriodDays { get; set; }

    /// <summary>
    /// 历史基准类型
    /// </summary>
    public BaselineType? BaselineType { get; set; }

    /// <summary>
    /// 增长目标百分比
    /// </summary>
    public decimal? GrowthTargetPercent { get; set; }
}

/// <summary>
/// KPI阈值配置
/// </summary>
public class KPIThresholds
{
    /// <summary>
    /// 优秀阈值
    /// </summary>
    public decimal ExcellentThreshold { get; set; }

    /// <summary>
    /// 良好阈值
    /// </summary>
    public decimal GoodThreshold { get; set; }

    /// <summary>
    /// 警告阈值
    /// </summary>
    public decimal WarningThreshold { get; set; }

    /// <summary>
    /// 危险阈值
    /// </summary>
    public decimal DangerThreshold { get; set; }

    /// <summary>
    /// 阈值比较方式
    /// </summary>
    public ThresholdComparisonType ComparisonType { get; set; } = ThresholdComparisonType.GreaterThan;
}

/// <summary>
/// KPI趋势配置
/// </summary>
public class KPITrendConfig
{
    /// <summary>
    /// 是否启用趋势分析
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 趋势分析周期
    /// </summary>
    public TrendPeriod Period { get; set; } = TrendPeriod.Last30Days;

    /// <summary>
    /// 趋势计算方式
    /// </summary>
    public TrendCalculationMethod CalculationMethod { get; set; } = TrendCalculationMethod.LinearRegression;

    /// <summary>
    /// 显示预测
    /// </summary>
    public bool ShowForecast { get; set; } = false;

    /// <summary>
    /// 预测天数
    /// </summary>
    public int ForecastDays { get; set; } = 7;
}

// =================== 枚举定义 ===================

/// <summary>
/// 仪表板类型
/// </summary>
public enum DashboardType
{
    /// <summary>
    /// 概览仪表板
    /// </summary>
    Overview = 1,

    /// <summary>
    /// 销售仪表板
    /// </summary>
    Sales = 2,

    /// <summary>
    /// 财务仪表板
    /// </summary>
    Finance = 3,

    /// <summary>
    /// 库存仪表板
    /// </summary>
    Inventory = 4,

    /// <summary>
    /// 客户仪表板
    /// </summary>
    Customer = 5,

    /// <summary>
    /// 供应商仪表板
    /// </summary>
    Supplier = 6,

    /// <summary>
    /// 运营仪表板
    /// </summary>
    Operations = 7,

    /// <summary>
    /// 质量仪表板
    /// </summary>
    Quality = 8,

    /// <summary>
    /// 自定义仪表板
    /// </summary>
    Custom = 99
}

/// <summary>
/// 仪表板分类
/// </summary>
public enum DashboardCategory
{
    /// <summary>
    /// 管理层
    /// </summary>
    Management = 1,

    /// <summary>
    /// 业务部门
    /// </summary>
    Business = 2,

    /// <summary>
    /// 运营部门
    /// </summary>
    Operations = 3,

    /// <summary>
    /// 财务部门
    /// </summary>
    Finance = 4,

    /// <summary>
    /// 技术部门
    /// </summary>
    Technical = 5
}

/// <summary>
/// 仪表板访问级别
/// </summary>
public enum DashboardAccessLevel
{
    /// <summary>
    /// 公开
    /// </summary>
    Public = 1,

    /// <summary>
    /// 部门级
    /// </summary>
    Department = 2,

    /// <summary>
    /// 角色级
    /// </summary>
    Role = 3,

    /// <summary>
    /// 用户级
    /// </summary>
    User = 4,

    /// <summary>
    /// 私有
    /// </summary>
    Private = 5
}

/// <summary>
/// 布局类型
/// </summary>
public enum LayoutType
{
    /// <summary>
    /// 网格布局
    /// </summary>
    Grid = 1,

    /// <summary>
    /// 自由布局
    /// </summary>
    Free = 2,

    /// <summary>
    /// 流式布局
    /// </summary>
    Flow = 3
}

/// <summary>
/// 组件类型
/// </summary>
public enum WidgetType
{
    /// <summary>
    /// 数据卡片
    /// </summary>
    Card = 1,

    /// <summary>
    /// 折线图
    /// </summary>
    LineChart = 2,

    /// <summary>
    /// 柱状图
    /// </summary>
    BarChart = 3,

    /// <summary>
    /// 饼图
    /// </summary>
    PieChart = 4,

    /// <summary>
    /// 环形图
    /// </summary>
    DonutChart = 5,

    /// <summary>
    /// 面积图
    /// </summary>
    AreaChart = 6,

    /// <summary>
    /// 散点图
    /// </summary>
    ScatterChart = 7,

    /// <summary>
    /// 仪表盘
    /// </summary>
    GaugeChart = 8,

    /// <summary>
    /// 雷达图
    /// </summary>
    RadarChart = 9,

    /// <summary>
    /// 热力图
    /// </summary>
    HeatmapChart = 10,

    /// <summary>
    /// 数据表格
    /// </summary>
    Table = 11,

    /// <summary>
    /// 进度条
    /// </summary>
    ProgressBar = 12,

    /// <summary>
    /// 文本显示
    /// </summary>
    Text = 13,

    /// <summary>
    /// 图片
    /// </summary>
    Image = 14,

    /// <summary>
    /// KPI指标
    /// </summary>
    KPI = 15,

    /// <summary>
    /// 实时监控
    /// </summary>
    RealTimeMonitor = 16
}

/// <summary>
/// 数据源类型
/// </summary>
public enum DataSourceType
{
    /// <summary>
    /// SQL查询
    /// </summary>
    SQL = 1,

    /// <summary>
    /// REST API
    /// </summary>
    RestAPI = 2,

    /// <summary>
    /// 静态数据
    /// </summary>
    Static = 3,

    /// <summary>
    /// 实时数据流
    /// </summary>
    RealTime = 4,

    /// <summary>
    /// 文件数据
    /// </summary>
    File = 5
}

/// <summary>
/// 筛选操作符
/// </summary>
public enum FilterOperator
{
    Equals = 1,
    NotEquals = 2,
    GreaterThan = 3,
    LessThan = 4,
    GreaterThanOrEqual = 5,
    LessThanOrEqual = 6,
    Contains = 7,
    StartsWith = 8,
    EndsWith = 9,
    In = 10,
    NotIn = 11,
    Between = 12,
    IsNull = 13,
    IsNotNull = 14
}

/// <summary>
/// 聚合函数
/// </summary>
public enum AggregationFunction
{
    Sum = 1,
    Average = 2,
    Count = 3,
    Max = 4,
    Min = 5,
    CountDistinct = 6,
    First = 7,
    Last = 8
}

/// <summary>
/// 排序方向
/// </summary>
public enum SortDirection
{
    Ascending = 1,
    Descending = 2
}

/// <summary>
/// 筛选字段类型
/// </summary>
public enum FilterFieldType
{
    Text = 1,
    Number = 2,
    Date = 3,
    DateTime = 4,
    Boolean = 5,
    Select = 6,
    MultiSelect = 7
}

/// <summary>
/// 仪表板访问类型
/// </summary>
public enum DashboardAccessType
{
    View = 1,
    Edit = 2,
    Share = 3,
    Export = 4,
    Print = 5
}

/// <summary>
/// 访问结果
/// </summary>
public enum AccessResult
{
    Success = 1,
    Failed = 2,
    Unauthorized = 3,
    Forbidden = 4,
    NotFound = 5,
    Error = 6
}

/// <summary>
/// KPI分类
/// </summary>
public enum KPICategory
{
    Sales = 1,
    Finance = 2,
    Operations = 3,
    Customer = 4,
    Quality = 5,
    Efficiency = 6,
    Growth = 7,
    Risk = 8
}

/// <summary>
/// 目标类型
/// </summary>
public enum TargetType
{
    Fixed = 1,
    Percentage = 2,
    Growth = 3,
    Baseline = 4
}

/// <summary>
/// 目标期间
/// </summary>
public enum TargetPeriod
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    Yearly = 5,
    Custom = 99
}

/// <summary>
/// 基准类型
/// </summary>
public enum BaselineType
{
    LastPeriod = 1,
    SamePeriodLastYear = 2,
    Average = 3,
    Best = 4
}

/// <summary>
/// 阈值比较类型
/// </summary>
public enum ThresholdComparisonType
{
    GreaterThan = 1,
    LessThan = 2,
    Between = 3
}

/// <summary>
/// 趋势周期
/// </summary>
public enum TrendPeriod
{
    Last7Days = 1,
    Last30Days = 2,
    Last90Days = 3,
    LastYear = 4,
    Custom = 99
}

/// <summary>
/// 趋势计算方法
/// </summary>
public enum TrendCalculationMethod
{
    LinearRegression = 1,
    MovingAverage = 2,
    ExponentialSmoothing = 3,
    SeasonalDecomposition = 4
}

#endif
