using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Purchase;

// 采购（PO）服务接口：提供列表/详情/CRUD/确认/收货
public interface IPurchaseOrdersService
{
    Task<Pagination<POListItemDto>> QueryAsync(POListQueryDto req, CancellationToken ct);
    Task<PODetailDto?> GetAsync(string id, CancellationToken ct);
    Task<PODetailDto> CreateAsync(POCreateDto req, string currentUserId, CancellationToken ct);
    Task<PODetailDto> UpdateAsync(string id, POUpdateDto req, string currentUserId, CancellationToken ct);
    Task DeleteAsync(string id, CancellationToken ct);
    Task ConfirmAsync(string id, string currentUserId, CancellationToken ct);

    // 收货：创建收货单 + 入库事务
    Task<POReceiveDetailDto> ReceiveAsync(string poId, POReceiveCreateDto req, string currentUserId, CancellationToken ct);
}

