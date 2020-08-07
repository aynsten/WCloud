using System;
using System.Reflection;
using FluentAssertions;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using WCloud.Framework.Common.Validator;
using WCloud.Framework.Storage;

namespace WCloud.CommonService.Application
{
    [DependsOn(
        typeof(Volo.Abp.EntityFrameworkCore.MySQL.AbpEntityFrameworkCoreMySQLModule),
        typeof(Volo.Abp.AutoMapper.AbpAutoMapperModule)
        )]
    public class CommonServiceModule : AbpModule
    {
        readonly Assembly __ass__ = typeof(CommonServiceModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();

            this.__add_dto_mapper__();

            this.__add_basic_dependency__(context.Services, _config);
            this.__add_db_context__(context.Services);
            context.Services.AutoRegister(new Assembly[] { this.__ass__ });
            context.Services.RegEntityValidators(new Assembly[] { this.__ass__ });
        }

        void __add_dto_mapper__()
        {
            this.Configure<AbpAutoMapperOptions>(options =>
            {
                //Add all mappings defined in the assembly of the MyModule class
                options.AddMaps<CommonServiceModule>(validate: true);
            });
        }

        void __add_basic_dependency__(IServiceCollection services, IConfiguration config)
        {
            var save_path = config["static_save_path"];

            if (ValidateHelper.IsNotEmpty(save_path))
            {
                System.IO.Directory.Exists(save_path).Should().BeTrue("文件上传保存目录不存在");
                services.AddLocalDiskStorage();
            }
            else
            {
                services.AddQiniu(config);
            }
        }

        void __add_db_context__(IServiceCollection collection)
        {
            this.Configure<AbpDbContextOptions>(option =>
            {
                option.Configure<CommonServiceDbContext>(config =>
                {
                    var cstr = config.ServiceProvider.ResolveConfig_().GetCommonServiceConnectionStringOrThrow();

                    config.DbContextOptions.UseMySql(cstr, db => db.CommandTimeout((int)TimeSpan.FromSeconds(5).TotalSeconds));
                });
            });

            collection.AddAbpDbContext<CommonServiceDbContext>(builder => { });
            //collection.AddTransient<ErpDbContext>(s => s.Resolve_<IDbContextProvider<ErpDbContext>>().GetDbContext());
            collection.AddScoped(typeof(ICommonServiceRepository<>), typeof(CommonServiceRepository<>));
        }
    }
}
