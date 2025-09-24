using System.Security.Cryptography;

namespace HeYuanERP.Api.Services.Attachments;

// 本地附件存储实现：以目录分层（refType/refId）组织文件
// - 根目录从环境变量 FILE_STORAGE_ROOT 读取，若为空则使用进程工作目录下 uploads
// - storageUri 采用 file:// 前缀，内容为绝对路径（不建议直接对外暴露）
public class LocalAttachmentStorage : IAttachmentStorage
{
    private readonly string _root;

    public LocalAttachmentStorage(string? root = null)
    {
        _root = string.IsNullOrWhiteSpace(root)
            ? Path.Combine(AppContext.BaseDirectory, "uploads")
            : root!;
        Directory.CreateDirectory(_root);
    }

    public async Task<(string storageUri, string? sha256)> SaveAsync(Stream content, string fileName, string refType, string refId, string? contentType, CancellationToken ct)
    {
        // 目录：{root}/{refType}/{refId}/
        var safeRefType = Sanitize(refType);
        var safeRefId = Sanitize(refId);
        var dir = Path.Combine(_root, safeRefType, safeRefId);
        Directory.CreateDirectory(dir);

        var ext = Path.GetExtension(fileName);
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        var safeName = Sanitize(baseName);
        var finalName = $"{DateTime.UtcNow:yyyyMMdd_HHmmssfff}_{safeName}{ext}";
        var fullPath = Path.Combine(dir, finalName);

        // 写入并计算 sha256
        string? sha256 = null;
        using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
        {
            using var sha = SHA256.Create();
            var hashStream = new CryptoStream(fs, sha, CryptoStreamMode.Write);
            await content.CopyToAsync(hashStream, ct);
            await hashStream.FlushAsync(ct);
            hashStream.FlushFinalBlock();
            sha256 = Convert.ToHexString(sha.Hash!).ToLowerInvariant();
        }

        var uri = $"file://{fullPath}";
        return (uri, sha256);
    }

    public Task DeleteAsync(string storageUri, CancellationToken ct)
    {
        try
        {
            var path = GetPhysicalPath(storageUri);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // 忽略删除异常（可根据需要打日志）
        }
        return Task.CompletedTask;
    }

    public string? GetPhysicalPath(string storageUri)
    {
        if (string.IsNullOrWhiteSpace(storageUri)) return null;
        if (storageUri.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
        {
            return storageUri.Substring("file://".Length);
        }
        // 兼容直接传绝对路径
        if (Path.IsPathRooted(storageUri)) return storageUri;
        return null;
    }

    private static string Sanitize(string name)
    {
        // 仅保留字母、数字、下划线与中划线
        var chars = name.Select(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-' ? ch : '_').ToArray();
        var s = new string(chars);
        return string.IsNullOrWhiteSpace(s) ? "_" : s;
    }
}

