using System;
using System.IO;

namespace HeYuanERP.Domain.Printing.Exceptions;

/// <summary>
/// 模板不存在异常：在模板查找失败时抛出。
/// </summary>
public class TemplateNotFoundException : FileNotFoundException
{
    /// <summary>
    /// 单据类型。
    /// </summary>
    public string DocType { get; }

    /// <summary>
    /// 模板名称。
    /// </summary>
    public string TemplateName { get; }

    /// <summary>
    /// 尝试访问的模板文件路径（若已知）。
    /// </summary>
    public string? TemplatePath { get; }

    public TemplateNotFoundException(string docType, string templateName, string? templatePath = null)
        : base($"未找到模板：{docType}/{templateName}.html" + (templatePath is null ? string.Empty : $"（路径：{templatePath}）"))
    {
        DocType = docType;
        TemplateName = templateName;
        TemplatePath = templatePath;
    }
}

