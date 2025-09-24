namespace HeYuanERP.Domain.Entities;

// 领域实体：联系人（隶属客户）
public class Contact
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string AccountId { get; set; } = string.Empty; // 客户 Id

    // 基本信息
    public string Name { get; set; } = string.Empty;      // 联系人姓名
    public string? Title { get; set; }                    // 职务
    public string? Mobile { get; set; }                   // 手机
    public string? Phone { get; set; }                    // 座机
    public string? Email { get; set; }                    // 电子邮箱
    public bool IsPrimary { get; set; }                   // 是否主要联系人
    public string? Remark { get; set; }

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public Account? Account { get; set; }
}

