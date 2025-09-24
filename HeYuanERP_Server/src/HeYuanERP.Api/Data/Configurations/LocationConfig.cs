using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// Location 实体的 Fluent 配置（库位）
public class LocationConfig : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> b)
    {
        b.ToTable("Locations");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.WarehouseId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Code).IsRequired().HasMaxLength(50);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        // 同一仓库内库位编码唯一
        b.HasIndex(x => new { x.WarehouseId, x.Code }).IsUnique();
        b.HasIndex(x => x.WarehouseId);

        b.HasOne(x => x.Warehouse)
            .WithMany(w => w.Locations)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

