﻿using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Database.MongoDB;
using WCloud.Framework.Database.MongoDB.Mapping;
using WCloud.Member.Domain;

namespace WCloud.Member.DataAccess.MongoDB
{
    public static class MongoDataAccessExtension
    {
        public static string GetMemberMongoDBConnectionStringOrThrow(this IConfiguration config)
        {
            var res = config.GetConnectionString("member_mongo");
            res.Should().NotBeNullOrEmpty();
            return res;
        }

        public static IServiceCollection AddMemberDataAccessMongoDBProvider(this IServiceCollection collection)
        {
            var config = collection.GetConfiguration();
            var connection_string = config.GetMemberMongoDBConnectionStringOrThrow();

            collection.AddMongoDB("wcloud_member_ship", connection_string);
            collection.AddMongoMapping(new[] { typeof(MemberDataAccessMongoDBModule).Assembly });

            collection.AddScoped(typeof(IMemberRepository<>), typeof(MemberShipMongoRepository<>));
            collection.AddScoped(typeof(IMemberMongoRepository<>), typeof(MemberShipMongoRepository<>));

            return collection;
        }
    }
}
