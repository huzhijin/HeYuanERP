using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HeYuanERP.Api.Services;

// JWT 签发服务：根据用户与权限生成访问 Token
public class JwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(JwtOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// 生成 JWT 访问令牌（包含基础身份信息、角色与权限声明）
    /// </summary>
    /// <param name="userId">用户唯一标识</param>
    /// <param name="loginId">登录名</param>
    /// <param name="name">显示名称</param>
    /// <param name="roles">角色列表</param>
    /// <param name="permissions">权限列表（例如：accounts.read、orders.create）</param>
    public string GenerateToken(string userId, string loginId, string name, IEnumerable<string>? roles = null, IEnumerable<string>? permissions = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new("loginId", loginId),
            new(ClaimTypes.Name, name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (roles != null)
        {
            claims.AddRange(roles.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => new Claim(ClaimTypes.Role, r)));
        }

        if (permissions != null)
        {
            // 使用自定义声明类型 "perm"，每个权限一条声明
            claims.AddRange(permissions.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => new Claim("perm", p)));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

