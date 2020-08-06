using FluentValidation;
using WCloud.Core.Validator.FluentValidatorImpl;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Domain.Validators
{
    /// <summary>
    /// https://github.com/JeremySkinner/FluentValidation/issues
    /// </summary>
    public class UserEntityValidator : ModelValidator<UserEntity>
    {
        public UserEntityValidator()
        {
            this.RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("昵称不能为空")
                .MinimumLength(3).WithMessage("用户名长度太短")
                .MaximumLength(20).WithMessage("用户名长度太长")
                .Must(x => !x.Contains("*")).WithMessage("不能包含*")
                .EmailAddress().When(x => x.UserName.Contains("@")).WithMessage("电子邮件格式错误");

            this.RuleFor(x => x.UserSex).MustBeSex().WithMessage("性别错误");
        }
    }
}