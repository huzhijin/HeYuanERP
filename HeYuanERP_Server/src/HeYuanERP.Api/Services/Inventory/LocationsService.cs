using HeYuanERP.Api.Common;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Inventory;

// 库位服务实现：CRUD + 同仓库内编码唯一性校验
public class LocationsService : ILocationsService
{
    private readonly AppDbContext _db;

    public LocationsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Pagination<LocationListItemDto>> QueryAsync(LocationListQueryDto req, CancellationToken ct)
    {
        var q = _db.Locations.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.WarehouseId))
        {
            q = q.Where(l => l.WarehouseId == req.WarehouseId);
        }
        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var kw = req.Keyword.Trim();
            q = q.Where(l => l.Code.Contains(kw) || l.Name.Contains(kw));
        }
        if (req.Active.HasValue)
        {
            q = q.Where(l => l.Active == req.Active.Value);
        }

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, req.Page);
        var size = Math.Clamp(req.Size, 1, 200);

        var list = await q.OrderBy(l => l.WarehouseId).ThenBy(l => l.Code)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(l => new LocationListItemDto
            {
                Id = l.Id,
                WarehouseId = l.WarehouseId,
                Code = l.Code,
                Name = l.Name,
                Active = l.Active
            })
            .ToListAsync(ct);

        return new Pagination<LocationListItemDto> { Items = list, Page = page, Size = size, Total = total };
    }

    public async Task<LocationDetailDto?> GetAsync(string id, CancellationToken ct)
    {
        var l = await _db.Locations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return l == null ? null : MapDetail(l);
    }

    public async Task<LocationDetailDto> CreateAsync(LocationCreateDto req, string currentUserId, CancellationToken ct)
    {
        await EnsureWarehouseExistsAsync(req.WarehouseId, ct);
        await EnsureCodeUniqueAsync(req.WarehouseId, req.Code, null, ct);
        var l = new Location
        {
            WarehouseId = req.WarehouseId.Trim(),
            Code = req.Code.Trim(),
            Name = req.Name.Trim(),
            Active = req.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };
        _db.Locations.Add(l);
        await _db.SaveChangesAsync(ct);
        return MapDetail(l);
    }

    public async Task<LocationDetailDto> UpdateAsync(string id, LocationUpdateDto req, string currentUserId, CancellationToken ct)
    {
        var l = await _db.Locations.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException("未找到库位");
        if (!string.Equals(l.WarehouseId, req.WarehouseId, StringComparison.Ordinal))
        {
            await EnsureWarehouseExistsAsync(req.WarehouseId, ct);
        }
        if (!string.Equals(l.WarehouseId, req.WarehouseId, StringComparison.Ordinal) || !string.Equals(l.Code, req.Code, StringComparison.Ordinal))
        {
            await EnsureCodeUniqueAsync(req.WarehouseId, req.Code, l.Id, ct);
        }
        l.WarehouseId = req.WarehouseId.Trim();
        l.Code = req.Code.Trim();
        l.Name = req.Name.Trim();
        l.Active = req.Active;
        l.UpdatedAt = DateTime.UtcNow;
        l.UpdatedBy = currentUserId;
        await _db.SaveChangesAsync(ct);
        return MapDetail(l);
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        var l = await _db.Locations.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException("未找到库位");
        _db.Locations.Remove(l);
        await _db.SaveChangesAsync(ct);
    }

    private async Task EnsureWarehouseExistsAsync(string warehouseId, CancellationToken ct)
    {
        var ok = await _db.Warehouses.AsNoTracking().AnyAsync(w => w.Id == warehouseId, ct);
        if (!ok) throw new ApplicationException("仓库不存在");
    }

    private async Task EnsureCodeUniqueAsync(string warehouseId, string code, string? excludeId, CancellationToken ct)
    {
        var c = code.Trim();
        var exists = await _db.Locations.AsNoTracking().AnyAsync(x => x.WarehouseId == warehouseId && x.Code == c && (excludeId == null || x.Id != excludeId), ct);
        if (exists) throw new ApplicationException("库位编码在该仓库下已存在");
    }

    private static LocationDetailDto MapDetail(Location l) => new()
    {
        Id = l.Id,
        WarehouseId = l.WarehouseId,
        Code = l.Code,
        Name = l.Name,
        Active = l.Active,
        CreatedAt = l.CreatedAt,
        CreatedBy = l.CreatedBy,
        UpdatedAt = l.UpdatedAt,
        UpdatedBy = l.UpdatedBy
    };
}

