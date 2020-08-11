using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;
using WCloud.Core.Helper;
using WCloud.Core.Mapper;
using WCloud.Core.MessageBus;

namespace WCloud.Core
{
    public interface IWCloudContext<T> : IDisposable
    {
        IServiceProvider Provider { get; }
        ILogger Logger { get; }
        WCloudAdminInfo CurrentAdminInfo { get; }
        WCloudUserInfo CurrentUserInfo { get; }

        IMessagePublisher MessagePublisher { get; }
        ICacheKeyManager CacheKeyManager { get; }
        IStringArraySerializer StringArraySerializer { get; }
        IObjectMapper ObjectMapper { get; }
    }

    internal class DefaultWCloudContext<T> : IWCloudContext<T>
    {
        public IServiceProvider Provider { get; private set; }

        private readonly Lazy<WCloudAdminInfo> lazy_admin_info;
        public WCloudAdminInfo CurrentAdminInfo => this.lazy_admin_info.Value;

        private readonly Lazy<WCloudUserInfo> lazy_user_info;
        public WCloudUserInfo CurrentUserInfo => this.lazy_user_info.Value;

        private readonly Lazy<IMessagePublisher> lazy_publisher;
        public IMessagePublisher MessagePublisher => this.lazy_publisher.Value;

        private readonly Lazy<ICacheKeyManager> lazy_cache_key;
        public ICacheKeyManager CacheKeyManager => this.lazy_cache_key.Value;

        private readonly Lazy<IStringArraySerializer> lazy_string_array_serializer;
        public IStringArraySerializer StringArraySerializer => this.lazy_string_array_serializer.Value;

        private readonly Lazy<IObjectMapper> lazy_object_mapper;
        public IObjectMapper ObjectMapper => this.lazy_object_mapper.Value;

        private readonly Lazy<ILogger<T>> lazy_logger;
        public ILogger Logger => lazy_logger.Value;

        public DefaultWCloudContext(IServiceProvider provider)
        {
            Lazy<x> __lazy_resolve__<x>() => new Lazy<x>(() => this.Provider.Resolve_<x>());

            this.Provider = provider;
            this.Provider.Should().NotBeNull();
            this.lazy_logger = __lazy_resolve__<ILogger<T>>();
            this.lazy_admin_info = __lazy_resolve__<WCloudAdminInfo>();
            this.lazy_user_info = __lazy_resolve__<WCloudUserInfo>();
            this.lazy_publisher = __lazy_resolve__<IMessagePublisher>();
            this.lazy_cache_key = __lazy_resolve__<ICacheKeyManager>();
            this.lazy_string_array_serializer = __lazy_resolve__<IStringArraySerializer>();
            this.lazy_object_mapper = __lazy_resolve__<IObjectMapper>();
        }

        public void Dispose()
        {
#if DEBUG
            this.Logger.LogInformation("wcloud context disposed");
#endif
        }
    }
}
