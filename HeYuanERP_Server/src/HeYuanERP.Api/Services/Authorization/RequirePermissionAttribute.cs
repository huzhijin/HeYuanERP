using Microsoft.AspNetCore.Authorization;

namespace HeYuanERP.Api.Services.Authorization;

/// <summary>
/// RBAC 权限装饰器：在控制器/动作上声明所需权限码（资源.动作）。
/// 使用方式：
/// - 标注 [Authorize(Policy = "Permission")] 启用基础策略；
/// - 再标注 [RequirePermission("orders.read")] 或 [RequirePermission("orders.create", "orders.confirm")]；
/// - 多个装饰器之间为“或”的关系；单个装饰器内可通过 RequireAll = true 改为“且”的关系。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionAttribute : Attribute
{
    /// <summary>
    /// 需命中的权限码集合（资源.动作）
    /// </summary>
    public string[] Codes { get; }

    /// <summary>
    /// 是否要求全部命中（默认 false：任一命中即通过）
    /// </summary>
    public bool RequireAll { get; init; } = false;

    public RequirePermissionAttribute(params string[] codes)
    {
        Codes = codes ?? Array.Empty<string>();
    }
}

