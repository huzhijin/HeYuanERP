using HeYuanERP.Api.Services.Inventory;

namespace HeYuanERP.Api.Services.BackgroundTasks;

/// <summary>
/// 库存预警定时任务服务
/// </summary>
public class InventoryAlertBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InventoryAlertBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30); // 每30分钟检查一次

    public InventoryAlertBackgroundService(IServiceProvider serviceProvider, ILogger<InventoryAlertBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("库存预警定时任务服务已启动");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckInventoryLevels();
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("库存预警定时任务服务正在停止");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "库存预警检查任务执行失败");
                // 发生错误时等待较短时间后重试
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("库存预警定时任务服务已停止");
    }

    private async Task CheckInventoryLevels()
    {
        using var scope = _serviceProvider.CreateScope();
        var alertService = scope.ServiceProvider.GetRequiredService<IInventoryAlertService>();

        _logger.LogDebug("开始执行库存水平检查");

        var result = await alertService.CheckInventoryLevelsAsync();

        if (result.HasAlerts)
        {
            _logger.LogInformation("库存检查完成: {Message}", result.Message);
        }
        else
        {
            _logger.LogDebug("库存检查完成: 无新预警");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("正在停止库存预警定时任务服务...");
        await base.StopAsync(cancellationToken);
    }
}