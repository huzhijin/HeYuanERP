using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.Services.Finance;

public interface IAccountsReceivablePayableService
{
    // 应收账款管理
    Task<ApiResponse<AccountReceivable>> CreateAccountReceivableAsync(AccountReceivable receivable);
    Task<ApiResponse<AccountReceivable>> UpdateAccountReceivableAsync(AccountReceivable receivable);
    Task<ApiResponse<bool>> DeleteAccountReceivableAsync(int id);
    Task<ApiResponse<AccountReceivable?>> GetAccountReceivableByIdAsync(int id);
    Task<ApiResponse<AccountReceivable?>> GetAccountReceivableByDocumentAsync(string documentNumber, string documentType);
    Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesByCustomerAsync(int customerId);
    Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesByStatusAsync(string status);
    Task<ApiResponse<List<AccountReceivable>>> GetOverdueAccountReceivablesAsync();
    Task<ApiResponse<List<AccountReceivable>>> GetAccountReceivablesByAgingAsync(string agingBucket);

    // 应付账款管理
    Task<ApiResponse<AccountPayable>> CreateAccountPayableAsync(AccountPayable payable);
    Task<ApiResponse<AccountPayable>> UpdateAccountPayableAsync(AccountPayable payable);
    Task<ApiResponse<bool>> DeleteAccountPayableAsync(int id);
    Task<ApiResponse<AccountPayable?>> GetAccountPayableByIdAsync(int id);
    Task<ApiResponse<AccountPayable?>> GetAccountPayableByDocumentAsync(string documentNumber, string documentType);
    Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesBySupplierAsync(int supplierId);
    Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesByStatusAsync(string status);
    Task<ApiResponse<List<AccountPayable>>> GetOverdueAccountPayablesAsync();
    Task<ApiResponse<List<AccountPayable>>> GetAccountPayablesByAgingAsync(string agingBucket);

    // 账龄分析
    Task<ApiResponse<List<object>>> GetCustomerAgingAnalysisAsync(DateTime asOfDate);
    Task<ApiResponse<List<object>>> GetSupplierAgingAnalysisAsync(DateTime asOfDate);
    Task<ApiResponse<object>> GetAgingAnalysisSummaryAsync(DateTime asOfDate);
    Task<ApiResponse<List<object>>> GetAgingTrendsAsync(int months = 12);
    Task<ApiResponse<object>> GetCustomerAgingDetailAsync(int customerId, DateTime asOfDate);
    Task<ApiResponse<object>> GetSupplierAgingDetailAsync(int supplierId, DateTime asOfDate);

    // 催收管理
    Task<ApiResponse<CollectionRecord>> CreateCollectionRecordAsync(CollectionRecord record);
    Task<ApiResponse<CollectionRecord>> UpdateCollectionRecordAsync(CollectionRecord record);
    Task<ApiResponse<bool>> DeleteCollectionRecordAsync(int id);
    Task<ApiResponse<CollectionRecord?>> GetCollectionRecordByIdAsync(int id);
    Task<ApiResponse<List<CollectionRecord>>> GetCollectionRecordsByReceivableAsync(int receivableId);
    Task<ApiResponse<List<CollectionRecord>>> GetCollectionRecordsByCollectorAsync(int collectorId);
    Task<ApiResponse<List<CollectionRecord>>> GetCollectionRecordsByDateAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetCollectionPlanAsync(int collectorId);
    Task<ApiResponse<List<object>>> GetCollectionEfficiencyAsync(int collectorId, DateTime startDate, DateTime endDate);

    // 催收计划和提醒
    Task<ApiResponse<List<object>>> GetPendingCollectionTasksAsync(int collectorId);
    Task<ApiResponse<List<object>>> GetOverdueCollectionTasksAsync();
    Task<ApiResponse<bool>> SendCollectionReminderAsync(int receivableId, string reminderType);
    Task<ApiResponse<bool>> ScheduleCollectionTaskAsync(int receivableId, DateTime scheduledDate, string taskType);
    Task<ApiResponse<List<object>>> GetCollectionCalendarAsync(int collectorId, DateTime startDate, DateTime endDate);

    // 收款核销管理
    Task<ApiResponse<PaymentApplication>> CreatePaymentApplicationAsync(PaymentApplication application);
    Task<ApiResponse<PaymentApplication>> UpdatePaymentApplicationAsync(PaymentApplication application);
    Task<ApiResponse<bool>> DeletePaymentApplicationAsync(int id);
    Task<ApiResponse<List<PaymentApplication>>> GetPaymentApplicationsByReceivableAsync(int receivableId);
    Task<ApiResponse<List<PaymentApplication>>> GetPaymentApplicationsByPaymentAsync(string paymentNumber);
    Task<ApiResponse<bool>> AutoMatchPaymentsAsync(int customerId);
    Task<ApiResponse<List<object>>> GetUnmatchedPaymentsAsync();

    // 付款计划管理
    Task<ApiResponse<PaymentSchedule>> CreatePaymentScheduleAsync(PaymentSchedule schedule);
    Task<ApiResponse<PaymentSchedule>> UpdatePaymentScheduleAsync(PaymentSchedule schedule);
    Task<ApiResponse<bool>> DeletePaymentScheduleAsync(int id);
    Task<ApiResponse<List<PaymentSchedule>>> GetPaymentSchedulesByPayableAsync(int payableId);
    Task<ApiResponse<List<PaymentSchedule>>> GetPaymentSchedulesByDateAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetPaymentPlanAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<bool>> ExecuteScheduledPaymentAsync(int scheduleId, decimal actualAmount);

    // 坏账管理
    Task<ApiResponse<AccountReceivable>> WriteOffBadDebtAsync(int receivableId, decimal writeOffAmount, string reason, string approvedBy);
    Task<ApiResponse<AccountReceivable>> RecoverWrittenOffDebtAsync(int receivableId, decimal recoveryAmount);
    Task<ApiResponse<List<AccountReceivable>>> GetWrittenOffAccountsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<object>>> GetBadDebtAnalysisAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<bool>> MarkHighRiskAccountAsync(int receivableId, string reason);
    Task<ApiResponse<List<object>>> GetHighRiskAccountsAsync();

    // 信用管理
    Task<ApiResponse<object>> GetCustomerCreditAnalysisAsync(int customerId);
    Task<ApiResponse<bool>> UpdateCustomerCreditLimitAsync(int customerId, decimal creditLimit);
    Task<ApiResponse<bool>> ValidateCreditLimitAsync(int customerId, decimal additionalAmount);
    Task<ApiResponse<List<object>>> GetCreditUtilizationAsync();
    Task<ApiResponse<bool>> SetCustomerPaymentTermsAsync(int customerId, string paymentTerms);

    // 资金流预测
    Task<ApiResponse<object>> GetCashFlowForecastAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetReceivableForecastAsync(DateTime forecastDate);
    Task<ApiResponse<object>> GetPayableForecastAsync(DateTime forecastDate);
    Task<ApiResponse<List<object>>> GetCashFlowTrendsAsync(int months = 12);
    Task<ApiResponse<object>> GetLiquidityAnalysisAsync();

    // 自动化处理
    Task<ApiResponse<bool>> AutoGenerateReceivablesFromInvoicesAsync();
    Task<ApiResponse<bool>> AutoGeneratePayablesFromPurchasesAsync();
    Task<ApiResponse<bool>> AutoUpdateAgingBucketsAsync();
    Task<ApiResponse<bool>> AutoCalculateRiskLevelsAsync();
    Task<ApiResponse<bool>> ProcessScheduledRemindersAsync();

    // 报表和统计
    Task<ApiResponse<object>> GetReceivableSummaryAsync(DateTime? asOfDate = null);
    Task<ApiResponse<object>> GetPayableSummaryAsync(DateTime? asOfDate = null);
    Task<ApiResponse<object>> GetCollectionPerformanceAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetPaymentPerformanceAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetTopDebtorsAsync(int topN = 10);
    Task<ApiResponse<List<object>>> GetTopCreditorsAsync(int topN = 10);
    Task<ApiResponse<object>> GetTurnoverAnalysisAsync(DateTime startDate, DateTime endDate);

    // DSO/DPO 分析
    Task<ApiResponse<object>> GetDSOAnalysisAsync(DateTime startDate, DateTime endDate); // Days Sales Outstanding
    Task<ApiResponse<object>> GetDPOAnalysisAsync(DateTime startDate, DateTime endDate); // Days Payable Outstanding
    Task<ApiResponse<List<object>>> GetDSOTrendsAsync(int months = 12);
    Task<ApiResponse<List<object>>> GetDPOTrendsAsync(int months = 12);

    // 对账和调节
    Task<ApiResponse<object>> ReconcileCustomerAccountAsync(int customerId, DateTime asOfDate);
    Task<ApiResponse<object>> ReconcileSupplierAccountAsync(int supplierId, DateTime asOfDate);
    Task<ApiResponse<List<object>>> GetReconciliationDifferencesAsync();
    Task<ApiResponse<bool>> ProcessReconciliationAdjustmentAsync(int accountId, decimal adjustmentAmount, string reason);

    // 客户对账单
    Task<ApiResponse<object>> GenerateCustomerStatementAsync(int customerId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<bool>> SendCustomerStatementAsync(int customerId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetCustomerStatementHistoryAsync(int customerId);

    // 供应商对账单
    Task<ApiResponse<object>> GenerateSupplierStatementAsync(int supplierId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<bool>> SendSupplierStatementAsync(int supplierId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetSupplierStatementHistoryAsync(int supplierId);

    // 审批流程
    Task<ApiResponse<bool>> SubmitWriteOffRequestAsync(int receivableId, decimal amount, string reason);
    Task<ApiResponse<bool>> ApproveWriteOffRequestAsync(int requestId, string approvedBy, string comments);
    Task<ApiResponse<bool>> RejectWriteOffRequestAsync(int requestId, string rejectedBy, string reason);
    Task<ApiResponse<List<object>>> GetPendingWriteOffRequestsAsync();

    // 通知和提醒
    Task<ApiResponse<bool>> SendPaymentReminderAsync(int receivableId, string reminderLevel);
    Task<ApiResponse<bool>> SendPaymentNotificationAsync(int payableId, string notificationType);
    Task<ApiResponse<List<object>>> GetPendingRemindersAsync();
    Task<ApiResponse<bool>> MarkReminderSentAsync(int reminderId);

    // 数据导入导出
    Task<ApiResponse<bool>> ImportReceivablesAsync(byte[] fileData, string fileName);
    Task<ApiResponse<bool>> ImportPayablesAsync(byte[] fileData, string fileName);
    Task<ApiResponse<byte[]>> ExportReceivablesAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "Excel");
    Task<ApiResponse<byte[]>> ExportPayablesAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "Excel");
    Task<ApiResponse<byte[]>> ExportAgingReportAsync(DateTime asOfDate, string format = "Excel");
    Task<ApiResponse<byte[]>> ExportCollectionReportAsync(DateTime startDate, DateTime endDate, string format = "Excel");

    // 集成接口
    Task<ApiResponse<bool>> SyncWithERPSystemAsync();
    Task<ApiResponse<bool>> SyncWithBankSystemAsync();
    Task<ApiResponse<bool>> SyncWithCRMSystemAsync();
    Task<ApiResponse<object>> GetIntegrationStatusAsync();

    // 批量操作
    Task<ApiResponse<bool>> BulkUpdateAgingAsync(List<int> receivableIds);
    Task<ApiResponse<bool>> BulkCreateCollectionTasksAsync(List<int> receivableIds, DateTime scheduledDate);
    Task<ApiResponse<bool>> BulkSendRemindersAsync(List<int> receivableIds, string reminderType);
    Task<ApiResponse<bool>> BulkWriteOffAsync(List<int> receivableIds, string reason, string approvedBy);

    // 配置管理
    Task<ApiResponse<object>> GetSystemConfigurationAsync();
    Task<ApiResponse<bool>> UpdateSystemConfigurationAsync(object configuration);
    Task<ApiResponse<List<object>>> GetAgingRulesAsync();
    Task<ApiResponse<bool>> UpdateAgingRulesAsync(object rules);
    Task<ApiResponse<List<object>>> GetCollectionRulesAsync();
    Task<ApiResponse<bool>> UpdateCollectionRulesAsync(object rules);

    // 审计和合规
    Task<ApiResponse<List<object>>> GetAuditTrailAsync(int accountId);
    Task<ApiResponse<object>> GetComplianceReportAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetRegulatoryReportingItemsAsync();
    Task<ApiResponse<bool>> MarkComplianceCheckedAsync(int accountId, string checkedBy);

    // 工作流管理
    Task<ApiResponse<bool>> StartCollectionWorkflowAsync(int receivableId);
    Task<ApiResponse<bool>> AdvanceCollectionWorkflowAsync(int receivableId, string action, string comments);
    Task<ApiResponse<object>> GetCollectionWorkflowStatusAsync(int receivableId);

    // 仪表板数据
    Task<ApiResponse<object>> GetReceivableDashboardDataAsync();
    Task<ApiResponse<object>> GetPayableDashboardDataAsync();
    Task<ApiResponse<object>> GetCashFlowDashboardDataAsync();
    Task<ApiResponse<object>> GetCollectionDashboardDataAsync();

    // KPI 指标
    Task<ApiResponse<object>> GetReceivableKPIsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetPayableKPIsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetCollectionKPIsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetKPITrendsAsync(string kpiType, int months = 12);
}