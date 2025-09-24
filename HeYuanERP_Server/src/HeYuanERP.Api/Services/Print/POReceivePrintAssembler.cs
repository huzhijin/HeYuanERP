using HeYuanERP.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Print;

// 采购收货打印数据组装器：聚合收货头、行、对应采购与供应商信息
public class POReceivePrintAssembler
{
    private readonly AppDbContext _db;

    public POReceivePrintAssembler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> BuildAsync(string receiveId, CancellationToken ct)
    {
        var r = await _db.POReceives
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == receiveId, ct)
            ?? throw new KeyNotFoundException("未找到收货单");

        var po = await _db.PurchaseOrders.AsNoTracking().FirstOrDefaultAsync(p => p.Id == r.PoId, ct);
        var vendor = po == null ? null : await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == po.VendorId, ct);
        var prodIds = r.Lines.Select(l => l.ProductId).Distinct().ToList();
        var prodMap = await _db.Products.AsNoTracking().Where(p => prodIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name, p.Spec, p.Unit }, ct);

        var lines = r.Lines.OrderBy(l => l.CreatedAt).Select(l => new
        {
            l.Id,
            l.ProductId,
            ProductCode = prodMap.TryGetValue(l.ProductId, out var p) ? p.Code : l.ProductId,
            ProductName = prodMap.TryGetValue(l.ProductId, out var p2) ? p2.Name : "",
            Spec = prodMap.TryGetValue(l.ProductId, out var p3) ? p3.Spec : null,
            Unit = prodMap.TryGetValue(l.ProductId, out var p4) ? p4.Unit : null,
            l.Qty,
            l.Whse,
            l.Loc
        }).ToList();

        var model = new
        {
            Header = new
            {
                r.Id,
                r.PoId,
                PoNo = po?.PoNo,
                VendorId = po?.VendorId,
                VendorName = vendor?.Name,
                r.ReceiveDate,
                r.Status,
                r.Remark
            },
            Lines = lines
        };

        return model;
    }
}

