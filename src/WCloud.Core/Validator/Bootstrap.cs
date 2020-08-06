using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.Validator.FluentValidatorImpl;

namespace WCloud.Core.Validator
{
    public static class Bootstrap
    {
        public static IServiceCollection AddFluentValidatorHelper(this IServiceCollection collection)
        {
            collection.AddTransient(typeof(IEntityValidationHelper<>), typeof(EntityFluentValidationHelper<>));
            return collection;
        }

    }
}
