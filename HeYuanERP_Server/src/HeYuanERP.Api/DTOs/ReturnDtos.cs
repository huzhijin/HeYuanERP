namespace HeYuanERP.Api.DTOs;

// 退货单相关 DTO：创建输入与详情输出

public class ReturnCreateDto
{
    public string OrderId { get; set; } = string.Empty;
    public string? SourceDeliveryId { get; set; }
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow.Date;
    public List<ReturnLineCreateDto> Lines { get; set; } = new();
}

public class ReturnLineCreateDto
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? Reason { get; set; }
}

public class ReturnDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string ReturnNo { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string? SourceDeliveryId { get; set; }
    public DateTime ReturnDate { get; set; }
    public string Status { get; set; } = "draft";
    public List<ReturnLineDto> Lines { get; set; } = new();
}

// 列表项
public class ReturnListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string ReturnNo { get; set; } = string.Empty;
    public string? OrderNo { get; set; }
    public string? SourceDeliveryNo { get; set; }
    public DateTime ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ReturnLineDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? Reason { get; set; }
}
