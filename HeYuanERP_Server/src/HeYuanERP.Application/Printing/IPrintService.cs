using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Application.Printing;

/// <summary>
/// 打印服务接口（应用层）：
/// - 负责协调模板渲染与无头浏览器（Playwright/PuppeteerSharp）生成 PDF；
/// - 由基础设施层提供具体实现（如 PlaywrightPrintService / PuppeteerSharpPrintService）。
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// 生成 PDF 文件的二进制内容。
    /// </summary>
    /// <param name="request">打印请求（包含单据类型、标识与模板名）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>PDF 二进制字节数组。</returns>
    Task<byte[]> GeneratePdfAsync(PrintRequest request, CancellationToken cancellationToken = default);
}

