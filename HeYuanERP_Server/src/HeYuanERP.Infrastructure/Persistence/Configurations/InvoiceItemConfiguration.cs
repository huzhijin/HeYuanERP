using HeYuanERP.Domain.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 发票明细实体映射配置。
/// </summary>
public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceId)
            .IsRequired();

        builder.Property(i => i.ProductCode)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(i => i.Specification)
            .HasMaxLength(128);

        builder.Property(i => i.Unit)
            .HasMaxLength(32);

        builder.Property(i => i.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.AmountExcludingTax)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.TaxRate)
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(i => i.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.AmountIncludingTax)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.SortOrder)
            .IsRequired();

        builder.Property(i => i.Remark)
            .HasMaxLength(512);

        builder.HasIndex(i => i.InvoiceId);
    }
}

