using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// AccountVisit 实体的 Fluent 配置
public class AccountVisitConfig : IEntityTypeConfiguration<AccountVisit>
{
    public void Configure(EntityTypeBuilder<AccountVisit> b)
    {
        b.ToTable("AccountVisits");
        b.HasKey(x => x.Id);

        // 字段约束
        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.AccountId).IsRequired().HasMaxLength(64);
        b.Property(x => x.ContactId).HasMaxLength(64);
        b.Property(x => x.VisitorId).HasMaxLength(64);
        b.Property(x => x.Subject).HasMaxLength(100);
        b.Property(x => x.Location).HasMaxLength(200);
        b.Property(x => x.Result).HasMaxLength(200);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        // 索引：按客户与时间、按拜访人与时间查询
        b.HasIndex(x => new { x.AccountId, x.VisitDate });
        b.HasIndex(x => new { x.VisitorId, x.VisitDate });

        // 关系：Account 1 - N AccountVisit
        b.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // 关系：Contact（可空）删除时置空
        b.HasOne(x => x.Contact)
            .WithMany()
            .HasForeignKey(x => x.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        // 关系：User（拜访人，可空）删除时置空
        b.HasOne(x => x.Visitor)
            .WithMany()
            .HasForeignKey(x => x.VisitorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

