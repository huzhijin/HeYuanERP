namespace HeYuanERP.Domain.Entities;

// 领域实体：采购收货（行）
// OpenAPI 对齐：productId, qty, whse, loc
public class POReceiveLine
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string ReceiveId { get; set; } = string.Empty; // 收货单 Id

    // 商品与数量
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }

    // 库存信息（可空）
    public string? Whse { get; set; }
    public string? Loc { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public POReceive? Receive { get; set; }
    public Product? Product { get; set; }
}

