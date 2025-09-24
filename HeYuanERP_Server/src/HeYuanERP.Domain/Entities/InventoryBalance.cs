namespace HeYuanERP.Domain.Entities;

// 领域实体：库存结存（按 产品/仓库/库位 聚合）
// OpenAPI 对齐（InventorySummary）：productId, whse, loc, onHand, reserved, available
public class InventoryBalance
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 维度
    public string ProductId { get; set; } = string.Empty; // 产品 Id
    public string Whse { get; set; } = string.Empty;      // 仓库编码
    public string? Loc { get; set; }                      // 库位（可空）

    // 数量（建议 decimal 精度在配置中统一设置）
    public decimal OnHand { get; set; }      // 现存量
    public decimal Reserved { get; set; }    // 已预留
    public decimal Available { get; set; }   // 可用量（通常 = OnHand - Reserved）

    // 审计
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }

    // 导航
    public Product? Product { get; set; }
}

