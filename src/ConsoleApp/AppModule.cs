using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace ConsoleApp
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(WCloud.Member.Application.MemberModule),
        typeof(WCloud.CommonService.Application.CommonServiceModule)
        )]
    public class AppModule : AbpModule
    {

    }
}
