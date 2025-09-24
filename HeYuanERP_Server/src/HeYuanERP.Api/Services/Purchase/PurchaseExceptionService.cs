using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HeYuanERP.Api.Services.Purchase;

/// <summary>
/// 采购异常处理服务实现
/// </summary>
public class PurchaseExceptionService : IPurchaseExceptionService
{
    private readonly AppDbContext _db;
    private readonly ILogger<PurchaseExceptionService> _logger;

    public PurchaseExceptionService(AppDbContext db, ILogger<PurchaseExceptionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PurchaseException> CreateExceptionAsync(CreatePurchaseExceptionRequest request, string userId, CancellationToken ct = default)
    {
        try
        {
            var exception = new PurchaseException
            {
                ExceptionNo = await GenerateExceptionNumberAsync(ct),
                PurchaseOrderId = request.PurchaseOrderId,
                ReceiptId = request.ReceiptId,
                ProductId = request.ProductId,
                SupplierId = request.SupplierId,
                Type = request.Type,
                Level = request.Level,
                Title = request.Title,
                Description = request.Description,
                ExpectedQuantity = request.ExpectedQuantity,
                ActualQuantity = request.ActualQuantity,
                DifferenceQuantity = request.ActualQuantity - request.ExpectedQuantity,
                ExpectedAmount = request.ExpectedAmount,
                ActualAmount = request.ActualAmount,
                DifferenceAmount = request.ActualAmount - request.ExpectedAmount,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                ActualDeliveryDate = request.ActualDeliveryDate,
                QualityDetails = request.QualityDetails,
                ExtensionData = request.ExtensionData,
                CreatedBy = userId
            };

            // 计算延期天数
            if (request.ExpectedDeliveryDate.HasValue && request.ActualDeliveryDate.HasValue)
            {
                exception.DelayDays = (int)(request.ActualDeliveryDate.Value.Date - request.ExpectedDeliveryDate.Value.Date).TotalDays;
            }

            _db.PurchaseExceptions.Add(exception);

            // 添加初始处理记录
            var initialRecord = new PurchaseExceptionHandlingRecord
            {
                ExceptionId = exception.Id,
                Action = "创建异常",
                Description = $"异常类型：{request.Type}，级别：{request.Level}",
                HandledBy = userId,
                StatusTo = PurchaseExceptionStatus.Open
            };
            _db.PurchaseExceptionHandlingRecords.Add(initialRecord);

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Purchase exception {ExceptionId} created for order {OrderId} by user {UserId}",
                exception.Id, request.PurchaseOrderId, userId);

            return exception;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase exception");
            throw;
        }
    }

    // 占位实现：收货自动检测（后续对接收货与采购模型后启用正式实现）
    public async Task<List<PurchaseException>> AutoDetectExceptionsAsync(string receiptId, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return new List<PurchaseException>();
    }

#if false

    public async Task<List<PurchaseException>> AutoDetectExceptionsAsync(string receiptId, CancellationToken ct = default)
    {
        var detectedExceptions = new List<PurchaseException>();

        try
        {
            // 这里需要根据实际的收货单和采购订单实体结构来实现
            // 假设有Receipts和PurchaseOrders表
            var receipt = await _db.Receipts.AsNoTracking()
                .Include(r => r.Lines)
                .FirstOrDefaultAsync(r => r.Id == receiptId, ct);

            if (receipt?.PurchaseOrderId == null) return detectedExceptions;

            var purchaseOrder = await _db.PurchaseOrders.AsNoTracking()
                .Include(po => po.Lines)
                .FirstOrDefaultAsync(po => po.Id == receipt.PurchaseOrderId, ct);

            if (purchaseOrder == null) return detectedExceptions;

            // 检测数量差异异常
            foreach (var receiptLine in receipt.Lines ?? new List<ReceiptLine>())
            {
                var orderLine = purchaseOrder.Lines?.FirstOrDefault(ol => ol.ProductId == receiptLine.ProductId);
                if (orderLine == null) continue;

                var quantityDifference = receiptLine.Qty - orderLine.Qty;

                if (Math.Abs(quantityDifference) > 0.001m)
                {
                    var exceptionType = quantityDifference > 0 ? PurchaseExceptionType.OverReceive : PurchaseExceptionType.ShortReceive;
                    var level = Math.Abs(quantityDifference) > orderLine.Qty * 0.1m ? ExceptionLevel.High : ExceptionLevel.Medium;

                    var exception = new PurchaseException
                    {
                        ExceptionNo = await GenerateExceptionNumberAsync(ct),
                        PurchaseOrderId = purchaseOrder.Id,
                        ReceiptId = receiptId,
                        ProductId = receiptLine.ProductId,
                        SupplierId = purchaseOrder.SupplierId,
                        Type = exceptionType,
                        Level = level,
                        Title = $"产品 {receiptLine.ProductId} {(quantityDifference > 0 ? "超收" : "短收")}",
                        Description = $"订单数量：{orderLine.Qty}，实收数量：{receiptLine.Qty}，差异：{quantityDifference}",
                        ExpectedQuantity = orderLine.Qty,
                        ActualQuantity = receiptLine.Qty,
                        DifferenceQuantity = quantityDifference,
                        ExpectedAmount = orderLine.Qty * orderLine.UnitPrice,
                        ActualAmount = receiptLine.Qty * orderLine.UnitPrice,
                        DifferenceAmount = quantityDifference * orderLine.UnitPrice,
                        CreatedBy = "SYSTEM"
                    };

                    detectedExceptions.Add(exception);
                }

                // 检测价格差异
                if (Math.Abs(receiptLine.UnitPrice - orderLine.UnitPrice) > 0.01m)
                {
                    var priceDifference = receiptLine.UnitPrice - orderLine.UnitPrice;
                    var amountDifference = priceDifference * receiptLine.Qty;

                    var exception = new PurchaseException
                    {
                        ExceptionNo = await GenerateExceptionNumberAsync(ct),
                        PurchaseOrderId = purchaseOrder.Id,
                        ReceiptId = receiptId,
                        ProductId = receiptLine.ProductId,
                        SupplierId = purchaseOrder.SupplierId,
                        Type = PurchaseExceptionType.PriceDifference,
                        Level = Math.Abs(priceDifference / orderLine.UnitPrice) > 0.05m ? ExceptionLevel.High : ExceptionLevel.Medium,
                        Title = $"产品 {receiptLine.ProductId} 价格差异",
                        Description = $"订单单价：{orderLine.UnitPrice:F2}，实际单价：{receiptLine.UnitPrice:F2}，差异：{priceDifference:F2}",
                        ExpectedQuantity = receiptLine.Qty,
                        ActualQuantity = receiptLine.Qty,
                        ExpectedAmount = receiptLine.Qty * orderLine.UnitPrice,
                        ActualAmount = receiptLine.Qty * receiptLine.UnitPrice,
                        DifferenceAmount = amountDifference,
                        CreatedBy = "SYSTEM"
                    };

                    detectedExceptions.Add(exception);
                }
            }

            // 检测交期异常
            if (purchaseOrder.ExpectedDeliveryDate.HasValue && receipt.ReceivedDate.HasValue)
            {
                var delayDays = (int)(receipt.ReceivedDate.Value.Date - purchaseOrder.ExpectedDeliveryDate.Value.Date).TotalDays;
                if (delayDays > 0)
                {
                    var level = delayDays > 7 ? ExceptionLevel.High : ExceptionLevel.Medium;

                    var exception = new PurchaseException
                    {
                        ExceptionNo = await GenerateExceptionNumberAsync(ct),
                        PurchaseOrderId = purchaseOrder.Id,
                        ReceiptId = receiptId,
                        SupplierId = purchaseOrder.SupplierId,
                        Type = PurchaseExceptionType.DeliveryDelay,
                        Level = level,
                        Title = $"交期延误 {delayDays} 天",
                        Description = $"预期交货日期：{purchaseOrder.ExpectedDeliveryDate:yyyy-MM-dd}，实际交货日期：{receipt.ReceivedDate:yyyy-MM-dd}",
                        ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                        ActualDeliveryDate = receipt.ReceivedDate,
                        DelayDays = delayDays,
                        CreatedBy = "SYSTEM"
                    };

                    detectedExceptions.Add(exception);
                }
            }

            // 批量保存检测到的异常
            if (detectedExceptions.Any())
            {
                _db.PurchaseExceptions.AddRange(detectedExceptions);

                // 为每个异常添加初始处理记录
                foreach (var exception in detectedExceptions)
                {
                    var record = new PurchaseExceptionHandlingRecord
                    {
                        ExceptionId = exception.Id,
                        Action = "自动检测",
                        Description = "系统自动检测到的异常",
                        HandledBy = "SYSTEM",
                        StatusTo = PurchaseExceptionStatus.Open
                    };
                    _db.PurchaseExceptionHandlingRecords.Add(record);
                }

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Auto-detected {Count} purchase exceptions for receipt {ReceiptId}",
                    detectedExceptions.Count, receiptId);
            }

            return detectedExceptions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error auto-detecting exceptions for receipt {ReceiptId}", receiptId);
            return detectedExceptions;
        }
    }
#endif

    public async Task<PurchaseExceptionProcessingResult> HandleExceptionAsync(string exceptionId, HandlePurchaseExceptionRequest request, string userId, CancellationToken ct = default)
    {
        try
        {
            var exception = await _db.PurchaseExceptions
                .FirstOrDefaultAsync(e => e.Id == exceptionId, ct);

            if (exception == null)
            {
                return PurchaseExceptionProcessingResult.Failure("异常记录不存在");
            }

            var oldStatus = exception.Status;

            // 更新异常状态和处理信息
            exception.Status = request.NewStatus;
            exception.Resolution = request.Resolution;
            exception.HandledBy = userId;
            exception.HandledAt = DateTime.UtcNow;
            exception.ImpactAssessment = request.ImpactAssessment;
            exception.PreventiveMeasures = request.PreventiveMeasures;
            exception.RootCauseAnalysis = request.RootCauseAnalysis;
            exception.UpdatedBy = userId;
            exception.UpdatedAt = DateTime.UtcNow;

            // 添加处理记录
            var handlingRecord = new PurchaseExceptionHandlingRecord
            {
                ExceptionId = exceptionId,
                Action = request.Action,
                Description = request.Description,
                HandledBy = userId,
                StatusFrom = oldStatus,
                StatusTo = request.NewStatus,
                AttachmentUrls = request.AttachmentUrls
            };

            _db.PurchaseExceptionHandlingRecords.Add(handlingRecord);

            await _db.SaveChangesAsync(ct);

            var result = PurchaseExceptionProcessingResult.Success("异常处理成功");

            // 根据异常类型和状态生成建议
            var suggestions = await GenerateProcessingSuggestionsAsync(exceptionId, ct);
            result.SuggestedActions.AddRange(suggestions);

            _logger.LogInformation("Purchase exception {ExceptionId} handled by {UserId} with status {Status}",
                exceptionId, userId, request.NewStatus);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling purchase exception {ExceptionId}", exceptionId);
            return PurchaseExceptionProcessingResult.Failure("处理异常时发生错误");
        }
    }

    public async Task<PurchaseException?> GetExceptionAsync(string exceptionId, CancellationToken ct = default)
    {
        return await _db.PurchaseExceptions.AsNoTracking()
            .Include(e => e.PurchaseOrder)
            .Include(e => e.Supplier)
            .Include(e => e.Product)
            .Include(e => e.HandlingRecords)
            .FirstOrDefaultAsync(e => e.Id == exceptionId, ct);
    }

    public async Task<(List<PurchaseException> exceptions, int total)> QueryExceptionsAsync(
        PurchaseExceptionType? type = null,
        PurchaseExceptionStatus? status = null,
        ExceptionLevel? level = null,
        string? supplierId = null,
        string? productId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int size = 20,
        CancellationToken ct = default)
    {
        var query = _db.PurchaseExceptions.AsNoTracking();

        if (type.HasValue)
            query = query.Where(e => e.Type == type.Value);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (level.HasValue)
            query = query.Where(e => e.Level == level.Value);

        if (!string.IsNullOrEmpty(supplierId))
            query = query.Where(e => e.SupplierId == supplierId);

        if (!string.IsNullOrEmpty(productId))
            query = query.Where(e => e.ProductId == productId);

        if (fromDate.HasValue)
            query = query.Where(e => e.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.CreatedAt <= toDate.Value);

        var total = await query.CountAsync(ct);

        var exceptions = await query
            .Include(e => e.Supplier)
            .Include(e => e.Product)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return (exceptions, total);
    }

    public async Task<PurchaseExceptionStatistics> GetExceptionStatisticsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? supplierId = null,
        CancellationToken ct = default)
    {
        var query = _db.PurchaseExceptions.AsNoTracking();

        if (fromDate.HasValue)
            query = query.Where(e => e.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.CreatedAt <= toDate.Value);

        if (!string.IsNullOrEmpty(supplierId))
            query = query.Where(e => e.SupplierId == supplierId);

        var exceptions = await query.ToListAsync(ct);

        var statistics = new PurchaseExceptionStatistics
        {
            TotalExceptions = exceptions.Count,
            OpenExceptions = exceptions.Count(e => e.Status == PurchaseExceptionStatus.Open),
            InProgressExceptions = exceptions.Count(e => e.Status == PurchaseExceptionStatus.InProgress),
            ResolvedExceptions = exceptions.Count(e => e.Status == PurchaseExceptionStatus.Resolved),
            ClosedExceptions = exceptions.Count(e => e.Status == PurchaseExceptionStatus.Closed),
            ExceptionsByType = exceptions.GroupBy(e => e.Type).ToDictionary(g => g.Key, g => g.Count()),
            ExceptionsByLevel = exceptions.GroupBy(e => e.Level).ToDictionary(g => g.Key, g => g.Count()),
            ExceptionsBySupplier = exceptions.Where(e => !string.IsNullOrEmpty(e.SupplierId))
                .GroupBy(e => e.SupplierId!)
                .ToDictionary(g => g.Key, g => g.Count()),
            TotalImpactAmount = exceptions.Sum(e => Math.Abs(e.DifferenceAmount)),
            EarliestExceptionDate = exceptions.Any() ? exceptions.Min(e => e.CreatedAt) : null,
            LatestExceptionDate = exceptions.Any() ? exceptions.Max(e => e.CreatedAt) : null
        };

        // 计算平均解决天数
        var resolvedExceptions = exceptions.Where(e => e.Status == PurchaseExceptionStatus.Resolved && e.HandledAt.HasValue);
        if (resolvedExceptions.Any())
        {
            statistics.AverageResolutionDays = (decimal)resolvedExceptions
                .Average(e => (e.HandledAt!.Value - e.CreatedAt).TotalDays);
        }

        return statistics;
    }

    public async Task<List<string>> GenerateProcessingSuggestionsAsync(string exceptionId, CancellationToken ct = default)
    {
        var suggestions = new List<string>();

        try
        {
            var exception = await GetExceptionAsync(exceptionId, ct);
            if (exception == null) return suggestions;

            switch (exception.Type)
            {
                case PurchaseExceptionType.OverReceive:
                    suggestions.Add("联系供应商确认是否为额外发货");
                    suggestions.Add("检查是否有其他订单的货物混发");
                    suggestions.Add("考虑退回多余货物或调整库存");
                    if (exception.DifferenceQuantity > exception.ExpectedQuantity * 0.1m)
                        suggestions.Add("建议重新评估库存成本影响");
                    break;

                case PurchaseExceptionType.ShortReceive:
                    suggestions.Add("联系供应商确认剩余货物交期");
                    suggestions.Add("检查是否有货物在运输途中");
                    suggestions.Add("评估对生产计划的影响");
                    suggestions.Add("考虑寻找替代供应商");
                    break;

                case PurchaseExceptionType.QualityIssue:
                    suggestions.Add("隔离不合格产品");
                    suggestions.Add("联系供应商进行质量分析");
                    suggestions.Add("要求供应商提供纠正措施");
                    suggestions.Add("考虑加强来料检验");
                    if (exception.Level == ExceptionLevel.Critical)
                        suggestions.Add("立即停止使用该批次产品");
                    break;

                case PurchaseExceptionType.PriceDifference:
                    suggestions.Add("核实采购合同价格条款");
                    suggestions.Add("联系供应商确认价格调整原因");
                    suggestions.Add("评估对成本的影响");
                    if (exception.DifferenceAmount > 0)
                        suggestions.Add("考虑要求供应商承担差价");
                    break;

                case PurchaseExceptionType.DeliveryDelay:
                    suggestions.Add("评估对生产计划的影响");
                    suggestions.Add("联系供应商了解延期原因");
                    suggestions.Add("考虑启用备用供应商");
                    if (exception.DelayDays > 7)
                        suggestions.Add("要求供应商提供补偿措施");
                    break;
            }

            // 基于历史处理经验的建议
            var similarExceptions = await _db.PurchaseExceptions.AsNoTracking()
                .Where(e => e.Type == exception.Type &&
                           e.SupplierId == exception.SupplierId &&
                           e.Status == PurchaseExceptionStatus.Resolved)
                .Take(5)
                .ToListAsync(ct);

            if (similarExceptions.Any())
            {
                suggestions.Add("参考类似异常的成功处理经验");
            }

            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating processing suggestions for exception {ExceptionId}", exceptionId);
            return suggestions;
        }
    }

    // 省略其他方法的实现...
    public async Task<List<PurchaseExceptionProcessingResult>> BatchHandleExceptionsAsync(List<string> exceptionIds, HandlePurchaseExceptionRequest request, string userId, CancellationToken ct = default)
    {
        var results = new List<PurchaseExceptionProcessingResult>();
        foreach (var exceptionId in exceptionIds)
        {
            var result = await HandleExceptionAsync(exceptionId, request, userId, ct);
            results.Add(result);
        }
        return results;
    }

    public async Task<bool> EscalateExceptionAsync(string exceptionId, ExceptionLevel newLevel, string reason, string userId, CancellationToken ct = default)
    {
        // 实现升级逻辑
        return true;
    }

    public async Task<bool> CloseExceptionAsync(string exceptionId, string reason, string userId, CancellationToken ct = default)
    {
        // 实现关闭逻辑
        return true;
    }

    public async Task<bool> ReopenExceptionAsync(string exceptionId, string reason, string userId, CancellationToken ct = default)
    {
        // 实现重新开放逻辑
        return true;
    }

    public async Task<List<PurchaseException>> GetSupplierExceptionHistoryAsync(string supplierId, int limit = 50, CancellationToken ct = default)
    {
        return await _db.PurchaseExceptions.AsNoTracking()
            .Where(e => e.SupplierId == supplierId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<List<PurchaseException>> GetProductExceptionHistoryAsync(string productId, int limit = 50, CancellationToken ct = default)
    {
        return await _db.PurchaseExceptions.AsNoTracking()
            .Where(e => e.ProductId == productId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<string> CalculateImpactAssessmentAsync(string exceptionId, CancellationToken ct = default)
    {
        // 实现影响评估计算
        return "影响评估计算结果";
    }

    public async Task<List<string>> GeneratePreventiveMeasuresAsync(PurchaseExceptionType type, string? supplierId = null, CancellationToken ct = default)
    {
        // 实现预防措施生成
        return new List<string>();
    }

    public async Task<bool> SendExceptionNotificationAsync(string exceptionId, string notificationType, CancellationToken ct = default)
    {
        // 实现通知发送
        return true;
    }

    public async Task<List<PurchaseExceptionHandlingRecord>> GetHandlingRecordsAsync(string exceptionId, CancellationToken ct = default)
    {
        return await _db.PurchaseExceptionHandlingRecords.AsNoTracking()
            .Where(r => r.ExceptionId == exceptionId)
            .OrderByDescending(r => r.HandledAt)
            .ToListAsync(ct);
    }

    public async Task<byte[]> ExportExceptionReportAsync(DateTime fromDate, DateTime toDate, PurchaseExceptionType? type = null, string? supplierId = null, CancellationToken ct = default)
    {
        // 实现报告导出
        return Array.Empty<byte>();
    }

    #region Private Methods

    private async Task<string> GenerateExceptionNumberAsync(CancellationToken ct)
    {
        var today = DateTime.Today;
        var prefix = $"PE{today:yyyyMMdd}";

        var lastException = await _db.PurchaseExceptions.AsNoTracking()
            .Where(e => e.ExceptionNo.StartsWith(prefix))
            .OrderByDescending(e => e.ExceptionNo)
            .FirstOrDefaultAsync(ct);

        var sequence = 1;
        if (lastException != null && lastException.ExceptionNo.Length >= prefix.Length + 3)
        {
            var lastSequenceStr = lastException.ExceptionNo.Substring(prefix.Length);
            if (int.TryParse(lastSequenceStr, out var lastSequence))
            {
                sequence = lastSequence + 1;
            }
        }

        return $"{prefix}{sequence:D3}";
    }

    #endregion
}

// 假设的实体类（需要根据实际项目调整）
#if false
public class Receipt
{
    public string Id { get; set; } = string.Empty;
    public string? PurchaseOrderId { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public List<ReceiptLine>? Lines { get; set; }
}

public class ReceiptLine
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PurchaseOrder
{
    public string Id { get; set; } = string.Empty;
    public string? SupplierId { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public List<PurchaseOrderLine>? Lines { get; set; }
}

public class PurchaseOrderLine
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}
#endif
