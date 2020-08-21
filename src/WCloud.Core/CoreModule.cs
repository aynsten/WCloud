using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace WCloud.Core
{
    //[DependsOn()]
    public class CoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddWCloudCore();

            context.Services.AutoRegister(new[] { this.GetType().Assembly });
        }
    }
}
