using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;
using WCloud.Framework.Common.Validator;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain;
using WCloud.Member.Shared;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.Application
{
    [DependsOn(
        typeof(MemberSharedModule),
        typeof(AbpEntityFrameworkCoreMySQLModule)
        )]
    public class MemberModule : AbpModule
    {
        readonly Assembly __ass__ = typeof(MemberModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            this.__add_basic_dependency__(context.Services);

            this.__add_db_context__(context.Services);

            context.Services.AutoRegister(new Assembly[] { this.__ass__ });
            context.Services.RegEntityValidators(new Assembly[] { this.__ass__ });
        }

        void __add_basic_dependency__(IServiceCollection collection)
        {
            //helper
            collection.AddSingleton<IPasswordHelper, DefaultPasswordHelper>();
            collection.AddSingleton<IMobilePhoneFormatter, DefaultMobilePhoneFormatter>();
        }

        void __add_db_context__(IServiceCollection collection)
        {
            this.Configure<AbpDbContextOptions>(option =>
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
        }
    }
}
