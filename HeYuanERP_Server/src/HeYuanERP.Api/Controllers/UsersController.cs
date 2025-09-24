using System.Security.Claims;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

// 用户相关接口：当前提供 /api/users/me 获取当前登录用户信息
// 说明：权限列表来自 JWT 声明（perm），无需额外查询数据库，性能更优。
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var user = HttpContext.User;
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub") ?? string.Empty;
        var loginId = user.FindFirstValue("loginId") ?? string.Empty;
        var name = user.FindFirstValue(ClaimTypes.Name) ?? loginId;
        var perms = user.FindAll("perm").Select(c => c.Value).ToList();

        var dto = new UserDto
        {
            Id = id,
            LoginId = loginId,
            Name = name,
            Permissions = perms
        };

        return Ok(ApiResponse<UserDto>.Ok(dto));
    }
}
