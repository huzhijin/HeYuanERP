namespace HeYuanERP.Api.Options;

/// <summary>
/// 打印配置项（从环境变量或配置文件绑定）。
/// 仅在 API 层使用，用于控制默认模板与下载文件名等。
/// </summary>
public class PrintingOptions
{
    /// <summary>
    /// 默认发票模板代码（P11 侧维护）。
    /// </summary>
    public string InvoiceTemplateCode { get; set; } = "INVOICE_STANDARD";

    /// <summary>
    /// 下载文件名前缀，例如：INV_。
    /// </summary>
    public string DownloadNamePrefix { get; set; } = "INV_";
}

