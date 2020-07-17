using System;
using System.Linq;

namespace WCloud.Core.Validator.FluentValidatorImpl
{
    /// <summary>
    /// 使用fluence validator验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class EntityFluentValidationHelper<T> : ValidationHelperBase<T> where T : class
    {
        private readonly IEntityFluentValidator<T> _validator;

        public EntityFluentValidationHelper(IEntityFluentValidator<T> validator = null)
        {
            this._validator = validator ??
                throw new ArgumentException($"找不到{typeof(T).FullName}的验证类");
        }

        public override bool __IsValid__(T model, out string[] messages)
        {
            var res = this._validator.Validate(model);

            messages = res.Errors.Select(x => x.ErrorMessage).ToArray();

            return res.IsValid;
        }
    }
}
