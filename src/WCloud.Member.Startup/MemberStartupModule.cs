#define mysql
#undef mysql
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using WCloud.Member.Application;

namespace WCloud.Member.Startup
{
    [DependsOn(
#if mysql
        //ef-mysql
        typeof(WCloud.Member.DataAccess.EF.MemberDataAccessEFModule),
#else
        //mongo
        typeof(WCloud.Member.DataAccess.MongoDB.MemberDataAccessMongoDBModule),
#endif
        typeof(MemberApplicationModule)
        )]
    public class MemberStartupModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var collection = context.Services;
            var config = collection.GetConfiguration();

            base.ConfigureServices(context);
        }
    }
}
