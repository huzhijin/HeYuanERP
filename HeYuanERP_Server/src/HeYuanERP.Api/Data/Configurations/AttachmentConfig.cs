using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// Attachment 实体的 Fluent 配置
public class AttachmentConfig : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> b)
    {
        b.ToTable("Attachments");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.BusinessType).IsRequired();
        b.Property(x => x.BusinessEntityId).HasMaxLength(64);
        b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        b.Property(x => x.ContentType).HasMaxLength(100);
        b.Property(x => x.StoragePath).IsRequired().HasMaxLength(500);
        b.Property(x => x.FileHash).HasMaxLength(64);
        b.Property(x => x.UploadedBy).HasMaxLength(50);

        b.HasIndex(x => new { x.BusinessType, x.BusinessEntityId });

        // EF Core 无法直接映射 Dictionary<string, object> 到关系型列，忽略并按需在应用层处理（或后续改为 JSON 列）
        b.Ignore(x => x.ExtendedProperties);

        // 可选：标签如需持久化可改为 JSON 字符串转换；当前最小运行场景下先忽略
        b.Ignore(x => x.Tags);
    }
}
