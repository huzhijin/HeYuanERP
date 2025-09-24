namespace HeYuanERP.Api.DTOs;

// 送货单相关 DTO：创建/打印所需

public class DeliveryCreateDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; } = DateTime.UtcNow.Date;
    public List<DeliveryLineCreateDto> Lines { get; set; } = new();
}

public class DeliveryLineCreateDto
{
    public string ProductId { get; set; } = string.Empty;
    public string? OrderLineId { get; set; }
    public decimal Qty { get; set; }
}

public class DeliveryDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string DeliveryNo { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public string Status { get; set; } = "draft";
    public List<DeliveryLineDto> Lines { get; set; } = new();
}

// 列表项
public class DeliveryListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string DeliveryNo { get; set; } = string.Empty;
    public string? OrderNo { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class DeliveryLineDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? OrderLineId { get; set; }
    public decimal Qty { get; set; }
}

// 打印请求（预览/生成）
public class DeliveryPrintRequest
{
    public string DeliveryId { get; set; } = string.Empty;
    public bool PreviewOnly { get; set; } = true;   // true: 返回预览 HTML/PDF 数据；false: 生成并返回文件链接
}
