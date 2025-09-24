using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// InventoryBalance 实体的 Fluent 配置（库存结存）
public class InventoryBalanceConfig : IEntityTypeConfiguration<InventoryBalance>
{
    public void Configure(EntityTypeBuilder<InventoryBalance> b)
    {
        b.ToTable("InventoryBalances");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.ProductId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Whse).IsRequired().HasMaxLength(50);
        b.Property(x => x.Loc).HasMaxLength(50);
        b.Property(x => x.OnHand).HasPrecision(18, 4);
        b.Property(x => x.Reserved).HasPrecision(18, 4);
        b.Property(x => x.Available).HasPrecision(18, 4);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        // 唯一键：产品+仓库+库位（同一维度仅一条结存）
        b.HasIndex(x => new { x.ProductId, x.Whse, x.Loc }).IsUnique();
        b.HasIndex(x => x.ProductId);

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

