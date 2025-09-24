namespace HeYuanERP.Api.Services.Orders;

// 价格计算服务：根据行明细计算总数量/金额/税额/含税金额
public class PricingService
{
    public record Totals(decimal TotalQty, decimal TotalAmount, decimal TotalTax, decimal TotalWithTax);

    /// <summary>
    /// 计算订单汇总（按行：数量、单价、折扣、税率）。
    /// 折扣/税率均为 0..1 的小数，例如 0.1=10% 折扣，0.13=13% 税率。
    /// </summary>
    public Totals Compute(IEnumerable<(decimal qty, decimal unitPrice, decimal discount, decimal taxRate)> lines)
    {
        decimal totalQty = 0m;
        decimal totalAmount = 0m;
        decimal totalTax = 0m;
        foreach (var (qty, unitPrice, discount, taxRate) in lines)
        {
            var price = unitPrice * (1 - discount);
            var amount = qty * price;
            var tax = amount * taxRate;
            totalQty += qty;
            totalAmount += Decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
            totalTax += Decimal.Round(tax, 2, MidpointRounding.AwayFromZero);
        }
        var totalWithTax = totalAmount + totalTax;
        return new Totals(
            Decimal.Round(totalQty, 4, MidpointRounding.AwayFromZero),
            Decimal.Round(totalAmount, 2, MidpointRounding.AwayFromZero),
            Decimal.Round(totalTax, 2, MidpointRounding.AwayFromZero),
            Decimal.Round(totalWithTax, 2, MidpointRounding.AwayFromZero)
        );
    }
}

