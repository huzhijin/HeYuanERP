using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeYuanERP.Api.Data.Configurations;

// Account 实体的 Fluent 配置
public class AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("Accounts");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasMaxLength(64);
        b.Property(x => x.Code).IsRequired().HasMaxLength(50);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.OwnerId).HasMaxLength(64);
        b.Property(x => x.TaxNo).HasMaxLength(50);
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(50);
        b.Property(x => x.UpdatedBy).HasMaxLength(50);

        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.Name);

        // 关系：Account 1 - N Contact
        b.HasMany(x => x.Contacts)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

