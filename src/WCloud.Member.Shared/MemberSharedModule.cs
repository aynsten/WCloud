using System.Reflection;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using WCloud.Member.Shared.Localization;

namespace WCloud.Member.Shared
{
    [DependsOn(
        typeof(AbpLocalizationModule)
        )]
    public class MemberSharedModule : AbpModule
    {
        private readonly Assembly __ass__ = typeof(MemberSharedModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            this.Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<MemberSharedModule>(baseNamespace: "WCloud.Member.Shared");
            });
            this.Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));

                options.Resources
                .Add<MemberShipResource>(defaultCultureName: "zh-Hans")
                .AddVirtualJson("/Localization/MemberShip");
                //.AddBaseTypes(typeof(AbpValidationResource));
            });
        }
    }
}
