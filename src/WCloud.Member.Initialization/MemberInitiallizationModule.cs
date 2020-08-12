using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace WCloud.Member.Initialization
{
    public class MemberInitiallizationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var collection = context.Services;
            collection.AddTransient<IInitDataHelper, InitDataHelper>();
        }
    }
}
