using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Inventory;

// 仓库服务接口：提供仓库的 CRUD 服务
public interface IWarehousesService
{
    Task<Pagination<WarehouseListItemDto>> QueryAsync(WarehouseListQueryDto req, CancellationToken ct);
    Task<WarehouseDetailDto?> GetAsync(string id, CancellationToken ct);
    Task<WarehouseDetailDto> CreateAsync(WarehouseCreateDto req, string currentUserId, CancellationToken ct);
    Task<WarehouseDetailDto> UpdateAsync(string id, WarehouseUpdateDto req, string currentUserId, CancellationToken ct);
    Task DeleteAsync(string id, CancellationToken ct);
}

