# OA/AI Mock 集成说明（API）

> 本文档说明在 API 层如何使用 OA/AI 的 Mock 客户端，以避免对主链路产生影响，并便于后续切换为真实外部系统。

## 注册与配置

- 入口：`src/HeYuanERP.Api/Program.cs`
- 注册：
  - `services.AddOaAiClients(Configuration)` 绑定 Options 并注册命名 `HttpClient`（含超时/重试/熔断）。
  - `IOaClient`、`IAiClient` 目前通过 `OaClientMock`、`AiClientMock` 注册（单例）。
- 环境变量（示例）：
  - `Clients__OA__BaseUrl=https://oa.example.com`
  - `Clients__OA__TimeoutSeconds=30`
  - `Clients__OA__RetryCount=3`
  - `Clients__OA__UseMock=true`
  - `Clients__AI__BaseUrl=https://ai.example.com`
  - `Clients__AI__UseMock=true`

## 审计日志

- 审计接口：`IAuditLogger`，实现：`SerilogAuditLogger`
- Mock 客户端与 `AuditHttpMessageHandler` 会记录：系统、动作、URL、状态码、耗时、成功标记、请求/响应内容摘要、TraceId。
- 查阅方式：控制台或 Serilog 目标（按实际 `appsettings.*` 配置）。

## 替换真实外部系统

1. 在基础设施层提供真实实现（例如 `OaClient`、`AiClient`）。
2. 在 `Program.cs` 或专用扩展方法中按配置切换注册（`UseMock=false` 时注册真实实现）。
3. 保持接口契约（`IOaClient`/`IAiClient`）与 OpenAPI 对齐，确保前后端联调稳定。

## 注意事项

- 所有日志与注释使用中文，便于排查与审计。
- Mock 不发起网络请求，不影响主链路；真实实现应复用命名 `HttpClient`，享受统一重试/熔断策略。
- 请求/响应体在审计中做长度截断，避免大报文影响性能与成本。

