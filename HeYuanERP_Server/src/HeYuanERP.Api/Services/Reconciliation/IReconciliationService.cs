using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.Services.Reconciliation;

public interface IReconciliationService
{
    // 对账记录管理
    Task<ApiResponse<ReconciliationRecord>> CreateReconciliationRecordAsync(ReconciliationRecord record);
    Task<ApiResponse<ReconciliationRecord>> UpdateReconciliationRecordAsync(ReconciliationRecord record);
    Task<ApiResponse<bool>> DeleteReconciliationRecordAsync(int id);
    Task<ApiResponse<ReconciliationRecord?>> GetReconciliationRecordByIdAsync(int id);
    Task<ApiResponse<List<ReconciliationRecord>>> GetReconciliationRecordsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<ReconciliationRecord>>> GetReconciliationRecordsByTypeAsync(string reconciliationType);
    Task<ApiResponse<List<ReconciliationRecord>>> GetReconciliationRecordsByStatusAsync(string status);
    Task<ApiResponse<ReconciliationRecord>> CompleteReconciliationAsync(int id);

    // 对账项目管理
    Task<ApiResponse<ReconciliationItem>> CreateReconciliationItemAsync(ReconciliationItem item);
    Task<ApiResponse<ReconciliationItem>> UpdateReconciliationItemAsync(ReconciliationItem item);
    Task<ApiResponse<bool>> DeleteReconciliationItemAsync(int id);
    Task<ApiResponse<ReconciliationItem?>> GetReconciliationItemByIdAsync(int id);
    Task<ApiResponse<List<ReconciliationItem>>> GetReconciliationItemsByRecordAsync(int recordId);
    Task<ApiResponse<List<ReconciliationItem>>> GetUnmatchedReconciliationItemsAsync(int recordId);
    Task<ApiResponse<ReconciliationItem>> MatchReconciliationItemAsync(int id, string matchType);
    Task<ApiResponse<bool>> BulkMatchReconciliationItemsAsync(List<int> itemIds, string matchType);

    // 对账差异管理
    Task<ApiResponse<ReconciliationDifference>> CreateReconciliationDifferenceAsync(ReconciliationDifference difference);
    Task<ApiResponse<ReconciliationDifference>> UpdateReconciliationDifferenceAsync(ReconciliationDifference difference);
    Task<ApiResponse<bool>> DeleteReconciliationDifferenceAsync(string id);
    Task<ApiResponse<ReconciliationDifference?>> GetReconciliationDifferenceByIdAsync(string id);
    Task<ApiResponse<List<ReconciliationDifference>>> GetReconciliationDifferencesByRecordAsync(int recordId);
    Task<ApiResponse<List<ReconciliationDifference>>> GetReconciliationDifferencesByStatusAsync(ReconciliationStatus status);
    Task<ApiResponse<ReconciliationDifference>> ResolveReconciliationDifferenceAsync(string id, string resolution, string handledBy);
    Task<ApiResponse<List<ReconciliationDifference>>> GetUnresolvedDifferencesAsync();

    // 调整分录管理
    Task<ApiResponse<AdjustmentEntry>> CreateAdjustmentEntryAsync(AdjustmentEntry entry);
    Task<ApiResponse<AdjustmentEntry>> UpdateAdjustmentEntryAsync(AdjustmentEntry entry);
    Task<ApiResponse<bool>> DeleteAdjustmentEntryAsync(int id);
    Task<ApiResponse<AdjustmentEntry?>> GetAdjustmentEntryByIdAsync(int id);
    Task<ApiResponse<List<AdjustmentEntry>>> GetAdjustmentEntriesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<AdjustmentEntry>>> GetAdjustmentEntriesByTypeAsync(string adjustmentType);
    Task<ApiResponse<List<AdjustmentEntry>>> GetAdjustmentEntriesByStatusAsync(string status);
    Task<ApiResponse<AdjustmentEntry>> SubmitAdjustmentEntryAsync(int id);
    Task<ApiResponse<AdjustmentEntry>> ApproveAdjustmentEntryAsync(int id, string comments, int approverId);
    Task<ApiResponse<AdjustmentEntry>> RejectAdjustmentEntryAsync(int id, string comments, int approverId);
    Task<ApiResponse<AdjustmentEntry>> PostAdjustmentEntryAsync(int id, int postedByUserId);

    // 调整分录行管理
    Task<ApiResponse<AdjustmentEntryLine>> AddAdjustmentEntryLineAsync(AdjustmentEntryLine line);
    Task<ApiResponse<AdjustmentEntryLine>> UpdateAdjustmentEntryLineAsync(AdjustmentEntryLine line);
    Task<ApiResponse<bool>> RemoveAdjustmentEntryLineAsync(int id);
    Task<ApiResponse<List<AdjustmentEntryLine>>> GetAdjustmentEntryLinesAsync(int entryId);
    Task<ApiResponse<bool>> ValidateAdjustmentEntryBalanceAsync(int entryId);

    // 凭证管理
    Task<ApiResponse<JournalEntry>> CreateJournalEntryAsync(JournalEntry entry);
    Task<ApiResponse<JournalEntry>> UpdateJournalEntryAsync(JournalEntry entry);
    Task<ApiResponse<bool>> DeleteJournalEntryAsync(int id);
    Task<ApiResponse<JournalEntry?>> GetJournalEntryByIdAsync(int id);
    Task<ApiResponse<JournalEntry?>> GetJournalEntryByNumberAsync(string journalNumber);
    Task<ApiResponse<List<JournalEntry>>> GetJournalEntriesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<JournalEntry>>> GetJournalEntriesByTypeAsync(string journalType);
    Task<ApiResponse<List<JournalEntry>>> GetJournalEntriesByStatusAsync(string status);
    Task<ApiResponse<JournalEntry>> PostJournalEntryAsync(int id, int postedByUserId);
    Task<ApiResponse<JournalEntry>> ReverseJournalEntryAsync(int id, string reason, int reversedByUserId);

    // 凭证行管理
    Task<ApiResponse<JournalEntryLine>> AddJournalEntryLineAsync(JournalEntryLine line);
    Task<ApiResponse<JournalEntryLine>> UpdateJournalEntryLineAsync(JournalEntryLine line);
    Task<ApiResponse<bool>> RemoveJournalEntryLineAsync(int id);
    Task<ApiResponse<List<JournalEntryLine>>> GetJournalEntryLinesAsync(int entryId);
    Task<ApiResponse<bool>> ValidateJournalEntryBalanceAsync(int entryId);

    // 自动对账
    Task<ApiResponse<ReconciliationRecord>> PerformAutoReconciliationAsync(string reconciliationType, DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<ReconciliationItem>>> AutoMatchReconciliationItemsAsync(int recordId, string matchingCriteria);
    Task<ApiResponse<object>> GetAutoReconciliationRulesAsync();
    Task<ApiResponse<bool>> UpdateAutoReconciliationRulesAsync(object rules);

    // 从调整分录生成凭证
    Task<ApiResponse<JournalEntry>> GenerateJournalEntryFromAdjustmentAsync(int adjustmentEntryId);
    Task<ApiResponse<List<JournalEntry>>> GenerateJournalEntriesFromReconciliationAsync(int reconciliationRecordId);

    // 闭环处理
    Task<ApiResponse<bool>> CompleteReconciliationLoopAsync(int reconciliationRecordId);
    Task<ApiResponse<object>> GetReconciliationLoopStatusAsync(int reconciliationRecordId);

    // 数据导入导出
    Task<ApiResponse<ReconciliationRecord>> ImportExternalDataAsync(byte[] fileData, string fileName, string reconciliationType);
    Task<ApiResponse<byte[]>> ExportReconciliationReportAsync(int reconciliationRecordId);
    Task<ApiResponse<byte[]>> ExportUnreconciledItemsAsync(int reconciliationRecordId);

    // 统计和分析
    Task<ApiResponse<object>> GetReconciliationStatsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetReconciliationDashboardDataAsync();
    Task<ApiResponse<List<object>>> GetReconciliationTrendsAsync(int months = 12);
    Task<ApiResponse<object>> GetDifferenceAnalysisAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetTopDifferenceReasonsAsync(int topN = 10);

    // 审计和合规
    Task<ApiResponse<List<object>>> GetReconciliationAuditTrailAsync(int recordId);
    Task<ApiResponse<object>> GetComplianceReportAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetRegulatoryReportingItemsAsync();

    // 工作流管理
    Task<ApiResponse<bool>> StartReconciliationWorkflowAsync(int recordId);
    Task<ApiResponse<bool>> AdvanceReconciliationWorkflowAsync(int recordId, string action, string comments);
    Task<ApiResponse<object>> GetReconciliationWorkflowStatusAsync(int recordId);

    // 通知和提醒
    Task<ApiResponse<bool>> SendReconciliationNotificationAsync(int recordId, string notificationType);
    Task<ApiResponse<List<object>>> GetPendingReconciliationTasksAsync(int userId);
    Task<ApiResponse<List<object>>> GetOverdueReconciliationItemsAsync();

    // 模板和配置
    Task<ApiResponse<List<object>>> GetReconciliationTemplatesAsync();
    Task<ApiResponse<bool>> SaveReconciliationTemplateAsync(object template);
    Task<ApiResponse<object>> GetReconciliationConfigurationAsync();
    Task<ApiResponse<bool>> UpdateReconciliationConfigurationAsync(object configuration);
}