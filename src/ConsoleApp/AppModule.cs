using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using WCloud.Core;
using WCloud.Member.Startup;

namespace ConsoleApp
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(CoreModule),
        typeof(MemberStartupModule),
        typeof(WCloud.CommonService.Application.CommonServiceModule)
        )]
    public class AppModule : AbpModule
    {

    }
}
