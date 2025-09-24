using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Printing;
using HeYuanERP.Domain.Printing;
using HeYuanERP.Domain.Printing.Models;
using Microsoft.Extensions.Options;

namespace HeYuanERP.Infrastructure.Printing.Snapshot;

/// <summary>
/// 打印快照文件系统存储：结构为 {SnapshotsRoot}/{docType}/{id}/{template}.json
/// </summary>
public class PrintSnapshotFileSystemStore : IPrintSnapshotStore
{
    private readonly PrintOptions _options;

    public PrintSnapshotFileSystemStore(IOptions<PrintOptions> options)
    {
        _options = options.Value;
    }

    public async Task SaveAsync(PrintSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        var (dir, file) = GetPaths(snapshot.DocType, snapshot.Id, snapshot.TemplateName);
        Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(file, json, Encoding.UTF8, cancellationToken);
    }

    public async Task<PrintSnapshot?> TryGetAsync(string docType, string id, string templateName, CancellationToken cancellationToken = default)
    {
        var (_, file) = GetPaths(docType, id, templateName);
        if (!File.Exists(file)) return null;
        var json = await File.ReadAllTextAsync(file, Encoding.UTF8, cancellationToken);
        try
        {
            return JsonSerializer.Deserialize<PrintSnapshot>(json);
        }
        catch
        {
            return null;
        }
    }

    private (string dir, string file) GetPaths(string docType, string id, string templateName)
    {
        var root = _options.SnapshotsRoot ?? Path.Combine(AppContext.BaseDirectory, "var", "print-snapshots");
        var safeDoc = docType.Trim().ToLowerInvariant();
        var safeTpl = templateName.Trim().ToLowerInvariant();
        var dir = Path.Combine(root, safeDoc, id);
        var file = Path.Combine(dir, $"{safeTpl}.json");
        return (dir, file);
    }
}

