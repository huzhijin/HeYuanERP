namespace HeYuanERP.Domain.Entities;

// 领域实体：客户/往来单位（Account）
// 对齐 OpenAPI：id, code, name, ownerId, taxNo, active, lastEventDate
// 说明：
// - 与联系人（Contact）一对多
// - 后续将通过配置类限制唯一索引与长度
public class Account
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 基本信息
    public string Code { get; set; } = string.Empty;   // 客户编码（唯一）
    public string Name { get; set; } = string.Empty;   // 客户名称
    public string? OwnerId { get; set; }               // 负责人（用户 Id）
    public string? TaxNo { get; set; }                 // 税号
    public bool Active { get; set; } = true;           // 是否启用
    public DateTime? LastEventDate { get; set; }       // 最近业务日期
    public string? Description { get; set; }           // 备注

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航：联系人
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}

