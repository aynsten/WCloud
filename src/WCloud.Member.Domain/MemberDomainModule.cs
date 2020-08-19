using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using WCloud.Member.Shared;

namespace WCloud.Member.Domain
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(MemberSharedModule)
        )]
    public class MemberDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var collection = context.Services;

            collection.AutoRegister(new[] { this.GetType().Assembly });

            collection.Configure<AbpAutoMapperOptions>(option =>
            {
                option.AddMaps<MemberDomainModule>(validate: true);
            });
        }
    }
}
