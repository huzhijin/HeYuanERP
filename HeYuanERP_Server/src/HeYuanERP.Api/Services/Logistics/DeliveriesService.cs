using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Logistics;

// 送货单服务：
// - 校验订单状态（须已确认）；
// - 不超发：订单行维度与产品维度的累计校验；
// - 创建送货单与明细；
// - 更新客户最近业务时间。
public class DeliveriesService : IDeliveriesService
{
    private readonly AppDbContext _db;

    public DeliveriesService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DeliveryDetailDto?> GetAsync(string id, CancellationToken ct)
    {
        var d = await _db.Deliveries.AsNoTracking().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (d == null) return null;
        return MapDetail(d);
    }

    public async Task<DeliveryDetailDto> CreateAsync(DeliveryCreateDto req, string currentUserId, CancellationToken ct)
    {
        // 订单存在且已确认
        var order = await _db.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == req.OrderId, ct)
            ?? throw new KeyNotFoundException("未找到订单");
        if (order.Status != "confirmed")
            throw new ApplicationException("仅已确认订单可创建送货单");
        if (req.Lines.Count == 0) throw new ApplicationException("至少需要一条送货明细");

        // 不超发校验（先按行，再按产品汇总）
        // 1) 行维度：若提供 orderLineId，则累计发货不得超过该行 qty
        var dlvByLine = await _db.DeliveryLines
            .Where(l => l.Delivery!.OrderId == req.OrderId && l.OrderLineId != null)
            .GroupBy(l => l.OrderLineId!)
            .Select(g => new { OrderLineId = g.Key, Qty = g.Sum(x => x.Qty) })
            .ToDictionaryAsync(x => x.OrderLineId, x => x.Qty, ct);

        foreach (var l in req.Lines)
        {
            if (!string.IsNullOrWhiteSpace(l.OrderLineId))
            {
                var ol = order.Lines.FirstOrDefault(x => x.Id == l.OrderLineId) ?? throw new ApplicationException("订单行不存在");
                var sent = dlvByLine.TryGetValue(ol.Id, out var s) ? s : 0m;
                if (sent + l.Qty > ol.Qty)
                    throw new ApplicationException($"订单行超发：产品 {ol.ProductId} 已发 {sent}，本次 {l.Qty}，订单数量 {ol.Qty}");
            }
        }

        // 2) 产品维度：若未指定订单行，则按产品汇总校验（累计发货 <= 订单同产品之和）
        var orderedByProd = order.Lines.GroupBy(x => x.ProductId).ToDictionary(g => g.Key, g => g.Sum(x => x.Qty));
        var sentByProd = await _db.DeliveryLines
            .Where(l => l.Delivery!.OrderId == req.OrderId)
            .GroupBy(l => l.ProductId)
            .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Qty) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Qty, ct);

        var incomingProd = req.Lines.GroupBy(x => x.ProductId).ToDictionary(g => g.Key, g => g.Sum(x => x.Qty));
        foreach (var (pid, incQty) in incomingProd)
        {
            if (!orderedByProd.TryGetValue(pid, out var ordQty))
                throw new ApplicationException($"产品不在订单中：{pid}");
            var sent = sentByProd.TryGetValue(pid, out var s) ? s : 0m;
            if (sent + incQty > ordQty)
                throw new ApplicationException($"产品超发：{pid} 已发 {sent}，本次 {incQty}，订单数量 {ordQty}");
        }

        // 创建送货单
        var dly = new Delivery
        {
            DeliveryNo = await NextDeliveryNoAsync(ct),
            OrderId = order.Id,
            DeliveryDate = req.DeliveryDate.Date,
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId,
        };
        foreach (var l in req.Lines)
        {
            dly.Lines.Add(new DeliveryLine
            {
                ProductId = l.ProductId.Trim(),
                OrderLineId = string.IsNullOrWhiteSpace(l.OrderLineId) ? null : l.OrderLineId!.Trim(),
                Qty = l.Qty,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            });
        }
        _db.Deliveries.Add(dly);
        await _db.SaveChangesAsync(ct);

        // 更新客户最近业务日期
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == order.AccountId, ct);
        if (acc != null)
        {
            acc.LastEventDate = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return MapDetail(dly);
    }

    private static DeliveryDetailDto MapDetail(Delivery d)
    {
        return new DeliveryDetailDto
        {
            Id = d.Id,
            DeliveryNo = d.DeliveryNo,
            OrderId = d.OrderId,
            DeliveryDate = d.DeliveryDate,
            Status = d.Status,
            Lines = d.Lines.OrderBy(l => l.CreatedAt).Select(l => new DeliveryLineDto
            {
                Id = l.Id,
                ProductId = l.ProductId,
                OrderLineId = l.OrderLineId,
                Qty = l.Qty
            }).ToList()
        };
    }

    private async Task<string> NextDeliveryNoAsync(CancellationToken ct)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        for (int i = 0; i < 5; i++)
        {
            var rnd = Random.Shared.Next(0, 9999).ToString("D4");
            var no = $"DL{date}-{rnd}";
            var exists = await _db.Deliveries.AsNoTracking().AnyAsync(o => o.DeliveryNo == no, ct);
            if (!exists) return no;
        }
        return $"DL{date}-{DateTime.UtcNow:HHmmssfff}";
    }
}

