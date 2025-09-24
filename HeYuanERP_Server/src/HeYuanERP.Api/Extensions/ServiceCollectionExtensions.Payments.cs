using FluentValidation;
using HeYuanERP.Application.DTOs.Payments;
using HeYuanERP.Application.Interfaces;
using HeYuanERP.Application.Interfaces.Repositories;
using HeYuanERP.Application.Interfaces.Storage;
using HeYuanERP.Application.Services.Payments;
using HeYuanERP.Application.Services.Reconciliation;
using HeYuanERP.Infrastructure.Repositories;
using HeYuanERP.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeYuanERP.Api.Extensions;

/// <summary>
/// 依赖注入扩展：支付/对账相关模块注册。
/// </summary>
public static class ServiceCollectionExtensionsPayments
{
    /// <summary>
    /// 注册支付与对账模块所需服务与仓储。
    /// </summary>
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 文件存储（本地）
        services.AddScoped<IFileStorage, LocalFileStorage>();

        // 仓储
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // 应用服务
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IReconciliationService, ReconciliationService>();

        // 验证器（FluentValidation）
        services.AddScoped<IValidator<PaymentCreateDto>, HeYuanERP.Application.Validators.Payments.PaymentCreateRequestValidator>();

        return services;
    }
}
