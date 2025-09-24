using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Data;

// 数据播种器：创建数据库并注入初始用户/角色/权限（开发期）
public class DbSeeder
{
    private readonly AppDbContext _db;

    public DbSeeder(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 在开发环境自动执行：应用迁移并注入最小可用的 Auth 数据。
    /// 生产环境建议使用迁移与独立初始化流程（禁止自动播种敏感账号）。
    /// </summary>
    public async Task SeedAsync(CancellationToken ct = default)
    {
        // 根据提供程序选择迁移/建库
        var provider = _db.Database.ProviderName ?? string.Empty;
        if (provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            // SQLite：使用 EnsureCreated（避免使用面向 SQL Server 的迁移导致不兼容）
            await _db.Database.EnsureCreatedAsync(ct);
        }
        else
        {
            // 默认：SQL Server 走迁移
            await _db.Database.MigrateAsync(ct);
        }

        // 若已有用户与角色，则认为已初始化
        if (await _db.Roles.AnyAsync(ct) || await _db.Users.AnyAsync(ct))
        {
            return;
        }

        // 种子：角色
        var roleAdmin = new Role { Code = "Admin", Name = "系统管理员" };
        var roleSales = new Role { Code = "Sales", Name = "销售员" };
        _db.Roles.AddRange(roleAdmin, roleSales);

        // 种子：权限（按“资源.动作”编码）
        var perms = new[]
        {
            // Accounts
            new Permission { Code = "accounts.read", Name = "客户查看" },
            new Permission { Code = "accounts.create", Name = "客户新增/编辑" },
            new Permission { Code = "accounts.share", Name = "客户共享" },
            new Permission { Code = "accounts.transfer", Name = "客户转移" },

            // Orders
            new Permission { Code = "orders.read", Name = "订单查看" },
            new Permission { Code = "orders.create", Name = "订单新增" },
            new Permission { Code = "orders.confirm", Name = "订单确认" },
            new Permission { Code = "orders.reverse", Name = "订单反审" },

            // Logistics
            new Permission { Code = "deliveries.read", Name = "送货单查看" },
            new Permission { Code = "deliveries.create", Name = "送货单创建" },
            new Permission { Code = "deliveries.print", Name = "送货单打印" },
            new Permission { Code = "returns.read", Name = "退货单查看" },
            new Permission { Code = "returns.create", Name = "退货单创建" },

            // Purchase
            new Permission { Code = "po.read", Name = "采购单查看" },
            new Permission { Code = "po.create", Name = "采购单新增" },

            // Inventory
            new Permission { Code = "inventory.read", Name = "库存查看" },

            // Finance
            new Permission { Code = "invoices.read", Name = "发票查看" },
            new Permission { Code = "payments.read", Name = "收款查看" },

            // Report & Print
            new Permission { Code = "reports.read", Name = "报表查看" },
            new Permission { Code = "print.read", Name = "打印权限" },

            // Orders - 状态机细分权限
            new Permission { Code = "orders.submit", Name = "订单提交" },
            new Permission { Code = "orders.production", Name = "订单生产" },
            new Permission { Code = "orders.delivery", Name = "订单发货标记" },
            new Permission { Code = "orders.invoice", Name = "订单开票标记" },
            new Permission { Code = "orders.close", Name = "订单关闭" },
            new Permission { Code = "orders.cancel", Name = "订单取消" },

            // Invoice 对账与验证
            new Permission { Code = "invoice.read", Name = "发票读取（对账/验证）" },
            new Permission { Code = "invoice.reconcile", Name = "发票对账" },
            new Permission { Code = "invoice.validate", Name = "发票校验" },
            new Permission { Code = "invoice.create", Name = "发票创建" },
            new Permission { Code = "invoice.cancel", Name = "发票作废" },

            // 附件管理
            new Permission { Code = "attachments.read", Name = "附件读取" },
            new Permission { Code = "attachments.upload", Name = "附件上传" },
            new Permission { Code = "attachments.download", Name = "附件下载" },
            new Permission { Code = "attachments.edit", Name = "附件编辑" },
            new Permission { Code = "attachments.delete", Name = "附件删除" },
            new Permission { Code = "attachments.admin", Name = "附件管理（清理等）" },

            // 库存预警与调整
            new Permission { Code = "inventory.adjust", Name = "库存调整/预警处理" }
        };
        _db.Permissions.AddRange(perms);

        await _db.SaveChangesAsync(ct);

        // 角色权限：Admin 拥有全部，Sales 拥有常用权限
        var allPermIds = perms.Select(p => p.Id).ToHashSet();
        _db.RolePermissions.AddRange(allPermIds.Select(pid => new RolePermission
        {
            RoleId = roleAdmin.Id,
            PermissionId = pid
        }));
        var salesPermCodes = new[]
        {
            "accounts.read",
            "orders.read","orders.create",
            "deliveries.read","deliveries.create",
            "returns.read","returns.create",
            "inventory.read",
            "print.read",
            // 常用：订单提交/生产、附件上传/下载、预警查看
            "orders.submit","orders.production",
            "attachments.read","attachments.upload","attachments.download"
        }.ToHashSet();
        _db.RolePermissions.AddRange(perms.Where(p => salesPermCodes.Contains(p.Code)).Select(p => new RolePermission
        {
            RoleId = roleSales.Id,
            PermissionId = p.Id
        }));

        await _db.SaveChangesAsync(ct);

        // 管理员账号（密码哈希从环境变量读取，未提供则使用占位值，后续请修改）
        var adminLoginId = Environment.GetEnvironmentVariable("ADMIN_LOGIN_ID") ?? "admin";
        var adminName = Environment.GetEnvironmentVariable("ADMIN_NAME") ?? "系统管理员";
        var adminPwdHash = Environment.GetEnvironmentVariable("ADMIN_PASSWORD_HASH") ?? "CHANGE_ME";

        var admin = new User
        {
            LoginId = adminLoginId,
            Name = adminName,
            PasswordHash = adminPwdHash,
            Active = true
        };
        _db.Users.Add(admin);
        await _db.SaveChangesAsync(ct);

        _db.UserRoles.Add(new UserRole
        {
            UserId = admin.Id,
            RoleId = roleAdmin.Id
        });

        await _db.SaveChangesAsync(ct);

        // 可选：播种演示业务数据（SQLite 或开启 DEV_DEMO_DATA=1 时）
        if (provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Environment.GetEnvironmentVariable("DEV_DEMO_DATA"), "1", StringComparison.Ordinal))
        {
            await SeedDemoDataAsync(ct);
        }
    }

    private async Task SeedDemoDataAsync(CancellationToken ct)
    {
        // 产品
        if (!await _db.Products.AnyAsync(ct))
        {
            _db.Products.Add(new Product
            {
                Id = "P001",
                Code = "P001",
                Name = "演示产品",
                Unit = "PCS",
                DefaultPrice = 100,
                Active = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // 供应商（沿用 Account 实体）
        if (!await _db.Accounts.AnyAsync(ct))
        {
            _db.Accounts.Add(new Account
            {
                Id = "V001",
                Code = "V001",
                Name = "演示供应商",
                Active = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // 仓库/库位
        if (!await _db.Warehouses.AnyAsync(ct))
        {
            var wh = new Warehouse { Code = "WH1", Name = "演示仓库", Active = true, CreatedAt = DateTime.UtcNow };
            _db.Warehouses.Add(wh);
            await _db.SaveChangesAsync(ct);

            _db.Locations.Add(new Location { WarehouseId = wh.Id, Code = "A01", Name = "默认库位", Active = true, CreatedAt = DateTime.UtcNow });
        }

        await _db.SaveChangesAsync(ct);

        // 演示库存：给 P001 在 WH1/A01 加一点现存量，便于库存汇总直观看到
        var prod = await _db.Products.FirstOrDefaultAsync(p => p.Id == "P001", ct);
        var whse = await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(ct);
        if (prod != null && whse != null)
        {
            var loc = await _db.Locations.AsNoTracking().FirstOrDefaultAsync(l => l.WarehouseId == whse.Id, ct);
            var exists = await _db.InventoryBalances.AnyAsync(b => b.ProductId == prod.Id && b.Whse == whse.Code && b.Loc == (loc != null ? loc.Code : null), ct);
            if (!exists)
            {
                _db.InventoryBalances.Add(new InventoryBalance
                {
                    ProductId = prod.Id,
                    Whse = whse.Code,
                    Loc = loc?.Code,
                    OnHand = 50,
                    Reserved = 0,
                    Available = 50,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "seed"
                });
                _db.InventoryTxns.Add(new InventoryTxn
                {
                    TxnCode = "IN",
                    ProductId = prod.Id,
                    Qty = 50,
                    Whse = whse.Code,
                    Loc = loc?.Code,
                    TxnDate = DateTime.UtcNow.Date,
                    RefType = "seed",
                    RefId = Guid.NewGuid().ToString("N"),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                });
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
