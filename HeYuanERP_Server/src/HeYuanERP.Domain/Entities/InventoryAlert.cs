namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 库存预警配置 - 用于设置产品的安全库存阈值
/// </summary>
public class InventoryAlert
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 产品ID
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 仓库ID（可选，为空表示所有仓库）
    /// </summary>
    public string? WarehouseId { get; set; }

    /// <summary>
    /// 库位ID（可选，为空表示所有库位）
    /// </summary>
    public string? LocationId { get; set; }

    /// <summary>
    /// 安全库存数量（低于此值触发缺货预警）
    /// </summary>
    public decimal SafetyStock { get; set; }

    /// <summary>
    /// 最大库存数量（高于此值触发超储预警）
    /// </summary>
    public decimal MaxStock { get; set; }

    /// <summary>
    /// 是否启用预警
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 预警接收人列表（用户ID，JSON数组格式）
    /// </summary>
    public string AlertRecipients { get; set; } = "[]";

    /// <summary>
    /// 预警频率（小时）- 避免频繁发送相同预警
    /// </summary>
    public int AlertFrequencyHours { get; set; } = 24;

    /// <summary>
    /// 最后预警时间
    /// </summary>
    public DateTime? LastAlertTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 导航属性：关联产品
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// 导航属性：关联仓库
    /// </summary>
    public Warehouse? Warehouse { get; set; }

    /// <summary>
    /// 导航属性：关联库位
    /// </summary>
    public Location? Location { get; set; }
}