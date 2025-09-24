using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Inventory;

// 库位服务接口：提供库位的 CRUD 服务
public interface ILocationsService
{
    Task<Pagination<LocationListItemDto>> QueryAsync(LocationListQueryDto req, CancellationToken ct);
    Task<LocationDetailDto?> GetAsync(string id, CancellationToken ct);
    Task<LocationDetailDto> CreateAsync(LocationCreateDto req, string currentUserId, CancellationToken ct);
    Task<LocationDetailDto> UpdateAsync(string id, LocationUpdateDto req, string currentUserId, CancellationToken ct);
    Task DeleteAsync(string id, CancellationToken ct);
}

