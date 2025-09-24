using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Logistics;

// 退货单服务：
// - 校验订单存在；
// - 不超退：按产品累计“已发-已退”作为可退上限；
// - 可选限制来源送货单（若提供 SourceDeliveryId 则校验该送货单存在且属于订单）。
public class ReturnsService : IReturnsService
{
    private readonly AppDbContext _db;

    public ReturnsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ReturnDetailDto?> GetAsync(string id, CancellationToken ct)
    {
        var r = await _db.Returns.AsNoTracking().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (r == null) return null;
        return MapDetail(r);
    }

    public async Task<ReturnDetailDto> CreateAsync(ReturnCreateDto req, string currentUserId, CancellationToken ct)
    {
        var order = await _db.SalesOrders.FirstOrDefaultAsync(o => o.Id == req.OrderId, ct)
            ?? throw new KeyNotFoundException("未找到订单");

        // 验证来源送货单（可选）
        if (!string.IsNullOrWhiteSpace(req.SourceDeliveryId))
        {
            var exists = await _db.Deliveries.AsNoTracking().AnyAsync(d => d.Id == req.SourceDeliveryId && d.OrderId == req.OrderId, ct);
            if (!exists) throw new ApplicationException("来源送货单不存在或不属于该订单");
        }

        if (req.Lines.Count == 0) throw new ApplicationException("至少需要一条退货明细");

        // 不超退校验：
        // 累计已发（按订单）与已退，计算各产品可退余量
        var delivered = await _db.DeliveryLines
            .Where(l => l.Delivery!.OrderId == req.OrderId)
            .GroupBy(l => l.ProductId)
            .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Qty) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Qty, ct);
        var returned = await _db.ReturnLines
            .Where(l => l.Return!.OrderId == req.OrderId)
            .GroupBy(l => l.ProductId)
            .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Qty) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Qty, ct);

        var incByProd = req.Lines.GroupBy(x => x.ProductId).ToDictionary(g => g.Key, g => g.Sum(x => x.Qty));
        foreach (var (pid, inc) in incByProd)
        {
            var sent = delivered.TryGetValue(pid, out var s) ? s : 0m;
            var ret = returned.TryGetValue(pid, out var r) ? r : 0m;
            var can = sent - ret;
            if (inc > can)
                throw new ApplicationException($"产品退货超限：{pid} 已发 {sent}，已退 {ret}，本次 {inc}");
        }

        var retDoc = new Return
        {
            ReturnNo = await NextReturnNoAsync(ct),
            OrderId = order.Id,
            SourceDeliveryId = string.IsNullOrWhiteSpace(req.SourceDeliveryId) ? null : req.SourceDeliveryId!.Trim(),
            ReturnDate = req.ReturnDate.Date,
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId,
        };
        foreach (var l in req.Lines)
        {
            retDoc.Lines.Add(new ReturnLine
            {
                ProductId = l.ProductId.Trim(),
                Qty = l.Qty,
                Reason = string.IsNullOrWhiteSpace(l.Reason) ? null : l.Reason!.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            });
        }
        _db.Returns.Add(retDoc);
        await _db.SaveChangesAsync(ct);

        // 更新客户最近业务日期
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == order.AccountId, ct);
        if (acc != null)
        {
            acc.LastEventDate = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return MapDetail(retDoc);
    }

    private static ReturnDetailDto MapDetail(Return r)
    {
        return new ReturnDetailDto
        {
            Id = r.Id,
            ReturnNo = r.ReturnNo,
            OrderId = r.OrderId,
            SourceDeliveryId = r.SourceDeliveryId,
            ReturnDate = r.ReturnDate,
            Status = r.Status,
            Lines = r.Lines.OrderBy(l => l.CreatedAt).Select(l => new ReturnLineDto
            {
                Id = l.Id,
                ProductId = l.ProductId,
                Qty = l.Qty,
                Reason = l.Reason
            }).ToList()
        };
    }

    private async Task<string> NextReturnNoAsync(CancellationToken ct)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        for (int i = 0; i < 5; i++)
        {
            var rnd = Random.Shared.Next(0, 9999).ToString("D4");
            var no = $"RT{date}-{rnd}";
            var exists = await _db.Returns.AsNoTracking().AnyAsync(o => o.ReturnNo == no, ct);
            if (!exists) return no;
        }
        return $"RT{date}-{DateTime.UtcNow:HHmmssfff}";
    }
}

