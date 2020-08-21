using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WCloud.Core
{
    public sealed class WCloudBuilder
    {
        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        public WCloudBuilder(IServiceCollection collection)
        {
            collection.Should().NotBeNull();
            this.Services = collection;
            this.Configuration = this.Services.GetConfiguration();
        }
    }

    public static class WCloudBuilderExtension
    {
        /// <summary>
        /// 创建builder
        /// </summary>
        public static WCloudBuilder AddWCloudBuilder(this IServiceCollection services)
        {
            var builder = new WCloudBuilder(services);

            services.AddSingleton(builder);
            services.AddWCloudCore();

            return builder;
        }

        /// <summary>
        /// 自动查找程序集注册依赖
        /// </summary>
        public static WCloudBuilder AutoRegister(this WCloudBuilder builder, Assembly[] ass)
        {
            builder.Services.AutoRegister(ass);
            return builder;
        }
    }
}
