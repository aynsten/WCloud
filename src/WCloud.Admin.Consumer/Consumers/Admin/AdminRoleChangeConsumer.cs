using Lib.cache;
using Lib.ioc;
using System;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Framework.MessageBus;
using WCloud.Member.Shared.MessageBody;

namespace WCloud.Admin.MessageConsumers
{
    /// <summary>
    /// 用户角色发生变更，用于清楚缓存
    /// </summary>
    public class AdminRoleChangedConsumer : IMessageConsumer<UserRoleChangedMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public AdminRoleChangedConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<UserRoleChangedMessage> context)
        {
            var cache = provider.ResolveDistributedCache_();
            var keyManager = provider.Resolve_<ICacheKeyManager>();

            var key = keyManager.AdminPermission(context.Message?.UserUID);

            await cache.RemoveAsync(key);
        }
    }
}
