using WCloud.Core.Cache;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Core.MessageBus;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Shared.MessageBody;

namespace WCloud.Admin.MessageConsumers
{
    public class RolePermissionChangedConsumer : IMessageConsumer<RolePermissionUpdatedMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public RolePermissionChangedConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<RolePermissionUpdatedMessage> context)
        {
            try
            {
                var role_id = context.Message?.RoleUID;
                if (ValidateHelper.IsEmpty(role_id))
                    throw new ArgumentNullException(nameof(context));

                var cache = provider.ResolveDistributedCache_();
                var keyManager = provider.Resolve_<ICacheKeyManager>();
                var repo = provider.Resolve_<IRoleRepository>();
                var query = repo.Queryable;

                var page = 1;
                var page_size = 500;

                while (true)
                {
                    var list = await repo.QueryAdminRoleEntity(role_id, page, page_size);

                    if (!list.Any())
                    {
                        break;
                    }

                    foreach (var m in list)
                    {
                        var key = keyManager.AdminPermission(m.AdminUID);
                        await cache.RemoveAsync(key);
                    }

                    ++page;
                }
            }
            catch (Exception e)
            {
                var logger = provider.Resolve_<ILogger<RolePermissionChangedConsumer>>();
                logger.AddErrorLog(nameof(RolePermissionChangedConsumer), e);
            }
        }
    }
}
