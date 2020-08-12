﻿using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using WCloud.Core;

namespace ConsoleApp
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(CoreModule),
        typeof(WCloud.Member.Application.MemberModule),
        typeof(WCloud.CommonService.Application.CommonServiceModule)
        )]
    public class AppModule : AbpModule
    {

    }
}
