using HeYuanERP.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Print;

// 送货单打印数据组装器：聚合送货头、行、对应订单与客户信息
public class DeliveryPrintAssembler
{
    private readonly AppDbContext _db;

    public DeliveryPrintAssembler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> BuildAsync(string deliveryId, CancellationToken ct)
    {
        var d = await _db.Deliveries
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == deliveryId, ct)
            ?? throw new KeyNotFoundException("未找到送货单");

        var order = await _db.SalesOrders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == d.OrderId, ct);
        var account = order == null ? null : await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == order.AccountId, ct);
        var prodIds = d.Lines.Select(l => l.ProductId).Distinct().ToList();
        var prodMap = await _db.Products.AsNoTracking().Where(p => prodIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name, p.Spec, p.Unit }, ct);

        var lines = d.Lines.OrderBy(l => l.CreatedAt).Select(l => new
        {
            l.Id,
            l.ProductId,
            ProductCode = prodMap.TryGetValue(l.ProductId, out var p) ? p.Code : l.ProductId,
            ProductName = prodMap.TryGetValue(l.ProductId, out var p2) ? p2.Name : "",
            Spec = prodMap.TryGetValue(l.ProductId, out var p3) ? p3.Spec : null,
            Unit = prodMap.TryGetValue(l.ProductId, out var p4) ? p4.Unit : null,
            l.OrderLineId,
            l.Qty
        }).ToList();

        var model = new
        {
            Header = new
            {
                d.Id,
                d.DeliveryNo,
                d.OrderId,
                OrderNo = order?.OrderNo,
                order?.AccountId,
                AccountName = account?.Name,
                d.DeliveryDate,
                d.Status
            },
            Lines = lines
        };

        return model;
    }
}

