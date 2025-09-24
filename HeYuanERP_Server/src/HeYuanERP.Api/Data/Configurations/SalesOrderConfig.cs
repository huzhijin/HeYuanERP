using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// SalesOrder 与 SalesOrderLine 的 Fluent 配置
public class SalesOrderConfig : IEntityTypeConfiguration<SalesOrder>, IEntityTypeConfiguration<SalesOrderLine>
{
    // 头表配置
    public void Configure(EntityTypeBuilder<SalesOrder> b)
    {
        b.ToTable("SalesOrders");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.OrderNo).IsRequired().HasMaxLength(50);
        b.Property(x => x.AccountId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Currency).IsRequired().HasMaxLength(10);
        b.Property(x => x.Status).IsRequired().HasMaxLength(20);
        b.Property(x => x.Remark).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.OrderNo).IsUnique();
        b.HasIndex(x => new { x.AccountId, x.OrderDate });

        b.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Lines)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // 行表配置
    public void Configure(EntityTypeBuilder<SalesOrderLine> b)
    {
        b.ToTable("SalesOrderLines");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.OrderId).IsRequired().HasMaxLength(64);
        b.Property(x => x.ProductId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Qty).HasPrecision(18, 4);
        b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        b.Property(x => x.Discount).HasPrecision(9, 6); // 0..1
        b.Property(x => x.TaxRate).HasPrecision(9, 6);  // 0..1
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.OrderId);
        b.HasIndex(x => x.ProductId);

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

