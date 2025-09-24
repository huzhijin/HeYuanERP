using System.Text.Json;
using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Inventory;

/// <summary>
/// 库存预警服务实现
/// </summary>
public class InventoryAlertService : IInventoryAlertService
{
    private readonly AppDbContext _db;
    private readonly ILogger<InventoryAlertService> _logger;

    public InventoryAlertService(AppDbContext db, ILogger<InventoryAlertService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<InventoryCheckResult> CheckInventoryLevelsAsync(CancellationToken ct = default)
    {
        var result = new InventoryCheckResult();

        try
        {
            // 获取所有启用的预警配置
            var configs = await _db.InventoryAlerts
                .Include(a => a.Product)
                .Include(a => a.Warehouse)
                .Include(a => a.Location)
                .Where(a => a.IsEnabled)
                .ToListAsync(ct);

            foreach (var config in configs)
            {
                // 检查该配置是否需要预警（避免频繁预警）
                if (config.LastAlertTime.HasValue &&
                    DateTime.UtcNow.Subtract(config.LastAlertTime.Value).TotalHours < config.AlertFrequencyHours)
                {
                    continue;
                }

                // 获取当前库存
                var currentStock = await GetCurrentStockAsync(config.ProductId, config.WarehouseId, config.LocationId, ct);

                // 检查是否需要生成预警
                var alertType = DetermineAlertType(currentStock, config);
                if (alertType.HasValue)
                {
                    var existingAlert = await _db.InventoryAlertRecords
                        .FirstOrDefaultAsync(r =>
                            r.ProductId == config.ProductId &&
                            r.WarehouseId == config.WarehouseId &&
                            r.LocationId == config.LocationId &&
                            r.AlertType == alertType.Value &&
                            r.Status == AlertStatus.Active, ct);

                    if (existingAlert == null)
                    {
                        // 创建新预警记录
                        var alertRecord = new InventoryAlertRecord
                        {
                            AlertConfigId = config.Id,
                            ProductId = config.ProductId,
                            WarehouseId = config.WarehouseId,
                            LocationId = config.LocationId,
                            AlertType = alertType.Value,
                            CurrentStock = currentStock,
                            ThresholdValue = alertType.Value == InventoryAlertType.LowStock ? config.SafetyStock : config.MaxStock,
                            Level = DetermineAlertLevel(currentStock, config, alertType.Value),
                            Message = GenerateAlertMessage(config, currentStock, alertType.Value),
                            Status = AlertStatus.Active
                        };

                        _db.InventoryAlertRecords.Add(alertRecord);

                        var alertInfo = MapToAlertInfo(alertRecord, config);
                        result.NewAlerts.Add(alertInfo);

                        // 更新配置的最后预警时间
                        config.LastAlertTime = DateTime.UtcNow;

                        _logger.LogInformation("Generated new inventory alert: {ProductId} - {AlertType}",
                            config.ProductId, alertType.Value);
                    }
                }
                else
                {
                    // 检查是否有可以自动解决的预警
                    var activeAlerts = await _db.InventoryAlertRecords
                        .Where(r =>
                            r.ProductId == config.ProductId &&
                            r.WarehouseId == config.WarehouseId &&
                            r.LocationId == config.LocationId &&
                            r.Status == AlertStatus.Active)
                        .ToListAsync(ct);

                    foreach (var alert in activeAlerts)
                    {
                        alert.Status = AlertStatus.AutoResolved;
                        alert.HandledAt = DateTime.UtcNow;
                        alert.HandledRemark = $"库存已恢复正常: {currentStock}";
                        result.ResolvedAlertIds.Add(alert.Id);
                    }
                }
            }

            await _db.SaveChangesAsync(ct);

            result.HasAlerts = result.NewAlerts.Any();
            result.NewAlertsCount = result.NewAlerts.Count;
            result.ResolvedAlertsCount = result.ResolvedAlertIds.Count;
            result.Message = $"检查完成: 新增 {result.NewAlertsCount} 个预警，自动解决 {result.ResolvedAlertsCount} 个预警";

            // 发送新预警通知
            foreach (var alert in result.NewAlerts)
            {
                await SendAlertNotificationAsync(alert, ct);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking inventory levels");
            result.Message = $"检查库存水平时发生错误: {ex.Message}";
            return result;
        }
    }

    public async Task<List<InventoryAlertInfo>> GetActiveAlertsAsync(CancellationToken ct = default)
    {
        var alerts = await _db.InventoryAlertRecords.AsNoTracking()
            .Include(r => r.Product)
            .Include(r => r.Warehouse)
            .Include(r => r.Location)
            .Where(r => r.Status == AlertStatus.Active)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return alerts.Select(r => MapToAlertInfo(r)).ToList();
    }

    public async Task<List<InventoryAlertInfo>> GetAlertHistoryAsync(string? productId = null, DateTime? fromDate = null,
        DateTime? toDate = null, int page = 1, int size = 20, CancellationToken ct = default)
    {
        var query = _db.InventoryAlertRecords.AsNoTracking()
            .Include(r => r.Product)
            .Include(r => r.Warehouse)
            .Include(r => r.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(productId))
        {
            query = query.Where(r => r.ProductId == productId);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(r => r.CreatedAt <= to);
        }

        var alerts = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return alerts.Select(r => MapToAlertInfo(r)).ToList();
    }

    public async Task<bool> HandleAlertAsync(string alertId, HandleAlertDto request, string userId, CancellationToken ct = default)
    {
        var alert = await _db.InventoryAlertRecords
            .FirstOrDefaultAsync(r => r.Id == alertId, ct);

        if (alert == null) return false;

        alert.Status = request.Status;
        alert.HandledBy = userId;
        alert.HandledAt = DateTime.UtcNow;
        alert.HandledRemark = request.Remark;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Alert {AlertId} handled by {UserId} with status {Status}",
            alertId, userId, request.Status);

        return true;
    }

    public async Task SendAlertNotificationAsync(InventoryAlertInfo alert, CancellationToken ct = default)
    {
        try
        {
            // 获取预警配置的接收人列表
            var config = await _db.InventoryAlerts
                .FirstOrDefaultAsync(a => a.ProductId == alert.ProductId &&
                                    a.WarehouseId == alert.WarehouseId &&
                                    a.LocationId == alert.LocationId, ct);

            if (config == null) return;

            var recipients = JsonSerializer.Deserialize<List<string>>(config.AlertRecipients) ?? new List<string>();

            // 这里可以集成邮件、短信、微信等通知方式
            // 目前记录日志作为通知
            _logger.LogWarning("库存预警通知: {Message} | 接收人: {Recipients}",
                alert.Message, string.Join(",", recipients));

            // TODO: 集成实际的通知服务
            // await _emailService.SendEmailAsync(recipients, "库存预警", alert.Message);
            // await _smsService.SendSmsAsync(recipients, alert.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending alert notification for {AlertId}", alert.Id);
        }
    }

    public async Task<InventoryAlertConfigDto> CreateAlertConfigAsync(CreateInventoryAlertConfigDto request, string userId, CancellationToken ct = default)
    {
        // 检查是否已存在相同配置
        var existing = await _db.InventoryAlerts
            .FirstOrDefaultAsync(a => a.ProductId == request.ProductId &&
                                a.WarehouseId == request.WarehouseId &&
                                a.LocationId == request.LocationId, ct);

        if (existing != null)
        {
            throw new ApplicationException("该产品和库位组合已存在预警配置");
        }

        var config = new InventoryAlert
        {
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            LocationId = request.LocationId,
            SafetyStock = request.SafetyStock,
            MaxStock = request.MaxStock,
            IsEnabled = request.IsEnabled,
            AlertRecipients = JsonSerializer.Serialize(request.AlertRecipients),
            AlertFrequencyHours = request.AlertFrequencyHours,
            CreatedBy = userId
        };

        _db.InventoryAlerts.Add(config);
        await _db.SaveChangesAsync(ct);

        return MapToConfigDto(config);
    }

    public async Task<InventoryAlertConfigDto> UpdateAlertConfigAsync(string configId, UpdateInventoryAlertConfigDto request, string userId, CancellationToken ct = default)
    {
        var config = await _db.InventoryAlerts
            .FirstOrDefaultAsync(a => a.Id == configId, ct);

        if (config == null)
        {
            throw new KeyNotFoundException("预警配置不存在");
        }

        config.SafetyStock = request.SafetyStock;
        config.MaxStock = request.MaxStock;
        config.IsEnabled = request.IsEnabled;
        config.AlertRecipients = JsonSerializer.Serialize(request.AlertRecipients);
        config.AlertFrequencyHours = request.AlertFrequencyHours;
        config.UpdatedBy = userId;
        config.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return MapToConfigDto(config);
    }

    public async Task<bool> DeleteAlertConfigAsync(string configId, CancellationToken ct = default)
    {
        var config = await _db.InventoryAlerts
            .FirstOrDefaultAsync(a => a.Id == configId, ct);

        if (config == null) return false;

        _db.InventoryAlerts.Remove(config);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<List<InventoryAlertConfigDto>> GetAlertConfigsAsync(string? productId = null, CancellationToken ct = default)
    {
        var query = _db.InventoryAlerts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(productId))
        {
            query = query.Where(a => a.ProductId == productId);
        }

        var configs = await query.OrderBy(a => a.ProductId).ToListAsync(ct);

        return configs.Select(MapToConfigDto).ToList();
    }

    public async Task<List<InventoryAlertConfigDto>> BatchCreateAlertConfigsAsync(List<CreateInventoryAlertConfigDto> requests, string userId, CancellationToken ct = default)
    {
        var results = new List<InventoryAlertConfigDto>();

        foreach (var request in requests)
        {
            try
            {
                var config = await CreateAlertConfigAsync(request, userId, ct);
                results.Add(config);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to create alert config for product {ProductId}: {Error}",
                    request.ProductId, ex.Message);
            }
        }

        return results;
    }

    #region Private Methods

    private async Task<decimal> GetCurrentStockAsync(string productId, string? warehouseId, string? locationId, CancellationToken ct)
    {
        // 适配现有领域模型：InventoryTxn(Whse/Loc/Qty)
        var query = _db.InventoryTxns.AsNoTracking()
            .Where(t => t.ProductId == productId);

        if (!string.IsNullOrWhiteSpace(warehouseId))
        {
            query = query.Where(t => t.Whse == warehouseId);
        }

        if (!string.IsNullOrWhiteSpace(locationId))
        {
            query = query.Where(t => t.Loc == locationId);
        }

        var balance = await query.SumAsync(t => t.Qty, ct);
        return balance;
    }

    private static InventoryAlertType? DetermineAlertType(decimal currentStock, InventoryAlert config)
    {
        if (currentStock <= 0)
        {
            return InventoryAlertType.ZeroStock;
        }

        if (currentStock <= config.SafetyStock)
        {
            return InventoryAlertType.LowStock;
        }

        if (currentStock >= config.MaxStock)
        {
            return InventoryAlertType.OverStock;
        }

        return null;
    }

    private static AlertLevel DetermineAlertLevel(decimal currentStock, InventoryAlert config, InventoryAlertType alertType)
    {
        return alertType switch
        {
            InventoryAlertType.ZeroStock => AlertLevel.Critical,
            InventoryAlertType.LowStock when currentStock <= config.SafetyStock * 0.5m => AlertLevel.Critical,
            InventoryAlertType.LowStock => AlertLevel.Warning,
            InventoryAlertType.OverStock when currentStock >= config.MaxStock * 1.5m => AlertLevel.Critical,
            InventoryAlertType.OverStock => AlertLevel.Warning,
            _ => AlertLevel.Info
        };
    }

    private static string GenerateAlertMessage(InventoryAlert config, decimal currentStock, InventoryAlertType alertType)
    {
        var productInfo = $"产品 {config.ProductId}";
        if (!string.IsNullOrWhiteSpace(config.WarehouseId))
        {
            productInfo += $" 仓库 {config.WarehouseId}";
        }
        if (!string.IsNullOrWhiteSpace(config.LocationId))
        {
            productInfo += $" 库位 {config.LocationId}";
        }

        return alertType switch
        {
            InventoryAlertType.ZeroStock => $"{productInfo} 零库存，当前库存: {currentStock}",
            InventoryAlertType.LowStock => $"{productInfo} 库存不足，当前库存: {currentStock}，安全库存: {config.SafetyStock}",
            InventoryAlertType.OverStock => $"{productInfo} 库存超储，当前库存: {currentStock}，最大库存: {config.MaxStock}",
            _ => $"{productInfo} 库存异常，当前库存: {currentStock}"
        };
    }

    private static InventoryAlertInfo MapToAlertInfo(InventoryAlertRecord record, InventoryAlert? config = null)
    {
        return new InventoryAlertInfo
        {
            Id = record.Id,
            ProductId = record.ProductId,
            ProductName = record.Product?.Name ?? record.ProductId,
            WarehouseId = record.WarehouseId,
            WarehouseName = record.Warehouse?.Name ?? record.WarehouseId,
            LocationId = record.LocationId,
            LocationName = record.Location?.Name ?? record.LocationId,
            AlertType = record.AlertType,
            AlertTypeName = record.AlertType.ToString(),
            CurrentStock = record.CurrentStock,
            ThresholdValue = record.ThresholdValue,
            Level = record.Level,
            LevelName = record.Level.ToString(),
            Message = record.Message,
            Status = record.Status,
            StatusName = record.Status.ToString(),
            CreatedAt = record.CreatedAt,
            HandledBy = record.HandledBy,
            HandledAt = record.HandledAt
        };
    }

    private static InventoryAlertConfigDto MapToConfigDto(InventoryAlert config)
    {
        var recipients = new List<string>();
        try
        {
            recipients = JsonSerializer.Deserialize<List<string>>(config.AlertRecipients) ?? new List<string>();
        }
        catch
        {
            // 忽略反序列化错误
        }

        return new InventoryAlertConfigDto
        {
            Id = config.Id,
            ProductId = config.ProductId,
            WarehouseId = config.WarehouseId,
            LocationId = config.LocationId,
            SafetyStock = config.SafetyStock,
            MaxStock = config.MaxStock,
            IsEnabled = config.IsEnabled,
            AlertRecipients = recipients,
            AlertFrequencyHours = config.AlertFrequencyHours,
            LastAlertTime = config.LastAlertTime
        };
    }

    #endregion
}
