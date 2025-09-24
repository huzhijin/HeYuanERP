using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Infrastructure.Persistence.Configurations;

/// <summary>
/// PaymentAttachment 实体的 EF Core 映射配置。
/// </summary>
public class PaymentAttachmentConfiguration : IEntityTypeConfiguration<PaymentAttachment>
{
    public void Configure(EntityTypeBuilder<PaymentAttachment> builder)
    {
        builder.ToTable("payment_attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.PaymentId)
            .IsRequired();

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(a => a.ContentType)
            .HasMaxLength(100);

        builder.Property(a => a.Size)
            .IsRequired();

        builder.Property(a => a.StorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.UploadedAtUtc)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        // 外键关系在 PaymentConfiguration 中已定义，这里可选重复声明
        builder.HasIndex(a => a.PaymentId).HasDatabaseName("IX_payment_attachments_payment_id");
        builder.HasIndex(a => a.StorageKey).HasDatabaseName("IX_payment_attachments_storage_key");
    }
}

