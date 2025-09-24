using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HeYuanERP.Api.Tests;

// 集成测试样例：使用 InMemory 数据库验证 Account 的 CRUD
public class AccountsCrudTests : IClassFixture<AccountsCrudTests.CustomFactory>
{
    private readonly CustomFactory _factory;

    public AccountsCrudTests(CustomFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Accounts_Crud_Works_WithInMemoryDb()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create
        var acc = new Account { Code = $"C{Guid.NewGuid():N}", Name = "测试客户A", Active = true };
        db.Accounts.Add(acc);
        await db.SaveChangesAsync();

        // Read
        var found = await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Code == acc.Code);
        Assert.NotNull(found);
        Assert.Equal("测试客户A", found!.Name);

        // Update
        found!.Name = "测试客户A-修改";
        db.Accounts.Update(found);
        await db.SaveChangesAsync();

        var updated = await db.Accounts.AsNoTracking().FirstAsync(a => a.Id == acc.Id);
        Assert.Equal("测试客户A-修改", updated.Name);

        // Delete
        db.Accounts.Remove(updated);
        await db.SaveChangesAsync();

        var missing = await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == acc.Id);
        Assert.Null(missing);
    }

    // 自定义工厂：覆盖 DbContext 为 InMemory，以便在 CI 中运行
    public class CustomFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                // 移除默认 SQL Server 注册
                var descriptors = services.Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) || d.ServiceType == typeof(AppDbContext)).ToList();
                foreach (var d in descriptors) services.Remove(d);

                // 使用 InMemory 数据库
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseInMemoryDatabase($"AccountsCrudTests_{Guid.NewGuid():N}"));
            });
        }
    }
}

