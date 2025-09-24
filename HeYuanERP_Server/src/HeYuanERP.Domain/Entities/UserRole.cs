namespace HeYuanERP.Domain.Entities;

// 关联实体：用户-角色（多对多桥接表）
// 复合键：UserId + RoleId（在 DbContext 中通过 Fluent API 配置）
public class UserRole
{
    // 外键
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航
    public User? User { get; set; }
    public Role? Role { get; set; }
}

