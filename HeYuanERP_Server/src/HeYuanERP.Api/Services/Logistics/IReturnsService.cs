using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Logistics;

// 退货单应用服务：创建与查询详情
public interface IReturnsService
{
    Task<ReturnDetailDto?> GetAsync(string id, CancellationToken ct);
    Task<ReturnDetailDto> CreateAsync(ReturnCreateDto req, string currentUserId, CancellationToken ct);
}

