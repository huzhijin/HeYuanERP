using FluentValidation;
using HeYuanERP.Application.Invoices.DTOs;
using HeYuanERP.Domain.Invoices;

namespace HeYuanERP.Api.Validators;

/// <summary>
/// 发票查询请求校验器。
/// </summary>
public class QueryInvoiceRequestValidator : AbstractValidator<QueryInvoiceRequest>
{
    public QueryInvoiceRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("页码必须从 1 开始");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithMessage("每页条数需在 1~200 之间");

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("发票状态不在有效范围内");
        });
    }
}

