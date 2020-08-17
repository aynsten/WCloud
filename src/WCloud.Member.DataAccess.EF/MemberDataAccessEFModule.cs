using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace WCloud.Member.DataAccess.EF
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreMySQLModule)
        )]
    public class MemberDataAccessEFModule : AbpModule
    {
        private readonly Assembly __this_ass__ = typeof(MemberDataAccessEFModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMemberDataAccessEFProvider();
            context.Services.AutoRegister(new[] { this.__this_ass__ });

            base.ConfigureServices(context);
        }
    }
}
