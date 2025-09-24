using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Invoices;

/// <summary>
/// 发票业务规则服务实现
/// </summary>
public class InvoiceBusinessRuleService : IInvoiceBusinessRuleService
{
    private readonly AppDbContext _db;
    private readonly ILogger<InvoiceBusinessRuleService> _logger;

    public InvoiceBusinessRuleService(AppDbContext db, ILogger<InvoiceBusinessRuleService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<InvoiceValidationResult> ValidateInvoiceAmountAsync(string orderId, decimal invoiceAmount, CancellationToken ct = default)
    {
        var result = new InvoiceValidationResult();

        try
        {
            // 获取订单信息
            var order = await _db.SalesOrders.AsNoTracking()
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);

            if (order == null)
            {
                result.AddError("订单不存在");
                return result;
            }

            // 计算订单总金额
            var orderTotalAmount = CalculateOrderTotal(order);

            // 获取已开票金额（领域发票模型未直接关联订单，此处先按0处理，后续对接领域发票）
            var invoicedAmount = 0m;

            // 检查是否超额开票
            var totalInvoiceAmount = invoicedAmount + invoiceAmount;
            if (totalInvoiceAmount > orderTotalAmount)
            {
                var exceedAmount = totalInvoiceAmount - orderTotalAmount;
                result.AddError($"发票金额超出订单金额 {exceedAmount:F2} 元（订单总额：{orderTotalAmount:F2}，已开票：{invoicedAmount:F2}，本次开票：{invoiceAmount:F2}）");
            }

            // 获取已发货金额
            var deliveredAmount = await GetDeliveredAmountAsync(orderId, ct);
            if (totalInvoiceAmount > deliveredAmount)
            {
                var exceedAmount = totalInvoiceAmount - deliveredAmount;
                result.AddWarning($"发票金额超出已发货金额 {exceedAmount:F2} 元（已发货：{deliveredAmount:F2}）");
            }

            // 添加统计信息
            result.Details["orderTotalAmount"] = orderTotalAmount;
            result.Details["invoicedAmount"] = invoicedAmount;
            result.Details["deliveredAmount"] = deliveredAmount;
            result.Details["requestedAmount"] = invoiceAmount;
            result.Details["remainingInvoicableAmount"] = Math.Max(0, orderTotalAmount - invoicedAmount);

            if (result.Errors.Count == 0)
            {
                result.IsValid = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating invoice amount for order {OrderId}", orderId);
            result.AddError($"验证发票金额时发生错误: {ex.Message}");
            return result;
        }
    }

    public async Task<InvoiceValidationResult> CanInvoiceDeliveryAsync(string deliveryId, decimal amount, CancellationToken ct = default)
    {
        var result = new InvoiceValidationResult();

        try
        {
            // 获取发货单信息
            var delivery = await _db.Deliveries.AsNoTracking()
                .Include(d => d.Lines)
                .FirstOrDefaultAsync(d => d.Id == deliveryId, ct);

            if (delivery == null)
            {
                result.AddError("发货单不存在");
                return result;
            }

            if (delivery.Status != "confirmed")
            {
                result.AddError("发货单未确认，无法开票");
                return result;
            }

            // 计算发货单总金额
            var deliveryTotalAmount = await CalculateDeliveryTotal(delivery);

            // 获取该发货单已开票金额
            // 获取该发货单已开票金额（暂按0处理，后续对接领域发票）
            var invoicedAmount = 0m;

            // 检查是否超额开票
            var totalInvoiceAmount = invoicedAmount + amount;
            if (totalInvoiceAmount > deliveryTotalAmount)
            {
                var exceedAmount = totalInvoiceAmount - deliveryTotalAmount;
                result.AddError($"发票金额超出发货金额 {exceedAmount:F2} 元（发货总额：{deliveryTotalAmount:F2}，已开票：{invoicedAmount:F2}，本次开票：{amount:F2}）");
            }

            result.Details["deliveryTotalAmount"] = deliveryTotalAmount;
            result.Details["invoicedAmount"] = invoicedAmount;
            result.Details["requestedAmount"] = amount;
            result.Details["remainingInvoicableAmount"] = Math.Max(0, deliveryTotalAmount - invoicedAmount);

            if (result.Errors.Count == 0)
            {
                result.IsValid = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating delivery invoice for delivery {DeliveryId}", deliveryId);
            result.AddError($"验证发货单开票时发生错误: {ex.Message}");
            return result;
        }
    }

    public async Task<InvoiceValidationResult> ValidateInvoiceItemsAsync(string orderId, List<InvoiceItemValidationInfo> items, CancellationToken ct = default)
    {
        var result = new InvoiceValidationResult();

        try
        {
            // 获取订单行信息
            var orderLines = await _db.SalesOrderLines.AsNoTracking()
                .Where(l => l.OrderId == orderId)
                .ToListAsync(ct);

            // 获取已发货数量
            var deliveredQuantities = await GetDeliveredQuantitiesAsync(orderId, ct);

            // 获取已开票数量
            var invoicedQuantities = await GetInvoicedQuantitiesAsync(orderId, ct);

            foreach (var item in items)
            {
                var orderLine = orderLines.FirstOrDefault(l => l.ProductId == item.ProductId);
                if (orderLine == null)
                {
                    result.AddError($"产品 {item.ProductId} 不在订单中");
                    continue;
                }

                var deliveredQty = deliveredQuantities.GetValueOrDefault(item.ProductId, 0);
                var invoicedQty = invoicedQuantities.GetValueOrDefault(item.ProductId, 0);

                // 检查数量是否超出订单数量
                var totalInvoiceQty = invoicedQty + item.RequestedQuantity;
                if (totalInvoiceQty > orderLine.Qty)
                {
                    var exceedQty = totalInvoiceQty - orderLine.Qty;
                    result.AddError($"产品 {item.ProductId} 开票数量超出订单数量 {exceedQty}（订单数量：{orderLine.Qty}，已开票：{invoicedQty}，本次开票：{item.RequestedQuantity}）");
                }

                // 检查数量是否超出已发货数量
                if (totalInvoiceQty > deliveredQty)
                {
                    var exceedQty = totalInvoiceQty - deliveredQty;
                    result.AddWarning($"产品 {item.ProductId} 开票数量超出已发货数量 {exceedQty}（已发货：{deliveredQty}）");
                }

                // 检查单价差异
                var expectedAmount = item.RequestedQuantity * orderLine.UnitPrice * (1 - orderLine.Discount) * (1 + orderLine.TaxRate);
                if (Math.Abs(item.RequestedAmount - expectedAmount) > 0.01m)
                {
                    result.AddWarning($"产品 {item.ProductId} 金额与订单单价不匹配（期望：{expectedAmount:F2}，实际：{item.RequestedAmount:F2}）");

                    result.AddDifference(new ReconciliationDifferenceInfo
                    {
                        Type = ReconciliationDifferenceType.AmountDifference,
                        ProductId = item.ProductId,
                        Description = $"产品 {item.ProductId} 发票金额与订单金额不匹配",
                        InvoiceQuantity = item.RequestedQuantity,
                        InvoiceAmount = item.RequestedAmount,
                        DeliveryAmount = expectedAmount,
                        DifferenceAmount = item.RequestedAmount - expectedAmount
                    });
                }
            }

            if (result.Errors.Count == 0)
            {
                result.IsValid = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating invoice items for order {OrderId}", orderId);
            result.AddError($"验证发票行项目时发生错误: {ex.Message}");
            return result;
        }
    }

    public async Task<ReconciliationResult> ReconcileInvoiceWithDeliveryAsync(string invoiceId, string deliveryId, CancellationToken ct = default)
    {
        var result = new ReconciliationResult();

        try
        {
            // 获取发票和发货单信息
            // 简化：暂不与领域发票行做逐项对账，仅返回匹配
            var delivery = await _db.Deliveries.AsNoTracking()
                .Include(d => d.Lines)
                .FirstOrDefaultAsync(d => d.Id == deliveryId, ct);

            if (delivery == null)
            {
                return result;
            }

            // 计算汇总信息（按发货单）
            result.Summary.TotalInvoiceAmount = 0m;
            result.Summary.TotalDeliveryAmount = await CalculateDeliveryTotal(delivery);
            result.Summary.TotalInvoiceQuantity = 0m;
            result.Summary.TotalDeliveryQuantity = delivery.Lines?.Sum(l => l.Qty) ?? 0m;
            result.Summary.TotalInvoiceItems = 0;
            result.Summary.TotalDeliveryItems = delivery.Lines?.Count ?? 0;
            result.Summary.InvoiceDate = null;

            // 暂不比较逐项差异，直接返回匹配
            result.IsMatched = true;

            // 计算总差异
            result.TotalDifferenceAmount = result.Differences.Sum(d => d.DifferenceAmount);
            result.TotalDifferenceQuantity = result.Differences.Sum(d => d.DifferenceQuantity);
            result.IsMatched = result.Differences.Count == 0;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reconciling invoice {InvoiceId} with delivery {DeliveryId}", invoiceId, deliveryId);
            return result;
        }
    }

    public async Task<ReconciliationResult> ReconcileInvoiceWithOrderAsync(string invoiceId, string orderId, CancellationToken ct = default)
    {
        // 类似于ReconcileInvoiceWithDeliveryAsync的实现，但对比的是订单
        // 为了简化，这里先返回基本实现
        return new ReconciliationResult { IsMatched = true };
    }

    public async Task<OrderInvoiceStatistics> GetOrderInvoiceStatisticsAsync(string orderId, CancellationToken ct = default)
    {
        var statistics = new OrderInvoiceStatistics { OrderId = orderId };

        try
        {
            // 获取订单信息
            var order = await _db.SalesOrders.AsNoTracking()
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);

            if (order != null)
            {
                statistics.OrderTotalAmount = CalculateOrderTotal(order);
            }

            // 获取已发货金额
            statistics.DeliveredAmount = await GetDeliveredAmountAsync(orderId, ct);

            // 获取发票信息
            // 领域发票模型暂未直接关联订单，统计值先按0处理（后续完善对接）
            statistics.TotalInvoices = 0;
            statistics.InvoicedAmount = 0;
            statistics.RemainingInvoicableAmount = Math.Max(0, statistics.OrderTotalAmount - statistics.InvoicedAmount);
            statistics.InvoiceablePercentage = statistics.OrderTotalAmount > 0
                ? (statistics.InvoicedAmount / statistics.OrderTotalAmount) * 100
                : 0;
            statistics.Invoices = new List<OrderInvoiceInfo>();

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice statistics for order {OrderId}", orderId);
            return statistics;
        }
    }

    public async Task<InvoiceValidationResult> CanCancelInvoiceAsync(string invoiceId, CancellationToken ct = default)
    {
        var result = new InvoiceValidationResult();

        try
        {
            // 简化：暂不根据领域发票状态/收款做校验，直接允许（后续完善）
            result.IsValid = true;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if invoice {InvoiceId} can be cancelled", invoiceId);
            result.AddError($"检查发票作废条件时发生错误: {ex.Message}");
            return result;
        }
    }

    public async Task<List<AdjustmentVoucherSuggestion>> GenerateAdjustmentSuggestionsAsync(string invoiceId, CancellationToken ct = default)
    {
        var suggestions = new List<AdjustmentVoucherSuggestion>();

        try
        {
            // 这里可以根据发票的具体情况生成调整凭证建议
            // 为了简化，这里提供基本的实现框架

            // 生成红字凭证建议（简化）
            var invGuid = Guid.Empty;
            decimal amount = 0m;
            if (Guid.TryParse(invoiceId, out invGuid))
            {
                var inv = await _db.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == invGuid, ct);
                if (inv != null)
                {
                    amount = inv.GrandTotal;
                }
            }

            // 红字凭证建议（发票作废）
            suggestions.Add(new AdjustmentVoucherSuggestion
            {
                Type = "红字凭证",
                Reason = "发票作废",
                Amount = amount,
                AccountingEntry = "借：应收账款（红字） 贷：主营业务收入（红字） 应交税费-应交增值税（红字）"
            });

            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating adjustment suggestions for invoice {InvoiceId}", invoiceId);
            return suggestions;
        }
    }

    public async Task<bool> HandleReconciliationDifferenceAsync(string differenceId, ReconciliationDifferenceResolution resolution, string userId, CancellationToken ct = default)
    {
        try
        {
            var difference = await _db.ReconciliationDifferences
                .FirstOrDefaultAsync(d => d.Id == differenceId, ct);

            if (difference == null) return false;

            difference.Status = resolution.Status;
            difference.Resolution = resolution.Resolution;
            difference.HandledBy = userId;
            difference.HandledAt = DateTime.UtcNow;
            difference.HandledRemark = resolution.Remark;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Reconciliation difference {DifferenceId} handled by {UserId} with status {Status}",
                differenceId, userId, resolution.Status);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling reconciliation difference {DifferenceId}", differenceId);
            return false;
        }
    }

    #region Private Helper Methods

    private static decimal CalculateOrderTotal(SalesOrder order)
    {
        return order.Lines?.Sum(l => l.Qty * l.UnitPrice * (1 - l.Discount) * (1 + l.TaxRate)) ?? 0;
    }

    private async Task<decimal> CalculateDeliveryTotal(Delivery delivery)
    {
        decimal total = 0m;
        if (delivery.Lines == null) return 0m;
        foreach (var l in delivery.Lines)
        {
            total += await CalculateDeliveryLineAmountAsync(l, CancellationToken.None);
        }
        return total;
    }

    private async Task<decimal> CalculateDeliveryLineAmountAsync(DeliveryLine line, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(line.OrderLineId))
        {
            var ol = await _db.SalesOrderLines.AsNoTracking().FirstOrDefaultAsync(x => x.Id == line.OrderLineId, ct);
            if (ol != null)
            {
                return line.Qty * ol.UnitPrice * (1 - ol.Discount) * (1 + ol.TaxRate);
            }
        }
        return 0m;
    }

    private async Task<decimal> GetDeliveredAmountAsync(string orderId, CancellationToken ct)
    {
        // 通过关联的订单行价格计算已送货金额
        var total = await _db.Deliveries.AsNoTracking()
            .Where(d => d.OrderId == orderId && d.Status == "confirmed")
            .SelectMany(d => d.Lines!)
            .Where(l => l.OrderLineId != null)
            .Join(_db.SalesOrderLines.AsNoTracking(),
                  l => l.OrderLineId!,
                  ol => ol.Id,
                  (l, ol) => new { l.Qty, ol.UnitPrice, ol.Discount, ol.TaxRate })
            .SumAsync(x => x.Qty * x.UnitPrice * (1 - x.Discount) * (1 + x.TaxRate), ct);

        return total;
    }

    private async Task<Dictionary<string, decimal>> GetDeliveredQuantitiesAsync(string orderId, CancellationToken ct)
    {
        var result = await _db.Deliveries.AsNoTracking()
            .Where(d => d.OrderId == orderId && d.Status == "confirmed")
            .Include(d => d.Lines)
            .SelectMany(d => d.Lines!)
            .GroupBy(l => l.ProductId)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(l => l.Qty), ct);

        return result;
    }

    private async Task<Dictionary<string, decimal>> GetInvoicedQuantitiesAsync(string orderId, CancellationToken ct)
    {
        // 领域发票模型未直接关联订单，暂返回空映射
        return new Dictionary<string, decimal>();
    }

    #endregion
}
