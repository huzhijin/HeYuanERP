// 版权所有(c) HeYuanERP
// 说明：发票统计参数 FluentValidation 校验器（中文注释）。

using FluentValidation;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Validation;

/// <summary>
/// 发票统计参数校验。
/// </summary>
public class InvoiceStatParamsValidator : AbstractValidator<InvoiceStatParamsDto>
{
    public InvoiceStatParamsValidator()
    {
        // 时间范围：若提供则必须合法（Start <= End）
        RuleFor(x => x.Range)
            .Must(r => r == null || r.IsValid)
            .WithMessage("时间范围不合法：起始时间不能晚于结束时间。");

        // 分组维度：允许的取值（或为空）
        RuleFor(x => x.GroupBy)
            .Must(g => string.IsNullOrWhiteSpace(g) || g!.Trim().ToLowerInvariant() is "day" or "month" or "account")
            .WithMessage("不支持的分组维度（允许：day/month/account）。");

        // 其他字符串参数长度约束（避免过长攻击）
        RuleFor(x => x.AccountId).MaximumLength(64);
        RuleFor(x => x.Status).MaximumLength(32);
        RuleFor(x => x.Currency).MaximumLength(16);
    }
}

