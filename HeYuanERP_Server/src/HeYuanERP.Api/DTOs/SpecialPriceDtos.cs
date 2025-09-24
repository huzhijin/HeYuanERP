namespace HeYuanERP.Api.DTOs;

// 特价/项目申请（留桩 DTO）：仅用于打通前后端流程，暂不持久化

public class OrderSpecialLineDto
{
    public string ProductId { get; set; } = string.Empty;   // 产品 Id
    public decimal TargetUnitPrice { get; set; }            // 目标单价（含税）
    public string? Remark { get; set; }                     // 行备注（可选）
}

public class OrderSpecialApplyDto
{
    public string OrderId { get; set; } = string.Empty;     // 关联订单 Id
    public string ProjectName { get; set; } = string.Empty;  // 项目/客户项目名
    public string? Reason { get; set; }                      // 申请原因
    public List<OrderSpecialLineDto> Lines { get; set; } = new(); // 涉及商品与目标价
}

public class OrderSpecialApplyResultDto
{
    public string ApplyId { get; set; } = string.Empty;  // 申请号（留桩生成）
    public string Status { get; set; } = "submitted";     // submitted/pending/approved/rejected（占位）
    public string Message { get; set; } = string.Empty;   // 提示信息
    public object? Echo { get; set; }                     // 回显原始请求（便于联调）
}

