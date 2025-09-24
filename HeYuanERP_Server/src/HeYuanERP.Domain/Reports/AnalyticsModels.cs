using System.ComponentModel.DataAnnotations;

namespace HeYuanERP.Domain.Reports;

// 基础数据维度
public class Dimension
{
    public string Name { get; set; } = string.Empty;  // 维度名称：时间、产品、客户、地区
    public string Type { get; set; } = string.Empty;  // 维度类型：DateTime, String, Number
    public List<string> Values { get; set; } = new List<string>(); // 维度值
}

// 度量指标
public class Measure
{
    public string Name { get; set; } = string.Empty;  // 度量名称：销售额、数量、利润
    public string Type { get; set; } = string.Empty;  // 度量类型：Sum, Avg, Count, Max, Min
    public string Format { get; set; } = string.Empty; // 格式化：Currency, Percentage, Number
    public decimal Value { get; set; }                 // 度量值
}

// 筛选条件
public class Filter
{
    public string Field { get; set; } = string.Empty;  // 筛选字段
    public string Operator { get; set; } = string.Empty; // 操作符：=, >, <, IN, BETWEEN
    public object Value { get; set; } = new object();   // 筛选值
}

// 日期范围
public class DateRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Period { get; set; } = "Month"; // Day, Week, Month, Quarter, Year
}

// 数据立方体
public class DataCube
{
    public List<Dimension> Dimensions { get; set; } = new List<Dimension>();
    public List<Measure> Measures { get; set; } = new List<Measure>();
    public List<Filter> Filters { get; set; } = new List<Filter>();
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}

// 钻取路径
public class DrillDownPath
{
    public string StartLevel { get; set; } = string.Empty;   // 起始层级
    public string TargetLevel { get; set; } = string.Empty;  // 目标层级
    public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
}

// 钻取请求
public class DrillDownRequest
{
    public string ReportType { get; set; } = string.Empty;   // 报表类型
    public string SourceLevel { get; set; } = string.Empty;  // 源层级
    public string TargetLevel { get; set; } = string.Empty;  // 目标层级
    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    public DateRange DateRange { get; set; } = new DateRange();
}

// 钻取结果
public class DrillDownResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object Data { get; set; } = new object();
    public DrillDownPath Path { get; set; } = new DrillDownPath();
    public List<string> AvailableDrillPaths { get; set; } = new List<string>();
}

// KPI指标卡片
public class KPICard
{
    public string Title { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal ChangePercent { get; set; }  // 环比变化百分比
    public string ChangeDirection { get; set; } = "Up"; // Up, Down, Flat
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = "#1890ff";
    public bool IsDrillable { get; set; } = true;
    public string DrillDownTarget { get; set; } = string.Empty;
}

// 图表数据
public class ChartData
{
    public string Type { get; set; } = string.Empty; // Line, Bar, Pie, Area, Radar
    public string Title { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = new List<string>();
    public List<ChartSeries> Series { get; set; } = new List<ChartSeries>();
    public ChartOptions Options { get; set; } = new ChartOptions();
}

public class ChartSeries
{
    public string Name { get; set; } = string.Empty;
    public List<decimal> Data { get; set; } = new List<decimal>();
    public string Color { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class ChartOptions
{
    public bool ShowLegend { get; set; } = true;
    public bool IsDrillable { get; set; } = true;
    public string XAxisTitle { get; set; } = string.Empty;
    public string YAxisTitle { get; set; } = string.Empty;
    public Dictionary<string, object> CustomOptions { get; set; } = new Dictionary<string, object>();
}

// 管理驾驶舱数据
public class ExecutiveDashboardData
{
    public List<KPICard> KPICards { get; set; } = new List<KPICard>();
    public ChartData SalesVsPurchaseChart { get; set; } = new ChartData();
    public ChartData SalesTrendChart { get; set; } = new ChartData();
    public ChartData TopProductsChart { get; set; } = new ChartData();
    public ChartData TopCustomersChart { get; set; } = new ChartData();
    public List<Dictionary<string, object>> DetailTable { get; set; } = new List<Dictionary<string, object>>();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

// 销售分析数据
public class SalesAnalysisData
{
    public List<KPICard> SalesKPIs { get; set; } = new List<KPICard>();
    public ChartData SalesTrendChart { get; set; } = new ChartData();
    public ChartData ProductSalesChart { get; set; } = new ChartData();
    public ChartData SalesRepPerformanceChart { get; set; } = new ChartData();
    public ChartData RegionSalesChart { get; set; } = new ChartData();
    public ChartData SalesFunnelChart { get; set; } = new ChartData();
    public List<Dictionary<string, object>> SalesDetailTable { get; set; } = new List<Dictionary<string, object>>();
}

// 采购分析数据
public class PurchaseAnalysisData
{
    public List<KPICard> PurchaseKPIs { get; set; } = new List<KPICard>();
    public ChartData PurchaseTrendChart { get; set; } = new ChartData();
    public ChartData SupplierRankingChart { get; set; } = new ChartData();
    public ChartData CategoryAnalysisChart { get; set; } = new ChartData();
    public ChartData CostAnalysisChart { get; set; } = new ChartData();
    public ChartData SupplierPerformanceRadar { get; set; } = new ChartData();
    public List<Dictionary<string, object>> PurchaseDetailTable { get; set; } = new List<Dictionary<string, object>>();
}

// 财务分析数据
public class FinanceAnalysisData
{
    public List<KPICard> FinanceKPIs { get; set; } = new List<KPICard>();
    public ChartData CashFlowChart { get; set; } = new ChartData();
    public ChartData ProfitAnalysisChart { get; set; } = new ChartData();
    public ChartData ARAgingChart { get; set; } = new ChartData();
    public ChartData APAgingChart { get; set; } = new ChartData();
    public ChartData CostStructureChart { get; set; } = new ChartData();
    public List<Dictionary<string, object>> FinanceDetailTable { get; set; } = new List<Dictionary<string, object>>();
}

// 库存分析数据
public class InventoryAnalysisData
{
    public List<KPICard> InventoryKPIs { get; set; } = new List<KPICard>();
    public ChartData InventoryTurnoverChart { get; set; } = new ChartData();
    public ChartData InventoryDistributionChart { get; set; } = new ChartData();
    public ChartData SlowMovingChart { get; set; } = new ChartData();
    public ChartData WarehouseUtilizationChart { get; set; } = new ChartData();
    public List<Dictionary<string, object>> InventoryDetailTable { get; set; } = new List<Dictionary<string, object>>();
}

// 分析请求基类
public abstract class AnalysisRequest
{
    public DateRange DateRange { get; set; } = new DateRange();
    public List<Filter> Filters { get; set; } = new List<Filter>();
    public List<string> Dimensions { get; set; } = new List<string>();
    public string GroupBy { get; set; } = string.Empty;
    public int TopN { get; set; } = 10;
}

// 销售分析请求
public class SalesAnalysisRequest : AnalysisRequest
{
    public List<int> CustomerIds { get; set; } = new List<int>();
    public List<int> ProductIds { get; set; } = new List<int>();
    public List<int> SalesRepIds { get; set; } = new List<int>();
    public string AnalysisType { get; set; } = "Overview"; // Overview, Trend, Product, Customer, Rep
}

// 采购分析请求
public class PurchaseAnalysisRequest : AnalysisRequest
{
    public List<int> SupplierIds { get; set; } = new List<int>();
    public List<int> ProductIds { get; set; } = new List<int>();
    public List<string> Categories { get; set; } = new List<string>();
    public string AnalysisType { get; set; } = "Overview"; // Overview, Cost, Supplier, Category
}

// 财务分析请求
public class FinanceAnalysisRequest : AnalysisRequest
{
    public string AnalysisType { get; set; } = "Overview"; // Overview, CashFlow, Profit, AR, AP
    public List<string> Accounts { get; set; } = new List<string>();
    public bool IncludeBudget { get; set; } = false;
}

// 库存分析请求
public class InventoryAnalysisRequest : AnalysisRequest
{
    public List<int> WarehouseIds { get; set; } = new List<int>();
    public List<int> ProductIds { get; set; } = new List<int>();
    public string AnalysisType { get; set; } = "Overview"; // Overview, Turnover, Aging, Alert
    public int SlowMovingDays { get; set; } = 90;
}