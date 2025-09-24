// 版权所有(c) HeYuanERP
// 说明：报表导出请求体 FluentValidation 校验器（中文注释）。

using System.Collections.Generic;
using FluentValidation;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Validation;

/// <summary>
/// 报表导出请求体校验。
/// </summary>
public class ReportExportRequestValidator : AbstractValidator<ReportExportRequestDto>
{
    public ReportExportRequestValidator()
    {
        // params 不可为空
        RuleFor(x => x.Params)
            .NotNull().WithMessage("参数对象(params)不能为空。")
            .Must(IsParamsAcceptable).WithMessage("参数键值不合法或过多（最多 50 个键，键名≤64）。");

        // format 可为空，若提供必须为 pdf/csv
        RuleFor(x => x.Format)
            .Must(f => string.IsNullOrWhiteSpace(f) || f!.Trim().ToLowerInvariant() is "pdf" or "csv")
            .WithMessage("导出格式仅支持 pdf/csv。");
    }

    private static bool IsParamsAcceptable(IDictionary<string, object?>? dict)
    {
        if (dict is null) return false;
        if (dict.Count > 50) return false; // 简单限制键数量，防滥用
        foreach (var key in dict.Keys)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length > 64) return false;
        }
        return true;
    }
}

