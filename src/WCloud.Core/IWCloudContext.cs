using System;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;
using WCloud.Core.Helper;
using WCloud.Core.MessageBus;

namespace WCloud.Core
{
    public interface IWCloudContext : IDisposable
    {
        IServiceProvider Provider { get; }
        WCloudAdminInfo CurrentAdminInfo { get; }
        WCloudUserInfo CurrentUserInfo { get; }

        IMessagePublisher MessagePublisher { get; }
        ICacheKeyManager CacheKeyManager { get; }
        IStringArraySerializer StringArraySerializer { get; }
    }
}
