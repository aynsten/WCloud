using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using WCloud.Member.Application;

namespace WCloud.Member.Startup
{
    [DependsOn(
        //typeof(WCloud.Member.DataAccess.EF.MemberDataAccessEFModule),
        //mongo
        typeof(WCloud.Member.DataAccess.MongoDB.MemberDataAccessMongoDBModule),
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
