using Microsoft.EntityFrameworkCore;
using HeYuanERP.Api.Data;

namespace HeYuanERP.Api.Services.Authorization;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string permissionCode, CancellationToken ct = default);
}

/// <summary>
/// 最小可用的权限查询服务：基于用户-角色-权限关系进行判断。
/// 说明：控制器层的 [RequirePermission] 仍依赖 JWT 的 perm 声明快速判定；
/// 本服务用于应用内部（例如状态机服务）做细粒度校验，防止越权调用。
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly AppDbContext _db;

    public PermissionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permissionCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(permissionCode))
            return false;

        // 管理员快捷判断：用户拥有 Admin 角色则放行
        var isAdmin = await _db.UserRoles
            .AsNoTracking()
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Code == "Admin", ct);
        if (isAdmin) return true;

        // 角色权限检查
        var hasPerm = await _db.RolePermissions
            .AsNoTracking()
            .Include(rp => rp.Permission)
            .Join(_db.UserRoles.AsNoTracking().Where(ur => ur.UserId == userId),
                rp => rp.RoleId,
                ur => ur.RoleId,
                (rp, ur) => rp.Permission.Code)
            .AnyAsync(code => code == permissionCode, ct);

        return hasPerm;
    }
}

