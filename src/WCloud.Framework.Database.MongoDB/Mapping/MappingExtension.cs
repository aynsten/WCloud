using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    public static class MappingExtension
    {
        public static IServiceCollection AddMongoMapping(this IServiceCollection collection, Assembly[] ass)
        {
            ass.Should().NotBeNullOrEmpty();

            var entities = ass.GetAllTypes().Where(x => x.IsNormalClass() && x.IsAssignableTo_<MongoEntityBase>()).ToArray();

            var mapping = entities.Select(x => new EntityMapping(x)).ToArray();

            var mapping_collection = mapping.ToList().AsReadOnly();

            collection.AddSingleton(mapping_collection);
            collection.AddSingleton<IReadOnlyCollection<EntityMapping>>(mapping_collection);

            return collection;
        }

        public static string GetMongoEntityCollectionName<T>(this IServiceProvider provider)
        {
            var res = GetMongoEntityCollectionName(provider, typeof(T));
            return res;
        }

        public static string GetMongoEntityCollectionName(this IServiceProvider provider, Type type)
        {
            var maps = provider.Resolve_<IReadOnlyCollection<EntityMapping>>();
            var res = maps.FirstOrDefault(x => x.EntityType == type)?.CollectionName;
            res.Should().NotBeNullOrEmpty();
            return res;
        }
    }
}
