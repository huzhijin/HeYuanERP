# HeYuanERP_Server - P9 功能说明（Payments 列表/创建、对账单导出、凭证接口桩）

> 本文档描述 P9 增量交付在后端的目录位置、接口约定、环境变量与本地运行/验证步骤。所有日志与注释均为中文。

## 模块与文件概览

- Domain 领域模型
  - `src/HeYuanERP.Domain/Entities/Payment.cs` 收款实体
  - `src/HeYuanERP.Domain/Entities/PaymentAttachment.cs` 收款附件实体
- Application 通用与应用层
  - `src/HeYuanERP.Application/Common/Models/ApiResponse.cs` 统一响应
  - `src/HeYuanERP.Application/Common/Models/PagedRequest.cs` 分页请求
  - `src/HeYuanERP.Application/Common/Models/PagedResult.cs` 分页结果
  - `src/HeYuanERP.Application/DTOs/Payments/PaymentDtos.cs` 支付相关 DTO
  - `src/HeYuanERP.Application/Validators/Payments/PaymentCreateRequestValidator.cs` 创建验证器（FluentValidation）
  - `src/HeYuanERP.Application/Interfaces/IPaymentService.cs` 应用服务接口
  - `src/HeYuanERP.Application/Services/Payments/PaymentService.cs` 应用服务实现
  - `src/HeYuanERP.Application/Services/Reconciliation/ReconciliationService.cs` 对账导出服务
- Infrastructure 基础设施
  - `src/HeYuanERP.Infrastructure/Persistence/Configurations/PaymentConfiguration.cs` EF 映射（Payment）
  - `src/HeYuanERP.Infrastructure/Persistence/Configurations/PaymentAttachmentConfiguration.cs` EF 映射（Attachment）
  - `src/HeYuanERP.Infrastructure/Repositories/IPaymentRepository.cs` 仓储接口
  - `src/HeYuanERP.Infrastructure/Repositories/PaymentRepository.cs` 仓储实现（EF）
  - `src/HeYuanERP.Infrastructure/Storage/IFileStorage.cs` 文件存储抽象
  - `src/HeYuanERP.Infrastructure/Storage/LocalFileStorage.cs` 本地文件存储实现
  - `src/HeYuanERP.Infrastructure/Persistence/Migrations/20250910_AddPayments.cs` 示例迁移
- Api 层（WebAPI）
  - `src/HeYuanERP.Api/Middleware/UnifiedResponseMiddleware.cs` 统一响应与异常捕获
  - `src/HeYuanERP.Api/Extensions/ApplicationBuilderExtensions.UnifiedResponse.cs` 中间件启用扩展
  - `src/HeYuanERP.Api/Extensions/ServiceCollectionExtensions.Payments.cs` 模块依赖注入注册
  - `src/HeYuanERP.Api/Requests/Payments/PaymentCreateForm.cs` 创建表单模型（multipart/form-data）
  - `src/HeYuanERP.Api/Requests/Payments/PaymentListQuery.cs` 列表查询参数
  - `src/HeYuanERP.Api/Controllers/PaymentsController.cs` 列表/创建/详情
  - `src/HeYuanERP.Api/Controllers/ReconciliationController.cs` 对账单导出（CSV）
  - `src/HeYuanERP.Api/Controllers/VouchersController.cs` 凭证接口桩

## 环境变量（示例见仓库根 `.env.example`）

必须通过环境变量注入连接串与密钥；以下为关键项：

- `ConnectionStrings__Default` SQL Server 连接串（示例：`Server=localhost,1433;Database=HeYuanERP;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;`）
- `JWT__Issuer`、`JWT__Audience`、`JWT__Secret` JWT 配置（开发环境请使用非空 Secret）
- `HEYUANERP_FILE_STORAGE_ROOT` 文件存储根目录（绝对路径；默认 `./storage`）
- `HEYUANERP_FILE_BASE_URL` 文件对外基础 URL（可选，用于拼接附件公开地址）
- `ASPNETCORE_ENVIRONMENT`（建议 `Development`）
- `ASPNETCORE_URLS`（示例 `http://0.0.0.0:5000`）
- OpenTelemetry（可选）：`OTEL_SERVICE_NAME`、`OTEL_EXPORTER_OTLP_ENDPOINT`、`OTEL_EXPORTER_OTLP_PROTOCOL`

说明：.NET 默认不自动读取 `.env` 文件，请在本地 shell 中 `export` 或通过容器编排注入。

## 构建与运行（本地）

1) 进入 API 项目目录并还原/构建

```bash
cd HeYuanERP_Server/src/HeYuanERP.Api
dotnet restore
dotnet build -c Debug
```

2) 配置环境变量（示例，macOS/Linux）

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://0.0.0.0:5000
export ConnectionStrings__Default="Server=localhost,1433;Database=HeYuanERP;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
export JWT__Issuer=heyuanerp.dev
export JWT__Audience=heyuanerp.web
export JWT__Secret=change_this_dev_secret_please
export HEYUANERP_FILE_STORAGE_ROOT="$(pwd)/../../storage"
```

3) 初始化数据库（如使用 EF Core 工具）

```bash
# 确保已安装 dotnet-ef 并配置 DbContext；此处示例迁移文件为手写
dotnet tool restore || true
dotnet ef database update
```

4) 运行 API

```bash
dotnet run
# 浏览器访问： http://localhost:5000/swagger
```

## 接口与验证

- 列表：`GET /api/payments`
  - 参：`page`、`pageSize`、`sortBy`、`sortOrder`、`method`、`minAmount`、`maxAmount`、`dateFrom`、`dateTo`、`keyword`
  - 出：`ApiResponse<PagedResult<PaymentListItemDto>>`
- 创建：`POST /api/payments`（`multipart/form-data`）
  - 字段：`Method`、`Amount`、`PaymentDate`、`Remark?`、`Attachments*`
  - 出：`ApiResponse<PaymentDetailDto>`
- 详情：`GET /api/payments/{id}`
- 对账导出：`GET /api/reconciliation/export`（CSV 文件流）
  - 参：`dateFrom?`、`dateTo?`、`method?`
- 凭证桩：`POST /api/vouchers/generate-from-payment/{paymentId}`

示例 cURL：

```bash
# 列表
curl -H "Authorization: Bearer <token>" "http://localhost:5000/api/payments?page=1&pageSize=10"

# 创建（含两个附件）
curl -X POST "http://localhost:5000/api/payments" \
  -H "Authorization: Bearer <token>" \
  -F "Method=现金" \
  -F "Amount=100.50" \
  -F "PaymentDate=2025-09-10" \
  -F "Remark=测试收款" \
  -F "Attachments=@/path/a.jpg" \
  -F "Attachments=@/path/b.pdf"

# 导出对账单
curl -L -H "Authorization: Bearer <token>" -o reconciliation.csv "http://localhost:5000/api/reconciliation/export?dateFrom=2025-09-01&dateTo=2025-09-10"
```

## 统一响应与追踪

- 统一响应：`ApiResponse<T>`（`code=0` 成功，`message` 消息，`data` 数据，`traceId` 追踪）
- 中间件：`UnifiedResponseMiddleware` 为所有响应注入 `X-Trace-Id`，并捕获未处理异常返回统一错误结构

## 安全与 RBAC（占位）

- 认证：通过 `Authorization: Bearer <jwt>` 鉴权；RBAC 与权限粒度将在后续任务接入
- 组织：使用请求头 `X-Org-Id` 透传组织标识

## 打印与监控（占位）

- 打印：计划使用 Chromium Headless（初期不在本次范围内）
- 监控：OpenTelemetry 采样与导出配置通过 `OTEL_*` 环境变量注入

## 注意事项

- 迁移文件当前为手写示例，正式集成请使用 `dotnet ef migrations add` 生成，确保与 DbContext 完整一致
- 文件存储默认使用本地实现 `LocalFileStorage`，生产环境可替换为对象存储实现（需实现 `IFileStorage`）

