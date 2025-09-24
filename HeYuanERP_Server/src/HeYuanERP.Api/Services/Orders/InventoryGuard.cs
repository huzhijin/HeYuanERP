using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Orders;

// 库存/主数据校验服务：
// - 校验产品是否存在且启用；
// - 预留库存可用量校验（交货时使用）；
// - 退货数量不超退（留桩，后续在 ReturnsService 中调用实现）。
public class InventoryGuard
{
    private readonly AppDbContext _db;

    public InventoryGuard(AppDbContext db)
    {
        _db = db;
    }

    // 校验产品有效性
    public async Task EnsureProductsValidAsync(IEnumerable<string> productIds, CancellationToken ct)
    {
        var ids = productIds.Where(id => !string.IsNullOrWhiteSpace(id)).Select(id => id.Trim()).Distinct().ToList();
        if (ids.Count == 0) return;
        var count = await _db.Products.AsNoTracking().CountAsync(p => ids.Contains(p.Id) && p.Active, ct);
        if (count != ids.Count)
        {
            throw new ApplicationException("存在无效或停用的产品");
        }
    }

    // 校验退货不超出已发货数量（占位，后续 ReturnsService 实现细节）
    public Task EnsureReturnNotExceedAsync(string orderId, ReturnCreateDto req, CancellationToken ct)
    {
        // 留桩：将统计 Delivery/Return 明细累计与订单行数量进行对比
        return Task.CompletedTask;
    }
}

