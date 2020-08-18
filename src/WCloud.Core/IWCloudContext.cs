using FluentAssertions;
using Lib.cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;
using WCloud.Core.DataSerializer;
using WCloud.Core.Mapper;
using WCloud.Core.MessageBus;
using WCloud.Core.Validator;

namespace WCloud.Core
{
    /// <summary>
    /// 在业务代码中使用，不要在底层基础框架中使用，不然容易导致循环依赖
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWCloudContext : IDisposable
    {
        IServiceProvider Provider { get; }
        ILogger Logger { get; }
        WCloudAdminInfo CurrentAdminInfo { get; }
        WCloudUserInfo CurrentUserInfo { get; }

        IMessagePublisher MessagePublisher { get; }
        ICacheProvider CacheProvider { get; }
        ICacheKeyManager CacheKeyManager { get; }
        IDataSerializer DataSerializer { get; }
        IDataMapper ObjectMapper { get; }
        IEntityValidationHelper EntityValidator { get; }
    }

    public interface IWCloudContext<T> : IWCloudContext { }

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

        private readonly Lazy<IDataSerializer> lazy_data_serializer;
        public IDataSerializer DataSerializer => this.lazy_data_serializer.Value;

        private readonly Lazy<IDataMapper> lazy_object_mapper;
        public IDataMapper ObjectMapper => this.lazy_object_mapper.Value;

        private readonly Lazy<ILogger<T>> lazy_logger;
        public ILogger Logger => lazy_logger.Value;

        private readonly Lazy<IEntityValidationHelper> lazy_entity_validator;
        public IEntityValidationHelper EntityValidator => this.lazy_entity_validator.Value;

        private readonly Lazy<ICacheProvider> lazy_cache_provider;
        public ICacheProvider CacheProvider => this.lazy_cache_provider.Value;

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
            this.lazy_cache_provider = __lazy_resolve__<ICacheProvider>();
            this.lazy_data_serializer = __lazy_resolve__<IDataSerializer>();
            this.lazy_object_mapper = __lazy_resolve__<IDataMapper>();
            this.lazy_entity_validator = __lazy_resolve__<IEntityValidationHelper>();
        }

        public void Dispose()
        {
#if DEBUG
            //todo 这里可能有问题，在销毁的时候获取logger
            this.Logger.LogInformation("wcloud context disposed");
#endif
        }
    }
}
