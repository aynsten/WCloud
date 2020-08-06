using FluentValidation;
using WCloud.Framework.Common.Validator.FluentValidatorImpl;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Domain.Validators
{
    public class AdminEntityValidator : ModelValidator<AdminEntity>
    {
        public AdminEntityValidator()
        {
            this.RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("用户名不能为空");
                //.Must(x => ValidateHelper.IsNUMBER_OR_CHAR(x)).WithMessage("用户名存在不允许的符号")
                //.MustBeMobilePhone().When(x => x.UserName.StartsWith("1")).WithMessage("手机号格式错误")
                //.MinimumLength(5).WithMessage("用户名长度最少5位")
                //.MaximumLength(15).WithMessage("用户名最长15位");
        }
    }
}
