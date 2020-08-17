using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.EntityFrameworkCore;
using WCloud.Member.Domain;

namespace WCloud.Member.DataAccess.EF
{
    public static class MemberDataAccessExtension
    {
        public static string GetMemberShipConnectionStringOrThrow(this IConfiguration config)
        {
            var constr = config.GetConnectionString("db_member_ship");
            constr.Should().NotBeNullOrEmpty();
            return constr;
        }

        public static IServiceCollection AddMemberDataAccessEFProvider(this IServiceCollection collection)
        {
            collection.Configure<AbpDbContextOptions>(option =>
            {
                option.Configure<MemberShipDbContext>(config =>
                {
                    var cstr = config.ServiceProvider.ResolveConfig_().GetMemberShipConnectionStringOrThrow();

                    config.DbContextOptions.UseMySql(cstr, db => db.CommandTimeout((int)TimeSpan.FromSeconds(5).TotalSeconds));
                });
            });

            collection.AddAbpDbContext<MemberShipDbContext>(builder => { });
            collection.AddScoped(typeof(IMemberRepository<>), typeof(MemberShipRepository<>));
            collection.AddScoped(typeof(IMSRepository<>), typeof(MemberShipRepository<>));

            return collection;
        }
    }
}
