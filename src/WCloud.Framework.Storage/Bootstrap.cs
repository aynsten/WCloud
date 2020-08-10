using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Storage.Qiniu_;

namespace WCloud.Framework.Storage
{
    public static class ExtraBootstrap
    {
        public static IServiceCollection AddQiniu(this IServiceCollection services, IConfiguration config)
        {
            var option = new QiniuOption()
            {
                QiniuAccessKey = config["qiniu:ak"],
                QiniuSecretKey = config["qiniu:sk"],
                QiniuBucketName = config["qiniu:bucket"],
                QiniuBaseUrl = config["qiniu:base_url"],
            };

            services.AddSingleton(x => option);
            services.AddSingleton<IQiniuHelper, QiniuHelper>();
            services.AddSingleton<IUploadHelper, QiniuHelper>();

            return services;
        }

        public static IServiceCollection AddLocalDiskStorage(this IServiceCollection services)
        {
            services.AddSingleton<IUploadHelper, LocalDiskUploadHelper>();

            return services;
        }
    }
}
