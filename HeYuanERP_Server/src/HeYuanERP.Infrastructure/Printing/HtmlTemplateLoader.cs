using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Domain.Printing;
using HeYuanERP.Domain.Printing.Exceptions;
using Microsoft.Extensions.Options;

namespace HeYuanERP.Infrastructure.Printing;

/// <summary>
/// HTML 模板加载器：从模板根目录读取模板，并处理简单的 include 指令。
/// 支持语法：<!--#include "shared/header.html"-->
/// </summary>
public class HtmlTemplateLoader
{
    private readonly PrintOptions _options;

    // 使用 C# 11 原始字符串字面量（单行），避免反斜杠转义问题。
    private static readonly Regex IncludeRegex = new(
        """<!--\s*#include\s+\"(?<path>[^\"]+)\"\s*-->""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public HtmlTemplateLoader(IOptions<PrintOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// 加载指定单据类型与模板名称的 HTML 文本。
    /// </summary>
    public async Task<string> LoadAsync(string docType, string templateName, CancellationToken cancellationToken = default)
    {
        var descriptor = new TemplateDescriptor { DocType = docType.Trim().ToLowerInvariant(), Name = templateName.Trim().ToLowerInvariant() };
        var root = EnsureDirectory(_options.TemplatesRoot ?? string.Empty);
        var fullPath = Path.Combine(root, descriptor.RelativePath);

        if (!File.Exists(fullPath))
        {
            throw new TemplateNotFoundException(descriptor.DocType, descriptor.Name, fullPath);
        }

        var html = await File.ReadAllTextAsync(fullPath, Encoding.UTF8, cancellationToken);
        html = await ResolveIncludesAsync(html, root, cancellationToken);
        return html;
    }

    private static string EnsureDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "assets", "templates");
        }
        return Path.GetFullPath(path);
    }

    private static async Task<string> ResolveIncludesAsync(string html, string root, CancellationToken cancellationToken)
    {
        // 递归展开 include，限定最大 5 层以防循环
        const int maxDepth = 5;
        var result = html;
        for (var depth = 0; depth < maxDepth; depth++)
        {
            var match = IncludeRegex.Match(result);
            if (!match.Success) break;

            result = IncludeRegex.Replace(result, m =>
            {
                var relPath = m.Groups["path"].Value.TrimStart('/', '\\');
                var candidate = Path.GetFullPath(Path.Combine(root, relPath));
                if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Empty; // 安全考虑：阻止越权路径
                }
                if (!File.Exists(candidate))
                {
                    return string.Empty; // 缺失包含文件时渲染为空
                }
                return File.ReadAllText(candidate, Encoding.UTF8);
            });
        }
        return result;
    }
}
