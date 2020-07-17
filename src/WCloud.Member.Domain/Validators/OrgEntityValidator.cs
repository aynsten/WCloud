using FluentValidation;
using WCloud.Core.Validator.FluentValidatorImpl;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.Domain.Validators
{
    /// <summary>
    /// https://github.com/JeremySkinner/FluentValidation/issues
    /// </summary>
    public class OrgEntityValidator : ModelValidator<OrgEntity>
    {
        public OrgEntityValidator()
        {
            this.RuleFor(x => x.OrgName)
                .NotEmpty().WithMessage("昵称不能为空")
                .MinimumLength(3).WithMessage("用户名长度太短")
                .MaximumLength(20).WithMessage("用户名长度太长")
                .Must(x => !x.Contains("*")).WithMessage("不能包含*");

            this.RuleFor(x => x.OrgWebSite)
                .Must(x => x.Contains(".com")).WithMessage("网站格式错误");
            this.RuleFor(x => x.Phone)
                .Length(11).WithMessage("手机号格式错误");
        }
    }
}