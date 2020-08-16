using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Modularity;
using WCloud.Member.Shared;

namespace WCloud.Member.Domain
{
    [DependsOn(
        typeof(MemberSharedModule)
        )]
    public class MemberDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
        }
    }
}
