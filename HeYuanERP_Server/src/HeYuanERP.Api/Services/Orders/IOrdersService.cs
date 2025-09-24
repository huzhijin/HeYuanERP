using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Orders;

// 订单应用服务接口：CRUD + 确认/反审
public interface IOrdersService
{
    Task<Pagination<OrderListItemDto>> QueryAsync(OrderListQueryDto req, CancellationToken ct);
    Task<OrderDetailDto?> GetAsync(string id, CancellationToken ct);
    Task<OrderDetailDto> CreateAsync(OrderCreateDto req, string currentUserId, CancellationToken ct);
    Task<OrderDetailDto> UpdateAsync(string id, OrderUpdateDto req, string currentUserId, CancellationToken ct);
    Task DeleteAsync(string id, CancellationToken ct);
    Task ConfirmAsync(string id, string currentUserId, CancellationToken ct);
    Task ReverseAsync(string id, string currentUserId, string? reason, CancellationToken ct);
}

