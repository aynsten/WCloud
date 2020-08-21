using Microsoft.Extensions.DependencyInjection;
using WCloud.Core;

namespace WCloud.Framework.HttpClient
{
    public static class StartupExtension
    {
        /// <summary>
        /// 注册网络请求依赖
        /// </summary>
        public static WCloudBuilder AddHttpClient(this WCloudBuilder builder)
        {
            builder.Services.AddHttpClient();
            return builder;
        }
    }
}
