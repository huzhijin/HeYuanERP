using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

/// <summary>
/// Payment 实体的最小 Fluent 配置（与当前领域模型对齐）。
/// 注意：最终映射在 Infrastructure 的 DbContext 中统一配置；
/// 该类仅保留以兼容历史文件结构，不作为主要入口。
/// </summary>
public class PaymentConfig : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("Payments");
        b.HasKey(x => x.Id);
        b.Property(x => x.Method).IsRequired().HasMaxLength(50);
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Remark).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(100);
        b.Property(x => x.OrgId).HasMaxLength(64);
        b.Property(x => x.RowVersion).IsRowVersion();
    }
}
