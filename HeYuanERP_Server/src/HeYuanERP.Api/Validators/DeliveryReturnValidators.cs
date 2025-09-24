using FluentValidation;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Validators;

// 送货/退货相关校验器

public class DeliveryLineCreateValidator : AbstractValidator<DeliveryLineCreateDto>
{
    public DeliveryLineCreateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Qty).GreaterThan(0);
        RuleFor(x => x.OrderLineId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.OrderLineId));
    }
}

public class DeliveryCreateValidator : AbstractValidator<DeliveryCreateDto>
{
    public DeliveryCreateValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("至少需要一条送货明细");
        RuleForEach(x => x.Lines).SetValidator(new DeliveryLineCreateValidator());
    }
}

public class ReturnLineCreateValidator : AbstractValidator<ReturnLineCreateDto>
{
    public ReturnLineCreateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Qty).GreaterThan(0);
        RuleFor(x => x.Reason).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}

public class ReturnCreateValidator : AbstractValidator<ReturnCreateDto>
{
    public ReturnCreateValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("至少需要一条退货明细");
        RuleForEach(x => x.Lines).SetValidator(new ReturnLineCreateValidator());
        RuleFor(x => x.SourceDeliveryId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.SourceDeliveryId));
    }
}

