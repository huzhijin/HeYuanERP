using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Infrastructure.Persistence.Configurations;

/// <summary>
/// Payment 实体的 EF Core 映射配置。
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // 表名
        builder.ToTable("payments");

        // 主键
        builder.HasKey(p => p.Id);

        // 字段约束
        builder.Property(p => p.Method)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.PaymentDate)
            .HasColumnType("date");

        builder.Property(p => p.Remark)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAtUtc)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.OrgId)
            .HasMaxLength(64);

        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        // 关系：一对多（Payment -> PaymentAttachment）
        builder.HasMany(p => p.Attachments)
            .WithOne(a => a.Payment!)
            .HasForeignKey(a => a.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // 索引
        builder.HasIndex(p => p.PaymentDate).HasDatabaseName("IX_payments_payment_date");
        builder.HasIndex(p => p.Method).HasDatabaseName("IX_payments_method");
        builder.HasIndex(p => p.Amount).HasDatabaseName("IX_payments_amount");
    }
}

