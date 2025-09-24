using HeYuanERP.Api.Common;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Accounts;

// 账户（客户）应用服务接口：封装列表/新建/编辑/共享/转移/拜访/附件列表等业务逻辑
public interface IAccountsService
{
    // 列表与详情
    Task<Pagination<AccountListItemDto>> QueryAsync(AccountListQueryDto req, CancellationToken ct);
    Task<AccountDetailDto?> GetAsync(string id, CancellationToken ct);

    // 新建/编辑
    Task<AccountDetailDto> CreateAsync(AccountCreateDto req, string currentUserId, CancellationToken ct);
    Task<AccountDetailDto> UpdateAsync(string id, AccountUpdateDto req, string currentUserId, CancellationToken ct);

    // 唯一校验（编码）
    Task<bool> ExistsCodeAsync(string code, string? excludeId, CancellationToken ct);

    // 共享/取消共享/转移
    Task ShareAsync(string accountId, AccountShareRequestDto req, string currentUserId, CancellationToken ct);
    Task UnshareAsync(string accountId, string targetUserId, CancellationToken ct);
    Task TransferAsync(string accountId, AccountTransferRequestDto req, string currentUserId, CancellationToken ct);

    // 拜访
    Task<Pagination<AccountVisitDto>> ListVisitsAsync(string accountId, int page, int size, CancellationToken ct);
    Task<AccountVisitDto> CreateVisitAsync(string accountId, AccountVisitCreateDto req, string currentUserId, CancellationToken ct);

    // 附件列表（本批仅列出，上传下批实现）
    Task<List<AttachmentDto>> ListAttachmentsAsync(string accountId, CancellationToken ct);
}

