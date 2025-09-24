namespace HeYuanERP.Domain.Entities;

// 领域实体：权限
// 用途：RBAC 权限点，采用“资源.动作”编码规范（如：accounts.read、orders.create）
// 说明：权限与角色为多对多关系，通过 RolePermission 关联
public class Permission
{
    // 主键（字符串 GUID）
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 权限编码：资源.动作（示例：accounts.read、orders.create）
    public string Code { get; set; } = string.Empty;

    // 显示名称/描述
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // 是否启用
    public bool Active { get; set; } = true;

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航属性：角色-权限（多对多中间表）
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

