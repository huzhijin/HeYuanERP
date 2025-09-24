using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// Product 实体的 Fluent 配置
public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.Code).IsRequired().HasMaxLength(50);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Spec).HasMaxLength(200);
        b.Property(x => x.Unit).HasMaxLength(20);
        b.Property(x => x.DefaultPrice).HasPrecision(18, 2);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.Name);
    }
}

