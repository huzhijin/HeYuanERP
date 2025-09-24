using FluentValidation;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Validators;

// 订单相关校验器：新建/编辑/确认/反审

public class OrderListQueryValidator : AbstractValidator<OrderListQueryDto>
{
    public OrderListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 200);
        RuleFor(x => x.Status).Must(s => string.IsNullOrWhiteSpace(s) || new[] { "draft", "confirmed", "reversed" }.Contains(s!))
            .WithMessage("状态仅支持 draft/confirmed/reversed");
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).When(x => x.From.HasValue && x.To.HasValue).WithMessage("截止日期需不早于起始日期");
    }
}

public class OrderLineCreateValidator : AbstractValidator<OrderLineCreateDto>
{
    public OrderLineCreateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Qty).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Discount).InclusiveBetween(0, 1);
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 1);
    }
}

public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("至少需要一条订单行");
        RuleForEach(x => x.Lines).SetValidator(new OrderLineCreateValidator());
    }
}

public class OrderLineUpdateValidator : AbstractValidator<OrderLineUpdateDto>
{
    public OrderLineUpdateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Qty).GreaterThan(0).When(x => x._deleted != true);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).When(x => x._deleted != true);
        RuleFor(x => x.Discount).InclusiveBetween(0, 1).When(x => x._deleted != true);
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 1).When(x => x._deleted != true);
    }
}

public class OrderUpdateValidator : AbstractValidator<OrderUpdateDto>
{
    public OrderUpdateValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("至少需要一条订单行");
        RuleForEach(x => x.Lines).SetValidator(new OrderLineUpdateValidator());
    }
}

public class OrderConfirmValidator : AbstractValidator<OrderConfirmDto>
{
    public OrderConfirmValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class OrderReverseValidator : AbstractValidator<OrderReverseDto>
{
    public OrderReverseValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}

