using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Services.Expense;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.Services.Authorization;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    // 费用申请管理
    [HttpPost("requests")]
    [RequirePermission("Expense.Request.Create")]
    public async Task<IActionResult> CreateExpenseRequest([FromBody] ExpenseRequest request)
    {
        var result = await _expenseService.CreateExpenseRequestAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("requests/{id}")]
    [RequirePermission("Expense.Request.Update")]
    public async Task<IActionResult> UpdateExpenseRequest(int id, [FromBody] ExpenseRequest request)
    {
        if (id != request.Id)
            return BadRequest("ID不匹配");

        var result = await _expenseService.UpdateExpenseRequestAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("requests/{id}")]
    [RequirePermission("Expense.Request.Delete")]
    public async Task<IActionResult> DeleteExpenseRequest(int id)
    {
        var result = await _expenseService.DeleteExpenseRequestAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/{id}")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequestById(int id)
    {
        var result = await _expenseService.GetExpenseRequestByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/number/{requestNumber}")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequestByNumber(string requestNumber)
    {
        var result = await _expenseService.GetExpenseRequestByNumberAsync(requestNumber);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequests([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _expenseService.GetExpenseRequestsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/requester/{requesterId}")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequestsByRequester(int requesterId)
    {
        var result = await _expenseService.GetExpenseRequestsByRequesterAsync(requesterId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/type/{requestType}")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequestsByType(string requestType)
    {
        var result = await _expenseService.GetExpenseRequestsByTypeAsync(requestType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/status/{status}")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequestsByStatus(string status)
    {
        var result = await _expenseService.GetExpenseRequestsByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/department/{department}")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequestsByDepartment(string department)
    {
        var result = await _expenseService.GetExpenseRequestsByDepartmentAsync(department);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 费用申请状态管理
    [HttpPost("requests/{id}/submit")]
    [RequirePermission("Expense.Request.Submit")]
    public async Task<IActionResult> SubmitExpenseRequest(int id)
    {
        var result = await _expenseService.SubmitExpenseRequestAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{id}/withdraw")]
    [RequirePermission("Expense.Request.Withdraw")]
    public async Task<IActionResult> WithdrawExpenseRequest(int id, [FromBody] WithdrawRequest request)
    {
        var result = await _expenseService.WithdrawExpenseRequestAsync(id, request.Reason);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{id}/cancel")]
    [RequirePermission("Expense.Request.Cancel")]
    public async Task<IActionResult> CancelExpenseRequest(int id, [FromBody] CancelRequest request)
    {
        var result = await _expenseService.CancelExpenseRequestAsync(id, request.Reason);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 审批流程
    [HttpPost("requests/{id}/approve")]
    [RequirePermission("Expense.Request.Approve")]
    public async Task<IActionResult> ApproveExpenseRequest(int id, [FromBody] ApprovalRequest request)
    {
        var result = await _expenseService.ApproveExpenseRequestAsync(id, request.ApproverId, request.Comments, request.ApprovedAmount);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{id}/reject")]
    [RequirePermission("Expense.Request.Approve")]
    public async Task<IActionResult> RejectExpenseRequest(int id, [FromBody] ApprovalRequest request)
    {
        var result = await _expenseService.RejectExpenseRequestAsync(id, request.ApproverId, request.Comments);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/pending-approvals/{approverId}")]
    [RequirePermission("Expense.Request.Approve")]
    public async Task<IActionResult> GetPendingApprovals(int approverId)
    {
        var result = await _expenseService.GetPendingApprovalsAsync(approverId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/approval-status/{approvalStatus}")]
    [RequirePermission("Expense.Request.View")]
    public async Task<IActionResult> GetExpenseRequestsByApprovalStatus(string approvalStatus)
    {
        var result = await _expenseService.GetExpenseRequestsByApprovalStatusAsync(approvalStatus);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 分级审批
    [HttpPost("requests/{id}/first-approval")]
    [RequirePermission("Expense.Request.FirstApproval")]
    public async Task<IActionResult> FirstLevelApproval(int id, [FromBody] LevelApprovalRequest request)
    {
        var result = await _expenseService.FirstLevelApprovalAsync(id, request.ApproverId, request.Status, request.Comments);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{id}/second-approval")]
    [RequirePermission("Expense.Request.SecondApproval")]
    public async Task<IActionResult> SecondLevelApproval(int id, [FromBody] LevelApprovalRequest request)
    {
        var result = await _expenseService.SecondLevelApprovalAsync(id, request.ApproverId, request.Status, request.Comments);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{id}/finance-approval")]
    [RequirePermission("Expense.Request.FinanceApproval")]
    public async Task<IActionResult> FinanceApproval(int id, [FromBody] LevelApprovalRequest request)
    {
        var result = await _expenseService.FinanceApprovalAsync(id, request.ApproverId, request.Status, request.Comments);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 费用申请行管理
    [HttpPost("request-lines")]
    [RequirePermission("Expense.RequestLine.Create")]
    public async Task<IActionResult> AddExpenseRequestLine([FromBody] ExpenseRequestLine line)
    {
        var result = await _expenseService.AddExpenseRequestLineAsync(line);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("request-lines/{id}")]
    [RequirePermission("Expense.RequestLine.Update")]
    public async Task<IActionResult> UpdateExpenseRequestLine(int id, [FromBody] ExpenseRequestLine line)
    {
        if (id != line.Id)
            return BadRequest("ID不匹配");

        var result = await _expenseService.UpdateExpenseRequestLineAsync(line);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("request-lines/{id}")]
    [RequirePermission("Expense.RequestLine.Delete")]
    public async Task<IActionResult> RemoveExpenseRequestLine(int id)
    {
        var result = await _expenseService.RemoveExpenseRequestLineAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/{requestId}/lines")]
    [RequirePermission("Expense.RequestLine.View")]
    public async Task<IActionResult> GetExpenseRequestLines(int requestId)
    {
        var result = await _expenseService.GetExpenseRequestLinesAsync(requestId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{requestId}/validate-total")]
    [RequirePermission("Expense.Request.Validate")]
    public async Task<IActionResult> ValidateExpenseRequestTotal(int requestId)
    {
        var result = await _expenseService.ValidateExpenseRequestTotalAsync(requestId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 支付管理
    [HttpPost("requests/{id}/initiate-payment")]
    [RequirePermission("Expense.Payment.Initiate")]
    public async Task<IActionResult> InitiatePayment(int id, [FromBody] PaymentInitiateRequest request)
    {
        var result = await _expenseService.InitiatePaymentAsync(id, request.PaymentMethod, request.PaymentAccount);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{id}/confirm-payment")]
    [RequirePermission("Expense.Payment.Confirm")]
    public async Task<IActionResult> ConfirmPayment(int id, [FromBody] PaymentConfirmRequest request)
    {
        var result = await _expenseService.ConfirmPaymentAsync(id, request.PaymentReference, request.PaidAmount);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/{id}/partial-payment")]
    [RequirePermission("Expense.Payment.Partial")]
    public async Task<IActionResult> RecordPartialPayment(int id, [FromBody] PartialPaymentRequest request)
    {
        var result = await _expenseService.RecordPartialPaymentAsync(id, request.PaidAmount, request.PaymentReference);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/for-payment")]
    [RequirePermission("Expense.Payment.View")]
    public async Task<IActionResult> GetExpenseRequestsForPayment()
    {
        var result = await _expenseService.GetExpenseRequestsForPaymentAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requests/paid")]
    [RequirePermission("Expense.Payment.View")]
    public async Task<IActionResult> GetPaidExpenseRequests([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _expenseService.GetPaidExpenseRequestsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 预算管理
    [HttpGet("budget/availability/{budgetCode}")]
    [RequirePermission("Expense.Budget.View")]
    public async Task<IActionResult> CheckBudgetAvailability(string budgetCode, [FromQuery] decimal amount)
    {
        var result = await _expenseService.CheckBudgetAvailabilityAsync(budgetCode, amount);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("budget/reserve")]
    [RequirePermission("Expense.Budget.Reserve")]
    public async Task<IActionResult> ReserveBudget([FromBody] BudgetReserveRequest request)
    {
        var result = await _expenseService.ReserveBudgetAsync(request.BudgetCode, request.Amount, request.RequestId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("budget/release/{requestId}")]
    [RequirePermission("Expense.Budget.Release")]
    public async Task<IActionResult> ReleaseBudgetReservation(int requestId)
    {
        var result = await _expenseService.ReleaseBudgetReservationAsync(requestId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("budget/utilization/{budgetCode}")]
    [RequirePermission("Expense.Budget.View")]
    public async Task<IActionResult> GetBudgetUtilization(string budgetCode)
    {
        var result = await _expenseService.GetBudgetUtilizationAsync(budgetCode);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("budget/summary")]
    [RequirePermission("Expense.Budget.View")]
    public async Task<IActionResult> GetBudgetSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _expenseService.GetBudgetSummaryAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 费用类别管理
    [HttpGet("categories")]
    [RequirePermission("Expense.Category.View")]
    public async Task<IActionResult> GetExpenseCategories()
    {
        var result = await _expenseService.GetExpenseCategoriesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("categories")]
    [RequirePermission("Expense.Category.Create")]
    public async Task<IActionResult> CreateExpenseCategory([FromBody] object category)
    {
        var result = await _expenseService.CreateExpenseCategoryAsync(category);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("categories/{id}")]
    [RequirePermission("Expense.Category.Update")]
    public async Task<IActionResult> UpdateExpenseCategory(int id, [FromBody] object category)
    {
        var result = await _expenseService.UpdateExpenseCategoryAsync(category);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("categories/{id}")]
    [RequirePermission("Expense.Category.Delete")]
    public async Task<IActionResult> DeleteExpenseCategory(int id)
    {
        var result = await _expenseService.DeleteExpenseCategoryAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 差旅费用专用
    [HttpPost("travel-requests")]
    [RequirePermission("Expense.Travel.Create")]
    public async Task<IActionResult> CreateTravelExpenseRequest([FromBody] ExpenseRequest request)
    {
        var result = await _expenseService.CreateTravelExpenseRequestAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("travel-requests/{id}/validate")]
    [RequirePermission("Expense.Travel.Validate")]
    public async Task<IActionResult> ValidateTravelExpense(int id, [FromBody] ExpenseRequest request)
    {
        var result = await _expenseService.ValidateTravelExpenseAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("travel/allowance")]
    [RequirePermission("Expense.Travel.View")]
    public async Task<IActionResult> CalculateTravelAllowance([FromQuery] string destination, [FromQuery] int days, [FromQuery] string level)
    {
        var result = await _expenseService.CalculateTravelAllowanceAsync(destination, days, level);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("travel/policy")]
    [RequirePermission("Expense.Travel.View")]
    public async Task<IActionResult> GetTravelPolicy()
    {
        var result = await _expenseService.GetTravelPolicyAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 报表和统计
    [HttpGet("stats/department")]
    [RequirePermission("Expense.Report.View")]
    public async Task<IActionResult> GetExpenseStatsByDepartment([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _expenseService.GetExpenseStatsByDepartmentAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/type")]
    [RequirePermission("Expense.Report.View")]
    public async Task<IActionResult> GetExpenseStatsByType([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _expenseService.GetExpenseStatsByTypeAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/requester/{requesterId}")]
    [RequirePermission("Expense.Report.View")]
    public async Task<IActionResult> GetExpenseStatsByRequester(int requesterId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _expenseService.GetExpenseStatsByRequesterAsync(requesterId, startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/trends")]
    [RequirePermission("Expense.Report.View")]
    public async Task<IActionResult> GetExpenseTrends([FromQuery] int months = 12)
    {
        var result = await _expenseService.GetExpenseTrendsAsync(months);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/top-requesters")]
    [RequirePermission("Expense.Report.View")]
    public async Task<IActionResult> GetTopExpenseRequesters([FromQuery] int topN = 10)
    {
        var result = await _expenseService.GetTopExpenseRequestersAsync(topN);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("stats/budget-variance")]
    [RequirePermission("Expense.Report.View")]
    public async Task<IActionResult> GetBudgetVarianceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _expenseService.GetBudgetVarianceReportAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 批量操作
    [HttpPost("requests/bulk-approve")]
    [RequirePermission("Expense.Request.BulkApprove")]
    public async Task<IActionResult> BulkApproveExpenseRequests([FromBody] BulkApprovalRequest request)
    {
        var result = await _expenseService.BulkApproveExpenseRequestsAsync(request.RequestIds, request.ApproverId, request.Comments);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/bulk-reject")]
    [RequirePermission("Expense.Request.BulkReject")]
    public async Task<IActionResult> BulkRejectExpenseRequests([FromBody] BulkApprovalRequest request)
    {
        var result = await _expenseService.BulkRejectExpenseRequestsAsync(request.RequestIds, request.ApproverId, request.Comments);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("requests/bulk-payment")]
    [RequirePermission("Expense.Payment.Bulk")]
    public async Task<IActionResult> BulkProcessPayments([FromBody] BulkPaymentRequest request)
    {
        var result = await _expenseService.BulkProcessPaymentsAsync(request.RequestIds);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 数据导入导出
    [HttpGet("export")]
    [RequirePermission("Expense.Export")]
    public async Task<IActionResult> ExportExpenseRequests([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string format = "Excel")
    {
        var result = await _expenseService.ExportExpenseRequestsAsync(startDate, endDate, format);
        if (result.Success)
        {
            return File(result.Data, "application/octet-stream", $"费用申请单_{DateTime.Now:yyyyMMdd}.{format.ToLower()}");
        }
        return BadRequest(result);
    }

    [HttpPost("import")]
    [RequirePermission("Expense.Import")]
    public async Task<IActionResult> ImportExpenseRequests(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("请选择要导入的文件");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var result = await _expenseService.ImportExpenseRequestsAsync(stream.ToArray(), file.FileName);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("import/template")]
    [RequirePermission("Expense.Import")]
    public async Task<IActionResult> GetImportTemplate()
    {
        var result = await _expenseService.GetImportTemplateAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 移动应用支持
    [HttpPost("mobile/quick-expense")]
    [RequirePermission("Expense.Mobile.Create")]
    public async Task<IActionResult> CreateQuickExpense([FromBody] object quickExpenseData)
    {
        var result = await _expenseService.CreateQuickExpenseAsync(quickExpenseData);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("mobile/expense-photo/{requestId}")]
    [RequirePermission("Expense.Mobile.Photo")]
    public async Task<IActionResult> SubmitExpensePhoto(int requestId, IFormFile photo)
    {
        if (photo == null || photo.Length == 0)
            return BadRequest("请选择照片文件");

        using var stream = new MemoryStream();
        await photo.CopyToAsync(stream);
        var result = await _expenseService.SubmitExpensePhotoAsync(requestId, stream.ToArray());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("mobile/forms")]
    [RequirePermission("Expense.Mobile.View")]
    public async Task<IActionResult> GetMobileExpenseForms()
    {
        var result = await _expenseService.GetMobileExpenseFormsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// 请求模型类
public class WithdrawRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class CancelRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class ApprovalRequest
{
    public int ApproverId { get; set; }
    public string Comments { get; set; } = string.Empty;
    public decimal? ApprovedAmount { get; set; }
}

public class LevelApprovalRequest
{
    public int ApproverId { get; set; }
    public string Status { get; set; } = string.Empty; // Approved, Rejected
    public string Comments { get; set; } = string.Empty;
}

public class PaymentInitiateRequest
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentAccount { get; set; } = string.Empty;
}

public class PaymentConfirmRequest
{
    public string PaymentReference { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
}

public class PartialPaymentRequest
{
    public decimal PaidAmount { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
}

public class BudgetReserveRequest
{
    public string BudgetCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int RequestId { get; set; }
}

public class BulkApprovalRequest
{
    public List<int> RequestIds { get; set; } = new List<int>();
    public int ApproverId { get; set; }
    public string Comments { get; set; } = string.Empty;
}

public class BulkPaymentRequest
{
    public List<int> RequestIds { get; set; } = new List<int>();
}
