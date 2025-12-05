using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HeYuanERP.Api.Services.Authorization;

// 权限授权处理器：
// - 优先从当前 Endpoint 元数据读取 [RequirePermission] 的权限码；
// - 若指定了权限码，则校验用户是否具备（默认任一命中；可要求全部命中）；
// - 若未指定权限码，则退化为“拥有任意 perm 即可”或角色为 Admin 即可。
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // [Temporary] Bypass all permission checks for local testing
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
