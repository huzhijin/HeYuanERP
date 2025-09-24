// 版权所有(c) HeYuanERP
// 说明：采购订单查询参数 FluentValidation 校验器（中文注释）。

using FluentValidation;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Validation;

/// <summary>
/// 采购订单查询参数校验。
/// </summary>
public class POQueryParamsValidator : AbstractValidator<POQueryParamsDto>
{
    public POQueryParamsValidator()
    {
        // 时间范围：若提供则必须合法（Start <= End）
        RuleFor(x => x.Range)
            .Must(r => r == null || r.IsValid)
            .WithMessage("时间范围不合法：起始时间不能晚于结束时间。");

        // 字符串参数长度约束
        RuleFor(x => x.VendorId).MaximumLength(64);
        RuleFor(x => x.Status).MaximumLength(32);

        // 分页校验
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("页码必须≥1。");
        RuleFor(x => x.Size).InclusiveBetween(1, 200).WithMessage("每页大小必须在 1~200 之间。");
    }
}

