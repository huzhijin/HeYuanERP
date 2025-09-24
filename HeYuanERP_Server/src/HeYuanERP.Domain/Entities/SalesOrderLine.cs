namespace HeYuanERP.Domain.Entities;

// 领域实体：销售订单（行）
// OpenAPI 对齐：productId, qty, unitPrice, discount, taxRate, deliveryDate
public class SalesOrderLine
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string OrderId { get; set; } = string.Empty;   // 销售订单 Id

    // 商品与数量
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }

    // 折扣/税率（0..1）
    public decimal Discount { get; set; }                 // 例如 0.1 表示 10%
    public decimal TaxRate { get; set; }                  // 例如 0.13 表示 13%

    // 计划交期
    public DateTime? DeliveryDate { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public SalesOrder? Order { get; set; }
    public Product? Product { get; set; }
}

