using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.Services.Expense;

public interface IExpenseService
{
    // 费用申请管理
    Task<ApiResponse<ExpenseRequest>> CreateExpenseRequestAsync(ExpenseRequest request);
    Task<ApiResponse<ExpenseRequest>> UpdateExpenseRequestAsync(ExpenseRequest request);
    Task<ApiResponse<bool>> DeleteExpenseRequestAsync(int id);
    Task<ApiResponse<ExpenseRequest?>> GetExpenseRequestByIdAsync(int id);
    Task<ApiResponse<ExpenseRequest?>> GetExpenseRequestByNumberAsync(string requestNumber);
    Task<ApiResponse<List<ExpenseRequest>>> GetExpenseRequestsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<ExpenseRequest>>> GetExpenseRequestsByRequesterAsync(int requesterId);
    Task<ApiResponse<List<ExpenseRequest>>> GetExpenseRequestsByTypeAsync(string requestType);
    Task<ApiResponse<List<ExpenseRequest>>> GetExpenseRequestsByStatusAsync(string status);
    Task<ApiResponse<List<ExpenseRequest>>> GetExpenseRequestsByDepartmentAsync(string department);

    // 费用申请状态管理
    Task<ApiResponse<ExpenseRequest>> SubmitExpenseRequestAsync(int id);
    Task<ApiResponse<ExpenseRequest>> WithdrawExpenseRequestAsync(int id, string reason);
    Task<ApiResponse<ExpenseRequest>> CancelExpenseRequestAsync(int id, string reason);

    // 审批流程
    Task<ApiResponse<ExpenseRequest>> ApproveExpenseRequestAsync(int id, int approverId, string comments, decimal? approvedAmount = null);
    Task<ApiResponse<ExpenseRequest>> RejectExpenseRequestAsync(int id, int approverId, string comments);
    Task<ApiResponse<List<ExpenseRequest>>> GetPendingApprovalsAsync(int approverId);
    Task<ApiResponse<List<ExpenseRequest>>> GetExpenseRequestsByApprovalStatusAsync(string approvalStatus);

    // 一级审批
    Task<ApiResponse<ExpenseRequest>> FirstLevelApprovalAsync(int id, int approverId, string status, string comments);

    // 二级审批
    Task<ApiResponse<ExpenseRequest>> SecondLevelApprovalAsync(int id, int approverId, string status, string comments);

    // 财务审批
    Task<ApiResponse<ExpenseRequest>> FinanceApprovalAsync(int id, int approverId, string status, string comments);

    // 费用申请行管理
    Task<ApiResponse<ExpenseRequestLine>> AddExpenseRequestLineAsync(ExpenseRequestLine line);
    Task<ApiResponse<ExpenseRequestLine>> UpdateExpenseRequestLineAsync(ExpenseRequestLine line);
    Task<ApiResponse<bool>> RemoveExpenseRequestLineAsync(int id);
    Task<ApiResponse<List<ExpenseRequestLine>>> GetExpenseRequestLinesAsync(int requestId);
    Task<ApiResponse<bool>> ValidateExpenseRequestTotalAsync(int requestId);

    // 支付管理
    Task<ApiResponse<ExpenseRequest>> InitiatePaymentAsync(int id, string paymentMethod, string paymentAccount);
    Task<ApiResponse<ExpenseRequest>> ConfirmPaymentAsync(int id, string paymentReference, decimal paidAmount);
    Task<ApiResponse<ExpenseRequest>> RecordPartialPaymentAsync(int id, decimal paidAmount, string paymentReference);
    Task<ApiResponse<List<ExpenseRequest>>> GetExpenseRequestsForPaymentAsync();
    Task<ApiResponse<List<ExpenseRequest>>> GetPaidExpenseRequestsAsync(DateTime? startDate = null, DateTime? endDate = null);

    // 预算管理
    Task<ApiResponse<bool>> CheckBudgetAvailabilityAsync(string budgetCode, decimal amount);
    Task<ApiResponse<bool>> ReserveBudgetAsync(string budgetCode, decimal amount, int requestId);
    Task<ApiResponse<bool>> ReleaseBudgetReservationAsync(int requestId);
    Task<ApiResponse<object>> GetBudgetUtilizationAsync(string budgetCode);
    Task<ApiResponse<List<object>>> GetBudgetSummaryAsync(DateTime startDate, DateTime endDate);

    // 费用类别管理
    Task<ApiResponse<List<object>>> GetExpenseCategoriesAsync();
    Task<ApiResponse<object>> CreateExpenseCategoryAsync(object category);
    Task<ApiResponse<object>> UpdateExpenseCategoryAsync(object category);
    Task<ApiResponse<bool>> DeleteExpenseCategoryAsync(int id);

    // 差旅费用专用方法
    Task<ApiResponse<ExpenseRequest>> CreateTravelExpenseRequestAsync(ExpenseRequest request);
    Task<ApiResponse<bool>> ValidateTravelExpenseAsync(ExpenseRequest request);
    Task<ApiResponse<object>> CalculateTravelAllowanceAsync(string destination, int days, string level);
    Task<ApiResponse<List<object>>> GetTravelPolicyAsync();

    // 报表和统计
    Task<ApiResponse<object>> GetExpenseStatsByDepartmentAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetExpenseStatsByTypeAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetExpenseStatsByRequesterAsync(int requesterId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetExpenseTrendsAsync(int months = 12);
    Task<ApiResponse<List<object>>> GetTopExpenseRequestersAsync(int topN = 10);
    Task<ApiResponse<object>> GetBudgetVarianceReportAsync(DateTime startDate, DateTime endDate);

    // 工作流管理
    Task<ApiResponse<bool>> StartExpenseWorkflowAsync(int requestId);
    Task<ApiResponse<bool>> AdvanceExpenseWorkflowAsync(int requestId, string action, string comments);
    Task<ApiResponse<object>> GetExpenseWorkflowStatusAsync(int requestId);
    Task<ApiResponse<List<object>>> GetWorkflowHistoryAsync(int requestId);

    // 通知和提醒
    Task<ApiResponse<bool>> SendExpenseNotificationAsync(int requestId, string notificationType);
    Task<ApiResponse<List<object>>> GetPendingExpenseTasksAsync(int userId);
    Task<ApiResponse<List<object>>> GetOverdueExpenseRequestsAsync();
    Task<ApiResponse<bool>> SendPaymentReminderAsync(int requestId);

    // 文档和附件
    Task<ApiResponse<bool>> UploadExpenseAttachmentAsync(int requestId, byte[] fileData, string fileName);
    Task<ApiResponse<bool>> RemoveExpenseAttachmentAsync(int requestId, string fileName);
    Task<ApiResponse<List<string>>> GetExpenseAttachmentsAsync(int requestId);
    Task<ApiResponse<bool>> ValidateReceiptsAsync(int requestId);

    // 税务处理
    Task<ApiResponse<bool>> CalculateExpenseTaxAsync(int requestId);
    Task<ApiResponse<object>> GetTaxDeductibilityRulesAsync();
    Task<ApiResponse<bool>> ValidateTaxComplianceAsync(int requestId);
    Task<ApiResponse<object>> GenerateTaxReportAsync(DateTime startDate, DateTime endDate);

    // 费用政策
    Task<ApiResponse<object>> GetExpensePolicyAsync(string requestType);
    Task<ApiResponse<bool>> ValidateExpensePolicyComplianceAsync(int requestId);
    Task<ApiResponse<List<object>>> GetPolicyViolationsAsync(int requestId);

    // 集成功能
    Task<ApiResponse<bool>> SyncWithFinanceSystemAsync(int requestId);
    Task<ApiResponse<bool>> ExportToAccountingSystemAsync(List<int> requestIds);
    Task<ApiResponse<object>> ImportExpenseDataAsync(byte[] fileData, string format);

    // 移动应用支持
    Task<ApiResponse<ExpenseRequest>> CreateQuickExpenseAsync(object quickExpenseData);
    Task<ApiResponse<bool>> SubmitExpensePhotoAsync(int requestId, byte[] photoData);
    Task<ApiResponse<object>> GetMobileExpenseFormsAsync();

    // 审计和合规
    Task<ApiResponse<List<object>>> GetExpenseAuditTrailAsync(int requestId);
    Task<ApiResponse<object>> GenerateComplianceReportAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetUnusualExpensePatternsAsync();
    Task<ApiResponse<bool>> FlagSuspiciousExpenseAsync(int requestId, string reason);

    // 批量操作
    Task<ApiResponse<bool>> BulkApproveExpenseRequestsAsync(List<int> requestIds, int approverId, string comments);
    Task<ApiResponse<bool>> BulkRejectExpenseRequestsAsync(List<int> requestIds, int approverId, string comments);
    Task<ApiResponse<bool>> BulkProcessPaymentsAsync(List<int> requestIds);
    Task<ApiResponse<bool>> BulkExportExpenseRequestsAsync(List<int> requestIds, string format);

    // 数据导入导出
    Task<ApiResponse<byte[]>> ExportExpenseRequestsAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "Excel");
    Task<ApiResponse<bool>> ImportExpenseRequestsAsync(byte[] fileData, string fileName);
    Task<ApiResponse<object>> GetImportTemplateAsync();
    Task<ApiResponse<object>> ValidateImportDataAsync(byte[] fileData);

    // 模板管理
    Task<ApiResponse<List<object>>> GetExpenseTemplatesAsync();
    Task<ApiResponse<object>> CreateExpenseTemplateAsync(object template);
    Task<ApiResponse<bool>> UpdateExpenseTemplateAsync(object template);
    Task<ApiResponse<bool>> DeleteExpenseTemplateAsync(int id);
    Task<ApiResponse<ExpenseRequest>> CreateExpenseFromTemplateAsync(int templateId, object parameters);
}