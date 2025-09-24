// 版权所有(c) HeYuanERP
// 说明：ReportJob 的 EF Core 实体配置（中文注释）。

using HeYuanERP.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Infrastructure.Data.Configurations;

/// <summary>
/// ReportJob 配置。
/// </summary>
public class ReportJobConfiguration : IEntityTypeConfiguration<ReportJob>
{
    public void Configure(EntityTypeBuilder<ReportJob> builder)
    {
        builder.ToTable("ReportJobs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.Format).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        builder.Property(x => x.ParametersJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.FileUri).HasMaxLength(1024);
        builder.Property(x => x.ErrorMessage).HasMaxLength(2048);
        builder.Property(x => x.CorrelationId).HasMaxLength(64);
        builder.Property(x => x.CreatedBy).HasMaxLength(64);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.StartedAtUtc);
        builder.Property(x => x.CompletedAtUtc);

        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}

