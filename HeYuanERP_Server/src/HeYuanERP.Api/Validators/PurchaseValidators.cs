using FluentValidation;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Validators;

// 采购（PO）相关校验器：列表/新建/编辑/确认/收货

public class POListQueryValidator : AbstractValidator<POListQueryDto>
{
    public POListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 200);
        RuleFor(x => x.Status).Must(s => string.IsNullOrWhiteSpace(s) || new[] { "draft", "confirmed" }.Contains(s!))
            .WithMessage("状态仅支持 draft/confirmed");
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).When(x => x.From.HasValue && x.To.HasValue).WithMessage("截止日期需不早于起始日期");
    }
}

public class POLineCreateValidator : AbstractValidator<POLineCreateDto>
{
    public POLineCreateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Qty).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}

public class POCreateValidator : AbstractValidator<POCreateDto>
{
    public POCreateValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("至少需要一条采购行");
        RuleForEach(x => x.Lines).SetValidator(new POLineCreateValidator());
    }
}

public class POLineUpdateValidator : AbstractValidator<POLineUpdateDto>
{
    public POLineUpdateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Qty).GreaterThan(0).When(x => x._deleted != true);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).When(x => x._deleted != true);
    }
}

public class POUpdateValidator : AbstractValidator<POUpdateDto>
{
    public POUpdateValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("至少需要一条采购行");
        RuleForEach(x => x.Lines).SetValidator(new POLineUpdateValidator());
    }
}

public class POConfirmValidator : AbstractValidator<POConfirmDto>
{
    public POConfirmValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class POReceiveLineCreateValidator : AbstractValidator<POReceiveLineCreateDto>
{
    public POReceiveLineCreateValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Qty).GreaterThan(0);
        RuleFor(x => x.Whse).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Loc).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Loc));
    }
}

public class POReceiveCreateValidator : AbstractValidator<POReceiveCreateDto>
{
    public POReceiveCreateValidator()
    {
        RuleFor(x => x.Lines).NotEmpty().WithMessage("至少需要一条收货行");
        RuleForEach(x => x.Lines).SetValidator(new POReceiveLineCreateValidator());
    }
}

