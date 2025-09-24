// 版权所有(c) HeYuanERP
// 说明：库存查询参数 FluentValidation 校验器（中文注释）。

using FluentValidation;
using HeYuanERP.Contracts.Reports;

namespace HeYuanERP.Application.Reports.Validation;

/// <summary>
/// 库存查询参数校验。
/// </summary>
public class InventoryParamsValidator : AbstractValidator<InventoryQueryParamsDto>
{
    public InventoryParamsValidator()
    {
        // 时间范围：若提供则必须合法（Start <= End）
        RuleFor(x => x.Range)
            .Must(r => r == null || r.IsValid)
            .WithMessage("时间范围不合法：起始时间不能晚于结束时间。");

        // 字符串参数长度约束
        RuleFor(x => x.ProductId).MaximumLength(64);
        RuleFor(x => x.Whse).MaximumLength(32);
        RuleFor(x => x.Loc).MaximumLength(64);

        // 分页（仅交易查询使用，但参数对象公用）
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("页码必须≥1。");
        RuleFor(x => x.Size).InclusiveBetween(1, 200).WithMessage("每页大小必须在 1~200 之间。");
    }
}

