using HeYuanERP.Api.Services.Purchase;
using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.DTOs.Purchase;

/// <summary>
/// 创建采购异常请求DTO
/// </summary>
public class CreatePurchaseExceptionDto
{
    public string? PurchaseOrderId { get; set; }
    public string? ReceiptId { get; set; }
    public string? ProductId { get; set; }
    public string? SupplierId { get; set; }
    public PurchaseExceptionType Type { get; set; }
    public ExceptionLevel Level { get; set; } = ExceptionLevel.Medium;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ExpectedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal ExpectedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public QualityIssueDetailsDto? QualityDetails { get; set; }
    public Dictionary<string, object> ExtensionData { get; set; } = new();
}

/// <summary>
/// 质量问题详情DTO
/// </summary>
public class QualityIssueDetailsDto
{
    public QualityIssueType IssueType { get; set; }
    public decimal DefectiveQuantity { get; set; }
    public string DefectDescription { get; set; } = string.Empty;
    public string InspectionStandard { get; set; } = string.Empty;
    public string? Inspector { get; set; }
    public DateTime? InspectionDate { get; set; }
    public List<string> AttachmentUrls { get; set; } = new();
    public string? ProcessingSuggestion { get; set; }
}

/// <summary>
/// 处理采购异常请求DTO
/// </summary>
public class HandlePurchaseExceptionDto
{
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PurchaseExceptionStatus NewStatus { get; set; }
    public string? Resolution { get; set; }
    public string? ImpactAssessment { get; set; }
    public string? PreventiveMeasures { get; set; }
    public string? RootCauseAnalysis { get; set; }
    public List<string> AttachmentUrls { get; set; } = new();
    public Dictionary<string, object> AdjustmentData { get; set; } = new();
}

/// <summary>
/// 采购异常响应DTO
/// </summary>
public class PurchaseExceptionDto
{
    public string Id { get; set; } = string.Empty;
    public string ExceptionNo { get; set; } = string.Empty;
    public string? PurchaseOrderId { get; set; }
    public string? ReceiptId { get; set; }
    public string? ProductId { get; set; }
    public string? SupplierId { get; set; }
    public PurchaseExceptionType Type { get; set; }
    public ExceptionLevel Level { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ExpectedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal DifferenceQuantity { get; set; }
    public decimal ExpectedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal DifferenceAmount { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public int DelayDays { get; set; }
    public PurchaseExceptionStatus Status { get; set; }
    public string? Resolution { get; set; }
    public string? HandledBy { get; set; }
    public DateTime? HandledAt { get; set; }
    public string? HandledRemark { get; set; }
    public string? ImpactAssessment { get; set; }
    public string? PreventiveMeasures { get; set; }
    public string? RootCauseAnalysis { get; set; }
    public QualityIssueDetailsDto? QualityDetails { get; set; }
    public Dictionary<string, object> ExtensionData { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 关联信息
    public string? SupplierName { get; set; }
    public string? ProductName { get; set; }
    public string? PurchaseOrderNo { get; set; }
}

/// <summary>
/// 异常处理记录DTO
/// </summary>
public class PurchaseExceptionHandlingRecordDto
{
    public string Id { get; set; } = string.Empty;
    public string ExceptionId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HandledBy { get; set; } = string.Empty;
    public DateTime HandledAt { get; set; }
    public PurchaseExceptionStatus? StatusFrom { get; set; }
    public PurchaseExceptionStatus? StatusTo { get; set; }
    public List<string> AttachmentUrls { get; set; } = new();
}

/// <summary>
/// 采购异常统计DTO
/// </summary>
public class PurchaseExceptionStatisticsDto
{
    public int TotalExceptions { get; set; }
    public int OpenExceptions { get; set; }
    public int InProgressExceptions { get; set; }
    public int ResolvedExceptions { get; set; }
    public int ClosedExceptions { get; set; }
    public Dictionary<string, int> ExceptionsByType { get; set; } = new();
    public Dictionary<string, int> ExceptionsByLevel { get; set; } = new();
    public Dictionary<string, int> ExceptionsBySupplier { get; set; } = new();
    public decimal AverageResolutionDays { get; set; }
    public decimal TotalImpactAmount { get; set; }
    public DateTime? EarliestExceptionDate { get; set; }
    public DateTime? LatestExceptionDate { get; set; }
}

/// <summary>
/// 异常查询请求DTO
/// </summary>
public class QueryPurchaseExceptionDto
{
    public PurchaseExceptionType? Type { get; set; }
    public PurchaseExceptionStatus? Status { get; set; }
    public ExceptionLevel? Level { get; set; }
    public string? SupplierId { get; set; }
    public string? ProductId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
}

/// <summary>
/// 批量处理异常请求DTO
/// </summary>
public class BatchHandleExceptionsDto
{
    public List<string> ExceptionIds { get; set; } = new();
    public HandlePurchaseExceptionDto Request { get; set; } = new();
}

/// <summary>
/// 升级异常请求DTO
/// </summary>
public class EscalateExceptionDto
{
    public ExceptionLevel NewLevel { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 关闭异常请求DTO
/// </summary>
public class CloseExceptionDto
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 重新开放异常请求DTO
/// </summary>
public class ReopenExceptionDto
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 异常报告导出请求DTO
/// </summary>
public class ExportExceptionReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public PurchaseExceptionType? Type { get; set; }
    public string? SupplierId { get; set; }
}

/// <summary>
/// 发送通知请求DTO
/// </summary>
public class SendNotificationDto
{
    public string NotificationType { get; set; } = string.Empty;
    public List<string> Recipients { get; set; } = new();
    public string? CustomMessage { get; set; }
}

/// <summary>
/// 异常处理结果DTO
/// </summary>
public class PurchaseExceptionProcessingResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<string> SuggestedActions { get; set; } = new();
    public Dictionary<string, object> ExtensionData { get; set; } = new();
}

/// <summary>
/// 扩展方法：业务对象转换为DTO
/// </summary>
public static class PurchaseExceptionExtensions
{
    public static PurchaseExceptionDto ToDto(this PurchaseException exception)
    {
        return new PurchaseExceptionDto
        {
            Id = exception.Id,
            ExceptionNo = exception.ExceptionNo,
            PurchaseOrderId = exception.PurchaseOrderId,
            ReceiptId = exception.ReceiptId,
            ProductId = exception.ProductId,
            SupplierId = exception.SupplierId,
            Type = exception.Type,
            Level = exception.Level,
            Title = exception.Title,
            Description = exception.Description,
            ExpectedQuantity = exception.ExpectedQuantity,
            ActualQuantity = exception.ActualQuantity,
            DifferenceQuantity = exception.DifferenceQuantity,
            ExpectedAmount = exception.ExpectedAmount,
            ActualAmount = exception.ActualAmount,
            DifferenceAmount = exception.DifferenceAmount,
            ExpectedDeliveryDate = exception.ExpectedDeliveryDate,
            ActualDeliveryDate = exception.ActualDeliveryDate,
            DelayDays = exception.DelayDays,
            Status = exception.Status,
            Resolution = exception.Resolution,
            HandledBy = exception.HandledBy,
            HandledAt = exception.HandledAt,
            HandledRemark = exception.HandledRemark,
            ImpactAssessment = exception.ImpactAssessment,
            PreventiveMeasures = exception.PreventiveMeasures,
            RootCauseAnalysis = exception.RootCauseAnalysis,
            QualityDetails = exception.QualityDetails?.ToDto(),
            ExtensionData = exception.ExtensionData,
            CreatedAt = exception.CreatedAt,
            CreatedBy = exception.CreatedBy,
            UpdatedAt = exception.UpdatedAt,
            UpdatedBy = exception.UpdatedBy,
            SupplierName = exception.Supplier?.Name,
            ProductName = exception.Product?.Name,
            PurchaseOrderNo = exception.PurchaseOrder?.PoNo
        };
    }

    public static QualityIssueDetailsDto ToDto(this QualityIssueDetails details)
    {
        return new QualityIssueDetailsDto
        {
            IssueType = details.IssueType,
            DefectiveQuantity = details.DefectiveQuantity,
            DefectDescription = details.DefectDescription,
            InspectionStandard = details.InspectionStandard,
            Inspector = details.Inspector,
            InspectionDate = details.InspectionDate,
            AttachmentUrls = details.AttachmentUrls,
            ProcessingSuggestion = details.ProcessingSuggestion
        };
    }

    public static PurchaseExceptionHandlingRecordDto ToDto(this PurchaseExceptionHandlingRecord record)
    {
        return new PurchaseExceptionHandlingRecordDto
        {
            Id = record.Id,
            ExceptionId = record.ExceptionId,
            Action = record.Action,
            Description = record.Description,
            HandledBy = record.HandledBy,
            HandledAt = record.HandledAt,
            StatusFrom = record.StatusFrom,
            StatusTo = record.StatusTo,
            AttachmentUrls = record.AttachmentUrls
        };
    }

    public static PurchaseExceptionStatisticsDto ToDto(this PurchaseExceptionStatistics statistics)
    {
        return new PurchaseExceptionStatisticsDto
        {
            TotalExceptions = statistics.TotalExceptions,
            OpenExceptions = statistics.OpenExceptions,
            InProgressExceptions = statistics.InProgressExceptions,
            ResolvedExceptions = statistics.ResolvedExceptions,
            ClosedExceptions = statistics.ClosedExceptions,
            ExceptionsByType = statistics.ExceptionsByType.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value),
            ExceptionsByLevel = statistics.ExceptionsByLevel.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value),
            ExceptionsBySupplier = statistics.ExceptionsBySupplier,
            AverageResolutionDays = statistics.AverageResolutionDays,
            TotalImpactAmount = statistics.TotalImpactAmount,
            EarliestExceptionDate = statistics.EarliestExceptionDate,
            LatestExceptionDate = statistics.LatestExceptionDate
        };
    }

    public static PurchaseExceptionProcessingResultDto ToDto(this PurchaseExceptionProcessingResult result)
    {
        return new PurchaseExceptionProcessingResultDto
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            Errors = result.Errors,
            SuggestedActions = result.SuggestedActions,
            ExtensionData = result.ExtensionData
        };
    }

    public static CreatePurchaseExceptionRequest ToBusinessObject(this CreatePurchaseExceptionDto dto)
    {
        return new CreatePurchaseExceptionRequest
        {
            PurchaseOrderId = dto.PurchaseOrderId,
            ReceiptId = dto.ReceiptId,
            ProductId = dto.ProductId,
            SupplierId = dto.SupplierId,
            Type = dto.Type,
            Level = dto.Level,
            Title = dto.Title,
            Description = dto.Description,
            ExpectedQuantity = dto.ExpectedQuantity,
            ActualQuantity = dto.ActualQuantity,
            ExpectedAmount = dto.ExpectedAmount,
            ActualAmount = dto.ActualAmount,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            ActualDeliveryDate = dto.ActualDeliveryDate,
            QualityDetails = dto.QualityDetails?.ToBusinessObject(),
            ExtensionData = dto.ExtensionData
        };
    }

    public static QualityIssueDetails ToBusinessObject(this QualityIssueDetailsDto dto)
    {
        return new QualityIssueDetails
        {
            IssueType = dto.IssueType,
            DefectiveQuantity = dto.DefectiveQuantity,
            DefectDescription = dto.DefectDescription,
            InspectionStandard = dto.InspectionStandard,
            Inspector = dto.Inspector,
            InspectionDate = dto.InspectionDate,
            AttachmentUrls = dto.AttachmentUrls,
            ProcessingSuggestion = dto.ProcessingSuggestion
        };
    }

    public static HandlePurchaseExceptionRequest ToBusinessObject(this HandlePurchaseExceptionDto dto)
    {
        return new HandlePurchaseExceptionRequest
        {
            Action = dto.Action,
            Description = dto.Description,
            NewStatus = dto.NewStatus,
            Resolution = dto.Resolution,
            ImpactAssessment = dto.ImpactAssessment,
            PreventiveMeasures = dto.PreventiveMeasures,
            RootCauseAnalysis = dto.RootCauseAnalysis,
            AttachmentUrls = dto.AttachmentUrls,
            AdjustmentData = dto.AdjustmentData
        };
    }
}
