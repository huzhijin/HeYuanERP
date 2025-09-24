namespace HeYuanERP.Domain.Entities;

// 领域实体：销售订单（头）
// OpenAPI 对齐：id, orderNo, accountId, orderDate, currency, status, remark, lines
public class SalesOrder
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 基本信息
    public string OrderNo { get; set; } = string.Empty;  // 单据编号（业务唯一）
    public string AccountId { get; set; } = string.Empty; // 客户 Id
    public DateTime OrderDate { get; set; } = DateTime.UtcNow.Date; // 订单日期
    public string Currency { get; set; } = "CNY";        // 币种
    public string Status { get; set; } = "draft";        // 状态：draft/confirmed/reversed
    public string? Remark { get; set; }                   // 备注

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public Account? Account { get; set; }
    public ICollection<SalesOrderLine> Lines { get; set; } = new List<SalesOrderLine>();

    // 新增：状态变更日志导航属性
    public ICollection<OrderStatusLog> StatusLogs { get; set; } = new List<OrderStatusLog>();
}

