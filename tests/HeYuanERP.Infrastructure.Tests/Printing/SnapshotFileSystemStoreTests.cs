using System;
using System.IO;
using System.Threading.Tasks;
using HeYuanERP.Domain.Printing;
using HeYuanERP.Domain.Printing.Models;
using HeYuanERP.Infrastructure.Printing.Snapshot;
using Microsoft.Extensions.Options;
using Xunit;

namespace HeYuanERP.Infrastructure.Tests.Printing;

/// <summary>
/// PrintSnapshotFileSystemStore 测试：验证保存与读取。
/// </summary>
public class SnapshotFileSystemStoreTests
{
    [Fact]
    public async Task Should_Save_And_Load_Snapshot()
    {
        var root = Path.Combine(Path.GetTempPath(), "heyuanerp-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var store = new PrintSnapshotFileSystemStore(Options.Create(new PrintOptions { SnapshotsRoot = root }));
            var snapshot = new PrintSnapshot
            {
                DocType = "order",
                Id = "ABC123",
                TemplateName = "default",
                DataJson = "{\"hello\":\"world\"}",
                DataHash = "hash123",
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            await store.SaveAsync(snapshot);
            var loaded = await store.TryGetAsync("order", "ABC123", "default");

            Assert.NotNull(loaded);
            Assert.Equal(snapshot.DocType, loaded!.DocType);
            Assert.Equal(snapshot.Id, loaded.Id);
            Assert.Equal(snapshot.TemplateName, loaded.TemplateName);
            Assert.Equal(snapshot.DataJson, loaded.DataJson);
            Assert.Equal(snapshot.DataHash, loaded.DataHash);
        }
        finally
        {
            try { Directory.Delete(root, recursive: true); } catch { /* 忽略清理失败 */ }
        }
    }
}

