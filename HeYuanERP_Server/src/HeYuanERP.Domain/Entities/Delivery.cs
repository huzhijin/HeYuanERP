namespace HeYuanERP.Domain.Entities;

// 领域实体：送货单（头）
// OpenAPI 对齐：id, deliveryNo, orderId, deliveryDate, status, lines
public class Delivery
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 基本
    public string DeliveryNo { get; set; } = string.Empty;  // 单据编号
    public string OrderId { get; set; } = string.Empty;     // 来源订单 Id
    public DateTime DeliveryDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "draft";

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public SalesOrder? Order { get; set; }
    public ICollection<DeliveryLine> Lines { get; set; } = new List<DeliveryLine>();
}

