namespace HeYuanERP.Domain.Entities;

// 领域实体：报表快照（与导出任务/文件对应）
// 说明：对齐 OpenAPI ReportTask（taskId/status/fileUri/message/createdAt/finishedAt）
public class ReportSnapshot
{
    // 主键（亦可用作 taskId）
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 报表标识与参数
    public string Name { get; set; } = string.Empty;    // 报表名称（如：sales-summary）
    public string? ParamsJson { get; set; }             // 查询参数（JSON）

    // 任务状态与结果
    public string Status { get; set; } = "queued";      // 任务状态：queued/running/completed/failed
    public string? FileUri { get; set; }                // 导出文件地址
    public string? Message { get; set; }                // 错误或提示信息

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? FinishedAt { get; set; }
}

