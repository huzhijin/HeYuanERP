namespace HeYuanERP.Domain.Entities;

// 关联实体：角色-权限（多对多桥接表）
// 复合键：RoleId + PermissionId（在 DbContext 中通过 Fluent API 配置）
public class RolePermission
{
    // 外键
    public string RoleId { get; set; } = string.Empty;
    public string PermissionId { get; set; } = string.Empty;

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}

