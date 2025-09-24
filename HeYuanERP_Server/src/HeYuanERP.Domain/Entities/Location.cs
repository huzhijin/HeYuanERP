namespace HeYuanERP.Domain.Entities;

// 领域实体：库位（Location）
// 说明：
// - 归属于某一仓库（Warehouse）；
// - Code 在同一仓库内唯一；用于精细化库存（InventoryBalance）的 loc 维度。
public class Location
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string WarehouseId { get; set; } = string.Empty; // 仓库 Id

    // 基本
    public string Code { get; set; } = string.Empty; // 库位编码（在同一仓库内唯一）
    public string Name { get; set; } = string.Empty; // 库位名称
    public bool Active { get; set; } = true;         // 是否启用

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public Warehouse? Warehouse { get; set; }
}

