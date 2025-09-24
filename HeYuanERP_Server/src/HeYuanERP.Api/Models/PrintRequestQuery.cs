using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Models;

/// <summary>
/// 打印请求的查询参数。
/// 目前仅包含模板名（template），后续可扩展打印选项（如纸张、方向、边距等）。
/// </summary>
public class PrintRequestQuery
{
    /// <summary>
    /// 模板名称（例如：default）。若为空则由服务端使用默认模板。
    /// </summary>
    [FromQuery(Name = "template")]
    public string? Template { get; set; }
}

