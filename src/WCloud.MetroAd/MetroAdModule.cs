using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Volo.Abp.AutoMapper;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using WCloud.Core;
using WCloud.MetroAd.Localization;

namespace WCloud.MetroAd
{
    [DependsOn(
        typeof(Volo.Abp.EntityFrameworkCore.AbpEntityFrameworkCoreModule),
        typeof(Volo.Abp.EntityFrameworkCore.MySQL.AbpEntityFrameworkCoreMySQLModule),
        typeof(Volo.Abp.AutoMapper.AbpAutoMapperModule)
        )]
    public class MetroAdModule : AbpModule
    {
        readonly Assembly __ass__ = typeof(MetroAdModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            this.__add_embedded_file__(context);
            this.__add_localize__();

            this.__add_dto_mapper__();
            this.__add_db_context__(context.Services);
            context.Services.AutoRegister(new Assembly[] { this.__ass__ });
            context.Services.RegEntityValidators(new Assembly[] { this.__ass__ });
        }

        void __add_embedded_file__(ServiceConfigurationContext context)
        {
            this.Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<MetroAdModule>(baseNamespace: "WCloud.MetroAd");
            });
        }

        void __add_localize__()
        {
            this.Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));

                options.Resources
                .Add<MetroadResource>(defaultCultureName: "zh-Hans")
                .AddVirtualJson("/Localization/Metroad");
                //.AddBaseTypes(typeof(AbpValidationResource));
            });
        }

        void __add_dto_mapper__()
        {
            this.Configure<AbpAutoMapperOptions>(options =>
            {
                //Add all mappings defined in the assembly of the MyModule class
                options.AddMaps<MetroAdModule>(validate: true);
            });
        }

        void __add_db_context__(IServiceCollection collection)
        {
            this.Configure<AbpDbContextOptions>(option =>
            {
                option.Configure<MetroAdDbContext>(config =>
                {
                    var cstr = config.ServiceProvider.ResolveConfig_().GetMetroAdConnectionStringOrThrow();

                    config.DbContextOptions.UseMySql(cstr, db => db.CommandTimeout((int)TimeSpan.FromSeconds(5).TotalSeconds));
                });
            });

            collection.AddAbpDbContext<MetroAdDbContext>(builder => { });
            collection.AddScoped(typeof(IMetroAdRepository<>), typeof(MetroAdRepository<>));
        }
    }
}
