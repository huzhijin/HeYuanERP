namespace HeYuanERP.Domain.Entities;

// 领域实体：退货单（行）
// OpenAPI 对齐：productId, qty, reason
public class ReturnLine
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string ReturnId { get; set; } = string.Empty; // 退货单 Id

    // 商品与数量
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? Reason { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public Return? Return { get; set; }
    public Product? Product { get; set; }
}

