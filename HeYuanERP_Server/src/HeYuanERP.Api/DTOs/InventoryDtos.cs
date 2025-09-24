namespace HeYuanERP.Api.DTOs;

// 库存相关 DTO：库存汇总与库存事务查询

// ========== 库存汇总（InventoryBalance 聚合视图） ==========
public class InventorySummaryQueryDto
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public string? ProductId { get; set; }     // 产品过滤
    public string? Whse { get; set; }          // 仓库编码过滤
    public string? Loc { get; set; }           // 库位过滤
}

public class InventorySummaryItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string Whse { get; set; } = string.Empty;
    public string? Loc { get; set; }

    public decimal OnHand { get; set; }
    public decimal Reserved { get; set; }
    public decimal Available { get; set; }

    // 平均成本：仅报表口径，当前未引入成本字段，返回空
    public decimal? AvgCost { get; set; }
}

// ========== 库存事务（InventoryTxn 明细） ==========
public class InventoryTxnQueryDto
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public DateTime? From { get; set; }        // 起始交易日期（包含）
    public DateTime? To { get; set; }          // 截止交易日期（包含）
    public string? ProductId { get; set; }
    public string? Whse { get; set; }
    public string? Loc { get; set; }
    public string? TxnCode { get; set; }       // IN/OUT/DELIVERY/RETURN/PORECEIVE/ADJ
}

public class InventoryTxnItemDto
{
    public string Id { get; set; } = string.Empty;
    public string TxnCode { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public decimal Qty { get; set; }
    public string Whse { get; set; } = string.Empty;
    public string? Loc { get; set; }
    public DateTime TxnDate { get; set; }
    public string RefType { get; set; } = string.Empty;
    public string RefId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

