using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Volo.Abp.Modularity;

namespace WCloud.Member.DataAccess.MongoDB
{
    [DependsOn(
        typeof(Volo.Abp.MongoDB.AbpMongoDbModule)
        )]
    public class MemberDataAccessMongoDBModule : AbpModule
    {
        private readonly Assembly __this_ass__ = typeof(MemberDataAccessMongoDBModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMemberDataAccessMongoDBProvider();
            context.Services.AutoRegister(new[] { this.__this_ass__ });
        }
    }
}
