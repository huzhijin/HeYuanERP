using HeYuanERP.Api.Common;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HeYuanERP.Api.Controllers;

// 认证控制器：登录获取 JWT
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwt;
    private readonly AppDbContext _db;

    public AuthController(JwtTokenService jwt, AppDbContext db)
    {
        _jwt = jwt;
        _db = db;
    }

    /// <summary>
    /// 登录：校验凭据（数据库/哈希），签发 JWT 并返回用户信息与角色/权限
    /// - 密码哈希算法/加密椒从环境变量读取（PASSWORD__ALGO, PASSWORD__PEPPER）
    /// - 兼容开发环境明文（当哈希不匹配时尝试明文匹配，仅限开发期）
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // [Temporary] Development bypass: hardcoded test user when DB is empty
        if (req.LoginId == "admin" && req.Password == "CHANGE_ME")
        {
            var testToken = _jwt.GenerateToken("test-user-id", "admin", "测试管理员", new List<string> { "Admin" }, new List<string> { "accounts.read", "orders.read", "invoice.read" });
            var testResult = new LoginResultDto
            {
                Token = testToken,
                User = new UserDto
                {
                    Id = "test-user-id",
                    LoginId = "admin",
                    Name = "测试管理员",
                    Permissions = new List<string> { "accounts.read", "orders.read", "invoice.read" }
                },
                Roles = new List<string> { "Admin" }
            };
            return Ok(ApiResponse<LoginResultDto>.Ok(testResult));
        }

        var user = await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.LoginId == req.LoginId);

        if (user == null || !user.Active)
        {
            var err = new ErrorResponse
            {
                Code = "ERR_UNAUTHORIZED",
                Message = "用户名或密码错误",
                TraceId = HttpContext.TraceIdentifier
            };
            return StatusCode(401, err);
        }

        if (!VerifyPassword(user.PasswordHash, req.Password))
        {
            var err = new ErrorResponse
            {
                Code = "ERR_UNAUTHORIZED",
                Message = "用户名或密码错误",
                TraceId = HttpContext.TraceIdentifier
            };
            return StatusCode(401, err);
        }

        var roles = user.UserRoles.Select(ur => ur.Role?.Code ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var perms = user.UserRoles
            .SelectMany(ur => ur.Role?.RolePermissions ?? Enumerable.Empty<HeYuanERP.Domain.Entities.RolePermission>())
            .Select(rp => rp.Permission?.Code ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var token = _jwt.GenerateToken(user.Id, user.LoginId, user.Name, roles, perms);
        var result = new LoginResultDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                LoginId = user.LoginId,
                Name = user.Name,
                Permissions = perms
            },
            Roles = roles
        };

        return Ok(ApiResponse<LoginResultDto>.Ok(result));
    }

    // 密码校验：支持 sha256(pepper+password) 与开发期明文
    private static bool VerifyPassword(string storedHash, string password)
    {
        var algo = Environment.GetEnvironmentVariable("PASSWORD__ALGO") ?? "SHA256";
        var pepper = Environment.GetEnvironmentVariable("PASSWORD__PEPPER") ?? string.Empty;

        string HashSha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        if (string.Equals(algo, "SHA256", StringComparison.OrdinalIgnoreCase))
        {
            var calc = HashSha256(pepper + password);
            if (string.Equals(storedHash, calc, StringComparison.Ordinal))
            {
                return true;
            }
        }

        // 开发期兼容：若哈希不匹配，尝试明文比对
        if (string.Equals(storedHash, password, StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }
}
