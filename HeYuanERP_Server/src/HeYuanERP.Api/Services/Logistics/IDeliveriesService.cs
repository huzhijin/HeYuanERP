using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Logistics;

// 送货单应用服务：创建与查询详情
public interface IDeliveriesService
{
    Task<DeliveryDetailDto?> GetAsync(string id, CancellationToken ct);
    Task<DeliveryDetailDto> CreateAsync(DeliveryCreateDto req, string currentUserId, CancellationToken ct);
}

