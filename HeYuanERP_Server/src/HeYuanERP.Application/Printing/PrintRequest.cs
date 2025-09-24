namespace HeYuanERP.Application.Printing;

/// <summary>
/// 打印请求模型：由 API 层传入，应用层据此完成模板渲染与打印。
/// </summary>
public class PrintRequest
{
    /// <summary>
    /// 单据类型（order/delivery/return/invoice/statement 等）。
    /// 建议统一使用小写英文与连字符风格：如 "order"、"delivery"。
    /// </summary>
    public string DocType { get; set; } = string.Empty;

    /// <summary>
    /// 单据标识（字符串或数值 ID 均可，由上层保持与业务一致）。
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 模板名称（默认为 default）。
    /// </summary>
    public string? Template { get; set; }
}

