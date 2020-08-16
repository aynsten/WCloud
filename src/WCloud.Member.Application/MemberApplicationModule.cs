using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using WCloud.Framework.Common.Validator;
using WCloud.Member.Domain;

namespace WCloud.Member.Application
{
    [DependsOn(
        typeof(MemberDomainModule)
        )]
    public class MemberApplicationModule : AbpModule
    {
        readonly Assembly __ass__ = typeof(MemberApplicationModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AutoRegister(new Assembly[] { this.__ass__ });
            context.Services.RegEntityValidators(new Assembly[] { this.__ass__ });
        }
    }
}
