namespace HeYuanERP.Domain.Entities;

// 领域实体：仓库（Warehouse）
// 说明：
// - 用于库存归集与业务单据（如采购收货）的默认入库地点。
// - Code 为业务唯一编码；Active 表示是否启用。
public class Warehouse
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 基本信息
    public string Code { get; set; } = string.Empty; // 仓库编码（唯一）
    public string Name { get; set; } = string.Empty; // 仓库名称
    public bool Active { get; set; } = true;         // 是否启用

    // 扩展信息（可选）
    public string? Address { get; set; }             // 地址
    public string? Contact { get; set; }             // 联系人
    public string? Phone { get; set; }               // 联系电话

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航：库位集合
    public ICollection<Location> Locations { get; set; } = new List<Location>();
}

