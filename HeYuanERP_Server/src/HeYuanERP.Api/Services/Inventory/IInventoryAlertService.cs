using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.Services.Inventory;

/// <summary>
/// 库存预警信息DTO
/// </summary>
public class InventoryAlertInfo
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? LocationId { get; set; }
    public string? LocationName { get; set; }
    public InventoryAlertType AlertType { get; set; }
    public string AlertTypeName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal ThresholdValue { get; set; }
    public AlertLevel Level { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public AlertStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? HandledBy { get; set; }
    public DateTime? HandledAt { get; set; }
}

/// <summary>
/// 库存预警配置DTO
/// </summary>
public class InventoryAlertConfigDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? WarehouseId { get; set; }
    public string? LocationId { get; set; }
    public decimal SafetyStock { get; set; }
    public decimal MaxStock { get; set; }
    public bool IsEnabled { get; set; }
    public List<string> AlertRecipients { get; set; } = new();
    public int AlertFrequencyHours { get; set; }
    public DateTime? LastAlertTime { get; set; }
}

/// <summary>
/// 创建预警配置请求DTO
/// </summary>
public class CreateInventoryAlertConfigDto
{
    public string ProductId { get; set; } = string.Empty;
    public string? WarehouseId { get; set; }
    public string? LocationId { get; set; }
    public decimal SafetyStock { get; set; }
    public decimal MaxStock { get; set; }
    public bool IsEnabled { get; set; } = true;
    public List<string> AlertRecipients { get; set; } = new();
    public int AlertFrequencyHours { get; set; } = 24;
}

/// <summary>
/// 更新预警配置请求DTO
/// </summary>
public class UpdateInventoryAlertConfigDto
{
    public decimal SafetyStock { get; set; }
    public decimal MaxStock { get; set; }
    public bool IsEnabled { get; set; }
    public List<string> AlertRecipients { get; set; } = new();
    public int AlertFrequencyHours { get; set; }
}

/// <summary>
/// 处理预警请求DTO
/// </summary>
public class HandleAlertDto
{
    public AlertStatus Status { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 库存预警检查结果
/// </summary>
public class InventoryCheckResult
{
    public bool HasAlerts { get; set; }
    public int NewAlertsCount { get; set; }
    public int ResolvedAlertsCount { get; set; }
    public List<InventoryAlertInfo> NewAlerts { get; set; } = new();
    public List<string> ResolvedAlertIds { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 库存预警服务接口
/// </summary>
public interface IInventoryAlertService
{
    /// <summary>
    /// 执行库存水平检查（定时任务调用）
    /// </summary>
    Task<InventoryCheckResult> CheckInventoryLevelsAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取活跃的预警列表
    /// </summary>
    Task<List<InventoryAlertInfo>> GetActiveAlertsAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取预警历史记录
    /// </summary>
    Task<List<InventoryAlertInfo>> GetAlertHistoryAsync(
        string? productId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int size = 20,
        CancellationToken ct = default);

    /// <summary>
    /// 处理预警（标记为已处理、已忽略等）
    /// </summary>
    Task<bool> HandleAlertAsync(string alertId, HandleAlertDto request, string userId, CancellationToken ct = default);

    /// <summary>
    /// 发送预警通知
    /// </summary>
    Task SendAlertNotificationAsync(InventoryAlertInfo alert, CancellationToken ct = default);

    /// <summary>
    /// 创建预警配置
    /// </summary>
    Task<InventoryAlertConfigDto> CreateAlertConfigAsync(CreateInventoryAlertConfigDto request, string userId, CancellationToken ct = default);

    /// <summary>
    /// 更新预警配置
    /// </summary>
    Task<InventoryAlertConfigDto> UpdateAlertConfigAsync(string configId, UpdateInventoryAlertConfigDto request, string userId, CancellationToken ct = default);

    /// <summary>
    /// 删除预警配置
    /// </summary>
    Task<bool> DeleteAlertConfigAsync(string configId, CancellationToken ct = default);

    /// <summary>
    /// 获取预警配置列表
    /// </summary>
    Task<List<InventoryAlertConfigDto>> GetAlertConfigsAsync(string? productId = null, CancellationToken ct = default);

    /// <summary>
    /// 批量创建预警配置（批量设置安全库存）
    /// </summary>
    Task<List<InventoryAlertConfigDto>> BatchCreateAlertConfigsAsync(List<CreateInventoryAlertConfigDto> requests, string userId, CancellationToken ct = default);
}