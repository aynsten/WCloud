using System;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.Validator;

namespace WCloud.Framework.Common.Validator.FluentValidatorImpl
{
    public abstract class ModelValidator<T> : AbstractValidator<T>, IValidator<T> { }

    /// <summary>
    /// 使用fluence validator验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class EntityFluentValidationHelper : ValidationHelperBase
    {
        private readonly IServiceProvider provider;
        public EntityFluentValidationHelper(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public override bool __IsValid__<T>(T model, out string[] messages)
        {
            var _validator = this.provider.Resolve_<IValidator<T>>();
            var res = _validator.Validate(model);

            messages = res.Errors.Select(x => x.ErrorMessage).ToArray();

            return res.IsValid;
        }
    }
}
