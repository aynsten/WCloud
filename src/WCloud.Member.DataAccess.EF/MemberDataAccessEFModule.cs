using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace WCloud.Member.DataAccess.EF
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreMySQLModule)
        )]
    public class MemberDataAccessEFModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMemberDataAccessEFProvider();
            base.ConfigureServices(context);
        }
    }
}
