using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// Return 与 ReturnLine 的 Fluent 配置
public class ReturnConfig : IEntityTypeConfiguration<Return>, IEntityTypeConfiguration<ReturnLine>
{
    public void Configure(EntityTypeBuilder<Return> b)
    {
        b.ToTable("Returns");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.ReturnNo).IsRequired().HasMaxLength(50);
        b.Property(x => x.OrderId).IsRequired().HasMaxLength(64);
        b.Property(x => x.SourceDeliveryId).HasMaxLength(64);
        b.Property(x => x.Status).IsRequired().HasMaxLength(20);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.ReturnNo).IsUnique();
        b.HasIndex(x => new { x.OrderId, x.ReturnDate });

        b.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.SourceDelivery)
            .WithMany()
            .HasForeignKey(x => x.SourceDeliveryId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Lines)
            .WithOne(x => x.Return)
            .HasForeignKey(x => x.ReturnId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<ReturnLine> b)
    {
        b.ToTable("ReturnLines");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.ReturnId).IsRequired().HasMaxLength(64);
        b.Property(x => x.ProductId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Qty).HasPrecision(18, 4);
        b.Property(x => x.Reason).HasMaxLength(200);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.ReturnId);
        b.HasIndex(x => x.ProductId);

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

