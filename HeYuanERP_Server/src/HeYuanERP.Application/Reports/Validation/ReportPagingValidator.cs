// 版权所有(c) HeYuanERP
// 说明：分页校验扩展（FluentValidation 扩展方法，中文注释）。

using FluentValidation;

namespace HeYuanERP.Application.Reports.Validation;

/// <summary>
/// 分页通用校验扩展：用于为 Page/Size 属性添加统一规则。
/// </summary>
public static class ReportPagingValidator
{
    /// <summary>
    /// 页码规则：必须 ≥ 1。
    /// 用法：RuleFor(x => x.Page).UsePageRule();
    /// </summary>
    public static IRuleBuilderOptions<T, int> UsePageRule<T>(this IRuleBuilder<T, int> rule)
        => rule.GreaterThanOrEqualTo(1).WithMessage("页码必须≥1。");

    /// <summary>
    /// 每页大小规则：1 ~ 200。
    /// 用法：RuleFor(x => x.Size).UseSizeRule();
    /// </summary>
    public static IRuleBuilderOptions<T, int> UseSizeRule<T>(this IRuleBuilder<T, int> rule)
        => rule.InclusiveBetween(1, 200).WithMessage("每页大小必须在 1~200 之间。");
}

