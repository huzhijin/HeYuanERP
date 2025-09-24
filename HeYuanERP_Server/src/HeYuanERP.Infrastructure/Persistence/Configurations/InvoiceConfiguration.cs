using HeYuanERP.Domain.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 发票实体映射配置。
/// </summary>
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Number)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(i => i.CustomerId)
            .IsRequired();

        builder.Property(i => i.CustomerName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(i => i.SourceType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.SourceId);

        builder.Property(i => i.SourceNumber)
            .HasMaxLength(64);

        builder.Property(i => i.DefaultTaxRate)
            .HasPrecision(9, 6);

        builder.Property(i => i.SubtotalExcludingTax)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.TotalTaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.GrandTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.IssuedAt);

        builder.Property(i => i.IsElectronic)
            .IsRequired();

        builder.Property(i => i.Remark)
            .HasMaxLength(1024);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        // 电子发票信息作为拥有实体（Owned Entity），映射为同表字段。
        builder.OwnsOne(i => i.EInvoice, nav =>
        {
            nav.Property(p => p.InvoiceCode).HasMaxLength(64).HasColumnName("EInvoice_InvoiceCode");
            nav.Property(p => p.InvoiceNumber).HasMaxLength(64).HasColumnName("EInvoice_InvoiceNumber");
            nav.Property(p => p.CheckCode).HasMaxLength(64).HasColumnName("EInvoice_CheckCode");
            nav.Property(p => p.PdfUrl).HasMaxLength(512).HasColumnName("EInvoice_PdfUrl");
            nav.Property(p => p.ViewUrl).HasMaxLength(512).HasColumnName("EInvoice_ViewUrl");
            nav.Property(p => p.QrCodeUrl).HasMaxLength(512).HasColumnName("EInvoice_QrCodeUrl");
            nav.Property(p => p.ElectronicIssuedAt).HasColumnName("EInvoice_ElectronicIssuedAt");
            nav.Property(p => p.BuyerTaxId).HasMaxLength(32).HasColumnName("EInvoice_BuyerTaxId");
            nav.Property(p => p.SellerTaxId).HasMaxLength(32).HasColumnName("EInvoice_SellerTaxId");
        });

        // 关系：一对多（发票-明细）
        builder.HasMany(i => i.Items)
            .WithOne()
            .HasForeignKey(it => it.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // 索引
        builder.HasIndex(i => i.Number).IsUnique();
        builder.HasIndex(i => i.CustomerId);
        builder.HasIndex(i => new { i.SourceType, i.SourceId });
        builder.HasIndex(i => i.Status);
    }
}

