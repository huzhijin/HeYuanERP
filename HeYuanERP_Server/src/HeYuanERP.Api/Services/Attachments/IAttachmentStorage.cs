namespace HeYuanERP.Api.Services.Attachments;

// 附件存储抽象：负责持久化二进制内容到本地或对象存储
// 说明：
// - 本阶段默认实现为本地文件系统（LocalAttachmentStorage）；
// - 接口返回的 storageUri 可为 file:// 或自定义 scheme，供后续预览/下载使用。
public interface IAttachmentStorage
{
    /// <summary>
    /// 保存文件流到存储，并返回 storageUri（以及可选 SHA256 校验）。
    /// </summary>
    /// <param name="content">文件数据流</param>
    /// <param name="fileName">原始文件名（用于保存时保留扩展名）</param>
    /// <param name="refType">业务类型（如：account、order）</param>
    /// <param name="refId">业务主键 Id</param>
    /// <param name="contentType">MIME 类型（可选）</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>返回 storageUri 与 sha256（可能为空）</returns>
    Task<(string storageUri, string? sha256)> SaveAsync(Stream content, string fileName, string refType, string refId, string? contentType, CancellationToken ct);

    /// <summary>
    /// 删除指定 storageUri 指向的文件（若存在）。
    /// </summary>
    Task DeleteAsync(string storageUri, CancellationToken ct);

    /// <summary>
    /// 将 storageUri 转换为本地物理路径（仅本地实现有效）。
    /// </summary>
    string? GetPhysicalPath(string storageUri);
}

