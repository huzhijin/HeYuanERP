using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Api.Services.Purchase;

/// <summary>
/// 采购异常处理结果
/// </summary>
public class PurchaseExceptionProcessingResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 错误列表
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// 建议的处理动作
    /// </summary>
    public List<string> SuggestedActions { get; set; } = new();

    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, object> ExtensionData { get; set; } = new();

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static PurchaseExceptionProcessingResult Success(string message = "处理成功")
    {
        return new PurchaseExceptionProcessingResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static PurchaseExceptionProcessingResult Failure(string message, params string[] errors)
    {
        return new PurchaseExceptionProcessingResult
        {
            IsSuccess = false,
            Message = message,
            Errors = errors.ToList()
        };
    }

    /// <summary>
    /// 添加错误
    /// </summary>
    public void AddError(string error)
    {
        Errors.Add(error);
        IsSuccess = false;
    }

    /// <summary>
    /// 添加建议动作
    /// </summary>
    public void AddSuggestedAction(string action)
    {
        SuggestedActions.Add(action);
    }
}

/// <summary>
/// 创建异常请求信息
/// </summary>
public class CreatePurchaseExceptionRequest
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
    public QualityIssueDetails? QualityDetails { get; set; }
    public Dictionary<string, object> ExtensionData { get; set; } = new();
}

/// <summary>
/// 异常处理请求信息
/// </summary>
public class HandlePurchaseExceptionRequest
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
/// 异常统计信息
/// </summary>
public class PurchaseExceptionStatistics
{
    public int TotalExceptions { get; set; }
    public int OpenExceptions { get; set; }
    public int InProgressExceptions { get; set; }
    public int ResolvedExceptions { get; set; }
    public int ClosedExceptions { get; set; }
    public Dictionary<PurchaseExceptionType, int> ExceptionsByType { get; set; } = new();
    public Dictionary<ExceptionLevel, int> ExceptionsByLevel { get; set; } = new();
    public Dictionary<string, int> ExceptionsBySupplier { get; set; } = new();
    public decimal AverageResolutionDays { get; set; }
    public decimal TotalImpactAmount { get; set; }
    public DateTime? EarliestExceptionDate { get; set; }
    public DateTime? LatestExceptionDate { get; set; }
}

/// <summary>
/// 采购异常处理服务接口
/// </summary>
public interface IPurchaseExceptionService
{
    /// <summary>
    /// 创建采购异常
    /// </summary>
    Task<PurchaseException> CreateExceptionAsync(CreatePurchaseExceptionRequest request, string userId, CancellationToken ct = default);

    /// <summary>
    /// 自动检测采购异常（收货时调用）
    /// </summary>
    Task<List<PurchaseException>> AutoDetectExceptionsAsync(string receiptId, CancellationToken ct = default);

    /// <summary>
    /// 处理采购异常
    /// </summary>
    Task<PurchaseExceptionProcessingResult> HandleExceptionAsync(string exceptionId, HandlePurchaseExceptionRequest request, string userId, CancellationToken ct = default);

    /// <summary>
    /// 获取异常详情
    /// </summary>
    Task<PurchaseException?> GetExceptionAsync(string exceptionId, CancellationToken ct = default);

    /// <summary>
    /// 查询异常列表
    /// </summary>
    Task<(List<PurchaseException> exceptions, int total)> QueryExceptionsAsync(
        PurchaseExceptionType? type = null,
        PurchaseExceptionStatus? status = null,
        ExceptionLevel? level = null,
        string? supplierId = null,
        string? productId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int size = 20,
        CancellationToken ct = default);

    /// <summary>
    /// 获取异常统计信息
    /// </summary>
    Task<PurchaseExceptionStatistics> GetExceptionStatisticsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? supplierId = null,
        CancellationToken ct = default);

    /// <summary>
    /// 批量处理异常
    /// </summary>
    Task<List<PurchaseExceptionProcessingResult>> BatchHandleExceptionsAsync(
        List<string> exceptionIds,
        HandlePurchaseExceptionRequest request,
        string userId,
        CancellationToken ct = default);

    /// <summary>
    /// 生成异常处理建议
    /// </summary>
    Task<List<string>> GenerateProcessingSuggestionsAsync(string exceptionId, CancellationToken ct = default);

    /// <summary>
    /// 升级异常级别
    /// </summary>
    Task<bool> EscalateExceptionAsync(string exceptionId, ExceptionLevel newLevel, string reason, string userId, CancellationToken ct = default);

    /// <summary>
    /// 关闭异常
    /// </summary>
    Task<bool> CloseExceptionAsync(string exceptionId, string reason, string userId, CancellationToken ct = default);

    /// <summary>
    /// 重新开放异常
    /// </summary>
    Task<bool> ReopenExceptionAsync(string exceptionId, string reason, string userId, CancellationToken ct = default);

    /// <summary>
    /// 获取供应商异常历史
    /// </summary>
    Task<List<PurchaseException>> GetSupplierExceptionHistoryAsync(string supplierId, int limit = 50, CancellationToken ct = default);

    /// <summary>
    /// 获取产品异常历史
    /// </summary>
    Task<List<PurchaseException>> GetProductExceptionHistoryAsync(string productId, int limit = 50, CancellationToken ct = default);

    /// <summary>
    /// 计算异常影响评估
    /// </summary>
    Task<string> CalculateImpactAssessmentAsync(string exceptionId, CancellationToken ct = default);

    /// <summary>
    /// 生成预防措施建议
    /// </summary>
    Task<List<string>> GeneratePreventiveMeasuresAsync(PurchaseExceptionType type, string? supplierId = null, CancellationToken ct = default);

    /// <summary>
    /// 发送异常通知
    /// </summary>
    Task<bool> SendExceptionNotificationAsync(string exceptionId, string notificationType, CancellationToken ct = default);

    /// <summary>
    /// 获取异常处理记录
    /// </summary>
    Task<List<PurchaseExceptionHandlingRecord>> GetHandlingRecordsAsync(string exceptionId, CancellationToken ct = default);

    /// <summary>
    /// 导出异常报告
    /// </summary>
    Task<byte[]> ExportExceptionReportAsync(
        DateTime fromDate,
        DateTime toDate,
        PurchaseExceptionType? type = null,
        string? supplierId = null,
        CancellationToken ct = default);
}