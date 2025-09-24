using FluentValidation;
using HeYuanERP.Application.Invoices.DTOs;

namespace HeYuanERP.Api.Validators;

/// <summary>
/// 发票明细校验器。
/// </summary>
public class InvoiceItemValidator : AbstractValidator<InvoiceItemDto>
{
    public InvoiceItemValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("商品编码不能为空")
            .MaximumLength(64).WithMessage("商品编码长度不能超过 64");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("商品名称不能为空")
            .MaximumLength(256).WithMessage("商品名称长度不能超过 256");

        RuleFor(x => x.Specification)
            .MaximumLength(128).When(x => !string.IsNullOrEmpty(x.Specification))
            .WithMessage("规格型号长度不能超过 128");

        RuleFor(x => x.Unit)
            .MaximumLength(32).When(x => !string.IsNullOrEmpty(x.Unit))
            .WithMessage("单位长度不能超过 32");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("数量不能为负数");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("单价不能为负数");

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 1).WithMessage("税率须在 0~1 之间");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("排序号不能为负数");

        RuleFor(x => x.Remark)
            .MaximumLength(512).When(x => !string.IsNullOrEmpty(x.Remark))
            .WithMessage("备注长度不能超过 512");
    }
}

