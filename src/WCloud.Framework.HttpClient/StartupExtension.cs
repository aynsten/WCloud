using Microsoft.Extensions.DependencyInjection;
using WCloud.Core;

namespace WCloud.Framework.HttpClient_
{
    public static class StartupExtension
    {
        /// <summary>
        /// 注册网络请求依赖
        /// </summary>
        public static IWCloudBuilder AddHttpClient(this IWCloudBuilder builder)
        {
            builder.Services.AddHttpClient();
            return builder;
        }
    }
}
