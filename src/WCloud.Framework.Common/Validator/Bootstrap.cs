using System.Linq;
using System.Reflection;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.Validator;
using WCloud.Framework.Common.Validator.FluentValidatorImpl;

namespace WCloud.Framework.Common.Validator
{
    public static class Bootstrap
    {
        public static IServiceCollection AddFluentValidatorHelper(this IServiceCollection collection)
        {
            collection.AddTransient(typeof(IEntityValidationHelper<>), typeof(EntityFluentValidationHelper<>));
            return collection;
        }

        public static IServiceCollection RegEntityValidators(this IServiceCollection collection, Assembly[] search_in_assembly)
        {
            var all_types = search_in_assembly.GetAllTypes().Where(x => x.IsNormalClass()).ToArray();

            foreach (var type in all_types)
            {
                var validators = type.GetAllInterfaces_()
                    .Where(x => x.IsGenericType_(typeof(IEntityFluentValidator<>))).ToArray();

                //这里事实上只有一个
                foreach (var m in validators)
                {
                    collection.AddTransient(m, type);
                }
            }

            return collection;
        }
    }
}
