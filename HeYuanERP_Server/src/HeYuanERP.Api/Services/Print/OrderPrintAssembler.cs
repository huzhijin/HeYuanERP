using HeYuanERP.Api.Data;
using HeYuanERP.Api.Services.Orders;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Print;

// 订单打印数据组装器：聚合订单头、行、客户与价格汇总，供模板渲染使用
public class OrderPrintAssembler
{
    private readonly AppDbContext _db;
    private readonly PricingService _pricing;

    public OrderPrintAssembler(AppDbContext db, PricingService pricing)
    {
        _db = db;
        _pricing = pricing;
    }

    public async Task<object> BuildAsync(string orderId, CancellationToken ct)
    {
        var o = await _db.SalesOrders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == orderId, ct)
            ?? throw new KeyNotFoundException("未找到订单");

        var account = await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == o.AccountId, ct);
        var prodIds = o.Lines.Select(l => l.ProductId).Distinct().ToList();
        var prodMap = await _db.Products.AsNoTracking().Where(p => prodIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name, p.Spec, p.Unit }, ct);

        var totals = _pricing.Compute(o.Lines.Select(l => (l.Qty, l.UnitPrice, l.Discount, l.TaxRate)));

        var lines = o.Lines.OrderBy(l => l.CreatedAt).Select(l => new
        {
            l.Id,
            l.ProductId,
            ProductCode = prodMap.TryGetValue(l.ProductId, out var p) ? p.Code : l.ProductId,
            ProductName = prodMap.TryGetValue(l.ProductId, out var p2) ? p2.Name : "",
            Spec = prodMap.TryGetValue(l.ProductId, out var p3) ? p3.Spec : null,
            Unit = prodMap.TryGetValue(l.ProductId, out var p4) ? p4.Unit : null,
            l.Qty,
            l.UnitPrice,
            l.Discount,
            l.TaxRate,
            Amount = Math.Round(l.Qty * l.UnitPrice * (1 - l.Discount), 2, MidpointRounding.AwayFromZero),
            Tax = Math.Round(l.Qty * l.UnitPrice * (1 - l.Discount) * l.TaxRate, 2, MidpointRounding.AwayFromZero),
            l.DeliveryDate
        }).ToList();

        var model = new
        {
            Header = new
            {
                o.Id,
                o.OrderNo,
                o.AccountId,
                AccountName = account?.Name,
                o.OrderDate,
                o.Currency,
                o.Status,
                o.Remark
            },
            Lines = lines,
            Totals = new
            {
                totals.TotalQty,
                totals.TotalAmount,
                totals.TotalTax,
                totals.TotalWithTax
            }
        };

        return model;
    }
}

