namespace HeYuanERP.Domain.Entities;

// 领域实体：用户
// 说明：
// - P0 阶段定义核心字段；后续可补充并发控制、软删除等
// - 与角色为多对多关系，通过 UserRole 维护
public class User
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 登录账号
    public string LoginId { get; set; } = string.Empty;

    // 显示名称
    public string Name { get; set; } = string.Empty;

    // 密码哈希（不存明文）
    public string PasswordHash { get; set; } = string.Empty;

    // 是否启用
    public bool Active { get; set; } = true;

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航属性：用户-角色关联
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
