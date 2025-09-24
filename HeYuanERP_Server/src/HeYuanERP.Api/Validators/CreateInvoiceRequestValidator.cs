using FluentValidation;
using HeYuanERP.Application.Invoices.DTOs;
using HeYuanERP.Domain.Invoices;

namespace HeYuanERP.Api.Validators;

/// <summary>
/// 创建发票请求校验器。
/// </summary>
public class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("客户 Id 不能为空");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("客户名称不能为空")
            .MaximumLength(256).WithMessage("客户名称长度不能超过 256");

        RuleFor(x => x.SourceType)
            .IsInEnum().WithMessage("来源类型不在有效范围内");

        RuleFor(x => x.Number)
            .MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.Number))
            .WithMessage("系统内发票号长度不能超过 64");

        RuleFor(x => x.SourceNumber)
            .MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.SourceNumber))
            .WithMessage("来源单据编号长度不能超过 64");

        RuleFor(x => x.DefaultTaxRate)
            .InclusiveBetween(0, 1).When(x => x.DefaultTaxRate.HasValue)
            .WithMessage("默认税率须在 0~1 之间");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("发票明细不能为空")
            .NotEmpty().WithMessage("发票明细不能为空");

        RuleForEach(x => x.Items)
            .SetValidator(new InvoiceItemValidator());

        When(x => x.EInvoice != null, () =>
        {
            RuleFor(x => x.EInvoice!.InvoiceCode)
                .MaximumLength(64).When(x => !string.IsNullOrEmpty(x.EInvoice!.InvoiceCode))
                .WithMessage("电票代码长度不能超过 64");

            RuleFor(x => x.EInvoice!.InvoiceNumber)
                .MaximumLength(64).When(x => !string.IsNullOrEmpty(x.EInvoice!.InvoiceNumber))
                .WithMessage("电票号码长度不能超过 64");

            RuleFor(x => x.EInvoice!.CheckCode)
                .MaximumLength(64).When(x => !string.IsNullOrEmpty(x.EInvoice!.CheckCode))
                .WithMessage("电票校验码长度不能超过 64");

            RuleFor(x => x.EInvoice!.PdfUrl)
                .MaximumLength(512).When(x => !string.IsNullOrEmpty(x.EInvoice!.PdfUrl))
                .WithMessage("电票 PDF 链接过长");

            RuleFor(x => x.EInvoice!.ViewUrl)
                .MaximumLength(512).When(x => !string.IsNullOrEmpty(x.EInvoice!.ViewUrl))
                .WithMessage("电票查看链接过长");

            RuleFor(x => x.EInvoice!.QrCodeUrl)
                .MaximumLength(512).When(x => !string.IsNullOrEmpty(x.EInvoice!.QrCodeUrl))
                .WithMessage("电票二维码链接过长");

            RuleFor(x => x.EInvoice!.BuyerTaxId)
                .MaximumLength(32).When(x => !string.IsNullOrEmpty(x.EInvoice!.BuyerTaxId))
                .WithMessage("购方税号长度不能超过 32");

            RuleFor(x => x.EInvoice!.SellerTaxId)
                .MaximumLength(32).When(x => !string.IsNullOrEmpty(x.EInvoice!.SellerTaxId))
                .WithMessage("销方税号长度不能超过 32");
        });
    }
}

