using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Application.Interfaces.Storage;

/// <summary>
/// 文件存储抽象：用于保存与读取附件文件。
/// 实现可为本地磁盘、对象存储（如 S3/OSS）等，由环境变量配置。
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// 保存文件并返回存储键（相对路径或对象键）。
    /// </summary>
    /// <param name="content">文件流</param>
    /// <param name="fileName">原始文件名</param>
    /// <param name="contentType">MIME 类型</param>
    /// <param name="ct">取消令牌</param>
    Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct = default);

    /// <summary>
    /// 读取文件内容（只读流）。
    /// </summary>
    Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct = default);

    /// <summary>
    /// 删除文件（幂等）。
    /// </summary>
    Task<bool> DeleteAsync(string storageKey, CancellationToken ct = default);

    /// <summary>
    /// 获取可公开访问的 URL（如实现支持），可带过期时间。
    /// </summary>
    Task<string?> GetPublicUrlAsync(string storageKey, TimeSpan? expires = null, CancellationToken ct = default);
}

