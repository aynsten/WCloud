using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace WCloud.Member.InternalApi.Client
{
    public class MemberInternalApiHttpClientModule : AbpModule
    {
        private readonly Assembly __this_ass__ = typeof(MemberInternalApiHttpClientModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var collection = context.Services;
            collection.AutoRegister(new[] { this.__this_ass__ });
        }
    }
}
