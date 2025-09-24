using System.Text.Json;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Orders;

/// <summary>
/// 订单状态流转服务实现
/// </summary>
public class OrderStateService : IOrderStateService
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissionService;

    // 定义状态流转规则矩阵
    private static readonly Dictionary<OrderStatus, List<OrderStatus>> TransitionMatrix = new()
    {
        [OrderStatus.Draft] = new() { OrderStatus.Submitted, OrderStatus.Cancelled },
        [OrderStatus.Submitted] = new() { OrderStatus.Confirmed, OrderStatus.Draft, OrderStatus.Cancelled },
        [OrderStatus.Confirmed] = new() { OrderStatus.InProduction, OrderStatus.Cancelled },
        [OrderStatus.InProduction] = new() { OrderStatus.PartialDelivered, OrderStatus.Delivered, OrderStatus.Cancelled },
        [OrderStatus.PartialDelivered] = new() { OrderStatus.Delivered, OrderStatus.PartialInvoiced, OrderStatus.Cancelled },
        [OrderStatus.Delivered] = new() { OrderStatus.PartialInvoiced, OrderStatus.Invoiced, OrderStatus.Closed },
        [OrderStatus.PartialInvoiced] = new() { OrderStatus.Invoiced, OrderStatus.Closed },
        [OrderStatus.Invoiced] = new() { OrderStatus.Closed },
        [OrderStatus.Closed] = new(), // 终止状态，无法流转
        [OrderStatus.Cancelled] = new() // 终止状态，无法流转
    };

    // 状态操作权限映射
    private static readonly Dictionary<OrderStatus, string> StatusPermissions = new()
    {
        [OrderStatus.Submitted] = "orders.submit",
        [OrderStatus.Confirmed] = "orders.confirm",
        [OrderStatus.InProduction] = "orders.production",
        [OrderStatus.PartialDelivered] = "orders.delivery",
        [OrderStatus.Delivered] = "orders.delivery",
        [OrderStatus.PartialInvoiced] = "orders.invoice",
        [OrderStatus.Invoiced] = "orders.invoice",
        [OrderStatus.Closed] = "orders.close",
        [OrderStatus.Cancelled] = "orders.cancel"
    };

    public OrderStateService(AppDbContext db, IPermissionService permissionService)
    {
        _db = db;
        _permissionService = permissionService;
    }

    public async Task<bool> CanTransitionAsync(string orderId, OrderStatus targetStatus, string userId, CancellationToken ct = default)
    {
        // 获取订单当前状态
        var order = await _db.SalesOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order == null) return false;

        // 解析当前状态
        if (!Enum.TryParse<OrderStatus>(order.Status, true, out var currentStatus))
        {
            // 兼容旧状态值
            currentStatus = MapLegacyStatus(order.Status);
        }

        // 检查状态流转规则
        if (!TransitionMatrix.TryGetValue(currentStatus, out var allowedTransitions) ||
            !allowedTransitions.Contains(targetStatus))
        {
            return false;
        }

        // 检查权限
        if (StatusPermissions.TryGetValue(targetStatus, out var requiredPermission))
        {
            return await _permissionService.HasPermissionAsync(userId, requiredPermission, ct);
        }

        return true;
    }

    public async Task<OrderStateResult> TransitionAsync(string orderId, OrderStatus targetStatus, string? reason,
        string userId, string? userAgent = null, string? clientIp = null, CancellationToken ct = default)
    {
        using var transaction = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            // 获取订单（加锁）
            var order = await _db.SalesOrders
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);

            if (order == null)
            {
                return OrderStateResult.Fail("订单不存在");
            }

            // 解析当前状态
            if (!Enum.TryParse<OrderStatus>(order.Status, true, out var currentStatus))
            {
                currentStatus = MapLegacyStatus(order.Status);
            }

            // 检查是否可以流转
            if (!await CanTransitionAsync(orderId, targetStatus, userId, ct))
            {
                return OrderStateResult.Fail($"不允许从 {currentStatus.GetDisplayName()} 流转到 {targetStatus.GetDisplayName()}");
            }

            // 执行状态变更前的业务逻辑检查
            var validationResult = await ValidateTransitionAsync(order, currentStatus, targetStatus, ct);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            // 更新订单状态
            order.Status = targetStatus.ToString().ToLowerInvariant();
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = userId;

            // 创建状态变更日志
            var statusLog = new OrderStatusLog
            {
                OrderId = orderId,
                FromStatus = currentStatus,
                ToStatus = targetStatus,
                Reason = reason,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = userId,
                ClientIp = clientIp,
                UserAgent = userAgent
            };

            // 获取用户名（用于日志显示）
            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user != null)
            {
                statusLog.ChangedByName = string.IsNullOrWhiteSpace(user.Name) ? user.LoginId : user.Name;
            }

            _db.OrderStatusLogs.Add(statusLog);

            // 执行状态变更后的业务逻辑
            await ExecuteTransitionBusinessLogicAsync(order, currentStatus, targetStatus, userId, ct);

            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return OrderStateResult.Ok(targetStatus, statusLog.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            return OrderStateResult.Fail($"状态流转失败: {ex.Message}");
        }
    }

    public async Task<List<OrderStatus>> GetAllowedTransitionsAsync(string orderId, string userId, CancellationToken ct = default)
    {
        var order = await _db.SalesOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order == null) return new List<OrderStatus>();

        // 解析当前状态
        if (!Enum.TryParse<OrderStatus>(order.Status, true, out var currentStatus))
        {
            currentStatus = MapLegacyStatus(order.Status);
        }

        if (!TransitionMatrix.TryGetValue(currentStatus, out var allowedTransitions))
        {
            return new List<OrderStatus>();
        }

        // 过滤权限
        var result = new List<OrderStatus>();
        foreach (var transition in allowedTransitions)
        {
            if (await CanTransitionAsync(orderId, transition, userId, ct))
            {
                result.Add(transition);
            }
        }

        return result;
    }

    public async Task<List<OrderStatusLogDto>> GetStatusHistoryAsync(string orderId, CancellationToken ct = default)
    {
        var logs = await _db.OrderStatusLogs.AsNoTracking()
            .Where(l => l.OrderId == orderId)
            .OrderByDescending(l => l.ChangedAt)
            .ToListAsync(ct);

        return logs.Select(l => new OrderStatusLogDto
        {
            Id = l.Id,
            OrderId = l.OrderId,
            FromStatus = l.FromStatus,
            ToStatus = l.ToStatus,
            FromStatusName = l.FromStatus.GetDisplayName(),
            ToStatusName = l.ToStatus.GetDisplayName(),
            Reason = l.Reason,
            ChangedAt = l.ChangedAt,
            ChangedBy = l.ChangedBy,
            ChangedByName = l.ChangedByName,
            ClientIp = l.ClientIp
        }).ToList();
    }

    public async Task<Dictionary<string, OrderStateResult>> BatchUpdateStatusAsync(
        List<OrderStatusBatchUpdate> updates, string systemUserId, CancellationToken ct = default)
    {
        var results = new Dictionary<string, OrderStateResult>();

        foreach (var update in updates)
        {
            var result = await TransitionAsync(
                update.OrderId,
                update.TargetStatus,
                update.Reason,
                systemUserId,
                "System",
                "127.0.0.1",
                ct);

            results[update.OrderId] = result;
        }

        return results;
    }

    /// <summary>
    /// 映射旧状态值到新枚举
    /// </summary>
    private static OrderStatus MapLegacyStatus(string legacyStatus)
    {
        return legacyStatus?.ToLowerInvariant() switch
        {
            "draft" => OrderStatus.Draft,
            "confirmed" => OrderStatus.Confirmed,
            "reversed" => OrderStatus.Cancelled,
            _ => OrderStatus.Draft
        };
    }

    /// <summary>
    /// 验证状态流转的业务逻辑
    /// </summary>
    private async Task<OrderStateResult> ValidateTransitionAsync(SalesOrder order, OrderStatus fromStatus,
        OrderStatus toStatus, CancellationToken ct)
    {
        switch (toStatus)
        {
            case OrderStatus.Confirmed:
                // 确认订单需要检查订单行
                var hasLines = await _db.SalesOrderLines.AsNoTracking()
                    .AnyAsync(l => l.OrderId == order.Id, ct);
                if (!hasLines)
                {
                    return OrderStateResult.Fail("订单至少需要一行商品才能确认");
                }
                break;

            case OrderStatus.InProduction:
                // 进入生产状态的业务验证
                // 可以在这里添加库存检查、产能检查等逻辑
                break;

            case OrderStatus.Delivered:
                // 交货完成需要检查是否有发货记录
                var hasDeliveries = await _db.Deliveries.AsNoTracking()
                    .AnyAsync(d => d.OrderId == order.Id && d.Status == "confirmed", ct);
                if (!hasDeliveries)
                {
                    return OrderStateResult.Fail("订单必须有确认的发货记录才能标记为已发货");
                }
                break;

            case OrderStatus.Invoiced:
                // 简化：发票金额校验将由发票对账服务负责；此处仅通过
                break;
        }

            return OrderStateResult.Ok(toStatus, "");
    }

    /// <summary>
    /// 执行状态变更后的业务逻辑
    /// </summary>
    private async Task ExecuteTransitionBusinessLogicAsync(SalesOrder order, OrderStatus fromStatus,
        OrderStatus toStatus, string userId, CancellationToken ct)
    {
        switch (toStatus)
        {
            case OrderStatus.Confirmed:
                // 确认订单后可以触发的业务逻辑
                // 例如：通知生产部门、预留库存等
                break;

            case OrderStatus.Cancelled:
                // 取消订单后的业务逻辑
                // 例如：释放预留库存、通知相关部门等
                await HandleOrderCancellationAsync(order, userId, ct);
                break;

            case OrderStatus.Closed:
                // 关闭订单后的业务逻辑
                // 例如：更新客户统计、生成业务分析数据等
                await HandleOrderClosureAsync(order, userId, ct);
                break;
        }
    }

    /// <summary>
    /// 处理订单取消的业务逻辑
    /// </summary>
    private async Task HandleOrderCancellationAsync(SalesOrder order, string userId, CancellationToken ct)
    {
        // 这里可以添加订单取消的具体业务逻辑
        // 例如：释放库存预留、取消相关的发货单和发票等

        // 标记相关发货单为取消
        var deliveries = await _db.Deliveries
            .Where(d => d.OrderId == order.Id && d.Status != "cancelled")
            .ToListAsync(ct);

        foreach (var delivery in deliveries)
        {
            delivery.Status = "cancelled";
            delivery.UpdatedAt = DateTime.UtcNow;
            delivery.UpdatedBy = userId;
        }
    }

    /// <summary>
    /// 处理订单关闭的业务逻辑
    /// </summary>
    private async Task HandleOrderClosureAsync(SalesOrder order, string userId, CancellationToken ct)
    {
        // 这里可以添加订单关闭的具体业务逻辑
        // 例如：更新客户最后交易时间、计算客户价值等

        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == order.AccountId, ct);
        if (account != null)
        {
            account.LastEventDate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// 计算订单总金额
    /// </summary>
    private async Task<decimal> CalculateOrderTotalAsync(string orderId, CancellationToken ct)
    {
        var orderLines = await _db.SalesOrderLines.AsNoTracking()
            .Where(l => l.OrderId == orderId)
            .ToListAsync(ct);

        decimal total = 0;
        foreach (var line in orderLines)
        {
            var lineAmount = line.Qty * line.UnitPrice * (1 - line.Discount);
            var taxAmount = lineAmount * line.TaxRate;
            total += lineAmount + taxAmount;
        }

        return total;
    }
}
