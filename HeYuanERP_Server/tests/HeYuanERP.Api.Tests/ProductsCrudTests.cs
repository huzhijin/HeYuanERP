using HeYuanERP.Api.Data;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HeYuanERP.Api.Tests;

// 集成测试样例：使用 InMemory 数据库验证 Product 的 CRUD
public class ProductsCrudTests : IClassFixture<ProductsCrudTests.CustomFactory>
{
    private readonly CustomFactory _factory;

    public ProductsCrudTests(CustomFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Products_Crud_Works_WithInMemoryDb()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create
        var p = new Product { Code = $"P{Guid.NewGuid():N}", Name = "测试物料A", Unit = "PCS", DefaultPrice = 12.34m };
        db.Products.Add(p);
        await db.SaveChangesAsync();

        // Read
        var found = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Code == p.Code);
        Assert.NotNull(found);
        Assert.Equal("测试物料A", found!.Name);

        // Update
        found!.Name = "测试物料A-修改";
        db.Products.Update(found);
        await db.SaveChangesAsync();

        var updated = await db.Products.AsNoTracking().FirstAsync(x => x.Id == p.Id);
        Assert.Equal("测试物料A-修改", updated.Name);

        // Delete
        db.Products.Remove(updated);
        await db.SaveChangesAsync();

        var missing = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == p.Id);
        Assert.Null(missing);
    }

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
                    opt.UseInMemoryDatabase($"ProductsCrudTests_{Guid.NewGuid():N}"));
            });
        }
    }
}

