using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.Storage;

namespace WCloud.Framework.Storage.MongoDB
{
    public static class MongoDBExtension
    {
        public static IServiceCollection AddMongoDBUploadProvider(this IServiceCollection services, IConfiguration config)
        {
            var option = new MongoDBStorageOption()
            {
            };

            services.AddSingleton(x => option);
            services.AddSingleton(new MongoUploadProviderConnectionWrapper("", ""));
            services.AddSingleton<IUploadHelper>(provider =>
            {
                var wrapper = provider.Resolve_<MongoUploadProviderConnectionWrapper>();
                var uploader_option = provider.Resolve_<MongoDBStorageOption>();
                var res = new MongoDBUploadProvider(wrapper, uploader_option);
                return res;
            });

            return services;
        }
    }
}