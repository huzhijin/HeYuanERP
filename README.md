# HeYuanERP（Plan A：.NET 8 + Vue 3 + SQL Server）

本仓库为 HeYuanERP 前后端分离重构代码（功能与口径等价的基础骨架）。

规范与输入：
- PRD：`~/Downloads/SmartSales2008_Specs/PRD/SmartSales2008_PlanA_功能与技术PRD.md`
- OpenAPI：`~/Downloads/SmartSales2008_Specs/OpenAPI/openapi.yaml`

技术栈与约束：
- 后端：.NET 8 WebAPI + EF Core + FluentValidation + Serilog + OpenTelemetry + Swagger
- 前端：Vue 3 + Vite + TypeScript + Pinia + Vue Router + Ant Design Vue
- 打印：Chromium Headless（当前提供 Mock，后续可接入 Playwright）
- 安全：JWT + RBAC（基于权限的策略）
- 配置：所有连接串与密钥通过环境变量传入
- 统一响应：Envelope + Pagination（与 OpenAPI 模板一致）
- 注释/日志/README 统一使用中文

目录结构：
- 后端：`HeYuanERP_Server/`
  - `src/HeYuanERP.Api/`：WebAPI 主工程（为简化演示，将 Domain/Infrastructure/Validators/Services 放于此项目内）
- 前端：`heyuan-erp-web/`

快速开始：请见文末《运行与验证步骤》。
