using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Application.Printing;

/// <summary>
/// 打印客户端接口（对接 P11 打印服务）。
/// 说明：使用 Headless Chromium 渲染，由 P11 服务负责模板与渲染。
/// </summary>
public interface IPrintClient
{
    /// <summary>
    /// 通过模板代码渲染 PDF 并返回字节流。
    /// </summary>
    /// <param name="templateCode">模板代码（由 P11 侧定义）。</param>
    /// <param name="model">模板数据模型（匿名对象或字典）。</param>
    /// <param name="ct">取消令牌。</param>
    Task<byte[]> RenderPdfAsync(string templateCode, object model, CancellationToken ct = default);
}

