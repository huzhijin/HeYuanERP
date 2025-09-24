// 版权所有(c) HeYuanERP
// 说明：报表导出后台任务选项（中文注释）。

namespace HeYuanERP.Api.BackgroundWorkers;

/// <summary>
/// 报表导出后台任务配置项。
/// </summary>
public class ReportExportWorkerOptions
{
    /// <summary>
    /// 队列容量（默认 200）。
    /// </summary>
    public int QueueCapacity { get; set; } = 200;

    /// <summary>
    /// 并发度（默认 1）。
    /// </summary>
    public int MaxDegreeOfParallelism { get; set; } = 1;
}

