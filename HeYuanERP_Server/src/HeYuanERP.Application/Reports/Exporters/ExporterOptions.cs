// 版权所有(c) HeYuanERP
// 说明：报表导出器配置项（中文注释）。

using System;
using System.IO;

namespace HeYuanERP.Application.Reports.Exporters;

/// <summary>
/// 导出器配置。
/// </summary>
public class ExporterOptions
{
    /// <summary>
    /// 导出文件根目录（绝对路径）。默认：进程目录下 exports。
    /// </summary>
    public string? OutputRoot { get; set; }

    /// <summary>
    /// 公网/站点基地址（可选）。若配置，将返回可公开访问的 URL；否则返回 file:// URI。
    /// 例如：https://static.example.com/reports
    /// </summary>
    public string? PublicBaseUrl { get; set; }

    /// <summary>
    /// 本机 Chromium/Chrome 可执行文件路径。若为空，将尝试从环境变量 CHROMIUM_PATH 读取，或使用系统默认可执行名。
    /// </summary>
    public string? ChromiumPath { get; set; }

    /// <summary>
    /// 生成默认配置（从环境变量读取并回退）。
    /// </summary>
    public static ExporterOptions FromEnvironment()
    {
        var root = Environment.GetEnvironmentVariable("REPORTS_OUTPUT_ROOT");
        var chromium = Environment.GetEnvironmentVariable("CHROMIUM_PATH");
        var publicBase = Environment.GetEnvironmentVariable("REPORTS_PUBLIC_BASE_URL");

        return new ExporterOptions
        {
            OutputRoot = string.IsNullOrWhiteSpace(root)
                ? Path.Combine(AppContext.BaseDirectory, "exports")
                : root,
            ChromiumPath = string.IsNullOrWhiteSpace(chromium) ? null : chromium,
            PublicBaseUrl = string.IsNullOrWhiteSpace(publicBase) ? null : publicBase
        };
    }

    /// <summary>
    /// 将本地文件路径转换为 URI（公开 URL 或 file://）。
    /// </summary>
    public string ToFileUri(string absolutePath)
    {
        if (!string.IsNullOrWhiteSpace(PublicBaseUrl))
        {
            // 若配置了 PublicBaseUrl，假定 OutputRoot 映射为其根目录
            var root = EnsureOutputRoot();
            var rel = Path.GetRelativePath(root, absolutePath).Replace(Path.DirectorySeparatorChar, '/');
            return $"{PublicBaseUrl!.TrimEnd('/')}/{rel}";
        }
        return new Uri(absolutePath).AbsoluteUri; // file://
    }

    /// <summary>
    /// 确保输出目录存在并返回绝对路径。
    /// </summary>
    public string EnsureOutputRoot()
    {
        var root = OutputRoot;
        if (string.IsNullOrWhiteSpace(root))
        {
            root = Path.Combine(AppContext.BaseDirectory, "exports");
            OutputRoot = root;
        }
        Directory.CreateDirectory(root!);
        return root!;
    }
}

