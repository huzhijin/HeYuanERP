// 版权所有(c) HeYuanERP
// 说明：报表类型与导出相关枚举定义（中文注释）。

namespace HeYuanERP.Domain.Reports;

/// <summary>
/// 报表类型枚举（用于查询、导出与快照分类）。
/// </summary>
public enum ReportType
{
    /// <summary>销售统计报表</summary>
    SalesStat = 1,

    /// <summary>发票统计报表</summary>
    InvoiceStat = 2,

    /// <summary>采购订单查询</summary>
    POQuery = 3,

    /// <summary>库存查询/统计</summary>
    Inventory = 4
}

/// <summary>
/// 报表导出格式。
/// </summary>
public enum ReportExportFormat
{
    /// <summary>PDF 文件（用于打印/归档）</summary>
    Pdf = 1,

    /// <summary>CSV 文件（用于数据分析/导入）</summary>
    Csv = 2
}

/// <summary>
/// 报表导出任务状态。
/// </summary>
public enum ReportJobStatus
{
    /// <summary>已创建，待入队</summary>
    Pending = 0,

    /// <summary>队列中/等待执行</summary>
    Queued = 1,

    /// <summary>执行中（生成报表/渲染）</summary>
    Running = 2,

    /// <summary>已完成（成功）</summary>
    Succeeded = 3,

    /// <summary>失败（含错误信息）</summary>
    Failed = 4,

    /// <summary>已取消</summary>
    Canceled = 5
}

