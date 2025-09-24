using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.Services.Invoices;

/// <summary>
/// 发票验证结果
/// </summary>
public class InvoiceValidationResult
{
    /// <summary>
    /// 是否通过验证
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 错误消息列表
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// 警告消息列表
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// 检测到的差异列表
    /// </summary>
    public List<ReconciliationDifferenceInfo> Differences { get; set; } = new();

    /// <summary>
    /// 验证详情
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static InvoiceValidationResult Success()
    {
        return new InvoiceValidationResult { IsValid = true };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static InvoiceValidationResult Failure(params string[] errors)
    {
        return new InvoiceValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    /// <summary>
    /// 添加错误
    /// </summary>
    public void AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
    }

    /// <summary>
    /// 添加警告
    /// </summary>
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }

    /// <summary>
    /// 添加差异
    /// </summary>
    public void AddDifference(ReconciliationDifferenceInfo difference)
    {
        Differences.Add(difference);
    }
}

/// <summary>
/// 对账差异信息
/// </summary>
public class ReconciliationDifferenceInfo
{
    public ReconciliationDifferenceType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public decimal DeliveryQuantity { get; set; }
    public decimal InvoiceQuantity { get; set; }
    public decimal DifferenceQuantity { get; set; }
    public decimal DeliveryAmount { get; set; }
    public decimal InvoiceAmount { get; set; }
    public decimal DifferenceAmount { get; set; }
    public Dictionary<string, object> ExtensionData { get; set; } = new();
}

/// <summary>
/// 对账结果
/// </summary>
public class ReconciliationResult
{
    /// <summary>
    /// 是否匹配
    /// </summary>
    public bool IsMatched { get; set; }

    /// <summary>
    /// 差异列表
    /// </summary>
    public List<ReconciliationDifferenceInfo> Differences { get; set; } = new();

    /// <summary>
    /// 总差异金额
    /// </summary>
    public decimal TotalDifferenceAmount { get; set; }

    /// <summary>
    /// 总差异数量
    /// </summary>
    public decimal TotalDifferenceQuantity { get; set; }

    /// <summary>
    /// 对账汇总信息
    /// </summary>
    public ReconciliationSummary Summary { get; set; } = new();
}

/// <summary>
/// 对账汇总信息
/// </summary>
public class ReconciliationSummary
{
    public decimal TotalDeliveryAmount { get; set; }
    public decimal TotalInvoiceAmount { get; set; }
    public decimal TotalDeliveryQuantity { get; set; }
    public decimal TotalInvoiceQuantity { get; set; }
    public int TotalDeliveryItems { get; set; }
    public int TotalInvoiceItems { get; set; }
    public DateTime? EarliestDeliveryDate { get; set; }
    public DateTime? LatestDeliveryDate { get; set; }
    public DateTime? InvoiceDate { get; set; }
}

/// <summary>
/// 发票行项目验证信息
/// </summary>
public class InvoiceItemValidationInfo
{
    public string ProductId { get; set; } = string.Empty;
    public decimal RequestedQuantity { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal AvailableAmount { get; set; }
    public decimal AlreadyInvoicedQuantity { get; set; }
    public decimal AlreadyInvoicedAmount { get; set; }
}

/// <summary>
/// 发票业务规则服务接口
/// </summary>
public interface IInvoiceBusinessRuleService
{
    /// <summary>
    /// 验证发票总金额是否合规（不可超过订单或发货金额）
    /// </summary>
    Task<InvoiceValidationResult> ValidateInvoiceAmountAsync(string orderId, decimal invoiceAmount, CancellationToken ct = default);

    /// <summary>
    /// 验证是否可以对指定发货单开票
    /// </summary>
    Task<InvoiceValidationResult> CanInvoiceDeliveryAsync(string deliveryId, decimal amount, CancellationToken ct = default);

    /// <summary>
    /// 验证发票行项目（数量、金额、产品匹配）
    /// </summary>
    Task<InvoiceValidationResult> ValidateInvoiceItemsAsync(string orderId, List<InvoiceItemValidationInfo> items, CancellationToken ct = default);

    /// <summary>
    /// 执行发票与发货的对账
    /// </summary>
    Task<ReconciliationResult> ReconcileInvoiceWithDeliveryAsync(string invoiceId, string deliveryId, CancellationToken ct = default);

    /// <summary>
    /// 执行发票与订单的对账
    /// </summary>
    Task<ReconciliationResult> ReconcileInvoiceWithOrderAsync(string invoiceId, string orderId, CancellationToken ct = default);

    /// <summary>
    /// 获取订单的发票统计信息
    /// </summary>
    Task<OrderInvoiceStatistics> GetOrderInvoiceStatisticsAsync(string orderId, CancellationToken ct = default);

    /// <summary>
    /// 检查发票是否可以作废（业务规则）
    /// </summary>
    Task<InvoiceValidationResult> CanCancelInvoiceAsync(string invoiceId, CancellationToken ct = default);

    /// <summary>
    /// 生成红蓝字调整凭证建议
    /// </summary>
    Task<List<AdjustmentVoucherSuggestion>> GenerateAdjustmentSuggestionsAsync(string invoiceId, CancellationToken ct = default);

    /// <summary>
    /// 处理对账差异
    /// </summary>
    Task<bool> HandleReconciliationDifferenceAsync(string differenceId, ReconciliationDifferenceResolution resolution, string userId, CancellationToken ct = default);
}

/// <summary>
/// 订单发票统计信息
/// </summary>
public class OrderInvoiceStatistics
{
    public string OrderId { get; set; } = string.Empty;
    public decimal OrderTotalAmount { get; set; }
    public decimal DeliveredAmount { get; set; }
    public decimal InvoicedAmount { get; set; }
    public decimal RemainingInvoicableAmount { get; set; }
    public decimal InvoiceablePercentage { get; set; }
    public int TotalInvoices { get; set; }
    public List<OrderInvoiceInfo> Invoices { get; set; } = new();
}

/// <summary>
/// 订单发票信息
/// </summary>
public class OrderInvoiceInfo
{
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
}

/// <summary>
/// 调整凭证建议
/// </summary>
public class AdjustmentVoucherSuggestion
{
    public string Type { get; set; } = string.Empty; // 红字/蓝字
    public string Reason { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string AccountingEntry { get; set; } = string.Empty;
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// 对账差异处理方案
/// </summary>
public class ReconciliationDifferenceResolution
{
    public ReconciliationStatus Status { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
    public Dictionary<string, object> AdjustmentData { get; set; } = new();
}