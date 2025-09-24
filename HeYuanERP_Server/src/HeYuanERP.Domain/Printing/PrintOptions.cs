namespace HeYuanERP.Domain.Printing;

/// <summary>
/// 打印功能的运行选项（通过环境变量/配置绑定）。
/// </summary>
public class PrintOptions
{
    /// <summary>
    /// 打印引擎：playwright 或 puppeteer（默认：playwright）。
    /// </summary>
    public string? Engine { get; set; } = "playwright";

    /// <summary>
    /// 模板根目录（例如：/app/assets/templates）。
    /// </summary>
    public string? TemplatesRoot { get; set; }

    /// <summary>
    /// 模板参数快照目录（例如：/app/var/print-snapshots）。
    /// </summary>
    public string? SnapshotsRoot { get; set; }

    /// <summary>
    /// 默认模板名（默认：default）。
    /// </summary>
    public string? DefaultTemplate { get; set; } = "default";

    /// <summary>
    /// 渲染超时时间（秒），默认 60 秒。
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}

