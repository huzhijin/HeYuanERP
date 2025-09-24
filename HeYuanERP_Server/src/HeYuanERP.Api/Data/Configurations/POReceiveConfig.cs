using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// POReceive 与 POReceiveLine 的 Fluent 配置
public class POReceiveConfig : IEntityTypeConfiguration<POReceive>, IEntityTypeConfiguration<POReceiveLine>
{
    public void Configure(EntityTypeBuilder<POReceive> b)
    {
        b.ToTable("POReceives");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.PoId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Status).IsRequired().HasMaxLength(20);
        b.Property(x => x.Remark).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => new { x.PoId, x.ReceiveDate });

        b.HasOne(x => x.Po)
            .WithMany()
            .HasForeignKey(x => x.PoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Lines)
            .WithOne(x => x.Receive)
            .HasForeignKey(x => x.ReceiveId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<POReceiveLine> b)
    {
        b.ToTable("POReceiveLines");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.ReceiveId).IsRequired().HasMaxLength(64);
        b.Property(x => x.ProductId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Qty).HasPrecision(18, 4);
        b.Property(x => x.Whse).HasMaxLength(50);
        b.Property(x => x.Loc).HasMaxLength(50);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.ReceiveId);
        b.HasIndex(x => x.ProductId);

        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

