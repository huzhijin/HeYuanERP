using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// ReportSnapshot 实体的 Fluent 配置
public class ReportSnapshotConfig : IEntityTypeConfiguration<ReportSnapshot>
{
    public void Configure(EntityTypeBuilder<ReportSnapshot> b)
    {
        b.ToTable("ReportSnapshots");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        // JSON 大字段：不限定列类型，
        // - SQL Server 将默认映射为 nvarchar(max)
        // - SQLite 将映射为 TEXT（兼容）
        // 避免在 SQLite 下使用 nvarchar(max) 造成语法错误
        b.Property(x => x.ParamsJson);
        b.Property(x => x.Status).IsRequired().HasMaxLength(20);
        b.Property(x => x.FileUri).HasMaxLength(500);
        b.Property(x => x.Message).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(50);

        b.HasIndex(x => new { x.Name, x.CreatedAt });
    }
}
