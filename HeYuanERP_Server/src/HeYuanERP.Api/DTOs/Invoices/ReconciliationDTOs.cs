using HeYuanERP.Api.Services.Invoices;
using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.DTOs.Invoices;

/// <summary>
/// 发票金额验证请求
/// </summary>
public class ValidateInvoiceAmountDto
{
    public string OrderId { get; set; } = string.Empty;
    public decimal InvoiceAmount { get; set; }
}

/// <summary>
/// 发货单开票验证请求
/// </summary>
public class ValidateDeliveryInvoiceDto
{
    public string DeliveryId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>
/// 发票行项目验证请求
/// </summary>
public class ValidateInvoiceItemsDto
{
    public string OrderId { get; set; } = string.Empty;
    public List<InvoiceItemValidationDto> Items { get; set; } = new();
}

/// <summary>
/// 发票行项目验证信息
/// </summary>
public class InvoiceItemValidationDto
{
    public string ProductId { get; set; } = string.Empty;
    public decimal RequestedQuantity { get; set; }
    public decimal RequestedAmount { get; set; }
}

/// <summary>
/// 对账请求
/// </summary>
public class ReconcileInvoiceDto
{
    public string InvoiceId { get; set; } = string.Empty;
    public string? DeliveryId { get; set; }
    public string? OrderId { get; set; }
}

/// <summary>
/// 对账差异处理请求
/// </summary>
public class HandleReconciliationDifferenceDto
{
    public ReconciliationStatus Status { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
    public Dictionary<string, object> AdjustmentData { get; set; } = new();
}

/// <summary>
/// 发票验证结果响应
/// </summary>
public class InvoiceValidationResponseDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<ReconciliationDifferenceDto> Differences { get; set; } = new();
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// 对账差异信息响应
/// </summary>
public class ReconciliationDifferenceDto
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
/// 对账结果响应
/// </summary>
public class ReconciliationResultDto
{
    public bool IsMatched { get; set; }
    public List<ReconciliationDifferenceDto> Differences { get; set; } = new();
    public decimal TotalDifferenceAmount { get; set; }
    public decimal TotalDifferenceQuantity { get; set; }
    public ReconciliationSummaryDto Summary { get; set; } = new();
}

/// <summary>
/// 对账汇总信息
/// </summary>
public class ReconciliationSummaryDto
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
/// 订单发票统计信息响应
/// </summary>
public class OrderInvoiceStatisticsDto
{
    public string OrderId { get; set; } = string.Empty;
    public decimal OrderTotalAmount { get; set; }
    public decimal DeliveredAmount { get; set; }
    public decimal InvoicedAmount { get; set; }
    public decimal RemainingInvoicableAmount { get; set; }
    public decimal InvoiceablePercentage { get; set; }
    public int TotalInvoices { get; set; }
    public List<OrderInvoiceInfoDto> Invoices { get; set; } = new();
}

/// <summary>
/// 订单发票信息
/// </summary>
public class OrderInvoiceInfoDto
{
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
}

/// <summary>
/// 调整凭证建议响应
/// </summary>
public class AdjustmentVoucherSuggestionDto
{
    public string Type { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string AccountingEntry { get; set; } = string.Empty;
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// 扩展方法：转换业务对象到DTO
/// </summary>
public static class ReconciliationExtensions
{
    public static InvoiceValidationResponseDto ToDto(this InvoiceValidationResult result)
    {
        return new InvoiceValidationResponseDto
        {
            IsValid = result.IsValid,
            Errors = result.Errors,
            Warnings = result.Warnings,
            Differences = result.Differences.Select(d => d.ToDto()).ToList(),
            Details = result.Details
        };
    }

    public static ReconciliationDifferenceDto ToDto(this ReconciliationDifferenceInfo difference)
    {
        return new ReconciliationDifferenceDto
        {
            Type = difference.Type,
            Description = difference.Description,
            ProductId = difference.ProductId,
            DeliveryQuantity = difference.DeliveryQuantity,
            InvoiceQuantity = difference.InvoiceQuantity,
            DifferenceQuantity = difference.DifferenceQuantity,
            DeliveryAmount = difference.DeliveryAmount,
            InvoiceAmount = difference.InvoiceAmount,
            DifferenceAmount = difference.DifferenceAmount,
            ExtensionData = difference.ExtensionData
        };
    }

    public static ReconciliationResultDto ToDto(this ReconciliationResult result)
    {
        return new ReconciliationResultDto
        {
            IsMatched = result.IsMatched,
            Differences = result.Differences.Select(d => d.ToDto()).ToList(),
            TotalDifferenceAmount = result.TotalDifferenceAmount,
            TotalDifferenceQuantity = result.TotalDifferenceQuantity,
            Summary = result.Summary.ToDto()
        };
    }

    public static ReconciliationSummaryDto ToDto(this ReconciliationSummary summary)
    {
        return new ReconciliationSummaryDto
        {
            TotalDeliveryAmount = summary.TotalDeliveryAmount,
            TotalInvoiceAmount = summary.TotalInvoiceAmount,
            TotalDeliveryQuantity = summary.TotalDeliveryQuantity,
            TotalInvoiceQuantity = summary.TotalInvoiceQuantity,
            TotalDeliveryItems = summary.TotalDeliveryItems,
            TotalInvoiceItems = summary.TotalInvoiceItems,
            EarliestDeliveryDate = summary.EarliestDeliveryDate,
            LatestDeliveryDate = summary.LatestDeliveryDate,
            InvoiceDate = summary.InvoiceDate
        };
    }

    public static OrderInvoiceStatisticsDto ToDto(this OrderInvoiceStatistics statistics)
    {
        return new OrderInvoiceStatisticsDto
        {
            OrderId = statistics.OrderId,
            OrderTotalAmount = statistics.OrderTotalAmount,
            DeliveredAmount = statistics.DeliveredAmount,
            InvoicedAmount = statistics.InvoicedAmount,
            RemainingInvoicableAmount = statistics.RemainingInvoicableAmount,
            InvoiceablePercentage = statistics.InvoiceablePercentage,
            TotalInvoices = statistics.TotalInvoices,
            Invoices = statistics.Invoices.Select(i => new OrderInvoiceInfoDto
            {
                InvoiceId = i.InvoiceId,
                InvoiceNo = i.InvoiceNo,
                Amount = i.Amount,
                Status = i.Status,
                InvoiceDate = i.InvoiceDate
            }).ToList()
        };
    }

    public static AdjustmentVoucherSuggestionDto ToDto(this AdjustmentVoucherSuggestion suggestion)
    {
        return new AdjustmentVoucherSuggestionDto
        {
            Type = suggestion.Type,
            Reason = suggestion.Reason,
            Amount = suggestion.Amount,
            AccountingEntry = suggestion.AccountingEntry,
            Details = suggestion.Details
        };
    }

    public static InvoiceItemValidationInfo ToBusinessObject(this InvoiceItemValidationDto dto)
    {
        return new InvoiceItemValidationInfo
        {
            ProductId = dto.ProductId,
            RequestedQuantity = dto.RequestedQuantity,
            RequestedAmount = dto.RequestedAmount
        };
    }

    public static ReconciliationDifferenceResolution ToBusinessObject(this HandleReconciliationDifferenceDto dto)
    {
        return new ReconciliationDifferenceResolution
        {
            Status = dto.Status,
            Resolution = dto.Resolution,
            Remark = dto.Remark,
            AdjustmentData = dto.AdjustmentData
        };
    }
}