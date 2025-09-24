using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Services.Finance;
using HeYuanERP.Api.Services.Authorization;
using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsReceivablePayableController : ControllerBase
{
    private readonly IAccountsReceivablePayableService _arpService;

    public AccountsReceivablePayableController(IAccountsReceivablePayableService arpService)
    {
        _arpService = arpService;
    }

    #region 应收账款管理

    [HttpPost("receivables")]
    [RequirePermission("Finance.Receivable.Create")]
    public async Task<IActionResult> CreateAccountReceivable([FromBody] AccountReceivable receivable)
    {
        var result = await _arpService.CreateAccountReceivableAsync(receivable);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("receivables/{id}")]
    [RequirePermission("Finance.Receivable.Update")]
    public async Task<IActionResult> UpdateAccountReceivable(int id, [FromBody] AccountReceivable receivable)
    {
        if (id != receivable.Id)
            return BadRequest("ID不匹配");

        var result = await _arpService.UpdateAccountReceivableAsync(receivable);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("receivables/{id}")]
    [RequirePermission("Finance.Receivable.Delete")]
    public async Task<IActionResult> DeleteAccountReceivable(int id)
    {
        var result = await _arpService.DeleteAccountReceivableAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("receivables/{id}")]
    [RequirePermission("Finance.Receivable.View")]
    public async Task<IActionResult> GetAccountReceivableById(int id)
    {
        var result = await _arpService.GetAccountReceivableByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("receivables/document/{documentNumber}/{documentType}")]
    [RequirePermission("Finance.Receivable.View")]
    public async Task<IActionResult> GetAccountReceivableByDocument(string documentNumber, string documentType)
    {
        var result = await _arpService.GetAccountReceivableByDocumentAsync(documentNumber, documentType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("receivables")]
    [RequirePermission("Finance.Receivable.View")]
    public async Task<IActionResult> GetAccountReceivables([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _arpService.GetAccountReceivablesAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("receivables/customer/{customerId}")]
    [RequirePermission("Finance.Receivable.View")]
    public async Task<IActionResult> GetAccountReceivablesByCustomer(int customerId)
    {
        var result = await _arpService.GetAccountReceivablesByCustomerAsync(customerId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("receivables/status/{status}")]
    [RequirePermission("Finance.Receivable.View")]
    public async Task<IActionResult> GetAccountReceivablesByStatus(string status)
    {
        var result = await _arpService.GetAccountReceivablesByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("receivables/overdue")]
    [RequirePermission("Finance.Receivable.View")]
    public async Task<IActionResult> GetOverdueAccountReceivables()
    {
        var result = await _arpService.GetOverdueAccountReceivablesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("receivables/aging/{agingBucket}")]
    [RequirePermission("Finance.Receivable.View")]
    public async Task<IActionResult> GetAccountReceivablesByAging(string agingBucket)
    {
        var result = await _arpService.GetAccountReceivablesByAgingAsync(agingBucket);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 应付账款管理

    [HttpPost("payables")]
    [RequirePermission("Finance.Payable.Create")]
    public async Task<IActionResult> CreateAccountPayable([FromBody] AccountPayable payable)
    {
        var result = await _arpService.CreateAccountPayableAsync(payable);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("payables/{id}")]
    [RequirePermission("Finance.Payable.Update")]
    public async Task<IActionResult> UpdateAccountPayable(int id, [FromBody] AccountPayable payable)
    {
        if (id != payable.Id)
            return BadRequest("ID不匹配");

        var result = await _arpService.UpdateAccountPayableAsync(payable);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("payables/{id}")]
    [RequirePermission("Finance.Payable.Delete")]
    public async Task<IActionResult> DeleteAccountPayable(int id)
    {
        var result = await _arpService.DeleteAccountPayableAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payables/{id}")]
    [RequirePermission("Finance.Payable.View")]
    public async Task<IActionResult> GetAccountPayableById(int id)
    {
        var result = await _arpService.GetAccountPayableByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payables/document/{documentNumber}/{documentType}")]
    [RequirePermission("Finance.Payable.View")]
    public async Task<IActionResult> GetAccountPayableByDocument(string documentNumber, string documentType)
    {
        var result = await _arpService.GetAccountPayableByDocumentAsync(documentNumber, documentType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payables")]
    [RequirePermission("Finance.Payable.View")]
    public async Task<IActionResult> GetAccountPayables([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _arpService.GetAccountPayablesAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payables/supplier/{supplierId}")]
    [RequirePermission("Finance.Payable.View")]
    public async Task<IActionResult> GetAccountPayablesBySupplier(int supplierId)
    {
        var result = await _arpService.GetAccountPayablesBySupplierAsync(supplierId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payables/status/{status}")]
    [RequirePermission("Finance.Payable.View")]
    public async Task<IActionResult> GetAccountPayablesByStatus(string status)
    {
        var result = await _arpService.GetAccountPayablesByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payables/overdue")]
    [RequirePermission("Finance.Payable.View")]
    public async Task<IActionResult> GetOverdueAccountPayables()
    {
        var result = await _arpService.GetOverdueAccountPayablesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payables/aging/{agingBucket}")]
    [RequirePermission("Finance.Payable.View")]
    public async Task<IActionResult> GetAccountPayablesByAging(string agingBucket)
    {
        var result = await _arpService.GetAccountPayablesByAgingAsync(agingBucket);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 账龄分析

    [HttpGet("aging/customers")]
    [RequirePermission("Finance.Aging.View")]
    public async Task<IActionResult> GetCustomerAgingAnalysis([FromQuery] DateTime asOfDate)
    {
        var result = await _arpService.GetCustomerAgingAnalysisAsync(asOfDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("aging/suppliers")]
    [RequirePermission("Finance.Aging.View")]
    public async Task<IActionResult> GetSupplierAgingAnalysis([FromQuery] DateTime asOfDate)
    {
        var result = await _arpService.GetSupplierAgingAnalysisAsync(asOfDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("aging/summary")]
    [RequirePermission("Finance.Aging.View")]
    public async Task<IActionResult> GetAgingAnalysisSummary([FromQuery] DateTime asOfDate)
    {
        var result = await _arpService.GetAgingAnalysisSummaryAsync(asOfDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("aging/trends")]
    [RequirePermission("Finance.Aging.View")]
    public async Task<IActionResult> GetAgingTrends([FromQuery] int months = 12)
    {
        var result = await _arpService.GetAgingTrendsAsync(months);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("aging/customer-detail/{customerId}")]
    [RequirePermission("Finance.Aging.View")]
    public async Task<IActionResult> GetCustomerAgingDetail(int customerId, [FromQuery] DateTime asOfDate)
    {
        var result = await _arpService.GetCustomerAgingDetailAsync(customerId, asOfDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("aging/supplier-detail/{supplierId}")]
    [RequirePermission("Finance.Aging.View")]
    public async Task<IActionResult> GetSupplierAgingDetail(int supplierId, [FromQuery] DateTime asOfDate)
    {
        var result = await _arpService.GetSupplierAgingDetailAsync(supplierId, asOfDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 催收管理

    [HttpPost("collection-records")]
    [RequirePermission("Finance.Collection.Create")]
    public async Task<IActionResult> CreateCollectionRecord([FromBody] CollectionRecord record)
    {
        var result = await _arpService.CreateCollectionRecordAsync(record);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("collection-records/{id}")]
    [RequirePermission("Finance.Collection.Update")]
    public async Task<IActionResult> UpdateCollectionRecord(int id, [FromBody] CollectionRecord record)
    {
        if (id != record.Id)
            return BadRequest("ID不匹配");

        var result = await _arpService.UpdateCollectionRecordAsync(record);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("collection-records/{id}")]
    [RequirePermission("Finance.Collection.Delete")]
    public async Task<IActionResult> DeleteCollectionRecord(int id)
    {
        var result = await _arpService.DeleteCollectionRecordAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection-records/{id}")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetCollectionRecordById(int id)
    {
        var result = await _arpService.GetCollectionRecordByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection-records/receivable/{receivableId}")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetCollectionRecordsByReceivable(int receivableId)
    {
        var result = await _arpService.GetCollectionRecordsByReceivableAsync(receivableId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection-records/collector/{collectorId}")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetCollectionRecordsByCollector(int collectorId)
    {
        var result = await _arpService.GetCollectionRecordsByCollectorAsync(collectorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection-records/date-range")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetCollectionRecordsByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetCollectionRecordsByDateAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection/plan/{collectorId}")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetCollectionPlan(int collectorId)
    {
        var result = await _arpService.GetCollectionPlanAsync(collectorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection/efficiency/{collectorId}")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetCollectionEfficiency(int collectorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetCollectionEfficiencyAsync(collectorId, startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection/pending-tasks/{collectorId}")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetPendingCollectionTasks(int collectorId)
    {
        var result = await _arpService.GetPendingCollectionTasksAsync(collectorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection/overdue-tasks")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetOverdueCollectionTasks()
    {
        var result = await _arpService.GetOverdueCollectionTasksAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("collection/send-reminder/{receivableId}")]
    [RequirePermission("Finance.Collection.Reminder")]
    public async Task<IActionResult> SendCollectionReminder(int receivableId, [FromBody] ReminderRequest request)
    {
        var result = await _arpService.SendCollectionReminderAsync(receivableId, request.ReminderType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("collection/schedule-task/{receivableId}")]
    [RequirePermission("Finance.Collection.Schedule")]
    public async Task<IActionResult> ScheduleCollectionTask(int receivableId, [FromBody] ScheduleTaskRequest request)
    {
        var result = await _arpService.ScheduleCollectionTaskAsync(receivableId, request.ScheduledDate, request.TaskType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("collection/calendar/{collectorId}")]
    [RequirePermission("Finance.Collection.View")]
    public async Task<IActionResult> GetCollectionCalendar(int collectorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetCollectionCalendarAsync(collectorId, startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 收款核销管理

    [HttpPost("payment-applications")]
    [RequirePermission("Finance.PaymentApplication.Create")]
    public async Task<IActionResult> CreatePaymentApplication([FromBody] PaymentApplication application)
    {
        var result = await _arpService.CreatePaymentApplicationAsync(application);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("payment-applications/{id}")]
    [RequirePermission("Finance.PaymentApplication.Update")]
    public async Task<IActionResult> UpdatePaymentApplication(int id, [FromBody] PaymentApplication application)
    {
        if (id != application.Id)
            return BadRequest("ID不匹配");

        var result = await _arpService.UpdatePaymentApplicationAsync(application);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("payment-applications/{id}")]
    [RequirePermission("Finance.PaymentApplication.Delete")]
    public async Task<IActionResult> DeletePaymentApplication(int id)
    {
        var result = await _arpService.DeletePaymentApplicationAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payment-applications/receivable/{receivableId}")]
    [RequirePermission("Finance.PaymentApplication.View")]
    public async Task<IActionResult> GetPaymentApplicationsByReceivable(int receivableId)
    {
        var result = await _arpService.GetPaymentApplicationsByReceivableAsync(receivableId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payment-applications/payment/{paymentNumber}")]
    [RequirePermission("Finance.PaymentApplication.View")]
    public async Task<IActionResult> GetPaymentApplicationsByPayment(string paymentNumber)
    {
        var result = await _arpService.GetPaymentApplicationsByPaymentAsync(paymentNumber);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("payment-applications/auto-match/{customerId}")]
    [RequirePermission("Finance.PaymentApplication.Match")]
    public async Task<IActionResult> AutoMatchPayments(int customerId)
    {
        var result = await _arpService.AutoMatchPaymentsAsync(customerId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payment-applications/unmatched")]
    [RequirePermission("Finance.PaymentApplication.View")]
    public async Task<IActionResult> GetUnmatchedPayments()
    {
        var result = await _arpService.GetUnmatchedPaymentsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 付款计划管理

    [HttpPost("payment-schedules")]
    [RequirePermission("Finance.PaymentSchedule.Create")]
    public async Task<IActionResult> CreatePaymentSchedule([FromBody] PaymentSchedule schedule)
    {
        var result = await _arpService.CreatePaymentScheduleAsync(schedule);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("payment-schedules/{id}")]
    [RequirePermission("Finance.PaymentSchedule.Update")]
    public async Task<IActionResult> UpdatePaymentSchedule(int id, [FromBody] PaymentSchedule schedule)
    {
        if (id != schedule.Id)
            return BadRequest("ID不匹配");

        var result = await _arpService.UpdatePaymentScheduleAsync(schedule);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("payment-schedules/{id}")]
    [RequirePermission("Finance.PaymentSchedule.Delete")]
    public async Task<IActionResult> DeletePaymentSchedule(int id)
    {
        var result = await _arpService.DeletePaymentScheduleAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payment-schedules/payable/{payableId}")]
    [RequirePermission("Finance.PaymentSchedule.View")]
    public async Task<IActionResult> GetPaymentSchedulesByPayable(int payableId)
    {
        var result = await _arpService.GetPaymentSchedulesByPayableAsync(payableId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payment-schedules/date-range")]
    [RequirePermission("Finance.PaymentSchedule.View")]
    public async Task<IActionResult> GetPaymentSchedulesByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetPaymentSchedulesByDateAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payment-schedules/plan")]
    [RequirePermission("Finance.PaymentSchedule.View")]
    public async Task<IActionResult> GetPaymentPlan([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetPaymentPlanAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("payment-schedules/{scheduleId}/execute")]
    [RequirePermission("Finance.PaymentSchedule.Execute")]
    public async Task<IActionResult> ExecuteScheduledPayment(int scheduleId, [FromBody] ExecutePaymentRequest request)
    {
        var result = await _arpService.ExecuteScheduledPaymentAsync(scheduleId, request.ActualAmount);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 坏账管理

    [HttpPost("bad-debt/write-off/{receivableId}")]
    [RequirePermission("Finance.BadDebt.WriteOff")]
    public async Task<IActionResult> WriteOffBadDebt(int receivableId, [FromBody] WriteOffRequest request)
    {
        var result = await _arpService.WriteOffBadDebtAsync(receivableId, request.WriteOffAmount, request.Reason, request.ApprovedBy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("bad-debt/recover/{receivableId}")]
    [RequirePermission("Finance.BadDebt.Recover")]
    public async Task<IActionResult> RecoverWrittenOffDebt(int receivableId, [FromBody] RecoverDebtRequest request)
    {
        var result = await _arpService.RecoverWrittenOffDebtAsync(receivableId, request.RecoveryAmount);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("bad-debt/written-off")]
    [RequirePermission("Finance.BadDebt.View")]
    public async Task<IActionResult> GetWrittenOffAccounts([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _arpService.GetWrittenOffAccountsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("bad-debt/analysis")]
    [RequirePermission("Finance.BadDebt.View")]
    public async Task<IActionResult> GetBadDebtAnalysis([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetBadDebtAnalysisAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("bad-debt/mark-high-risk/{receivableId}")]
    [RequirePermission("Finance.BadDebt.MarkRisk")]
    public async Task<IActionResult> MarkHighRiskAccount(int receivableId, [FromBody] MarkRiskRequest request)
    {
        var result = await _arpService.MarkHighRiskAccountAsync(receivableId, request.Reason);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("bad-debt/high-risk")]
    [RequirePermission("Finance.BadDebt.View")]
    public async Task<IActionResult> GetHighRiskAccounts()
    {
        var result = await _arpService.GetHighRiskAccountsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 资金流预测

    [HttpGet("cash-flow/forecast")]
    [RequirePermission("Finance.CashFlow.View")]
    public async Task<IActionResult> GetCashFlowForecast([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetCashFlowForecastAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("cash-flow/receivable-forecast")]
    [RequirePermission("Finance.CashFlow.View")]
    public async Task<IActionResult> GetReceivableForecast([FromQuery] DateTime forecastDate)
    {
        var result = await _arpService.GetReceivableForecastAsync(forecastDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("cash-flow/payable-forecast")]
    [RequirePermission("Finance.CashFlow.View")]
    public async Task<IActionResult> GetPayableForecast([FromQuery] DateTime forecastDate)
    {
        var result = await _arpService.GetPayableForecastAsync(forecastDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("cash-flow/trends")]
    [RequirePermission("Finance.CashFlow.View")]
    public async Task<IActionResult> GetCashFlowTrends([FromQuery] int months = 12)
    {
        var result = await _arpService.GetCashFlowTrendsAsync(months);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("cash-flow/liquidity")]
    [RequirePermission("Finance.CashFlow.View")]
    public async Task<IActionResult> GetLiquidityAnalysis()
    {
        var result = await _arpService.GetLiquidityAnalysisAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 报表和统计

    [HttpGet("reports/receivable-summary")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetReceivableSummary([FromQuery] DateTime? asOfDate = null)
    {
        var result = await _arpService.GetReceivableSummaryAsync(asOfDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/payable-summary")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetPayableSummary([FromQuery] DateTime? asOfDate = null)
    {
        var result = await _arpService.GetPayableSummaryAsync(asOfDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/collection-performance")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetCollectionPerformance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetCollectionPerformanceAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/payment-performance")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetPaymentPerformance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetPaymentPerformanceAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/top-debtors")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetTopDebtors([FromQuery] int topN = 10)
    {
        var result = await _arpService.GetTopDebtorsAsync(topN);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/top-creditors")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetTopCreditors([FromQuery] int topN = 10)
    {
        var result = await _arpService.GetTopCreditorsAsync(topN);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/turnover-analysis")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetTurnoverAnalysis([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetTurnoverAnalysisAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/dso-analysis")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetDSOAnalysis([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetDSOAnalysisAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/dpo-analysis")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetDPOAnalysis([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _arpService.GetDPOAnalysisAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/dso-trends")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetDSOTrends([FromQuery] int months = 12)
    {
        var result = await _arpService.GetDSOTrendsAsync(months);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("reports/dpo-trends")]
    [RequirePermission("Finance.Report.View")]
    public async Task<IActionResult> GetDPOTrends([FromQuery] int months = 12)
    {
        var result = await _arpService.GetDPOTrendsAsync(months);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 数据导入导出

    [HttpPost("import/receivables")]
    [RequirePermission("Finance.Import")]
    public async Task<IActionResult> ImportReceivables(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("请选择要导入的文件");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var result = await _arpService.ImportReceivablesAsync(stream.ToArray(), file.FileName);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("import/payables")]
    [RequirePermission("Finance.Import")]
    public async Task<IActionResult> ImportPayables(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("请选择要导入的文件");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var result = await _arpService.ImportPayablesAsync(stream.ToArray(), file.FileName);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("export/receivables")]
    [RequirePermission("Finance.Export")]
    public async Task<IActionResult> ExportReceivables([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string format = "Excel")
    {
        var result = await _arpService.ExportReceivablesAsync(startDate, endDate, format);
        if (result.Success)
        {
            return File(result.Data, "application/octet-stream", $"应收账款_{DateTime.Now:yyyyMMdd}.{format.ToLower()}");
        }
        return BadRequest(result);
    }

    [HttpGet("export/payables")]
    [RequirePermission("Finance.Export")]
    public async Task<IActionResult> ExportPayables([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string format = "Excel")
    {
        var result = await _arpService.ExportPayablesAsync(startDate, endDate, format);
        if (result.Success)
        {
            return File(result.Data, "application/octet-stream", $"应付账款_{DateTime.Now:yyyyMMdd}.{format.ToLower()}");
        }
        return BadRequest(result);
    }

    [HttpGet("export/aging-report")]
    [RequirePermission("Finance.Export")]
    public async Task<IActionResult> ExportAgingReport([FromQuery] DateTime asOfDate, [FromQuery] string format = "Excel")
    {
        var result = await _arpService.ExportAgingReportAsync(asOfDate, format);
        if (result.Success)
        {
            return File(result.Data, "application/octet-stream", $"账龄分析_{asOfDate:yyyyMMdd}.{format.ToLower()}");
        }
        return BadRequest(result);
    }

    [HttpGet("export/collection-report")]
    [RequirePermission("Finance.Export")]
    public async Task<IActionResult> ExportCollectionReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "Excel")
    {
        var result = await _arpService.ExportCollectionReportAsync(startDate, endDate, format);
        if (result.Success)
        {
            return File(result.Data, "application/octet-stream", $"催收报告_{DateTime.Now:yyyyMMdd}.{format.ToLower()}");
        }
        return BadRequest(result);
    }

    #endregion

    #region 自动化处理

    [HttpPost("automation/generate-receivables")]
    [RequirePermission("Finance.Automation")]
    public async Task<IActionResult> AutoGenerateReceivablesFromInvoices()
    {
        var result = await _arpService.AutoGenerateReceivablesFromInvoicesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("automation/generate-payables")]
    [RequirePermission("Finance.Automation")]
    public async Task<IActionResult> AutoGeneratePayablesFromPurchases()
    {
        var result = await _arpService.AutoGeneratePayablesFromPurchasesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("automation/update-aging")]
    [RequirePermission("Finance.Automation")]
    public async Task<IActionResult> AutoUpdateAgingBuckets()
    {
        var result = await _arpService.AutoUpdateAgingBucketsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("automation/calculate-risk")]
    [RequirePermission("Finance.Automation")]
    public async Task<IActionResult> AutoCalculateRiskLevels()
    {
        var result = await _arpService.AutoCalculateRiskLevelsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("automation/process-reminders")]
    [RequirePermission("Finance.Automation")]
    public async Task<IActionResult> ProcessScheduledReminders()
    {
        var result = await _arpService.ProcessScheduledRemindersAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion

    #region 仪表板

    [HttpGet("dashboard/receivable")]
    [RequirePermission("Finance.Dashboard.View")]
    public async Task<IActionResult> GetReceivableDashboardData()
    {
        var result = await _arpService.GetReceivableDashboardDataAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("dashboard/payable")]
    [RequirePermission("Finance.Dashboard.View")]
    public async Task<IActionResult> GetPayableDashboardData()
    {
        var result = await _arpService.GetPayableDashboardDataAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("dashboard/cash-flow")]
    [RequirePermission("Finance.Dashboard.View")]
    public async Task<IActionResult> GetCashFlowDashboardData()
    {
        var result = await _arpService.GetCashFlowDashboardDataAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("dashboard/collection")]
    [RequirePermission("Finance.Dashboard.View")]
    public async Task<IActionResult> GetCollectionDashboardData()
    {
        var result = await _arpService.GetCollectionDashboardDataAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #endregion
}

// 请求模型类
public class ReminderRequest
{
    public string ReminderType { get; set; } = string.Empty;
}

public class ScheduleTaskRequest
{
    public DateTime ScheduledDate { get; set; }
    public string TaskType { get; set; } = string.Empty;
}

public class ExecutePaymentRequest
{
    public decimal ActualAmount { get; set; }
}

public class WriteOffRequest
{
    public decimal WriteOffAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
}

public class RecoverDebtRequest
{
    public decimal RecoveryAmount { get; set; }
}

public class MarkRiskRequest
{
    public string Reason { get; set; } = string.Empty;
}
