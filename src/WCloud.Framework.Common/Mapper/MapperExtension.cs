using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.Mapper;

namespace WCloud.Framework.Common.Mapper
{
    public static class MapperExtension
    {
        public static IServiceCollection AddAbpObjectMapperProvider(this IServiceCollection collection)
        {
            collection.AddTransient<IDataMapper, AbpObjectMapperProvider>();
            return collection;
        }

        public static IServiceCollection AddAutoMapperProvider(this IServiceCollection collection)
        {
            collection.AddTransient<IDataMapper, AutoMapperProvider>();
            return collection;
        }
    }
}
