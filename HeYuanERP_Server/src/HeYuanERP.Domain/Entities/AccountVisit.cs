namespace HeYuanERP.Domain.Entities;

// 领域实体：客户拜访记录（AccountVisit）
// 功能：记录针对客户的拜访日志（时间、主题、内容、参与联系人、拜访人等）；包含审计字段。
public class AccountVisit
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属关系
    public string AccountId { get; set; } = string.Empty;    // 客户 Id（必填）
    public string? ContactId { get; set; }                   // 被拜访联系人（可选）
    public string? VisitorId { get; set; }                   // 拜访人（用户 Id，可选）

    // 业务信息
    public DateTime VisitDate { get; set; } = DateTime.UtcNow; // 拜访日期/时间
    public string? Subject { get; set; }                      // 主题（简要）
    public string? Content { get; set; }                      // 内容/纪要（长文本）
    public string? Location { get; set; }                     // 地点（可选）
    public string? Result { get; set; }                       // 结果/结论（可选）
    public DateTime? NextActionAt { get; set; }               // 下次跟进时间（可选）

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航属性（可选）
    public Account? Account { get; set; }
    public Contact? Contact { get; set; }
    public User? Visitor { get; set; }
}

