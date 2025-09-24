using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// AccountShare 实体的 Fluent 配置
public class AccountShareConfig : IEntityTypeConfiguration<AccountShare>
{
    public void Configure(EntityTypeBuilder<AccountShare> b)
    {
        b.ToTable("AccountShares");
        b.HasKey(x => x.Id);

        // 字段约束
        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.AccountId).IsRequired().HasMaxLength(64);
        b.Property(x => x.TargetUserId).IsRequired().HasMaxLength(64);
        b.Property(x => x.Permission).IsRequired().HasMaxLength(20);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        // 唯一：同一客户对同一用户仅一条共享记录
        b.HasIndex(x => new { x.AccountId, x.TargetUserId }).IsUnique();

        // 关系：Account 1 - N AccountShare
        b.HasOne(x => x.Account)
            .WithMany() // 不在 Account 上暴露集合导航，减少耦合
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // 关系：User 1 - N AccountShare（目标用户）
        b.HasOne(x => x.TargetUser)
            .WithMany()
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

