using FluentValidation;
using Lib.helper;
using System;

namespace WCloud.Framework.Common.Validator.FluentValidatorImpl
{
    /// <summary>
    /// https://fluentvalidation.net/custom-validators
    /// </summary>
    public static class FluentValidatorExtension
    {
        [Obsolete]
        public static IRuleBuilderOptions<T, string> StrMustNotContainEmoji<T>(this IRuleBuilder<T, string> builder)
        {
            var res = builder.Must(x => !ValidateHelper._IsEmoji(x));
            return res;
        }

        public static IRuleBuilderOptions<T, string> MustBeUserName<T>(this IRuleBuilderOptions<T, string> builder)
        {
            builder = builder.NotEmpty().WithMessage("用户名不能为空");
            builder = builder.Must(x => x.StartsWith("1") && x.Length == 11).WithMessage("手机号格式不正确");

            return builder;
        }

        /// <summary>
        /// -1,0,1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, int> MustBeSex<T>(this IRuleBuilder<T, int> builder)
        {
            return builder.InclusiveBetween(-1, 1);
        }

        public static IRuleBuilderOptions<T, string> MustBeUrl<T>(this IRuleBuilder<T, string> builder)
        {
            var res = builder.Must(ValidateHelper.IsURL);
            return res;
        }

        public static IRuleBuilderOptions<T, string> MustBeMobilePhone<T>(this IRuleBuilder<T, string> builder)
        {
            var res = builder.Must(ValidateHelper.IsMobilePhone);
            return res;
        }

        public static IRuleBuilderOptions<T, string> MustBeDomain<T>(this IRuleBuilder<T, string> builder)
        {
            var res = builder.Must(ValidateHelper.IsDomain);
            return res;
        }

        public static IRuleBuilderOptions<T, string> MustBeIDCard<T>(this IRuleBuilder<T, string> builder)
        {
            var res = builder.Must(ValidateHelper.IsIDCardNo);
            return res;
        }
    }
}
