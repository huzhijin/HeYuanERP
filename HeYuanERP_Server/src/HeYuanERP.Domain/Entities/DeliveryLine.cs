namespace HeYuanERP.Domain.Entities;

// 领域实体：送货单（行）
// OpenAPI 对齐：productId, orderLineId, qty
public class DeliveryLine
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string DeliveryId { get; set; } = string.Empty;

    // 来源订单行（可空）
    public string? OrderLineId { get; set; }

    // 商品与数量
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public Delivery? Delivery { get; set; }
    public Product? Product { get; set; }
    public SalesOrderLine? OrderLine { get; set; }
}

