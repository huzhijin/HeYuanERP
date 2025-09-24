namespace HeYuanERP.Domain.Entities;

// 领域实体：采购订单（行）
// OpenAPI 对齐：productId, qty, unitPrice
public class POLine
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string PoId { get; set; } = string.Empty;     // 采购订单 Id

    // 商品与数量
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public PurchaseOrder? Po { get; set; }
    public Product? Product { get; set; }
}

