using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// PurchaseOrder 与 POLine 的 Fluent 配置
public class PurchaseOrderConfig : IEntityTypeConfiguration<PurchaseOrder>, IEntityTypeConfiguration<POLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> b)
    {
        b.ToTable("PurchaseOrders");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.PoNo).IsRequired().HasMaxLength(50);
        b.Property(x => x.VendorId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Status).IsRequired().HasMaxLength(20);
        b.Property(x => x.Remark).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.PoNo).IsUnique();
        b.HasIndex(x => new { x.VendorId, x.PoDate });

        b.HasOne(x => x.Vendor)
            .WithMany()
            .HasForeignKey(x => x.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Lines)
            .WithOne(x => x.Po)
            .HasForeignKey(x => x.PoId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<POLine> b)
    {
        b.ToTable("POLines");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.PoId).IsRequired().HasMaxLength(64);
        b.Property(x => x.ProductId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Qty).HasPrecision(18, 4);
        b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.PoId);
        b.HasIndex(x => x.ProductId);

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

