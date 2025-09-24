# 打印服务（IPrintService）与模板说明

本文档说明 HeYuanERP 的打印架构、环境变量、安装步骤、接口规范及故障排查。所有说明与示例均以中文编写。

## 架构概览

- 打印接口：`GET /api/print/{docType}/{id}?template=xxx` 返回 `application/pdf`。
- 引擎选择：Chromium Headless（可选 `Playwright` 或 `PuppeteerSharp`），由环境变量 `PRINT_ENGINE` 控制。
- 模板渲染：应用层接口 `IPrintTemplateRenderer`，基础实现为简易占位符替换（`{{ key }}`）。
- 参数快照：`IPrintSnapshotStore` 保存打印时视图模型 JSON，保障复打一致性（默认文件系统）。
- 统一配置：`PrintOptions` 通过环境变量/配置文件绑定，日志与注释均为中文。

## 环境变量

在 `HeYuanERP_Server/.env.example` 中提供了示例，复制为 `.env` 后根据环境调整。

- `PRINT_ENGINE`：打印引擎，`playwright` 或 `puppeteer`，默认 `playwright`。
- `PRINT_TEMPLATES_ROOT`：模板根目录，默认 `<AppBase>/assets/templates`。
- `PRINT_SNAPSHOT_DIR`：快照根目录，默认 `<AppBase>/var/print-snapshots`。
- `PRINT_DEFAULT_TEMPLATE`：默认模板名，默认 `default`。
- `CHROMIUM_EXECUTABLE_PATH`：仅 PuppeteerSharp 可选，显式指定 Chromium 路径。

## 安装 Playwright 浏览器

Playwright 依赖独立的浏览器二进制。提供了跨平台脚本：

```bash
# macOS / Linux
bash HeYuanERP_Server/scripts/install-playwright-browsers.sh chromium

# Windows PowerShell
pwsh HeYuanERP_Server/scripts/install-playwright-browsers.ps1 -Browsers chromium
```

说明：脚本将尝试使用 `.NET` 全局工具 `Microsoft.Playwright.CLI` 执行 `playwright install`；若不可用会尝试在构建产物中调用 `playwright.ps1`/`playwright.sh`。

## 模板与资源组织

约定目录：`assets/templates/{docType}/{template}.html`，共享资源放在 `assets/templates/shared/`。

- 公共样式：`shared/common.css`、扩展打印样式：`shared/print.css`。
- 共享片段：`shared/header.html`、`shared/footer.html`，通过 `<!--#include "..."-->` 引入。
- 水印资源：`shared/watermark.svg`。

模板中变量占位符使用 `{{ key }}`，支持点号路径（如 `{{ customer.name }}`）。资源相对路径依赖运行时注入的 `<base href="...">`，无需手写绝对路径。

## 依赖注册（示例）

在 `Program.cs`（或 Startup）中注册依赖：

```csharp
// 选项与 Swagger（API 层扩展）
builder.Services.AddPrintingApi(builder.Configuration);

// 应用层验证器
builder.Services.AddScoped<FluentValidation.IValidator<HeYuanERP.Application.Printing.PrintRequest>, HeYuanERP.Application.Printing.Validation.PrintRequestValidator>();

// 基础设施实现
builder.Services.AddSingleton<HeYuanERP.Infrastructure.Printing.HtmlTemplateLoader>();
builder.Services.AddScoped<HeYuanERP.Application.Printing.IPrintTemplateRenderer, HeYuanERP.Infrastructure.Printing.PrintTemplateRenderer>();
builder.Services.AddScoped<HeYuanERP.Application.Printing.IPrintSnapshotStore, HeYuanERP.Infrastructure.Printing.Snapshot.PrintSnapshotFileSystemStore>();

// 根据配置选择打印引擎
builder.Services.AddScoped<HeYuanERP.Application.Printing.IPrintService>(sp =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HeYuanERP.Domain.Printing.PrintOptions>>().Value;
    return (opts.Engine ?? "playwright").Trim().ToLowerInvariant() switch
    {
        "puppeteer" => sp.GetRequiredService<HeYuanERP.Infrastructure.Printing.PuppeteerSharpPrintService>(),
        _ => sp.GetRequiredService<HeYuanERP.Infrastructure.Printing.PlaywrightPrintService>()
    };
});

// 注入两种引擎实现（供上面工厂选择）
builder.Services.AddScoped<HeYuanERP.Infrastructure.Printing.PlaywrightPrintService>();
builder.Services.AddScoped<HeYuanERP.Infrastructure.Printing.PuppeteerSharpPrintService>();
```

> 注意：上面仅为参考示例，实际项目请根据现有 `Program.cs` 结构融入。

## 运行与验证

1) 准备环境变量（复制 `.env.example` 为 `.env`，或通过操作系统变量配置）。

2) 安装 Playwright 浏览器：

```bash
bash HeYuanERP_Server/scripts/install-playwright-browsers.sh chromium
```

3) 启动 API（以下为示意命令，具体以项目为准）：

```bash
dotnet run --project HeYuanERP_Server/src/HeYuanERP.Api
```

4) 访问打印接口（示例）：

```
GET /api/print/order/12345?template=default
Accept: application/pdf
```

预期：返回 `200 OK`，响应体为 PDF。文件名形如 `order-12345.pdf`。

## 故障排查

- 404 模板不存在：检查 `assets/templates/{docType}/{template}.html` 是否存在，大小写是否匹配。
- 500 打印失败：查看应用日志（Serilog）与控制台输出；确认已安装 Playwright 浏览器。
- Puppeteer 启动失败：设置 `CHROMIUM_EXECUTABLE_PATH` 为系统可用的 Chromium 路径。
- 变量无内容：确认快照或后端渲染数据键名与模板占位符一致。

## 附录：快照目录结构

```
var/print-snapshots/
 └── {docType}/
     └── {id}/
         └── {template}.json
```

示例键：`order/12345/default`。

