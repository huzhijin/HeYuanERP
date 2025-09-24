using System;
using FluentValidation;
using HeYuanERP.Application.DTOs.Payments;

namespace HeYuanERP.Application.Validators.Payments;

/// <summary>
/// 收款创建请求的验证器（基于 FluentValidation）。
/// 说明：验证应用层 DTO（与 API 表单解耦），API 将在绑定后映射到该 DTO 再进行验证。
/// </summary>
public class PaymentCreateRequestValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateRequestValidator()
    {
        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("收款方式不能为空")
            .MaximumLength(50).WithMessage("收款方式长度不能超过 50 字符");

        RuleFor(x => x.Amount)
            .GreaterThan(0m).WithMessage("收款金额必须大于 0");

        RuleFor(x => x.PaymentDate)
            .Must(d => d.Year >= 2000).WithMessage("收款日期不应早于 2000-01-01")
            .Must(NotGreaterThanTomorrow).WithMessage("收款日期不应晚于明天（考虑时区差异）");

        RuleFor(x => x.Remark)
            .MaximumLength(500).WithMessage("备注长度不能超过 500 字符");
    }

    private static bool NotGreaterThanTomorrow(DateOnly date)
    {
        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);
        var tomorrowUtc = todayUtc.AddDays(1);
        return date <= tomorrowUtc;
    }
}

