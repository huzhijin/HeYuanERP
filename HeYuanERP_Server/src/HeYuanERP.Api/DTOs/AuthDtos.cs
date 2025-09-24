namespace HeYuanERP.Api.DTOs;

// 认证相关 DTO（与 OpenAPI 对齐）
// - LoginRequest: 登录请求参数
// - UserDto: 基础用户信息，permissions 用于前端路由与按钮权限控制
// - LoginResultDto: 登录结果（JWT + 用户信息 + 角色）
public class LoginRequest
{
    public string LoginId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string LoginId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class LoginResultDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
    public List<string> Roles { get; set; } = new();
}
