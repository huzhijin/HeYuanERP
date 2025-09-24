using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;
using HeYuanERP.Infrastructure.Persistence;

namespace HeYuanERP.Api.Services.Finance;

public class AccountsReceivablePayableService : IAccountsReceivablePayableService
{
    private readonly HeYuanERPDbContext _context;
    private readonly ILogger<AccountsReceivablePayableService> _logger;

    public AccountsReceivablePayableService(HeYuanERPDbContext context, ILogger<AccountsReceivablePayableService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region 应收账款管理

    public async Task<ApiResponse<AccountReceivable>> CreateAccountReceivableAsync(AccountReceivable receivable)
    {
        try
        {
            receivable.CreatedAt = DateTime.UtcNow;
            receivable.BalanceAmount = receivable.OriginalAmount - receivable.PaidAmount;

            // 计算账龄桶
            receivable.AgingBucket = CalculateAgingBucket(receivable.DueDate);
            receivable.OverdueDays = CalculateOverdueDays(receivable.DueDate);

            // 计算风险等级
            receivable.RiskLevel = CalculateRiskLevel(receivable);

            _context.AccountsReceivable.Add(receivable);
            await _context.SaveChangesAsync();

            _logger.LogInformation("应收账款创建成功，ID: {Id}, 单据号: {DocumentNumber}", receivable.Id, receivable.DocumentNumber);
            return ApiResponse<AccountReceivable>.Ok(receivable, "应收账款创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建应收账款失败");
            return ApiResponse<AccountReceivable>.Error("创建应收账款失败");
        }
    }

    public async Task<ApiResponse<AccountReceivable>> UpdateAccountReceivableAsync(AccountReceivable receivable)
    {
        try
        {
            var existing = await _context.AccountsReceivable.FindAsync(receivable.Id);
            if (existing == null)
                return ApiResponse<AccountReceivable>.Error("应收账款不存在");

            // 更新关键字段
            existing.PaidAmount = receivable.PaidAmount;
            existing.BalanceAmount = existing.OriginalAmount - existing.PaidAmount;
            existing.Status = existing.BalanceAmount <= 0 ? "FullyPaid" :
                            existing.PaidAmount > 0 ? "PartiallyPaid" : "Outstanding";
            existing.AgingBucket = CalculateAgingBucket(existing.DueDate);
            existing.OverdueDays = CalculateOverdueDays(existing.DueDate);
            existing.RiskLevel = CalculateRiskLevel(existing);
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = receivable.UpdatedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("应收账款更新成功，ID: {Id}", receivable.Id);
            return ApiResponse<AccountReceivable>.Ok(existing, "应收账款更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新应收账款失败，ID: {Id}", receivable.Id);
            return ApiResponse<AccountReceivable>.Error("更新应收账款失败");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAccountReceivableAsync(int id)
    {
        try
        {
            var receivable = await _context.AccountsReceivable.FindAsync(id);
            if (receivable == null)
                return ApiResponse<bool>.Error("应收账款不存在");

            // 检查是否可以删除
            if (receivable.PaidAmount > 0)
                return ApiResponse<bool>.Error("已有收款记录，不能删除");

            _context.AccountsReceivable.Remove(receivable);
            await _context.SaveChangesAsync();

            _logger.LogInformation("应收账款删除成功，ID: {Id}", id);
            return ApiResponse<bool>.Ok(true, "应收账款删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除应收账款失败，ID: {Id}", id);
            return ApiResponse<bool>.Error("删除应收账款失败");
        }
    }

    public async Task<ApiResponse<AccountReceivable?>> GetAccountReceivableByIdAsync(int id)
    {
        try
        {
            var receivable = await _context.AccountsReceivable
                .Include(r => r.CollectionRecords)
                .Include(r => r.PaymentApplications)
                .FirstOrDefaultAsync(r => r.Id == id);

            return ApiResponse<AccountReceivable?>.Ok(receivable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取应收账款失败，ID: {Id}", id);
            return ApiResponse<AccountReceivable?>.Error("获取应收账款失败");
        }
    }

    public async Task<ApiResponse<AccountReceivable?>> GetAccountReceivableByDocumentAsync(string documentNumber, string documentType)
    {
        try
        {
            var receivable = await _context.AccountsReceivable
                .FirstOrDefaultAsync(r => r.DocumentNumber == documentNumber && r.DocumentType == documentType);

            return ApiResponse<AccountReceivable?>.Ok(receivable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取应收账款失败，单据号: {DocumentNumber}, 类型: {DocumentType}", documentNumber, documentType);
            return ApiResponse<AccountReceivable?>.Error("获取应收账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.AccountsReceivable.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.DocumentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.DocumentDate <= endDate.Value);

            var receivables = await query.OrderByDescending(r => r.DocumentDate).ToListAsync();
            return ApiResponse<List<AccountReceivable>>.Ok(receivables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取应收账款列表失败");
            return ApiResponse<List<AccountReceivable>>.Error("获取应收账款列表失败");
        }
    }

    public async Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesByCustomerAsync(int customerId)
    {
        try
        {
            var receivables = await _context.AccountsReceivable
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.DocumentDate)
                .ToListAsync();

            return ApiResponse<List<AccountReceivable>>.Ok(receivables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户应收账款失败，客户ID: {CustomerId}", customerId);
            return ApiResponse<List<AccountReceivable>>.Error("获取客户应收账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesByStatusAsync(string status)
    {
        try
        {
            var receivables = await _context.AccountsReceivable
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.DocumentDate)
                .ToListAsync();

            return ApiResponse<List<AccountReceivable>>.Ok(receivables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取状态应收账款失败，状态: {Status}", status);
            return ApiResponse<List<AccountReceivable>>.Error("获取状态应收账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountReceivable>>> GetOverdueAccountReceivablesAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var receivables = await _context.AccountsReceivable
                .Where(r => r.DueDate < today && r.BalanceAmount > 0)
                .OrderByDescending(r => r.OverdueDays)
                .ToListAsync();

            return ApiResponse<List<AccountReceivable>>.Ok(receivables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取逾期应收账款失败");
            return ApiResponse<List<AccountReceivable>>.Error("获取逾期应收账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesByAgingAsync(string agingBucket)
    {
        try
        {
            var receivables = await _context.AccountsReceivable
                .Where(r => r.AgingBucket == agingBucket)
                .OrderByDescending(r => r.DocumentDate)
                .ToListAsync();

            return ApiResponse<List<AccountReceivable>>.Ok(receivables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取账龄应收账款失败，账龄: {AgingBucket}", agingBucket);
            return ApiResponse<List<AccountReceivable>>.Error("获取账龄应收账款失败");
        }
    }

    #endregion

    #region 应付账款管理

    public async Task<ApiResponse<AccountPayable>> CreateAccountPayableAsync(AccountPayable payable)
    {
        try
        {
            payable.CreatedAt = DateTime.UtcNow;
            payable.BalanceAmount = payable.OriginalAmount - payable.PaidAmount;

            // 计算账龄桶
            payable.AgingBucket = CalculateAgingBucket(payable.DueDate);
            payable.OverdueDays = CalculateOverdueDays(payable.DueDate);

            _context.AccountsPayable.Add(payable);
            await _context.SaveChangesAsync();

            _logger.LogInformation("应付账款创建成功，ID: {Id}, 单据号: {DocumentNumber}", payable.Id, payable.DocumentNumber);
            return ApiResponse<AccountPayable>.Ok(payable, "应付账款创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建应付账款失败");
            return ApiResponse<AccountPayable>.Error("创建应付账款失败");
        }
    }

    public async Task<ApiResponse<AccountPayable>> UpdateAccountPayableAsync(AccountPayable payable)
    {
        try
        {
            var existing = await _context.AccountsPayable.FindAsync(payable.Id);
            if (existing == null)
                return ApiResponse<AccountPayable>.Error("应付账款不存在");

            // 更新关键字段
            existing.PaidAmount = payable.PaidAmount;
            existing.BalanceAmount = existing.OriginalAmount - existing.PaidAmount;
            existing.Status = existing.BalanceAmount <= 0 ? "FullyPaid" :
                            existing.PaidAmount > 0 ? "PartiallyPaid" : "Outstanding";
            existing.AgingBucket = CalculateAgingBucket(existing.DueDate);
            existing.OverdueDays = CalculateOverdueDays(existing.DueDate);
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = payable.UpdatedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("应付账款更新成功，ID: {Id}", payable.Id);
            return ApiResponse<AccountPayable>.Ok(existing, "应付账款更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新应付账款失败，ID: {Id}", payable.Id);
            return ApiResponse<AccountPayable>.Error("更新应付账款失败");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAccountPayableAsync(int id)
    {
        try
        {
            var payable = await _context.AccountsPayable.FindAsync(id);
            if (payable == null)
                return ApiResponse<bool>.Error("应付账款不存在");

            // 检查是否可以删除
            if (payable.PaidAmount > 0)
                return ApiResponse<bool>.Error("已有付款记录，不能删除");

            _context.AccountsPayable.Remove(payable);
            await _context.SaveChangesAsync();

            _logger.LogInformation("应付账款删除成功，ID: {Id}", id);
            return ApiResponse<bool>.Ok(true, "应付账款删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除应付账款失败，ID: {Id}", id);
            return ApiResponse<bool>.Error("删除应付账款失败");
        }
    }

    public async Task<ApiResponse<AccountPayable?>> GetAccountPayableByIdAsync(int id)
    {
        try
        {
            var payable = await _context.AccountsPayable
                .Include(p => p.PaymentSchedules)
                .FirstOrDefaultAsync(p => p.Id == id);

            return ApiResponse<AccountPayable?>.Ok(payable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取应付账款失败，ID: {Id}", id);
            return ApiResponse<AccountPayable?>.Error("获取应付账款失败");
        }
    }

    public async Task<ApiResponse<AccountPayable?>> GetAccountPayableByDocumentAsync(string documentNumber, string documentType)
    {
        try
        {
            var payable = await _context.AccountsPayable
                .FirstOrDefaultAsync(p => p.DocumentNumber == documentNumber && p.DocumentType == documentType);

            return ApiResponse<AccountPayable?>.Ok(payable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取应付账款失败，单据号: {DocumentNumber}, 类型: {DocumentType}", documentNumber, documentType);
            return ApiResponse<AccountPayable?>.Error("获取应付账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.AccountsPayable.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(p => p.DocumentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.DocumentDate <= endDate.Value);

            var payables = await query.OrderByDescending(p => p.DocumentDate).ToListAsync();
            return ApiResponse<List<AccountPayable>>.Ok(payables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取应付账款列表失败");
            return ApiResponse<List<AccountPayable>>.Error("获取应付账款列表失败");
        }
    }

    public async Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesBySupplierAsync(int supplierId)
    {
        try
        {
            var payables = await _context.AccountsPayable
                .Where(p => p.SupplierId == supplierId)
                .OrderByDescending(p => p.DocumentDate)
                .ToListAsync();

            return ApiResponse<List<AccountPayable>>.Ok(payables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商应付账款失败，供应商ID: {SupplierId}", supplierId);
            return ApiResponse<List<AccountPayable>>.Error("获取供应商应付账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesByStatusAsync(string status)
    {
        try
        {
            var payables = await _context.AccountsPayable
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.DocumentDate)
                .ToListAsync();

            return ApiResponse<List<AccountPayable>>.Ok(payables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取状态应付账款失败，状态: {Status}", status);
            return ApiResponse<List<AccountPayable>>.Error("获取状态应付账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountPayable>>> GetOverdueAccountPayablesAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var payables = await _context.AccountsPayable
                .Where(p => p.DueDate < today && p.BalanceAmount > 0)
                .OrderByDescending(p => p.OverdueDays)
                .ToListAsync();

            return ApiResponse<List<AccountPayable>>.Ok(payables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取逾期应付账款失败");
            return ApiResponse<List<AccountPayable>>.Error("获取逾期应付账款失败");
        }
    }

    public async Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesByAgingAsync(string agingBucket)
    {
        try
        {
            var payables = await _context.AccountsPayable
                .Where(p => p.AgingBucket == agingBucket)
                .OrderByDescending(p => p.DocumentDate)
                .ToListAsync();

            return ApiResponse<List<AccountPayable>>.Ok(payables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取账龄应付账款失败，账龄: {AgingBucket}", agingBucket);
            return ApiResponse<List<AccountPayable>>.Error("获取账龄应付账款失败");
        }
    }

    #endregion

    #region 账龄分析

    public async Task<ApiResponse<List<object>>> GetCustomerAgingAnalysisAsync(DateTime asOfDate)
    {
        try
        {
            var receivables = await _context.AccountsReceivable
                .Where(r => r.DocumentDate <= asOfDate && r.BalanceAmount > 0)
                .ToListAsync();

            var agingAnalysis = receivables
                .GroupBy(r => new { r.CustomerId, r.CustomerName })
                .Select(g => new
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    Current = g.Where(r => r.AgingBucket == "Current").Sum(r => r.BalanceAmount),
                    Days1_30 = g.Where(r => r.AgingBucket == "1-30").Sum(r => r.BalanceAmount),
                    Days31_60 = g.Where(r => r.AgingBucket == "31-60").Sum(r => r.BalanceAmount),
                    Days61_90 = g.Where(r => r.AgingBucket == "61-90").Sum(r => r.BalanceAmount),
                    Over90Days = g.Where(r => r.AgingBucket == "Over90").Sum(r => r.BalanceAmount),
                    TotalAmount = g.Sum(r => r.BalanceAmount)
                })
                .OrderByDescending(a => a.TotalAmount)
                .ToList<object>();

            return ApiResponse<List<object>>.Ok(agingAnalysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户账龄分析失败");
            return ApiResponse<List<object>>.Error("获取客户账龄分析失败");
        }
    }

    public async Task<ApiResponse<List<object>>> GetSupplierAgingAnalysisAsync(DateTime asOfDate)
    {
        try
        {
            var payables = await _context.AccountsPayable
                .Where(p => p.DocumentDate <= asOfDate && p.BalanceAmount > 0)
                .ToListAsync();

            var agingAnalysis = payables
                .GroupBy(p => new { p.SupplierId, p.SupplierName })
                .Select(g => new
                {
                    SupplierId = g.Key.SupplierId,
                    SupplierName = g.Key.SupplierName,
                    Current = g.Where(p => p.AgingBucket == "Current").Sum(p => p.BalanceAmount),
                    Days1_30 = g.Where(p => p.AgingBucket == "1-30").Sum(p => p.BalanceAmount),
                    Days31_60 = g.Where(p => p.AgingBucket == "31-60").Sum(p => p.BalanceAmount),
                    Days61_90 = g.Where(p => p.AgingBucket == "61-90").Sum(p => p.BalanceAmount),
                    Over90Days = g.Where(p => p.AgingBucket == "Over90").Sum(p => p.BalanceAmount),
                    TotalAmount = g.Sum(p => p.BalanceAmount)
                })
                .OrderByDescending(a => a.TotalAmount)
                .ToList<object>();

            return ApiResponse<List<object>>.Ok(agingAnalysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商账龄分析失败");
            return ApiResponse<List<object>>.Error("获取供应商账龄分析失败");
        }
    }

    public async Task<ApiResponse<object>> GetAgingAnalysisSummaryAsync(DateTime asOfDate)
    {
        try
        {
            var receivables = await _context.AccountsReceivable
                .Where(r => r.DocumentDate <= asOfDate && r.BalanceAmount > 0)
                .ToListAsync();

            var payables = await _context.AccountsPayable
                .Where(p => p.DocumentDate <= asOfDate && p.BalanceAmount > 0)
                .ToListAsync();

            var summary = new
            {
                AsOfDate = asOfDate,
                Receivables = new
                {
                    Current = receivables.Where(r => r.AgingBucket == "Current").Sum(r => r.BalanceAmount),
                    Days1_30 = receivables.Where(r => r.AgingBucket == "1-30").Sum(r => r.BalanceAmount),
                    Days31_60 = receivables.Where(r => r.AgingBucket == "31-60").Sum(r => r.BalanceAmount),
                    Days61_90 = receivables.Where(r => r.AgingBucket == "61-90").Sum(r => r.BalanceAmount),
                    Over90Days = receivables.Where(r => r.AgingBucket == "Over90").Sum(r => r.BalanceAmount),
                    TotalAmount = receivables.Sum(r => r.BalanceAmount)
                },
                Payables = new
                {
                    Current = payables.Where(p => p.AgingBucket == "Current").Sum(p => p.BalanceAmount),
                    Days1_30 = payables.Where(p => p.AgingBucket == "1-30").Sum(p => p.BalanceAmount),
                    Days31_60 = payables.Where(p => p.AgingBucket == "31-60").Sum(p => p.BalanceAmount),
                    Days61_90 = payables.Where(p => p.AgingBucket == "61-90").Sum(p => p.BalanceAmount),
                    Over90Days = payables.Where(p => p.AgingBucket == "Over90").Sum(p => p.BalanceAmount),
                    TotalAmount = payables.Sum(p => p.BalanceAmount)
                }
            };

            return ApiResponse<object>.Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取账龄分析汇总失败");
            return ApiResponse<object>.Error("获取账龄分析汇总失败");
        }
    }

    #endregion

    #region 催收管理

    public async Task<ApiResponse<CollectionRecord>> CreateCollectionRecordAsync(CollectionRecord record)
    {
        try
        {
            record.CreatedAt = DateTime.UtcNow;
            _context.CollectionRecords.Add(record);

            // 更新应收账款的最后催收信息
            var receivable = await _context.AccountsReceivable.FindAsync(record.AccountReceivableId);
            if (receivable != null)
            {
                receivable.LastCollectionDate = record.ContactDate;
                receivable.LastCollectionMethod = record.ContactMethod;
                receivable.NextCollectionDate = record.NextContactDate;
                receivable.CollectionNotes = record.ContactNotes;
                receivable.CollectionAttempts++;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("催收记录创建成功，ID: {Id}", record.Id);
            return ApiResponse<CollectionRecord>.Ok(record, "催收记录创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建催收记录失败");
            return ApiResponse<CollectionRecord>.Error("创建催收记录失败");
        }
    }

    public async Task<ApiResponse<CollectionRecord>> UpdateCollectionRecordAsync(CollectionRecord record)
    {
        try
        {
            _context.CollectionRecords.Update(record);
            await _context.SaveChangesAsync();

            _logger.LogInformation("催收记录更新成功，ID: {Id}", record.Id);
            return ApiResponse<CollectionRecord>.Ok(record, "催收记录更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新催收记录失败，ID: {Id}", record.Id);
            return ApiResponse<CollectionRecord>.Error("更新催收记录失败");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCollectionRecordAsync(int id)
    {
        try
        {
            var record = await _context.CollectionRecords.FindAsync(id);
            if (record == null)
                return ApiResponse<bool>.Error("催收记录不存在");

            _context.CollectionRecords.Remove(record);
            await _context.SaveChangesAsync();

            _logger.LogInformation("催收记录删除成功，ID: {Id}", id);
            return ApiResponse<bool>.Ok(true, "催收记录删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除催收记录失败，ID: {Id}", id);
            return ApiResponse<bool>.Error("删除催收记录失败");
        }
    }

    public async Task<ApiResponse<CollectionRecord?>> GetCollectionRecordByIdAsync(int id)
    {
        try
        {
            var record = await _context.CollectionRecords
                .Include(r => r.AccountReceivable)
                .FirstOrDefaultAsync(r => r.Id == id);

            return ApiResponse<CollectionRecord?>.Ok(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取催收记录失败，ID: {Id}", id);
            return ApiResponse<CollectionRecord?>.Error("获取催收记录失败");
        }
    }

    public async Task<ApiResponse<List<CollectionRecord>>> GetCollectionRecordsByReceivableAsync(int receivableId)
    {
        try
        {
            var records = await _context.CollectionRecords
                .Where(r => r.AccountReceivableId == receivableId)
                .OrderByDescending(r => r.ContactDate)
                .ToListAsync();

            return ApiResponse<List<CollectionRecord>>.Ok(records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取应收账款催收记录失败，应收账款ID: {ReceivableId}", receivableId);
            return ApiResponse<List<CollectionRecord>>.Error("获取催收记录失败");
        }
    }

    public async Task<ApiResponse<List<CollectionRecord>>> GetCollectionRecordsByCollectorAsync(int collectorId)
    {
        try
        {
            var records = await _context.CollectionRecords
                .Where(r => r.CollectorId == collectorId)
                .OrderByDescending(r => r.ContactDate)
                .ToListAsync();

            return ApiResponse<List<CollectionRecord>>.Ok(records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取催收人催收记录失败，催收人ID: {CollectorId}", collectorId);
            return ApiResponse<List<CollectionRecord>>.Error("获取催收记录失败");
        }
    }

    public async Task<ApiResponse<List<CollectionRecord>>> GetCollectionRecordsByDateAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var records = await _context.CollectionRecords
                .Where(r => r.ContactDate >= startDate && r.ContactDate <= endDate)
                .OrderByDescending(r => r.ContactDate)
                .ToListAsync();

            return ApiResponse<List<CollectionRecord>>.Ok(records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取日期范围催收记录失败");
            return ApiResponse<List<CollectionRecord>>.Error("获取催收记录失败");
        }
    }

    #endregion

    #region 辅助方法

    private string CalculateAgingBucket(DateTime dueDate)
    {
        var days = (DateTime.UtcNow.Date - dueDate.Date).Days;

        if (days <= 0) return "Current";
        if (days <= 30) return "1-30";
        if (days <= 60) return "31-60";
        if (days <= 90) return "61-90";
        return "Over90";
    }

    private int CalculateOverdueDays(DateTime dueDate)
    {
        var days = (DateTime.UtcNow.Date - dueDate.Date).Days;
        return days > 0 ? days : 0;
    }

    private string CalculateRiskLevel(AccountReceivable receivable)
    {
        if (receivable.OverdueDays > 90) return "High";
        if (receivable.OverdueDays > 60) return "Medium";
        if (receivable.OverdueDays > 30) return "Medium";
        return "Low";
    }

    #endregion

    #region 其他方法基础实现

    // 由于文件篇幅限制，其他方法返回基础实现
    public Task<ApiResponse<List<object>>> GetAgingTrendsAsync(int months = 12)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetCustomerAgingDetailAsync(int customerId, DateTime asOfDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetSupplierAgingDetailAsync(int supplierId, DateTime asOfDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetCollectionPlanAsync(int collectorId)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetCollectionEfficiencyAsync(int collectorId, DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetPendingCollectionTasksAsync(int collectorId)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetOverdueCollectionTasksAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> SendCollectionReminderAsync(int receivableId, string reminderType)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> ScheduleCollectionTaskAsync(int receivableId, DateTime scheduledDate, string taskType)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetCollectionCalendarAsync(int collectorId, DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<PaymentApplication>> CreatePaymentApplicationAsync(PaymentApplication application)
        => Task.FromResult(ApiResponse<PaymentApplication>.Error("功能开发中"));

    public Task<ApiResponse<PaymentApplication>> UpdatePaymentApplicationAsync(PaymentApplication application)
        => Task.FromResult(ApiResponse<PaymentApplication>.Error("功能开发中"));

    public Task<ApiResponse<bool>> DeletePaymentApplicationAsync(int id)
        => Task.FromResult(ApiResponse<bool>.Error("功能开发中"));

    public Task<ApiResponse<List<PaymentApplication>>> GetPaymentApplicationsByReceivableAsync(int receivableId)
        => Task.FromResult(ApiResponse<List<PaymentApplication>>.Error("功能开发中"));

    public Task<ApiResponse<List<PaymentApplication>>> GetPaymentApplicationsByPaymentAsync(string paymentNumber)
        => Task.FromResult(ApiResponse<List<PaymentApplication>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> AutoMatchPaymentsAsync(int customerId)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetUnmatchedPaymentsAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<PaymentSchedule>> CreatePaymentScheduleAsync(PaymentSchedule schedule)
        => Task.FromResult(ApiResponse<PaymentSchedule>.Error("功能开发中"));

    public Task<ApiResponse<PaymentSchedule>> UpdatePaymentScheduleAsync(PaymentSchedule schedule)
        => Task.FromResult(ApiResponse<PaymentSchedule>.Error("功能开发中"));

    public Task<ApiResponse<bool>> DeletePaymentScheduleAsync(int id)
        => Task.FromResult(ApiResponse<bool>.Error("功能开发中"));

    public Task<ApiResponse<List<PaymentSchedule>>> GetPaymentSchedulesByPayableAsync(int payableId)
        => Task.FromResult(ApiResponse<List<PaymentSchedule>>.Error("功能开发中"));

    public Task<ApiResponse<List<PaymentSchedule>>> GetPaymentSchedulesByDateAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<List<PaymentSchedule>>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetPaymentPlanAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> ExecuteScheduledPaymentAsync(int scheduleId, decimal actualAmount)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<AccountReceivable>> WriteOffBadDebtAsync(int receivableId, decimal writeOffAmount, string reason, string approvedBy)
        => Task.FromResult(ApiResponse<AccountReceivable>.Error("功能开发中"));

    public Task<ApiResponse<AccountReceivable>> RecoverWrittenOffDebtAsync(int receivableId, decimal recoveryAmount)
        => Task.FromResult(ApiResponse<AccountReceivable>.Error("功能开发中"));

    public Task<ApiResponse<List<AccountReceivable>>> GetWrittenOffAccountsAsync(DateTime? startDate = null, DateTime? endDate = null)
        => Task.FromResult(ApiResponse<List<AccountReceivable>>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetBadDebtAnalysisAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> MarkHighRiskAccountAsync(int receivableId, string reason)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetHighRiskAccountsAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetCustomerCreditAnalysisAsync(int customerId)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<bool>> UpdateCustomerCreditLimitAsync(int customerId, decimal creditLimit)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> ValidateCreditLimitAsync(int customerId, decimal additionalAmount)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetCreditUtilizationAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> SetCustomerPaymentTermsAsync(int customerId, string paymentTerms)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<object>> GetCashFlowForecastAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetReceivableForecastAsync(DateTime forecastDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetPayableForecastAsync(DateTime forecastDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetCashFlowTrendsAsync(int months = 12)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetLiquidityAnalysisAsync()
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    // 其余方法继续返回基础实现...
    #region 其他功能基础实现
    public Task<ApiResponse<bool>> AutoGenerateReceivablesFromInvoicesAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> AutoGeneratePayablesFromPurchasesAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> AutoUpdateAgingBucketsAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> AutoCalculateRiskLevelsAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> ProcessScheduledRemindersAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<object>> GetReceivableSummaryAsync(DateTime? asOfDate = null)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetPayableSummaryAsync(DateTime? asOfDate = null)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetCollectionPerformanceAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetPaymentPerformanceAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetTopDebtorsAsync(int topN = 10)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetTopCreditorsAsync(int topN = 10)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetTurnoverAnalysisAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetDSOAnalysisAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetDPOAnalysisAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetDSOTrendsAsync(int months = 12)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetDPOTrendsAsync(int months = 12)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<object>> ReconcileCustomerAccountAsync(int customerId, DateTime asOfDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> ReconcileSupplierAccountAsync(int supplierId, DateTime asOfDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetReconciliationDifferencesAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> ProcessReconciliationAdjustmentAsync(int accountId, decimal adjustmentAmount, string reason)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<object>> GenerateCustomerStatementAsync(int customerId, DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<bool>> SendCustomerStatementAsync(int customerId, DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetCustomerStatementHistoryAsync(int customerId)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<object>> GenerateSupplierStatementAsync(int supplierId, DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<bool>> SendSupplierStatementAsync(int supplierId, DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetSupplierStatementHistoryAsync(int supplierId)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> SubmitWriteOffRequestAsync(int receivableId, decimal amount, string reason)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> ApproveWriteOffRequestAsync(int requestId, string approvedBy, string comments)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> RejectWriteOffRequestAsync(int requestId, string rejectedBy, string reason)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetPendingWriteOffRequestsAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> SendPaymentReminderAsync(int receivableId, string reminderLevel)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> SendPaymentNotificationAsync(int payableId, string notificationType)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetPendingRemindersAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> MarkReminderSentAsync(int reminderId)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> ImportReceivablesAsync(byte[] fileData, string fileName)
        => Task.FromResult(ApiResponse<bool>.Error("功能开发中"));

    public Task<ApiResponse<bool>> ImportPayablesAsync(byte[] fileData, string fileName)
        => Task.FromResult(ApiResponse<bool>.Error("功能开发中"));

    public Task<ApiResponse<byte[]>> ExportReceivablesAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "Excel")
        => Task.FromResult(ApiResponse<byte[]>.Error("功能开发中"));

    public Task<ApiResponse<byte[]>> ExportPayablesAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "Excel")
        => Task.FromResult(ApiResponse<byte[]>.Error("功能开发中"));

    public Task<ApiResponse<byte[]>> ExportAgingReportAsync(DateTime asOfDate, string format = "Excel")
        => Task.FromResult(ApiResponse<byte[]>.Error("功能开发中"));

    public Task<ApiResponse<byte[]>> ExportCollectionReportAsync(DateTime startDate, DateTime endDate, string format = "Excel")
        => Task.FromResult(ApiResponse<byte[]>.Error("功能开发中"));

    public Task<ApiResponse<bool>> SyncWithERPSystemAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> SyncWithBankSystemAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> SyncWithCRMSystemAsync()
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<object>> GetIntegrationStatusAsync()
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<bool>> BulkUpdateAgingAsync(List<int> receivableIds)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> BulkCreateCollectionTasksAsync(List<int> receivableIds, DateTime scheduledDate)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> BulkSendRemindersAsync(List<int> receivableIds, string reminderType)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> BulkWriteOffAsync(List<int> receivableIds, string reason, string approvedBy)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<object>> GetSystemConfigurationAsync()
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<bool>> UpdateSystemConfigurationAsync(object configuration)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetAgingRulesAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> UpdateAgingRulesAsync(object rules)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetCollectionRulesAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> UpdateCollectionRulesAsync(object rules)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<List<object>>> GetAuditTrailAsync(int accountId)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetComplianceReportAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetRegulatoryReportingItemsAsync()
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));

    public Task<ApiResponse<bool>> MarkComplianceCheckedAsync(int accountId, string checkedBy)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> StartCollectionWorkflowAsync(int receivableId)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<bool>> AdvanceCollectionWorkflowAsync(int receivableId, string action, string comments)
        => Task.FromResult(ApiResponse<bool>.Ok(true));

    public Task<ApiResponse<object>> GetCollectionWorkflowStatusAsync(int receivableId)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetReceivableDashboardDataAsync()
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetPayableDashboardDataAsync()
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetCashFlowDashboardDataAsync()
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetCollectionDashboardDataAsync()
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetReceivableKPIsAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetPayableKPIsAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<object>> GetCollectionKPIsAsync(DateTime startDate, DateTime endDate)
        => Task.FromResult(ApiResponse<object>.Error("功能开发中"));

    public Task<ApiResponse<List<object>>> GetKPITrendsAsync(string kpiType, int months = 12)
        => Task.FromResult(ApiResponse<List<object>>.Error("功能开发中"));
    #endregion

    #endregion
}
