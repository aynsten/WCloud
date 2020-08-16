using Volo.Abp.Modularity;

namespace WCloud.Member.DataAccess.MongoDB
{
    public class MemberDataAccessMongoDBModule:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMemberDataAccessMongoDBProvider();
        }
    }
}
