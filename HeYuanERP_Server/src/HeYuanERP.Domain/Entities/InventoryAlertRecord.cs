namespace HeYuanERP.Domain.Entities;

/// <summary>
/// 库存预警记录 - 记录预警历史和处理状态
/// </summary>
public class InventoryAlertRecord
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 预警配置ID
    /// </summary>
    public string AlertConfigId { get; set; } = string.Empty;

    /// <summary>
    /// 产品ID
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 仓库ID
    /// </summary>
    public string? WarehouseId { get; set; }

    /// <summary>
    /// 库位ID
    /// </summary>
    public string? LocationId { get; set; }

    /// <summary>
    /// 预警类型
    /// </summary>
    public InventoryAlertType AlertType { get; set; }

    /// <summary>
    /// 当前库存数量
    /// </summary>
    public decimal CurrentStock { get; set; }

    /// <summary>
    /// 阈值（安全库存或最大库存）
    /// </summary>
    public decimal ThresholdValue { get; set; }

    /// <summary>
    /// 预警级别
    /// </summary>
    public AlertLevel Level { get; set; }

    /// <summary>
    /// 预警消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 预警状态
    /// </summary>
    public AlertStatus Status { get; set; } = AlertStatus.Active;

    /// <summary>
    /// 处理人
    /// </summary>
    public string? HandledBy { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? HandledAt { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    public string? HandledRemark { get; set; }

    /// <summary>
    /// 预警生成时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 扩展数据（JSON格式）
    /// </summary>
    public string? ExtensionData { get; set; }

    /// <summary>
    /// 导航属性：预警配置
    /// </summary>
    public InventoryAlert? AlertConfig { get; set; }

    /// <summary>
    /// 导航属性：产品
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// 导航属性：仓库
    /// </summary>
    public Warehouse? Warehouse { get; set; }

    /// <summary>
    /// 导航属性：库位
    /// </summary>
    public Location? Location { get; set; }
}

/// <summary>
/// 库存预警类型
/// </summary>
public enum InventoryAlertType
{
    /// <summary>
    /// 库存不足
    /// </summary>
    LowStock = 1,

    /// <summary>
    /// 库存超储
    /// </summary>
    OverStock = 2,

    /// <summary>
    /// 零库存
    /// </summary>
    ZeroStock = 3
}

/// <summary>
/// 预警级别
/// </summary>
public enum AlertLevel
{
    /// <summary>
    /// 信息
    /// </summary>
    Info = 1,

    /// <summary>
    /// 警告
    /// </summary>
    Warning = 2,

    /// <summary>
    /// 紧急
    /// </summary>
    Critical = 3
}

/// <summary>
/// 预警状态
/// </summary>
public enum AlertStatus
{
    /// <summary>
    /// 活跃（未处理）
    /// </summary>
    Active = 1,

    /// <summary>
    /// 已处理
    /// </summary>
    Handled = 2,

    /// <summary>
    /// 已忽略
    /// </summary>
    Ignored = 3,

    /// <summary>
    /// 自动解决（库存已恢复正常）
    /// </summary>
    AutoResolved = 4
}