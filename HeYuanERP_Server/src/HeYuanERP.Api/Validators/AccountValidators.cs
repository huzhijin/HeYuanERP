using FluentValidation;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Validators;

// 账户（客户）相关请求 DTO 的 FluentValidation 校验器

public class AccountListQueryValidator : AbstractValidator<AccountListQueryDto>
{
    public AccountListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("页码必须大于 0");
        RuleFor(x => x.Size).InclusiveBetween(1, 200).WithMessage("每页大小需在 1~200 之间");
        RuleFor(x => x.Keyword).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Keyword));
    }
}

public class AccountCreateValidator : AbstractValidator<AccountCreateDto>
{
    public AccountCreateValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("客户编码必填").MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().WithMessage("客户名称必填").MaximumLength(200);
        RuleFor(x => x.OwnerId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.OwnerId));
        RuleFor(x => x.TaxNo).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.TaxNo));
        RuleFor(x => x.Description).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

public class AccountUpdateValidator : AbstractValidator<AccountUpdateDto>
{
    public AccountUpdateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("客户名称必填").MaximumLength(200);
        RuleFor(x => x.Code).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Code));
        RuleFor(x => x.OwnerId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.OwnerId));
        RuleFor(x => x.TaxNo).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.TaxNo));
        RuleFor(x => x.Description).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

public class AccountShareRequestValidator : AbstractValidator<AccountShareRequestDto>
{
    public AccountShareRequestValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty().WithMessage("目标用户必填").MaximumLength(64);
        RuleFor(x => x.Permission)
            .NotEmpty().WithMessage("权限必填")
            .Must(p => new[] { "read", "edit" }.Contains(p))
            .WithMessage("权限仅支持 read/edit");
    }
}

public class AccountTransferRequestValidator : AbstractValidator<AccountTransferRequestDto>
{
    public AccountTransferRequestValidator()
    {
        RuleFor(x => x.NewOwnerId).NotEmpty().WithMessage("新负责人必填").MaximumLength(64);
    }
}

public class AccountVisitCreateValidator : AbstractValidator<AccountVisitCreateDto>
{
    public AccountVisitCreateValidator()
    {
        RuleFor(x => x.VisitDate).NotEmpty().WithMessage("拜访日期必填");
        RuleFor(x => x.Subject).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Subject));
        RuleFor(x => x.Location).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Location));
        RuleFor(x => x.Result).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Result));
        RuleFor(x => x.ContactId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.ContactId));
        RuleFor(x => x.VisitorId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.VisitorId));
    }
}

