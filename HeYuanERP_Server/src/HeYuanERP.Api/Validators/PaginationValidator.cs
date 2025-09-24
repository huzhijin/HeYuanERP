using FluentValidation;

namespace HeYuanERP.Api.Validators;

// 通用分页参数模型与校验器（供列表接口使用）
public class PaginationQuery
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
}

public class PaginationQueryValidator : AbstractValidator<PaginationQuery>
{
    public PaginationQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("页码必须≥1");
        RuleFor(x => x.Size).InclusiveBetween(1, 200).WithMessage("分页大小需在 1~200 之间");
    }
}

