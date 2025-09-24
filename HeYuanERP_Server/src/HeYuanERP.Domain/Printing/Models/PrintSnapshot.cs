using System;

namespace HeYuanERP.Domain.Printing.Models;

/// <summary>
/// 打印参数快照：用于记录打印时的视图模型数据，确保二次打印/审计可复现。
/// </summary>
public class PrintSnapshot
{
    /// <summary>
    /// 单据类型（如 order）。
    /// </summary>
    public string DocType { get; set; } = string.Empty;

    /// <summary>
    /// 单据标识。
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 模板名称（如 default）。
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 视图模型序列化后的 JSON 字符串。
    /// </summary>
    public string DataJson { get; set; } = "{}";

    /// <summary>
    /// 数据摘要（可选，用于一致性校验）。
    /// </summary>
    public string? DataHash { get; set; }

    /// <summary>
    /// 快照创建时间（UTC）。
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 生成快照的唯一键：{docType}/{id}/{template}
    /// </summary>
    public string GetKey()
        => $"{DocType.Trim().ToLowerInvariant()}/{Id}/{TemplateName.Trim().ToLowerInvariant()}";
}

