// 版权所有(c) HeYuanERP
// 说明：查询参数白名单提示过滤器（可选使用，中文注释）。

using System;
using System.Collections.Generic;
using System.Linq;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Domain.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Api.Filters;

/// <summary>
/// 查询参数白名单提示过滤器：
/// - 根据指定报表类型，对当前请求 QueryString 进行白名单过滤；
/// - 若发现未知参数，将记录日志；
/// - 不会中断请求，仅作提示与追踪（实际绑定/校验仍建议在控制器中进行）。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ValidateQueryParametersAttribute : ActionFilterAttribute
{
    private readonly ReportType _type;

    public ValidateQueryParametersAttribute(ReportType type)
    {
        _type = type;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<ValidateQueryParametersAttribute>)) as ILogger<ValidateQueryParametersAttribute>;
        var whitelist = context.HttpContext.RequestServices.GetService(typeof(IReportParameterWhitelist)) as IReportParameterWhitelist;
        if (whitelist == null)
        {
            base.OnActionExecuting(context);
            return;
        }

        var query = context.HttpContext.Request.Query.ToDictionary(kv => kv.Key, kv => (object?)kv.Value.ToString(), StringComparer.OrdinalIgnoreCase);
        var safe = whitelist.Filter(_type, query);

        var knownKeys = new HashSet<string>(safe.Keys, StringComparer.OrdinalIgnoreCase);
        // range 作为整体允许，内部字段忽略
        knownKeys.Add("range");
        var unknown = query.Keys.Where(k => !knownKeys.Contains(k)).ToArray();
        if (unknown.Length > 0)
        {
            logger?.LogWarning("发现未在白名单中的查询参数：{Keys}", string.Join(',', unknown));
        }

        base.OnActionExecuting(context);
    }
}

