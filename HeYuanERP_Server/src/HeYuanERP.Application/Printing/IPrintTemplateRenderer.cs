using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Application.Printing;

/// <summary>
/// 模板渲染接口：将视图模型数据渲染为 HTML 字符串。
/// 基础设施层可通过 RazorLight、Scriban 或自定义占位符引擎等实现。
/// </summary>
public interface IPrintTemplateRenderer
{
    /// <summary>
    /// 渲染指定单据类型与模板的 HTML。
    /// </summary>
    /// <param name="docType">单据类型（如 order）。</param>
    /// <param name="templateName">模板名称（如 default）。</param>
    /// <param name="viewModel">用于渲染的视图模型（匿名对象/字典/强类型均可）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>渲染后的 HTML 字符串。</returns>
    Task<string> RenderHtmlAsync(string docType, string templateName, object viewModel, CancellationToken cancellationToken = default);
}

