using System.IO;

namespace HeYuanERP.Domain.Printing;

/// <summary>
/// 模板描述：标识一个具体的模板（单据类型 + 模板名）。
/// </summary>
public class TemplateDescriptor
{
    /// <summary>
    /// 单据类型（如 order）。
    /// </summary>
    public string DocType { get; set; } = string.Empty;

    /// <summary>
    /// 模板名称（如 default）。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 生成相对路径（相对于模板根目录），约定目录结构：{docType}/{name}.html
    /// </summary>
    public string RelativePath => Path.Combine(DocType, $"{Name}.html");

    /// <summary>
    /// 生成绝对路径：<paramref name="templatesRoot"/>/{docType}/{name}.html
    /// </summary>
    public string GetAbsolutePath(string templatesRoot)
        => Path.Combine(templatesRoot, DocType, $"{Name}.html");
}

