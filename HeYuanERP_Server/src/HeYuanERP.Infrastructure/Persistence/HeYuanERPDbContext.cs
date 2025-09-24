using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Reports;
using ReportSnapshotEntity = HeYuanERP.Domain.Reports.ReportSnapshot;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Infrastructure.Persistence;

/// <summary>
/// EF Core 数据上下文（SQLite 临时方案）。
/// 说明：为最小可运行验证，先覆盖常用实体；后续可逐步完善各模块映射。
/// </summary>
public class HeYuanERPDbContext : DbContext
{
    public HeYuanERPDbContext(DbContextOptions<HeYuanERPDbContext> options) : base(options) { }

    // 原有实体
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAttachment> PaymentAttachments => Set<PaymentAttachment>();
    public DbSet<ReportJob> ReportJobs => Set<ReportJob>();
    public DbSet<ReportSnapshotEntity> ReportSnapshots => Set<ReportSnapshotEntity>();

    // P1阶段新增实体
    // CRM模块
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<SalesOpportunity> SalesOpportunities => Set<SalesOpportunity>();
    public DbSet<CustomerVisit> CustomerVisits => Set<CustomerVisit>();
    public DbSet<SalesTarget> SalesTargets => Set<SalesTarget>();
    public DbSet<CustomerValueAnalysis> CustomerValueAnalyses => Set<CustomerValueAnalysis>();

    // 产品价格模块
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<PriceStrategy> PriceStrategies => Set<PriceStrategy>();
    public DbSet<PriceRequest> PriceRequests => Set<PriceRequest>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();

    // 对账凭证模块
    public DbSet<ReconciliationRecord> ReconciliationRecords => Set<ReconciliationRecord>();
    public DbSet<ReconciliationItem> ReconciliationItems => Set<ReconciliationItem>();
    public DbSet<ReconciliationDifference> ReconciliationDifferences => Set<ReconciliationDifference>();
    public DbSet<AdjustmentEntry> AdjustmentEntries => Set<AdjustmentEntry>();
    public DbSet<AdjustmentEntryLine> AdjustmentEntryLines => Set<AdjustmentEntryLine>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();

    // 费用管理模块
    public DbSet<ExpenseRequest> ExpenseRequests => Set<ExpenseRequest>();
    public DbSet<ExpenseRequestLine> ExpenseRequestLines => Set<ExpenseRequestLine>();

    // 应收应付模块
    public DbSet<AccountReceivable> AccountsReceivable => Set<AccountReceivable>();
    public DbSet<AccountPayable> AccountsPayable => Set<AccountPayable>();
    public DbSet<CollectionRecord> CollectionRecords => Set<CollectionRecord>();
    public DbSet<PaymentApplication> PaymentApplications => Set<PaymentApplication>();
    public DbSet<PaymentSchedule> PaymentSchedules => Set<PaymentSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Payment 简要映射（遵循领域模型当前字段）
        modelBuilder.Entity<Payment>(b =>
        {
            b.ToTable("Payments");
            b.HasKey(x => x.Id);
            b.Property(x => x.Method).IsRequired().HasMaxLength(50);
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.Remark).HasMaxLength(500);
            b.Property(x => x.CreatedBy).HasMaxLength(100);
            b.Property(x => x.OrgId).HasMaxLength(64);
            b.Property(x => x.RowVersion).IsRowVersion();
            b.HasMany(x => x.Attachments).WithOne().HasForeignKey("PaymentId").OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PaymentAttachment>(b =>
        {
            b.ToTable("PaymentAttachments");
            b.HasKey(x => x.Id);
            b.Property(x => x.FileName).HasMaxLength(255);
            b.Property(x => x.ContentType).HasMaxLength(100);
            b.Property(x => x.StorageKey).HasMaxLength(500);
        });

        modelBuilder.Entity<ReportJob>(b =>
        {
            b.ToTable("ReportJobs");
            b.HasKey(x => x.Id);
            b.Property(x => x.ParametersJson).IsRequired();
            b.Property(x => x.FileUri).HasMaxLength(1000);
            b.Property(x => x.ErrorMessage).HasMaxLength(1000);
            b.Property(x => x.CorrelationId).HasMaxLength(128);
            b.Property(x => x.CreatedBy).HasMaxLength(100);
            b.Property(x => x.RowVersion).IsRowVersion();
        });

        modelBuilder.Entity<ReportSnapshotEntity>(b =>
        {
            b.ToTable("ReportSnapshots");
            b.HasKey(x => x.Id);
            b.Property(x => x.ParametersJson).IsRequired();
            b.Property(x => x.FileUri).IsRequired().HasMaxLength(1000);
            b.Property(x => x.ParamHash).HasMaxLength(128);
            b.Property(x => x.CreatedBy).HasMaxLength(100);
            b.Property(x => x.ClientIp).HasMaxLength(64);
            b.Property(x => x.UserAgent).HasMaxLength(256);
            b.Property(x => x.CorrelationId).HasMaxLength(128);
            b.Property(x => x.RowVersion).IsRowVersion();
            b.HasIndex(x => new { x.Type, x.ParamHash });
        });
    }
}
