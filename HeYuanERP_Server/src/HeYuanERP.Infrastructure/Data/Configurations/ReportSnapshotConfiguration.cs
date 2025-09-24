// 版权所有(c) HeYuanERP
// 说明：ReportSnapshot 的 EF Core 实体配置（中文注释）。

using HeYuanERP.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Infrastructure.Data.Configurations;

/// <summary>
/// ReportSnapshot 配置。
/// </summary>
public class ReportSnapshotConfiguration : IEntityTypeConfiguration<ReportSnapshot>
{
    public void Configure(EntityTypeBuilder<ReportSnapshot> builder)
    {
        builder.ToTable("ReportSnapshots");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.ParametersJson).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.FileUri).IsRequired().HasMaxLength(1024);
        builder.Property(x => x.ParamHash).HasMaxLength(64);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(64);
        builder.Property(x => x.ClientIp).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(256);
        builder.Property(x => x.CorrelationId).HasMaxLength(64);

        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => new { x.Type, x.ParamHash });
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}

