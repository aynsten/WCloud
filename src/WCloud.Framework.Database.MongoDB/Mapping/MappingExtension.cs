using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Reflection;
using WCloud.Framework.Database.Abstractions.Entity;

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
                collection.AddSingleton(typeof(IMongoEntityMapping), m.MappingType);

                var entity_type = m.MappingInterface.GetGenericArguments().FirstOrDefault();
                entity_type.Should().NotBeNull();
                var map_instance = Activator.CreateInstance(m.MappingType);

                reg_method.MakeGenericMethod(entity_type).Invoke(null, new[] { map_instance });
            }

            return collection;
        }

        public static string TryGetMongoEntityCollectionName<T>(this IServiceProvider provider)
        {
            var res = TryGetMongoEntityCollectionName(provider, typeof(T));
            return res;
        }

        public static string TryGetMongoEntityCollectionName(this IServiceProvider provider, Type type)
        {
            var mapping_type = typeof(IMongoEntityMapping<>).MakeGenericType(type);
            var service = provider.GetService(mapping_type);
            if (service != null && service is IMongoEntityMapping instance)
            {
                var res = instance.CollectionName;
                res.Should().NotBeNullOrEmpty();
                return res;
            }
            else
            {
                return type.Name;
            }
        }

        public static void BasicConfig<T>(this BsonClassMap<T> config) where T : EntityBase
        {
            config.MapIdProperty(x => x.Id);
        }

        public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase db, IServiceProvider provider)
        {
            var collection_name = provider.TryGetMongoEntityCollectionName<T>();
            var res = db.GetCollection<T>(collection_name);
            return res;
        }
    }
}
