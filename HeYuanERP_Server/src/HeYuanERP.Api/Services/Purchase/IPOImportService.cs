using HeYuanERP.Api.DTOs;
using Microsoft.AspNetCore.Http;

namespace HeYuanERP.Api.Services.Purchase;

// PO 导入服务接口：提供预检与提交导入
public interface IPOImportService
{
    Task<POImportPrecheckResultDto> PrecheckAsync(string vendorId, IFormFile file, CancellationToken ct);
    Task<POImportReceiptDto> ImportAsync(string vendorId, IFormFile file, string currentUserId, CancellationToken ct);
}

