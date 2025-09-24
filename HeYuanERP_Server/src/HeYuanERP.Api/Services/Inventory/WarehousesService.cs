using HeYuanERP.Api.Common;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Inventory;

// 仓库服务实现：CRUD + 唯一性校验
public class WarehousesService : IWarehousesService
{
    private readonly AppDbContext _db;

    public WarehousesService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Pagination<WarehouseListItemDto>> QueryAsync(WarehouseListQueryDto req, CancellationToken ct)
    {
        var q = _db.Warehouses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var kw = req.Keyword.Trim();
            q = q.Where(w => w.Code.Contains(kw) || w.Name.Contains(kw));
        }
        if (req.Active.HasValue)
        {
            q = q.Where(w => w.Active == req.Active.Value);
        }

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, req.Page);
        var size = Math.Clamp(req.Size, 1, 200);

        var list = await q.OrderBy(w => w.Code)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(w => new WarehouseListItemDto
            {
                Id = w.Id,
                Code = w.Code,
                Name = w.Name,
                Active = w.Active,
                Address = w.Address,
                Contact = w.Contact,
                Phone = w.Phone
            })
            .ToListAsync(ct);

        return new Pagination<WarehouseListItemDto> { Items = list, Page = page, Size = size, Total = total };
    }

    public async Task<WarehouseDetailDto?> GetAsync(string id, CancellationToken ct)
    {
        var w = await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return w == null ? null : MapDetail(w);
    }

    public async Task<WarehouseDetailDto> CreateAsync(WarehouseCreateDto req, string currentUserId, CancellationToken ct)
    {
        await EnsureCodeUniqueAsync(req.Code, null, ct);
        var w = new Warehouse
        {
            Code = req.Code.Trim(),
            Name = req.Name.Trim(),
            Active = req.Active,
            Address = string.IsNullOrWhiteSpace(req.Address) ? null : req.Address!.Trim(),
            Contact = string.IsNullOrWhiteSpace(req.Contact) ? null : req.Contact!.Trim(),
            Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone!.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };
        _db.Warehouses.Add(w);
        await _db.SaveChangesAsync(ct);
        return MapDetail(w);
    }

    public async Task<WarehouseDetailDto> UpdateAsync(string id, WarehouseUpdateDto req, string currentUserId, CancellationToken ct)
    {
        var w = await _db.Warehouses.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException("未找到仓库");
        if (!string.Equals(w.Code, req.Code, StringComparison.Ordinal))
        {
            await EnsureCodeUniqueAsync(req.Code, w.Id, ct);
            w.Code = req.Code.Trim();
        }
        w.Name = req.Name.Trim();
        w.Active = req.Active;
        w.Address = string.IsNullOrWhiteSpace(req.Address) ? null : req.Address!.Trim();
        w.Contact = string.IsNullOrWhiteSpace(req.Contact) ? null : req.Contact!.Trim();
        w.Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone!.Trim();
        w.UpdatedAt = DateTime.UtcNow;
        w.UpdatedBy = currentUserId;
        await _db.SaveChangesAsync(ct);
        return MapDetail(w);
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        var w = await _db.Warehouses.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException("未找到仓库");
        _db.Warehouses.Remove(w);
        await _db.SaveChangesAsync(ct);
    }

    private async Task EnsureCodeUniqueAsync(string code, string? excludeId, CancellationToken ct)
    {
        var c = code.Trim();
        var exists = await _db.Warehouses.AsNoTracking().AnyAsync(x => x.Code == c && (excludeId == null || x.Id != excludeId), ct);
        if (exists) throw new ApplicationException("仓库编码已存在");
    }

    private static WarehouseDetailDto MapDetail(Warehouse w) => new()
    {
        Id = w.Id,
        Code = w.Code,
        Name = w.Name,
        Active = w.Active,
        Address = w.Address,
        Contact = w.Contact,
        Phone = w.Phone,
        CreatedAt = w.CreatedAt,
        CreatedBy = w.CreatedBy,
        UpdatedAt = w.UpdatedAt,
        UpdatedBy = w.UpdatedBy
    };
}

