namespace HeYuanERP.Domain.Enums;

/// <summary>
/// 订单状态枚举 - 支持完整的业务流程状态流转
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 草稿 - 初始状态，可编辑
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 已提交 - 等待确认
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// 已确认 - 订单确认，进入生产流程
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// 生产中 - 订单正在生产
    /// </summary>
    InProduction = 3,

    /// <summary>
    /// 部分发货 - 订单部分商品已发货
    /// </summary>
    PartialDelivered = 4,

    /// <summary>
    /// 已发货 - 订单全部商品已发货
    /// </summary>
    Delivered = 5,

    /// <summary>
    /// 部分开票 - 订单部分金额已开发票
    /// </summary>
    PartialInvoiced = 6,

    /// <summary>
    /// 已开票 - 订单全部金额已开发票
    /// </summary>
    Invoiced = 7,

    /// <summary>
    /// 已关闭 - 订单正常结束
    /// </summary>
    Closed = 8,

    /// <summary>
    /// 已取消 - 订单被取消（反审）
    /// </summary>
    Cancelled = 9
}

/// <summary>
/// 订单状态扩展方法
/// </summary>
public static class OrderStatusExtensions
{
    /// <summary>
    /// 获取状态显示名称
    /// </summary>
    public static string GetDisplayName(this OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Draft => "草稿",
            OrderStatus.Submitted => "已提交",
            OrderStatus.Confirmed => "已确认",
            OrderStatus.InProduction => "生产中",
            OrderStatus.PartialDelivered => "部分发货",
            OrderStatus.Delivered => "已发货",
            OrderStatus.PartialInvoiced => "部分开票",
            OrderStatus.Invoiced => "已开票",
            OrderStatus.Closed => "已关闭",
            OrderStatus.Cancelled => "已取消",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// 获取状态样式类名（用于前端显示）
    /// </summary>
    public static string GetStyleClass(this OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Draft => "status-draft",
            OrderStatus.Submitted => "status-submitted",
            OrderStatus.Confirmed => "status-confirmed",
            OrderStatus.InProduction => "status-production",
            OrderStatus.PartialDelivered => "status-partial-delivered",
            OrderStatus.Delivered => "status-delivered",
            OrderStatus.PartialInvoiced => "status-partial-invoiced",
            OrderStatus.Invoiced => "status-invoiced",
            OrderStatus.Closed => "status-closed",
            OrderStatus.Cancelled => "status-cancelled",
            _ => "status-unknown"
        };
    }

    /// <summary>
    /// 判断是否为终止状态（无法再流转）
    /// </summary>
    public static bool IsTerminalStatus(this OrderStatus status)
    {
        return status is OrderStatus.Closed or OrderStatus.Cancelled;
    }

    /// <summary>
    /// 判断是否可编辑
    /// </summary>
    public static bool IsEditable(this OrderStatus status)
    {
        return status == OrderStatus.Draft;
    }

    /// <summary>
    /// 判断是否可删除
    /// </summary>
    public static bool IsDeletable(this OrderStatus status)
    {
        return status == OrderStatus.Draft;
    }
}