namespace HeYuanERP.Domain.Entities;

// 领域实体：退货单（头）
// OpenAPI 对齐：id, returnNo, orderId, sourceDeliveryId, returnDate, status, lines
public class Return
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 基本
    public string ReturnNo { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;         // 来源订单
    public string? SourceDeliveryId { get; set; }                // 可选：来源送货单
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "draft";

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public SalesOrder? Order { get; set; }
    public Delivery? SourceDelivery { get; set; }
    public ICollection<ReturnLine> Lines { get; set; } = new List<ReturnLine>();
}

