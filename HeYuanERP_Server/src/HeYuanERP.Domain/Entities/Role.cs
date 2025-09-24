namespace HeYuanERP.Domain.Entities;

// 领域实体：角色
// 说明：
// - 角色管理与权限分配的基础对象
// - 与用户、权限均为多对多关系
public class Role
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 角色编码/名称
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // 状态
    public bool Active { get; set; } = true;

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航属性：用户-角色、角色-权限
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
