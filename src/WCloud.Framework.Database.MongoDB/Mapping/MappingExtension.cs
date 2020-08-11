using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    public static class MappingExtension
    {
        static void __reg_map__<T>(IMongoEntityMapping<T> config)
        {
            BsonClassMap.RegisterClassMap<T>(x =>
            {
                config.ConfigMap(x);
            });
        }

        public static IServiceCollection AddMongoMapping(this IServiceCollection collection, Assembly[] ass)
        {
            ass.Should().NotBeNullOrEmpty();

            Type __find_mapping_interface__(Type t) =>
                t.GetAllInterfaces_().FirstOrDefault(m => m.IsGenericType_(typeof(IMongoEntityMapping<>)));

            var mapping = ass.GetAllTypes()
                .Where(x => x.IsNormalClass())
                .Select(x => new
                {
                    MappingType = x,
                    MappingInterface = __find_mapping_interface__(x),
                })
                .Where(x => x.MappingInterface != null)
                .ToArray();

            var reg_method = typeof(MappingExtension).GetMethods(BindingFlags.NonPublic).FirstOrDefault(x => x.Name == nameof(__reg_map__));
            reg_method.Should().NotBeNull();

            foreach (var m in mapping)
            {
                collection.AddSingleton(m.MappingType);
                collection.AddSingleton(m.MappingInterface, m.MappingType);

                var entity_type = m.MappingInterface.GetGenericArguments().FirstOrDefault();
                entity_type.Should().NotBeNull();
                var map_instance = Activator.CreateInstance(m.MappingType);

                reg_method.MakeGenericMethod(entity_type).Invoke(null, new[] { map_instance });
            }

            return collection;
        }

        public static string GetMongoEntityCollectionName<T>(this IServiceProvider provider)
        {
            var res = GetMongoEntityCollectionName(provider, typeof(T));
            return res;
        }

        public static string GetMongoEntityCollectionName(this IServiceProvider provider, Type type)
        {
            var mapping_type = typeof(IMongoEntityMapping<>).MakeGenericType(type);

            var instance = (IMongoEntityMapping)provider.GetRequiredService(mapping_type);

            var res = instance.CollectionName;
            res.Should().NotBeNullOrEmpty();
            return res;
        }

        public static void BasicConfig<T>(this BsonClassMap<T> config)
            where T : MongoEntityBase
        {
            config.MapIdProperty(x => x.Id);
        }
    }
}
