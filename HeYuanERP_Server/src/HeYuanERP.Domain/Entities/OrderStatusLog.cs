using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 订单状态变更日志 - 记录订单状态流转历史
/// </summary>
public class OrderStatusLog
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 关联订单ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 原状态
    /// </summary>
    public OrderStatus FromStatus { get; set; }

    /// <summary>
    /// 目标状态
    /// </summary>
    public OrderStatus ToStatus { get; set; }

    /// <summary>
    /// 变更原因
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 变更时间
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 操作人
    /// </summary>
    public string? ChangedBy { get; set; }

    /// <summary>
    /// 操作人姓名（冗余字段，避免用户删除后查询问题）
    /// </summary>
    public string? ChangedByName { get; set; }

    /// <summary>
    /// 客户端IP
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 扩展数据（JSON格式）
    /// </summary>
    public string? ExtensionData { get; set; }

    /// <summary>
    /// 导航属性：关联订单
    /// </summary>
    public SalesOrder? Order { get; set; }
}