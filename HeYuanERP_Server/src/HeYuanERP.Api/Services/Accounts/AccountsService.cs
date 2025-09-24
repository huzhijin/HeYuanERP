using HeYuanERP.Api.Common;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Accounts;

// 账户（客户）应用服务实现：
// - 处理唯一性校验、审计字段填充；
// - 实现共享/转移与拜访记录增查；
// - 附件仅提供列表功能（上传于下一批实现）。
public class AccountsService : IAccountsService
{
    private readonly AppDbContext _db;

    public AccountsService(AppDbContext db)
    {
        _db = db;
    }

    // 列表
    public async Task<Pagination<AccountListItemDto>> QueryAsync(AccountListQueryDto req, CancellationToken ct)
    {
        var query = _db.Accounts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var kw = req.Keyword.Trim();
            query = query.Where(x => x.Code.Contains(kw) || x.Name.Contains(kw));
        }
        if (req.Active.HasValue)
        {
            query = query.Where(x => x.Active == req.Active.Value);
        }
        if (!string.IsNullOrWhiteSpace(req.OwnerId))
        {
            query = query.Where(x => x.OwnerId == req.OwnerId);
        }

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, req.Page);
        var size = Math.Clamp(req.Size, 1, 200);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new AccountListItemDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                OwnerId = x.OwnerId,
                Active = x.Active,
                LastEventDate = x.LastEventDate,
                Description = x.Description
            })
            .ToListAsync(ct);

        return new Pagination<AccountListItemDto>
        {
            Items = items,
            Page = page,
            Size = size,
            Total = total
        };
    }

    // 详情
    public async Task<AccountDetailDto?> GetAsync(string id, CancellationToken ct)
    {
        var entity = await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        return MapDetail(entity);
    }

    // 新建
    public async Task<AccountDetailDto> CreateAsync(AccountCreateDto req, string currentUserId, CancellationToken ct)
    {
        if (await _db.Accounts.AnyAsync(x => x.Code == req.Code, ct))
        {
            throw new ApplicationException($"客户编码已存在：{req.Code}");
        }

        var now = DateTime.UtcNow;
        var entity = new Account
        {
            Code = req.Code.Trim(),
            Name = req.Name.Trim(),
            OwnerId = string.IsNullOrWhiteSpace(req.OwnerId) ? null : req.OwnerId!.Trim(),
            TaxNo = string.IsNullOrWhiteSpace(req.TaxNo) ? null : req.TaxNo!.Trim(),
            Active = req.Active,
            Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description!.Trim(),
            CreatedAt = now,
            CreatedBy = currentUserId
        };

        _db.Accounts.Add(entity);
        await _db.SaveChangesAsync(ct);
        return MapDetail(entity);
    }

    // 编辑
    public async Task<AccountDetailDto> UpdateAsync(string id, AccountUpdateDto req, string currentUserId, CancellationToken ct)
    {
        var entity = await _db.Accounts.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("未找到指定客户");

        if (!string.IsNullOrWhiteSpace(req.Code))
        {
            var newCode = req.Code.Trim();
            if (!string.Equals(newCode, entity.Code, StringComparison.OrdinalIgnoreCase))
            {
                if (await _db.Accounts.AnyAsync(x => x.Code == newCode && x.Id != id, ct))
                {
                    throw new ApplicationException($"客户编码已存在：{newCode}");
                }
                entity.Code = newCode;
            }
        }

        entity.Name = req.Name.Trim();
        entity.OwnerId = string.IsNullOrWhiteSpace(req.OwnerId) ? null : req.OwnerId!.Trim();
        entity.TaxNo = string.IsNullOrWhiteSpace(req.TaxNo) ? null : req.TaxNo!.Trim();
        entity.Active = req.Active;
        entity.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description!.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = currentUserId;

        await _db.SaveChangesAsync(ct);
        return MapDetail(entity);
    }

    public async Task<bool> ExistsCodeAsync(string code, string? excludeId, CancellationToken ct)
    {
        code = code.Trim();
        return await _db.Accounts.AsNoTracking().AnyAsync(x => x.Code == code && (excludeId == null || x.Id != excludeId), ct);
    }

    // 共享
    public async Task ShareAsync(string accountId, AccountShareRequestDto req, string currentUserId, CancellationToken ct)
    {
        // 校验客户存在
        if (!await _db.Accounts.AnyAsync(x => x.Id == accountId, ct))
        {
            throw new KeyNotFoundException("未找到指定客户");
        }

        var share = await _db.Set<AccountShare>().FirstOrDefaultAsync(x => x.AccountId == accountId && x.TargetUserId == req.TargetUserId, ct);
        if (share == null)
        {
            share = new AccountShare
            {
                AccountId = accountId,
                TargetUserId = req.TargetUserId,
                Permission = req.Permission,
                ExpireAt = req.ExpireAt,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            };
            _db.Add(share);
        }
        else
        {
            share.Permission = req.Permission;
            share.ExpireAt = req.ExpireAt;
            share.UpdatedAt = DateTime.UtcNow;
            share.UpdatedBy = currentUserId;
        }

        await _db.SaveChangesAsync(ct);
    }

    // 取消共享
    public async Task UnshareAsync(string accountId, string targetUserId, CancellationToken ct)
    {
        var share = await _db.Set<AccountShare>().FirstOrDefaultAsync(x => x.AccountId == accountId && x.TargetUserId == targetUserId, ct)
            ?? throw new KeyNotFoundException("未找到共享记录");
        _db.Remove(share);
        await _db.SaveChangesAsync(ct);
    }

    // 转移负责人
    public async Task TransferAsync(string accountId, AccountTransferRequestDto req, string currentUserId, CancellationToken ct)
    {
        var entity = await _db.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, ct)
            ?? throw new KeyNotFoundException("未找到指定客户");
        entity.OwnerId = req.NewOwnerId.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = currentUserId;
        await _db.SaveChangesAsync(ct);
    }

    // 拜访列表
    public async Task<Pagination<AccountVisitDto>> ListVisitsAsync(string accountId, int page, int size, CancellationToken ct)
    {
        var q = _db.Set<AccountVisit>().AsNoTracking().Where(v => v.AccountId == accountId);
        var total = await q.CountAsync(ct);
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 200);
        var items = await q
            .OrderByDescending(v => v.VisitDate)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(v => new AccountVisitDto
            {
                Id = v.Id,
                AccountId = v.AccountId,
                ContactId = v.ContactId,
                VisitorId = v.VisitorId,
                VisitDate = v.VisitDate,
                Subject = v.Subject,
                Content = v.Content,
                Location = v.Location,
                Result = v.Result,
                NextActionAt = v.NextActionAt,
                CreatedAt = v.CreatedAt,
                CreatedBy = v.CreatedBy,
                UpdatedAt = v.UpdatedAt,
                UpdatedBy = v.UpdatedBy
            })
            .ToListAsync(ct);
        return new Pagination<AccountVisitDto>
        {
            Items = items,
            Page = page,
            Size = size,
            Total = total
        };
    }

    // 新建拜访
    public async Task<AccountVisitDto> CreateVisitAsync(string accountId, AccountVisitCreateDto req, string currentUserId, CancellationToken ct)
    {
        // 基础存在性验证
        var accountExists = await _db.Accounts.AsNoTracking().AnyAsync(a => a.Id == accountId, ct);
        if (!accountExists)
        {
            throw new KeyNotFoundException("未找到指定客户");
        }
        if (!string.IsNullOrWhiteSpace(req.ContactId))
        {
            var contactBelongs = await _db.Contacts.AsNoTracking().AnyAsync(c => c.Id == req.ContactId && c.AccountId == accountId, ct);
            if (!contactBelongs)
            {
                throw new ApplicationException("联系人不属于该客户");
            }
        }

        var visit = new AccountVisit
        {
            AccountId = accountId,
            ContactId = string.IsNullOrWhiteSpace(req.ContactId) ? null : req.ContactId!.Trim(),
            VisitorId = string.IsNullOrWhiteSpace(req.VisitorId) ? currentUserId : req.VisitorId!.Trim(),
            VisitDate = req.VisitDate,
            Subject = string.IsNullOrWhiteSpace(req.Subject) ? null : req.Subject!.Trim(),
            Content = string.IsNullOrWhiteSpace(req.Content) ? null : req.Content!.Trim(),
            Location = string.IsNullOrWhiteSpace(req.Location) ? null : req.Location!.Trim(),
            Result = string.IsNullOrWhiteSpace(req.Result) ? null : req.Result!.Trim(),
            NextActionAt = req.NextActionAt,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };
        _db.Add(visit);
        await _db.SaveChangesAsync(ct);

        return new AccountVisitDto
        {
            Id = visit.Id,
            AccountId = visit.AccountId,
            ContactId = visit.ContactId,
            VisitorId = visit.VisitorId,
            VisitDate = visit.VisitDate,
            Subject = visit.Subject,
            Content = visit.Content,
            Location = visit.Location,
            Result = visit.Result,
            NextActionAt = visit.NextActionAt,
            CreatedAt = visit.CreatedAt,
            CreatedBy = visit.CreatedBy
        };
    }

    // 附件列表
    public async Task<List<AttachmentDto>> ListAttachmentsAsync(string accountId, CancellationToken ct)
    {
        // 约定：BusinessType = Customer，BusinessEntityId = Account.Id
        var list = await _db.Attachments.AsNoTracking()
            .Where(a => a.BusinessType == AttachmentBusinessType.Customer && a.BusinessEntityId == accountId)
            .OrderByDescending(a => a.UploadedAt)
            .Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = a.ContentType,
                Size = a.FileSize,
                StorageUri = a.StoragePath,
                UploadedAt = a.UploadedAt,
                UploadedBy = a.UploadedBy
            })
            .ToListAsync(ct);
        return list;
    }

    private static AccountDetailDto MapDetail(Account src) => new()
    {
        Id = src.Id,
        Code = src.Code,
        Name = src.Name,
        OwnerId = src.OwnerId,
        TaxNo = src.TaxNo,
        Active = src.Active,
        LastEventDate = src.LastEventDate,
        Description = src.Description,
        CreatedAt = src.CreatedAt,
        CreatedBy = src.CreatedBy,
        UpdatedAt = src.UpdatedAt,
        UpdatedBy = src.UpdatedBy
    };
}
