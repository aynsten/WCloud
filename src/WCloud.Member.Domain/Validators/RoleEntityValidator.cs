using FluentValidation;
using WCloud.Framework.Common.Validator.FluentValidatorImpl;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Domain.Validators
{
    /// <summary>
    /// https://github.com/JeremySkinner/FluentValidation/issues
    /// </summary>
    public class RoleEntityValidator : ModelValidator<RoleEntity>
    {
        public RoleEntityValidator()
        {
            this.RuleFor(x => x.NodeName)
                .NotEmpty().WithMessage("角色名不能为空")
                .Must(x => !x.Contains("*")).WithMessage("不能包含*")
                .MinimumLength(1).WithMessage("角色名长度太短")
                .MaximumLength(20).WithMessage("角色名长度太长");

        }
    }
}