using HeYuanERP.Domain.Enums;

namespace HeYuanERP.Api.Services.Orders;

/// <summary>
/// 订单状态流转结果
/// </summary>
public class OrderStateResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 变更后的状态
    /// </summary>
    public OrderStatus? NewStatus { get; set; }

    /// <summary>
    /// 状态变更日志ID
    /// </summary>
    public string? LogId { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static OrderStateResult Ok(OrderStatus newStatus, string logId)
    {
        return new OrderStateResult
        {
            Success = true,
            NewStatus = newStatus,
            LogId = logId
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static OrderStateResult Fail(string errorMessage)
    {
        return new OrderStateResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// 订单状态流转服务接口
/// </summary>
public interface IOrderStateService
{
    /// <summary>
    /// 检查是否可以流转到指定状态
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="targetStatus">目标状态</param>
    /// <param name="userId">当前用户ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>是否可以流转</returns>
    Task<bool> CanTransitionAsync(string orderId, OrderStatus targetStatus, string userId, CancellationToken ct = default);

    /// <summary>
    /// 执行状态流转
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="targetStatus">目标状态</param>
    /// <param name="reason">变更原因</param>
    /// <param name="userId">当前用户ID</param>
    /// <param name="userAgent">用户代理</param>
    /// <param name="clientIp">客户端IP</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>状态流转结果</returns>
    Task<OrderStateResult> TransitionAsync(string orderId, OrderStatus targetStatus, string? reason,
        string userId, string? userAgent = null, string? clientIp = null, CancellationToken ct = default);

    /// <summary>
    /// 获取指定订单当前可流转的状态列表
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="userId">当前用户ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>可流转的状态列表</returns>
    Task<List<OrderStatus>> GetAllowedTransitionsAsync(string orderId, string userId, CancellationToken ct = default);

    /// <summary>
    /// 获取订单状态变更历史
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>状态变更历史</returns>
    Task<List<OrderStatusLogDto>> GetStatusHistoryAsync(string orderId, CancellationToken ct = default);

    /// <summary>
    /// 批量更新订单状态（系统内部使用）
    /// </summary>
    /// <param name="updates">批量更新信息</param>
    /// <param name="systemUserId">系统用户ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>更新结果</returns>
    Task<Dictionary<string, OrderStateResult>> BatchUpdateStatusAsync(
        List<OrderStatusBatchUpdate> updates, string systemUserId, CancellationToken ct = default);
}

/// <summary>
/// 订单状态变更日志DTO
/// </summary>
public class OrderStatusLogDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public string FromStatusName { get; set; } = string.Empty;
    public string ToStatusName { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public string? ClientIp { get; set; }
}

/// <summary>
/// 批量状态更新信息
/// </summary>
public class OrderStatusBatchUpdate
{
    public string OrderId { get; set; } = string.Empty;
    public OrderStatus TargetStatus { get; set; }
    public string? Reason { get; set; }
    public Dictionary<string, object>? ExtensionData { get; set; }
}
