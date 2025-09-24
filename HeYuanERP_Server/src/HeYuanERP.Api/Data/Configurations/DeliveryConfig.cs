using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// Delivery 与 DeliveryLine 的 Fluent 配置
public class DeliveryConfig : IEntityTypeConfiguration<Delivery>, IEntityTypeConfiguration<DeliveryLine>
{
    public void Configure(EntityTypeBuilder<Delivery> b)
    {
        b.ToTable("Deliveries");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.DeliveryNo).IsRequired().HasMaxLength(50);
        b.Property(x => x.OrderId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Status).IsRequired().HasMaxLength(20);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.DeliveryNo).IsUnique();
        b.HasIndex(x => new { x.OrderId, x.DeliveryDate });

        b.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Lines)
            .WithOne(x => x.Delivery)
            .HasForeignKey(x => x.DeliveryId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<DeliveryLine> b)
    {
        b.ToTable("DeliveryLines");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.DeliveryId).IsRequired().HasMaxLength(64);
        b.Property(x => x.OrderLineId).HasMaxLength(64);
        b.Property(x => x.ProductId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Qty).HasPrecision(18, 4);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.DeliveryId);
        b.HasIndex(x => x.ProductId);

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.OrderLine)
            .WithMany()
            .HasForeignKey(x => x.OrderLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

