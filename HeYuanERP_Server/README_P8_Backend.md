# 后端（P8 发票）运行与说明

## 环境变量
参考仓库根目录 `.env.example` 并设置：
- `DB_CONNECTION`：SQL Server 连接串（或设置 `DB_PROVIDER=sqlite` + `SQLITE_CONNECTION` 使用 SQLite 本地文件）
- `JWT__Issuer`、`JWT__Audience`、`JWT__Secret`
- 打印服务（P11）：`PRINTING_BASE_URL`（必填）、`PRINTING_API_KEY`（可选）、`PRINTING_RENDER_PATH`（默认 `/api/print/render`）、`PRINTING_TIMEOUT_SECONDS`

## 构建与运行
```bash
cd HeYuanERP_Server/src/HeYuanERP.Api
dotnet build
dotnet run
# 启动后默认监听 http://localhost:5080
```

## 验证接口
- Swagger：访问 `http://localhost:5080/swagger`
- 健康检查：`http://localhost:5080/healthz`
- 发票接口：
  - `POST /api/Invoices` 从订单/交货创建发票
  - `GET /api/Invoices` 分页查询
  - `GET /api/Invoices/{id}` 详情
  - `GET /api/invoices/{id}/print/pdf` 打印 PDF（调用 P11）

> 说明：打印客户端基于 `PRINTING_*` 环境变量自动配置，需保证 P11 打印服务可用。

