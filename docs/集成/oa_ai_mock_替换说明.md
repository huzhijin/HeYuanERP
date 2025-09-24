# OA / AI Mock 替换说明与审计

本文档面向实施与运维，说明在初期以 Mock 方式接入 OA（SSO/待办/回写/附件）与 AI（定价/预测/信用）能力，并在后续平滑切换到真实外部系统的方法。

## 目标

- 不影响主链路：外部依赖未准备就绪时，系统仍可联调与演示。
- 可观测：所有外部交互（含 Mock）纳入审计日志，便于排查与复盘。
- 可切换：通过环境变量与 DI 注册切换 Mock/真实实现。

## 架构与契约

- 接口契约：
  - OA：`IOaClient`（SSO/待办/回写/附件），DTO 位于 `HeYuanERP.Application.OA`。
  - AI：`IAiClient`（定价/预测/信用），DTO 位于 `HeYuanERP.Application.AI`。
- 实现：
  - 初期：`OaClientMock`、`AiClientMock`（纯内存模拟、稳定快速）。
  - 后续：提供 `OaClient`、`AiClient`（REST，复用统一 HttpClient + Polly 策略）。

## 配置项（环境变量）

- 命名空间：`Clients:OA` 与 `Clients:AI`（环境变量形如 `Clients__OA__BaseUrl`）。
- 关键项：
  - `BaseUrl`、`TimeoutSeconds`、`RetryCount`、`RetryBaseDelayMs`、`RetryMaxDelayMs`
  - `CircuitBreakerFailureThreshold`、`CircuitBreakerSamplingSeconds`、`CircuitBreakerMinimumThroughput`、`CircuitBreakerBreakSeconds`
  - `UseMock`：是否启用 Mock（默认 true）。

## 切换步骤

1. 部署真实外部系统或网关，确认连通性与鉴权方式。
2. 在基础设施层实现真实客户端，复用命名 `HttpClient` 与 `AuditHttpMessageHandler`。
3. 配置 `Clients__OA__UseMock=false` / `Clients__AI__UseMock=false`。
4. 在 `Program.cs` 或扩展方法中注册真实实现（取消 Mock 注册）。
5. 通过 Swagger / Postman 验证接口，确认审计日志记录外部请求与响应。

## 审计与追踪

- 审计接口：`IAuditLogger`，当前实现：`SerilogAuditLogger`。
- 记录字段：系统、动作、URL、HTTP 状态码、耗时、是否成功、请求/响应内容摘要、TraceId（OpenTelemetry）。
- 查看方式：控制台/文件/集中日志方案（取决于 Serilog Sink 配置）。

## 回滚策略

- 发现外部系统不稳定或 SLA 不满足时，可临时将 `UseMock` 切回 `true`，恢复演示/联调能力。
- 熔断策略（Polly）会在失败率高时自动断路，保护主链路；同时审计日志保留完整现场信息。

