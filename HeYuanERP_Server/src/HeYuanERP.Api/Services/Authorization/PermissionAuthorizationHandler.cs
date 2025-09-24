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
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        // 若用户为管理员角色，直接放行
        var isAdmin = context.User.IsInRole("Admin");
        if (isAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 尝试获取 Endpoint 上声明的权限码
        HttpContext? httpContext = context.Resource switch
        {
            HttpContext hc => hc,
            AuthorizationFilterContext afc => afc.HttpContext,
            _ => null
        };

        var endpoint = httpContext?.GetEndpoint();
        var requires = endpoint?.Metadata?.OfType<RequirePermissionAttribute>()?.ToList() ?? new();

        // 用户已有的权限（来自 JWT 声明 perm）
        var userPerms = context.User.Claims
            .Where(c => c.Type == "perm" && !string.IsNullOrWhiteSpace(c.Value))
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (requires.Count == 0)
        {
            // 未声明具体权限：拥有任意 perm 即可
            if (userPerms.Count > 0)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

        // 存在权限声明：逐个装饰器判定（任一装饰器满足即通过）
        foreach (var req in requires)
        {
            var codes = (req.Codes ?? Array.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (codes.Count == 0)
            {
                continue;
            }

            bool ok = req.RequireAll
                ? codes.All(code => userPerms.Contains(code))
                : codes.Any(code => userPerms.Contains(code));

            if (ok)
            {
                context.Succeed(requirement);
                break;
            }
        }

        return Task.CompletedTask;
    }
}
