using System;
using System.Linq;
using System.Threading.Tasks;
using Lib.cache;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WCloud.Core.Cache;
using WCloud.Core.MessageBus;
using WCloud.Member.DataAccess.EF;
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
                var repo = provider.Resolve_<IMSRepository<AdminRoleEntity>>();
                var query = repo.NoTrackingQueryable;

                var max_id = -1L;
                var batch_size = 500;
                while (true)
                {
                    var list = await query
                        .Where(x => x.RoleUID == role_id)
                        .Where(x => x.Id > max_id)
                        .OrderBy(x => x.Id).Take(batch_size)
                        .Select(x => new { x.Id, x.AdminUID }).ToListAsync();

                    if (!list.Any())
                    {
                        break;
                    }

                    foreach (var m in list)
                    {
                        var key = keyManager.AdminPermission(m.AdminUID);
                        await cache.RemoveAsync(key);
                    }

                    max_id = list.Max(x => x.Id);
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
