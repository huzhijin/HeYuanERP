using FluentValidation;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Validators;

// 登录请求参数校验
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.LoginId).NotEmpty().WithMessage("登录名不能为空");
        RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        RuleFor(x => x.LoginId).MaximumLength(50).WithMessage("登录名过长");
        RuleFor(x => x.Password).MaximumLength(100).WithMessage("密码过长");
    }
}

