using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// InventoryTxn 实体的 Fluent 配置（库存事务流水）
public class InventoryTxnConfig : IEntityTypeConfiguration<InventoryTxn>
{
    public void Configure(EntityTypeBuilder<InventoryTxn> b)
    {
        b.ToTable("InventoryTxns");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.TxnCode).IsRequired().HasMaxLength(20);
        b.Property(x => x.ProductId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Qty).HasPrecision(18, 4);
        b.Property(x => x.Whse).IsRequired().HasMaxLength(50);
        b.Property(x => x.Loc).HasMaxLength(50);
        b.Property(x => x.RefType).IsRequired().HasMaxLength(50);
        b.Property(x => x.RefId).IsRequired().HasMaxLength(64);
        b.Property(x => x.CreatedBy).HasMaxLength(50);

        b.HasIndex(x => new { x.ProductId, x.TxnDate });
        b.HasIndex(x => new { x.TxnCode, x.TxnDate });

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

