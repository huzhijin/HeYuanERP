using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace HeYuanERP.Application.Printing.Validation;

/// <summary>
/// 打印请求校验（FluentValidation）：
/// - docType 非空且在允许列表内；
/// - id 非空；
/// - template（若提供）仅允许小写字母/数字/连字符/下划线。
/// </summary>
public class PrintRequestValidator : AbstractValidator<PrintRequest>
{
    private static readonly HashSet<string> AllowedDocTypes = new(new[]
    {
        "order", "delivery", "return", "invoice", "statement"
    });

    public PrintRequestValidator()
    {
        RuleFor(x => x.DocType)
            .NotEmpty().WithMessage("docType 不能为空")
            .Must(dt => AllowedDocTypes.Contains(dt.Trim().ToLowerInvariant()))
            .WithMessage($"docType 不在允许范围：{string.Join(", ", AllowedDocTypes)}");

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("id 不能为空");

        RuleFor(x => x.Template)
            .Cascade(CascadeMode.Stop)
            .Must(t => t == null || t.All(c => char.IsLower(c) || char.IsDigit(c) || c == '-' || c == '_'))
            .WithMessage("template 仅允许小写字母、数字、连字符与下划线");
    }
}

