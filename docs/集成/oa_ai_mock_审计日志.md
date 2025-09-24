% OA/AI 审计日志规范与示例

本文档定义对外部系统（OA/AI）交互的审计日志口径，适用于 Mock 与未来真实实现，便于问题定位与安全合规审计。

## 审计范围
- Mock 实现：`OaClientMock`、`AiClientMock` 均会在每次调用时写审计日志。
- 真实实现：应复用 `AuditHttpMessageHandler` 与 `IAuditLogger`，对所有 REST 调用写审计。

## 字段说明
- system：目标系统，取值示例 `OA`、`AI`。
- action：业务动作或 HTTP 方法，示例 `SSO.Login`、`Todo.Query`、`GET`。
- url：请求地址（Mock 以 `mock://` 前缀展示）。
- status：HTTP 状态码（异常时为空）。
- duration_ms：调用耗时（毫秒）。
- success：是否成功。
- req/resp：请求与响应内容（已做长度截断，避免大报文）。
- trace_id：链路追踪标识（OpenTelemetry TraceId，以便跨服务串联）。

## 示例日志（结构化 Serilog）
```
{ "event":"audit.external_call.ok", "system":"OA", "action":"SSO.Login", "url":"mock://oa/sso/login", "status":200, "duration_ms":12, "success":true, "trace_id":"0f...", "req":"user=U001", "resp":"{token: mock}" }
{ "event":"audit.external_call.ok", "system":"AI", "action":"Price", "url":"mock://ai/price", "status":200, "duration_ms":1, "success":true, "trace_id":"0f...", "req":"sku=SKU001,cost=12.5,margin=0.2", "resp":"price=16.37" }
{ "event":"audit.replacement", "component":"IOaClient", "impl":"OaClientMock", "target":"OA", "reason":"初期 Mock，按 OpenAPI 契约联调占位，不影响主链路" }
```

## 采集与查询
- 控制台/文件：由 Serilog Sink 决定（参考 `appsettings.*`）。
- 检索建议：
  - 按 `event` / `system` / `action` 聚合，定位失败高发接口。
  - 使用 `trace_id` 回放整条链路，结合 OpenTelemetry Trace 查看上游/下游耗时分布。

## 安全注意事项
- 不记录敏感凭证（密码、私钥、令牌原文等）；若不可避免，请在写日志前做遮蔽/脱敏。
- 大报文与二进制内容已做长度截断；必要时可只记录摘要（如 SHA-256）。
- 建议按环境变量控制日志详细程度（开发更详细，生产更克制）。

