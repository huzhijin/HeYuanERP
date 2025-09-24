using System;

namespace HeYuanERP.Domain.Invoices;

/// <summary>
/// 电子发票信息（可选）。
/// 说明：领域层仅定义结构，具体持久化映射在基础设施层配置（例如 EF Core Owned Entity）。
/// </summary>
public class EInvoiceInfo
{
    /// <summary>
    /// 发票代码（电票可为空）。
    /// </summary>
    public string? InvoiceCode { get; set; }

    /// <summary>
    /// 发票号码（电票可为空）。
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// 校验码（部分电票可用）。
    /// </summary>
    public string? CheckCode { get; set; }

    /// <summary>
    /// PDF 下载链接（由电票服务返回）。
    /// </summary>
    public string? PdfUrl { get; set; }

    /// <summary>
    /// 查看链接（可用于在线预览）。
    /// </summary>
    public string? ViewUrl { get; set; }

    /// <summary>
    /// 二维码图片链接（如有）。
    /// </summary>
    public string? QrCodeUrl { get; set; }

    /// <summary>
    /// 开具时间（电票服务回填）。
    /// </summary>
    public DateTimeOffset? ElectronicIssuedAt { get; set; }

    /// <summary>
    /// 购方税号（可选）。
    /// </summary>
    public string? BuyerTaxId { get; set; }

    /// <summary>
    /// 销方税号（可选）。
    /// </summary>
    public string? SellerTaxId { get; set; }
}

