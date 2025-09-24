using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Interfaces.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Infrastructure.Storage;

/// <summary>
/// 本地文件存储实现（用于开发/测试环境）。
/// 环境变量：
/// - HEYUANERP_FILE_STORAGE_ROOT：文件根目录（绝对路径），默认：应用运行目录下的 storage/。
/// - HEYUANERP_FILE_BASE_URL：公共基础 URL（如通过 Nginx 映射 storage），用于拼接可公开访问的地址；若未配置则返回 null。
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private readonly ILogger<LocalFileStorage> _logger;
    private readonly string _root;
    private readonly string? _publicBaseUrl;

    public LocalFileStorage(IConfiguration configuration, ILogger<LocalFileStorage> logger)
    {
        _logger = logger;
        _root = configuration["HEYUANERP_FILE_STORAGE_ROOT"]
                ?? Environment.GetEnvironmentVariable("HEYUANERP_FILE_STORAGE_ROOT")
                ?? Path.Combine(AppContext.BaseDirectory, "storage");
        _publicBaseUrl = configuration["HEYUANERP_FILE_BASE_URL"]
                ?? Environment.GetEnvironmentVariable("HEYUANERP_FILE_BASE_URL");

        Directory.CreateDirectory(_root);
        _logger.LogInformation("本地文件存储初始化，根目录：{Root}", _root);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct = default)
    {
        // 以日期分桶：payments/yyyy/MM/dd
        var today = DateTime.UtcNow;
        var folder = Path.Combine("payments", today.ToString("yyyy"), today.ToString("MM"), today.ToString("dd"));
        var ext = Path.GetExtension(fileName);
        var safeExt = string.IsNullOrWhiteSpace(ext) ? string.Empty : ext;
        var newName = $"{Guid.NewGuid():N}{safeExt}";
        var storageKey = Path.Combine(folder, newName).Replace('\\', '/');

        var physicalFolder = Path.Combine(_root, folder);
        Directory.CreateDirectory(physicalFolder);
        var physicalPath = Path.Combine(physicalFolder, newName);

        _logger.LogDebug("开始保存文件：{FileName} => {Path}", fileName, physicalPath);

        const int bufferSize = 81920; // 80KB 默认缓冲
        await using (var fs = new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
        {
            await content.CopyToAsync(fs, bufferSize, ct);
        }

        _logger.LogInformation("文件已保存：{StorageKey} ({Size} bytes)", storageKey, TryGetLength(content));
        return storageKey.Replace('\\', '/');
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct = default)
    {
        var path = MapToPhysicalPath(storageKey);
        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task<bool> DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        var path = MapToPhysicalPath(storageKey);
        try
        {
            if (!File.Exists(path)) return Task.FromResult(true);
            File.Delete(path);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "删除文件失败：{StorageKey}", storageKey);
            return Task.FromResult(false);
        }
    }

    public Task<string?> GetPublicUrlAsync(string storageKey, TimeSpan? expires = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_publicBaseUrl)) return Task.FromResult<string?>(null);
        var url = _publicBaseUrl!.TrimEnd('/') + "/" + storageKey.Replace('\\', '/');
        return Task.FromResult<string?>(url);
    }

    private string MapToPhysicalPath(string storageKey)
    {
        var cleaned = storageKey.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        // 防止目录穿越
        if (cleaned.Contains(".."))
        {
            throw new InvalidOperationException("非法的存储键（包含目录穿越）");
        }
        var path = Path.Combine(_root, cleaned);
        return path;
    }

    private static long TryGetLength(Stream stream)
    {
        try { return stream.Length; } catch { return -1; }
    }
}
