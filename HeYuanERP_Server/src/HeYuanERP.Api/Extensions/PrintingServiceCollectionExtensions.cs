using HeYuanERP.Api.Swagger;
using HeYuanERP.Domain.Printing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HeYuanERP.Api.Extensions;

/// <summary>
/// 打印相关服务的依赖注册扩展。
/// - 选项绑定：<see cref="PrintOptions"/>（读取环境变量/配置）
/// - Swagger 配置：为打印接口声明 application/pdf 响应
/// </summary>
public static class PrintingServiceCollectionExtensions
{
    /// <summary>
    /// 注册打印相关的 API 层依赖（选项与 Swagger）。
    /// 注意：IPrintService 的具体实现由 Infrastructure 层单独注册。
    /// </summary>
    public static IServiceCollection AddPrintingApi(this IServiceCollection services, IConfiguration configuration)
    {
        // 选项：PrintOptions 使用 IConfigureOptions 进行集中绑定
        services.AddOptions<PrintOptions>();
        services.AddSingleton<IConfigureOptions<PrintOptions>>(sp =>
            new Configurations.PrintingOptionsSetup(
                sp.GetRequiredService<IConfiguration>(),
                sp.GetRequiredService<ILogger<Configurations.PrintingOptionsSetup>>()));

        // Swagger：为打印接口声明 application/pdf
        services.Configure<SwaggerGenOptions>(options =>
        {
            options.OperationFilter<ProducesPdfOperationFilter>();
        });

        return services;
    }
}

