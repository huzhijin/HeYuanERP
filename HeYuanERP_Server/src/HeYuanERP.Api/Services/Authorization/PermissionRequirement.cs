using Microsoft.AspNetCore.Authorization;

namespace HeYuanERP.Api.Services.Authorization;

// RBAC 权限策略需求：
// - 通过 Authorize(Policy = "Permission") 启用；
// - 实际所需权限码由 [RequirePermission] 装饰器提供（见同目录）；
// - 处理逻辑在 PermissionAuthorizationHandler 中实现。
public class PermissionRequirement : IAuthorizationRequirement
{
}
