using HeYuanERP.Domain.Entities;
using HeYuanERP.Domain.Invoices;
using HeYuanERP.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Data;

// 应用数据库上下文（认证/授权模块）：
// - 连接 SQL Server（连接串来自环境变量 DB_CONNECTION 或 appsettings.json）
// - 定义用户/角色/权限与多对多关联表
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 实体集（Auth）
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // 实体集（主数据/业务单据/库存/财务）
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<ReportSnapshot> ReportSnapshots => Set<ReportSnapshot>();

    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryLine> DeliveryLines => Set<DeliveryLine>();
    public DbSet<Return> Returns => Set<Return>();
    public DbSet<ReturnLine> ReturnLines => Set<ReturnLine>();

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<POLine> POLines => Set<POLine>();
    public DbSet<POReceive> POReceives => Set<POReceive>();
    public DbSet<POReceiveLine> POReceiveLines => Set<POReceiveLine>();

    public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();
    public DbSet<InventoryTxn> InventoryTxns => Set<InventoryTxn>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();

    // 注意：此处的 Invoice 指向领域发票模型（HeYuanERP.Domain.Invoices.Invoice）
    public DbSet<HeYuanERP.Domain.Invoices.Invoice> Invoices => Set<HeYuanERP.Domain.Invoices.Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();

    // 新增：P0 模块实体集（库存预警/订单状态日志/采购异常/附件扩展/对账差异）
    public DbSet<InventoryAlert> InventoryAlerts => Set<InventoryAlert>();
    public DbSet<InventoryAlertRecord> InventoryAlertRecords => Set<InventoryAlertRecord>();
    public DbSet<OrderStatusLog> OrderStatusLogs => Set<OrderStatusLog>();
    public DbSet<PurchaseException> PurchaseExceptions => Set<PurchaseException>();
    public DbSet<AttachmentAccessRecord> AttachmentAccessRecords => Set<AttachmentAccessRecord>();
    public DbSet<AttachmentVersion> AttachmentVersions => Set<AttachmentVersion>();
    public DbSet<ReconciliationDifference> ReconciliationDifferences => Set<ReconciliationDifference>();
    public DbSet<PurchaseExceptionHandlingRecord> PurchaseExceptionHandlingRecords => Set<PurchaseExceptionHandlingRecord>();

    // 客户分级（暂未启用的服务使用）
    public DbSet<CustomerClassification> CustomerClassifications => Set<CustomerClassification>();
    public DbSet<CustomerClassificationHistory> CustomerClassificationHistories => Set<CustomerClassificationHistory>();
    public DbSet<CustomerClassificationRule> CustomerClassificationRules => Set<CustomerClassificationRule>();
    // Customers 别名（复用 Accounts）
    public DbSet<Account> Customers => Set<Account>();

    // 预留：核心通用配置（如默认模式、排序规则等）
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 用户
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasMaxLength(64);
            b.Property(x => x.LoginId).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
            b.Property(x => x.CreatedBy).HasMaxLength(50);
            b.Property(x => x.UpdatedBy).HasMaxLength(50);
            b.HasIndex(x => x.LoginId).IsUnique();
        });

        // 角色
        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasMaxLength(64);
            b.Property(x => x.Code).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.CreatedBy).HasMaxLength(50);
            b.Property(x => x.UpdatedBy).HasMaxLength(50);
            b.HasIndex(x => x.Code).IsUnique();
        });

        // 权限
        modelBuilder.Entity<Permission>(b =>
        {
            b.ToTable("Permissions");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasMaxLength(64);
            b.Property(x => x.Code).IsRequired().HasMaxLength(100);
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.CreatedBy).HasMaxLength(50);
            b.Property(x => x.UpdatedBy).HasMaxLength(50);
            b.HasIndex(x => x.Code).IsUnique();
        });

        // 用户-角色（多对多桥接）
        modelBuilder.Entity<UserRole>(b =>
        {
            b.ToTable("UserRoles");
            b.HasKey(x => new { x.UserId, x.RoleId });
            b.Property(x => x.UserId).HasMaxLength(64);
            b.Property(x => x.RoleId).HasMaxLength(64);
            b.HasOne(x => x.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 角色-权限（多对多桥接）
        modelBuilder.Entity<RolePermission>(b =>
        {
            b.ToTable("RolePermissions");
            b.HasKey(x => new { x.RoleId, x.PermissionId });
            b.Property(x => x.RoleId).HasMaxLength(64);
            b.Property(x => x.PermissionId).HasMaxLength(64);
            b.HasOne(x => x.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 应用同程序集的 Fluent 配置（主数据/业务/库存/财务等）
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        // 应用基础设施层中的实体配置（Invoice/InvoiceItem 等）
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceConfiguration).Assembly);

        // 最小表名/索引保证（无专用配置类时，确保包含于模型）
        modelBuilder.Entity<InventoryAlert>().ToTable("InventoryAlerts");
        modelBuilder.Entity<InventoryAlertRecord>().ToTable("InventoryAlertRecords");
        modelBuilder.Entity<OrderStatusLog>().ToTable("OrderStatusLogs");
        modelBuilder.Entity<PurchaseException>().ToTable("PurchaseExceptions");
        modelBuilder.Entity<AttachmentAccessRecord>().ToTable("AttachmentAccessRecords");
        modelBuilder.Entity<AttachmentVersion>().ToTable("AttachmentVersions");
        modelBuilder.Entity<ReconciliationDifference>().ToTable("ReconciliationDifferences");
        modelBuilder.Entity<PurchaseExceptionHandlingRecord>().ToTable("PurchaseExceptionHandlingRecords");
        modelBuilder.Entity<CustomerClassification>().ToTable("CustomerClassifications");
        modelBuilder.Entity<CustomerClassificationHistory>().ToTable("CustomerClassificationHistories");
        modelBuilder.Entity<CustomerClassificationRule>().ToTable("CustomerClassificationRules");

        // 复杂类型拥有者（拥有实体）
        modelBuilder.Entity<PurchaseException>().OwnsOne(x => x.QualityDetails);

        // 关系型数据库下忽略不可直接映射的字典属性（可后续改为 JSON 映射）
        modelBuilder.Entity<AttachmentAccessRecord>().Ignore(x => x.ExtendedInfo);
        modelBuilder.Entity<CustomerClassification>().Ignore(x => x.ScoreDetails);
        modelBuilder.Entity<CustomerClassificationHistory>().Ignore(x => x.ExtensionData);
    }
}
