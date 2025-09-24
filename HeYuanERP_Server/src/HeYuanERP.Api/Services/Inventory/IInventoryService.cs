using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Inventory;

// 库存服务接口：库存汇总与库存事务查询
public interface IInventoryService
{
    Task<Pagination<InventorySummaryItemDto>> QuerySummaryAsync(InventorySummaryQueryDto req, CancellationToken ct);
    Task<Pagination<InventoryTxnItemDto>> QueryTransactionsAsync(InventoryTxnQueryDto req, CancellationToken ct);
}

