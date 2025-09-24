using System.Text;
using HeYuanERP.Api.Configuration;
using HeYuanERP.Api.Extensions;
using HeYuanERP.Api.Swagger;
using HeYuanERP.Domain.Printing;
using HeYuanERP.Infrastructure.Persistence;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.Services;
using HeYuanERP.Application.Printing;
using HeYuanERP.Infrastructure.Printing;
using HeYuanERP.Infrastructure.Printing.Snapshot;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Services.Orders;
using HeYuanERP.Api.Services.Invoices;
using HeYuanERP.Api.Services.Inventory;
using HeYuanERP.Api.Services.Purchase;
using HeYuanERP.Api.Services.Attachments;
using HeYuanERP.Api.Services.BackgroundTasks;
using HeYuanERP.Api.Services.CRM;
using HeYuanERP.Api.Services.ProductPrice;
using HeYuanERP.Api.Services.Reconciliation;
using HeYuanERP.Api.Services.Expense;
using HeYuanERP.Api.Services.Finance;

var builder = WebApplication.CreateBuilder(args);

// 读取环境变量优先
var configuration = builder.Configuration
    .AddEnvironmentVariables()
    .Build();

// 基础服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<ProducesPdfOperationFilter>();
});

// 打印配置
builder.Services.Configure<PrintOptions>(configuration);
// 打印实现（Playwright + 文件快照 + 简易模板渲染）
builder.Services.AddScoped<IPrintTemplateRenderer, PrintTemplateRenderer>();
builder.Services.AddScoped<IPrintSnapshotStore, PrintSnapshotFileSystemStore>();
builder.Services.AddScoped<IPrintService, PlaywrightPrintService>();

// 依赖注入：支付/对账模块
builder.Services.AddPaymentsModule(configuration);

// 依赖注入：核心业务服务（P0 补全）
builder.Services.AddScoped<IOrderStateService, OrderStateService>();
builder.Services.AddScoped<IInvoiceBusinessRuleService, InvoiceBusinessRuleService>();
builder.Services.AddScoped<IInventoryAlertService, InventoryAlertService>();
builder.Services.AddScoped<IPurchaseExceptionService, PurchaseExceptionService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

// P1阶段新增服务注册
builder.Services.AddScoped<HeYuanERP.Api.Services.CRM.ICRMService, HeYuanERP.Api.Services.CRM.CRMService>();
builder.Services.AddScoped<HeYuanERP.Api.Services.ProductPrice.IProductPriceService, HeYuanERP.Api.Services.ProductPrice.ProductPriceService>();
builder.Services.AddScoped<HeYuanERP.Api.Services.Reconciliation.IReconciliationService, HeYuanERP.Api.Services.Reconciliation.ReconciliationService>();
builder.Services.AddScoped<HeYuanERP.Api.Services.Expense.IExpenseService, HeYuanERP.Api.Services.Expense.ExpenseService>();
builder.Services.AddScoped<HeYuanERP.Api.Services.Finance.IAccountsReceivablePayableService, HeYuanERP.Api.Services.Finance.AccountsReceivablePayableService>();

// 权限查询服务（供应用内部使用，与 [RequirePermission] 相辅相成）
builder.Services.AddScoped<IPermissionService, PermissionService>();

// 后台任务：库存预警扫描
builder.Services.AddHostedService<InventoryAlertBackgroundService>();

// 注册 OA/AI HttpClient 与弹性策略 + 可选 Mock
builder.Services.AddOaAiClients(configuration)
                .AddExternalAudit()
                .AddMockClientsIfEnabled(configuration);

// 数据库（SQLite）：业务与认证上下文均使用同一 SQLite 文件
var provider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER") ?? configuration["DATABASE_PROVIDER"];
var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? configuration.GetConnectionString("Default") ?? "Data Source=heyuanerp_dev.db";

// 仅启用 Sqlite（按你的要求，不再改动已用 Sqlite 的前提）
builder.Services.AddDbContext<HeYuanERPDbContext>(opt => opt.UseSqlite(connStr));
// 让依赖注入到 DbContext 的实现复用同一个上下文实例
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<HeYuanERPDbContext>());

// 认证上下文（AppDbContext）同样使用 SQLite
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connStr));
// 播种器
builder.Services.AddScoped<DbSeeder>();

// CORS（本地前端）
var corsOrigins = (Environment.GetEnvironmentVariable("CORS__ALLOWEDORIGINS") ?? configuration["CORS:AllowedOrigins"] ?? "http://localhost:5173")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

// 认证授权（JWT）
var issuer = Environment.GetEnvironmentVariable("JWT__ISSUER") ?? configuration["JWT:Issuer"] ?? "heyuanerp";
var audience = Environment.GetEnvironmentVariable("JWT__AUDIENCE") ?? configuration["JWT:Audience"] ?? "heyuanerp.web";
var secret = Environment.GetEnvironmentVariable("JWT__SECRET") ?? configuration["JWT:Secret"] ?? "LocalDev_ChangeMe_1234567890";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission", policy => policy.Requirements.Add(new PermissionRequirement()));
});

// 注册 JwtTokenService（供 AuthController 使用）
builder.Services.AddSingleton(new JwtTokenService(new JwtOptions(issuer, audience, secret)));

var app = builder.Build();

// 开发环境 Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 简单主页重定向到 Swagger，避免根路径 404
app.MapGet("/", () => Results.Redirect("/swagger"));

// 开发环境自动播种（默认账号/权限等）：admin / CHANGE_ME
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    // 认证与主数据库（AppDbContext）播种
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();

    // 业务库（HeYuanERPDbContext）最小确保创建（SQLite 开发期）
    var bizDb = scope.ServiceProvider.GetRequiredService<HeYuanERPDbContext>();
    await bizDb.Database.EnsureCreatedAsync();
}

app.Run();

// 为测试项目 (WebApplicationFactory) 暴露入口点类型
public partial class Program { }
