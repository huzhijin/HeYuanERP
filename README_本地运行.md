# HeYuanERP 本地运行与调试指南（macOS + Rider）

本指南用于在本机以开发模式运行后端（.NET 8 WebAPI）与前端（Vue 3 + Vite）。

## 1. 前置条件
- .NET SDK 8.0+
- Node.js 18+（建议 20+）与 npm/pnpm/yarn（任选其一）
- JetBrains Rider 2024.x 或 VS Code
- 可选：Docker（用于本地 SQL Server 容器）

## 2. 环境变量与证书
- 复制根目录 `.env.example` 为 `.env`，按需填写变量（不要提交真实值）。
- 生成并信任开发证书（如使用 HTTPS）：
  - `dotnet dev-certs https --trust`

## 3. 后端（WebAPI）
目录：`HeYuanERP_Server/src/HeYuanERP.Api`

- 还原与构建
  - `dotnet restore`
  - `dotnet build -c Debug`
- 运行（默认 http://localhost:5080）
  - `export $(grep -v '^#' .env | xargs)`  # macOS zsh/bash 加载环境变量（可选）
  - `dotnet run`
- 访问验证
  - 健康检查：`http://localhost:5080/healthz`
  - Swagger：`http://localhost:5080/swagger`

说明：
- 本阶段使用 SQL Server 连接串 `DB_CONNECTION`，如需本地容器，可参考：
  - `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \`
    `-p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest`
- Rider 调试：打开 `HeYuanERP_Server/HeYuanERP.sln`，配置 `HeYuanERP.Api` 作为启动项目。
- 运行配置使用 `launchSettings.json`（开发环境，URL 建议 `http://localhost:5080`）。

## 4. 前端（heyuan-erp-web）
目录：`heyuan-erp-web`

- 安装依赖：`npm i`（或 `pnpm i`）
- 运行开发服务：`npm run dev`
- 访问：`http://localhost:5173`（Vite 默认端口）
- 接口代理/基址：`.env.development` 中 `VITE_API_BASE`，应与后端地址一致（默认 `http://localhost:5080`）。

## 5. 测试与日志
- 后端测试（待后续批次补充 tests 项目）：`dotnet test HeYuanERP_Server/HeYuanERP.sln`
- 日志输出：控制台 + 文件（`HeYuanERP_Server/src/HeYuanERP.Api/logs/`）。

## 6. 常见问题
- 端口被占用：修改 `.env` 中 `ASPNETCORE_URLS` 或 `Properties/launchSettings.json`。
- 数据库连接失败：确认 SQL Server 已启动、网络可达、密码正确，或在开发阶段关闭自动数据播种。
- Swagger 无法访问：仅在 Development 环境启用，确认 `ASPNETCORE_ENVIRONMENT=Development`。

## 7. 路径速查
- 解决方案：`HeYuanERP_Server/HeYuanERP.sln`
- 后端入口：`HeYuanERP_Server/src/HeYuanERP.Api/Program.cs`
- 配置：`HeYuanERP_Server/src/HeYuanERP.Api/appsettings.json` + 环境变量
- 健康与文档：`/healthz`、`/swagger`

