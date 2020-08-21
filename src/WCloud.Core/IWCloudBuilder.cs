using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        public static WCloudBuilder AddWCloudBuilder(this IServiceCollection services)
        {
            var builder = new WCloudBuilder(services);

            services.AddSingleton(builder);
            services.AddWCloudCore();

            return builder;
        }
    }
}
