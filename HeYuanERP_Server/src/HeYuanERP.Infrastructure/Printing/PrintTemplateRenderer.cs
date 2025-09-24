using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.Printing;
using HeYuanERP.Domain.Printing;
using Microsoft.Extensions.Options;

namespace HeYuanERP.Infrastructure.Printing;

/// <summary>
/// 简易模板渲染实现：
/// - 通过 HtmlTemplateLoader 载入模板；
/// - 使用 {{ key }} 占位符替换，支持点号路径（如 {{ customer.name }}）；
/// - 若模板未显式声明 &lt;base&gt;，自动注入以支持相对资源（CSS/图片）。
/// </summary>
public class PrintTemplateRenderer : IPrintTemplateRenderer
{
    private readonly HtmlTemplateLoader _loader;
    private readonly PrintOptions _options;

    public PrintTemplateRenderer(HtmlTemplateLoader loader, IOptions<PrintOptions> options)
    {
        _loader = loader;
        _options = options.Value;
    }

    public async Task<string> RenderHtmlAsync(string docType, string templateName, object viewModel, CancellationToken cancellationToken = default)
    {
        var raw = await _loader.LoadAsync(docType, templateName, cancellationToken);

        // 注入 <base> 以正确解析相对资源
        raw = InjectBaseHref(raw, _options.TemplatesRoot);

        // 将对象拍平为字典，执行 {{key}} 替换
        var flat = Flatten(viewModel);
        var rendered = ReplaceTokens(raw, flat);
        return rendered;
    }

    private static string InjectBaseHref(string html, string? templatesRoot)
    {
        var root = string.IsNullOrWhiteSpace(templatesRoot)
            ? System.IO.Path.Combine(AppContext.BaseDirectory, "assets", "templates")
            : templatesRoot!;
        var baseUri = new Uri(System.IO.Path.GetFullPath(root) + System.IO.Path.DirectorySeparatorChar, UriKind.Absolute).AbsoluteUri;
        if (Regex.IsMatch(html, "<base\\s+href=", RegexOptions.IgnoreCase))
        {
            return html; // 已存在 base
        }
        var injection = $"<base href=\"{baseUri}\">";
        if (Regex.IsMatch(html, "<head[^>]*>", RegexOptions.IgnoreCase))
        {
            return Regex.Replace(html, "<head([^>]*)>", m => $"<head{m.Groups[1].Value}>{injection}", RegexOptions.IgnoreCase);
        }
        return injection + html;
    }

    private static Dictionary<string, string> Flatten(object model)
    {
        if (model is null) return new();
        try
        {
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(model));
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            void Walk(JsonElement el, string prefix)
            {
                switch (el.ValueKind)
                {
                    case JsonValueKind.Object:
                        foreach (var p in el.EnumerateObject())
                        {
                            var key = string.IsNullOrEmpty(prefix) ? p.Name : $"{prefix}.{p.Name}";
                            Walk(p.Value, key);
                        }
                        break;
                    case JsonValueKind.Array:
                        var i = 0;
                        foreach (var item in el.EnumerateArray())
                        {
                            Walk(item, $"{prefix}[{i}]");
                            i++;
                        }
                        break;
                    case JsonValueKind.String:
                        dict[prefix] = el.GetString() ?? string.Empty;
                        break;
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        dict[prefix] = el.ToString();
                        break;
                    default:
                        dict[prefix] = string.Empty;
                        break;
                }
            }
            Walk(doc.RootElement, string.Empty);
            return dict;
        }
        catch
        {
            return new();
        }
    }

    private static string ReplaceTokens(string html, Dictionary<string, string> values)
    {
        if (values.Count == 0) return html;
        // 匹配 {{ key }}，忽略空格
        return Regex.Replace(html, "{{\\s*([^}\\s]+(?:\\.[^}\\s]+|\\[[^]]+])*)\\s*}}", m =>
        {
            var key = m.Groups[1].Value;
            return values.TryGetValue(key, out var v) ? v : string.Empty;
        });
    }
}

