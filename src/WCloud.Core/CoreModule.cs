using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace WCloud.Core
{
    //[DependsOn()]
    public class CoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var deft = typeof(DefaultWCloudContext<>).MakeGenericType(typeof(CoreModule));
            context.Services.AddScoped(typeof(IWCloudContext), deft);
            context.Services.AddScoped(typeof(IWCloudContext<>), typeof(DefaultWCloudContext<>));

            context.Services.AutoRegister(new[] { this.GetType().Assembly });
        }
    }
}
