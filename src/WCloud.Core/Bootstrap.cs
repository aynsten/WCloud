using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using WCloud.Core.Validator.FluentValidatorImpl;

namespace WCloud.Core
{
    public static class InfrastructureBootstrap
    {
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
