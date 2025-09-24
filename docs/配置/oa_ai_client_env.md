% OA/AI 客户端环境变量配置

本文档描述 OA 与 AI 客户端的环境变量，包括 Endpoint、超时、重试与熔断参数，以及 Mock 开关。建议在本地使用 `.env`，在生产使用平台级环境变量/密钥管理。

## 变量命名
- OA 命名空间：`Clients__OA__*`
- AI 命名空间：`Clients__AI__*`

## 关键变量
- `BaseUrl`：外部服务基础地址（含协议），示例 `https://oa.example.com`、`https://ai.example.com`
- `TimeoutSeconds`：HttpClient 超时（秒）
- `RetryCount`：重试次数（瞬时错误：5xx/网络/408）
- `RetryBaseDelayMs` / `RetryMaxDelayMs`：重试指数退避的基数与上限（毫秒）
- `CircuitBreakerFailureThreshold`：失败比例阈值（0-1）
- `CircuitBreakerSamplingSeconds`：采样窗口（秒）
- `CircuitBreakerMinimumThroughput`：窗口内最小吞吐量
- `CircuitBreakerBreakSeconds`：断开时长（秒）
- `UseMock`：是否启用 Mock 实现（默认 `true`）

## 示例（.env）
```
# OA
Clients__OA__BaseUrl=https://oa.example.com
Clients__OA__TimeoutSeconds=30
Clients__OA__RetryCount=3
Clients__OA__RetryBaseDelayMs=200
Clients__OA__RetryMaxDelayMs=2000
Clients__OA__CircuitBreakerFailureThreshold=0.5
Clients__OA__CircuitBreakerSamplingSeconds=60
Clients__OA__CircuitBreakerMinimumThroughput=20
Clients__OA__CircuitBreakerBreakSeconds=30
Clients__OA__UseMock=true

# AI
Clients__AI__BaseUrl=https://ai.example.com
Clients__AI__TimeoutSeconds=30
Clients__AI__RetryCount=3
Clients__AI__RetryBaseDelayMs=200
Clients__AI__RetryMaxDelayMs=2000
Clients__AI__CircuitBreakerFailureThreshold=0.5
Clients__AI__CircuitBreakerSamplingSeconds=60
Clients__AI__CircuitBreakerMinimumThroughput=20
Clients__AI__CircuitBreakerBreakSeconds=30
Clients__AI__UseMock=true
```

## 生效方式
- ASP.NET Core 会自动从环境变量读取配置；`OaAiClientBootstrap` 绑定到 `Clients:OA`、`Clients:AI` 节点。
- 本地开发可使用 `dotnet user-secrets` 或 `.env` + 终端导入：`export $(grep -v '^#' .env | xargs)`。

## 建议值
- 开发：`RetryCount=1~3`、`TimeoutSeconds=10~30`，`UseMock=true`
- 生产：根据 SLA 调整重试/熔断窗口；`UseMock=false`，接入真实外部系统。

