namespace HeYuanERP.Api.DTOs;

// 订单相关 DTO：用于列表/详情/新建/编辑/确认/反审

// 列表查询
public class OrderListQueryDto
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public string? Keyword { get; set; }         // 订单号模糊
    public string? AccountId { get; set; }       // 客户过滤
    public DateTime? From { get; set; }          // 起始日期（OrderDate）
    public DateTime? To { get; set; }            // 截止日期（OrderDate）
    public string? Status { get; set; }          // 状态：draft/confirmed/reversed
}

// 列表项
public class OrderListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Currency { get; set; } = "CNY";
    public string Status { get; set; } = "draft";
    public string? Remark { get; set; }

    // 汇总
    public decimal TotalQty { get; set; }
    public decimal TotalAmount { get; set; }        // 不含税
    public decimal TotalTax { get; set; }
    public decimal TotalWithTax { get; set; }
}

// 详情（含行）
public class OrderDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Currency { get; set; } = "CNY";
    public string Status { get; set; } = "draft";
    public string? Remark { get; set; }

    public List<OrderLineDto> Lines { get; set; } = new();

    // 审计
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 汇总
    public decimal TotalQty { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalWithTax { get; set; }
}

public class OrderLineDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public DateTime? DeliveryDate { get; set; }
}

// 新建/编辑输入
public class OrderCreateDto
{
    public string AccountId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow.Date;
    public string Currency { get; set; } = "CNY";
    public string? Remark { get; set; }
    public List<OrderLineCreateDto> Lines { get; set; } = new();
}

public class OrderLineCreateDto
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }   // 0..1
    public decimal TaxRate { get; set; }    // 0..1
    public DateTime? DeliveryDate { get; set; }
}

public class OrderUpdateDto
{
    public string AccountId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow.Date;
    public string Currency { get; set; } = "CNY";
    public string? Remark { get; set; }
    public List<OrderLineUpdateDto> Lines { get; set; } = new();
}

public class OrderLineUpdateDto : OrderLineCreateDto
{
    public string? Id { get; set; }          // 行Id（可为空：新增行）
    public bool? _deleted { get; set; }      // 软删除标记：true 表示删除该行
}

// 操作输入：确认/反审
public class OrderConfirmDto
{
    public string Id { get; set; } = string.Empty;
}

public class OrderReverseDto
{
    public string Id { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

// =================== 新增：订单状态管理相关 DTO ===================

/// <summary>
/// 订单状态流转请求DTO
/// </summary>
public class OrderStatusTransitionDto
{
    /// <summary>
    /// 目标状态（如：confirmed, inproduction, delivered等）
    /// </summary>
    public string TargetStatus { get; set; } = string.Empty;

    /// <summary>
    /// 流转原因
    /// </summary>
    public string? Reason { get; set; }
}

/// <summary>
/// 订单状态操作原因DTO
/// </summary>
public class OrderStatusReasonDto
{
    /// <summary>
    /// 操作原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}

