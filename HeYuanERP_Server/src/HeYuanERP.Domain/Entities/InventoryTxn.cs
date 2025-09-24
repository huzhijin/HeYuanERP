namespace HeYuanERP.Domain.Entities;

// 领域实体：库存事务（流水）
// OpenAPI 对齐：txnId, txnCode(IN/OUT/DELIVERY/RETURN/PORECEIVE/ADJ), productId, qty, whse, loc, txnDate, refType, refId
public class InventoryTxn
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 事务代码（IN 入库、OUT 出库、DELIVERY 发货、RETURN 退货、PORECEIVE 采购收货、ADJ 调整）
    public string TxnCode { get; set; } = string.Empty;

    // 维度与数量
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string Whse { get; set; } = string.Empty;
    public string? Loc { get; set; }

    // 业务关联
    public DateTime TxnDate { get; set; } = DateTime.UtcNow;
    public string RefType { get; set; } = string.Empty; // 参考类型（order/delivery/return/po/receive/invoice 等）
    public string RefId { get; set; } = string.Empty;   // 参考单据主键

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    // 导航
    public Product? Product { get; set; }
}

