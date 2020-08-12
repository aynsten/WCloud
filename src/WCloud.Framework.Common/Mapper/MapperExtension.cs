using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.Mapper;

namespace WCloud.Framework.Common.Mapper
{
    public static class MapperExtension
    {
        public static IServiceCollection AddAbpObjectMapperProvider(this IServiceCollection collection)
        {
            collection.AddTransient<IObjectMapper, AbpObjectMapperProvider>();
            return collection;
        }
    }
}
