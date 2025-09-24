namespace HeYuanERP.Domain.Entities;

// 领域实体：应收发票（Invoice）
// OpenAPI 对齐：id, invoiceNo, invoiceDate, accountId, orderId?, deliveryId?, amount, taxRate, amountWithTax, status
public class Invoice
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 基本信息
    public string InvoiceNo { get; set; } = string.Empty;    // 发票编号（业务唯一）
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow.Date;
    public string AccountId { get; set; } = string.Empty;    // 客户 Id
    public string? OrderId { get; set; }                     // 来源订单（可空）
    public string? DeliveryId { get; set; }                  // 来源送货（可空）

    // 金额（建议在配置中指定 decimal 精度/小数位）
    public decimal Amount { get; set; }           // 不含税金额
    public decimal TaxRate { get; set; }          // 税率（0..1）
    public decimal AmountWithTax { get; set; }    // 含税金额

    public string Status { get; set; } = "draft"; // 草稿/开票/作废 等
    public string? Remark { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public Account? Account { get; set; }
    public SalesOrder? Order { get; set; }
    public Delivery? Delivery { get; set; }
}

