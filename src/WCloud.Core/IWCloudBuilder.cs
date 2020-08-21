using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WCloud.Core
{
    public interface IWCloudBuilder
    {
        IConfiguration Configuration { get; }
        IServiceCollection Services { get; }
    }

    public sealed class WCloudBuilder : IWCloudBuilder
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
        public static IWCloudBuilder AddWCloudBuilder(this IServiceCollection services)
        {
            var builder = new WCloudBuilder(services);

            services.AddSingleton(builder);
            services.AddSingleton<IWCloudBuilder>(builder);
            services.AddWCloudCore();

            return builder;
        }

        public static IWCloudBuilder GetWCloudBuilder(this IServiceCollection services)
        {
            var res = services.GetSingletonInstance<IWCloudBuilder>();
            return res;
        }

        /// <summary>
        /// 自动查找程序集注册依赖
        /// </summary>
        public static IWCloudBuilder AutoRegister(this IWCloudBuilder builder, Assembly[] ass)
        {
            builder.Services.AutoRegister(ass);
            return builder;
        }
    }
}
