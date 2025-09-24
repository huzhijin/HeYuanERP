using System.Collections.Generic;
using System.Linq;

namespace HeYuanERP.Domain.Printing;

/// <summary>
/// 单据类型常量与工具方法。
/// 说明：为避免跨层耦合，保持为字符串常量，统一小写。
/// </summary>
public static class DocTypes
{
    /// <summary>订单</summary>
    public const string Order = "order";

    /// <summary>送货单</summary>
    public const string Delivery = "delivery";

    /// <summary>退货单</summary>
    public const string Return = "return";

    /// <summary>发票</summary>
    public const string Invoice = "invoice";

    /// <summary>对账单</summary>
    public const string Statement = "statement";

    private static readonly string[] AllValues =
    {
        Order, Delivery, Return, Invoice, Statement
    };

    /// <summary>
    /// 支持的全部单据类型（只读）。
    /// </summary>
    public static IReadOnlyCollection<string> All => AllValues;

    /// <summary>
    /// 判断给定值是否为受支持的单据类型（大小写不敏感）。
    /// </summary>
    public static bool IsKnown(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var v = value.Trim().ToLowerInvariant();
        return AllValues.Contains(v);
    }
}

