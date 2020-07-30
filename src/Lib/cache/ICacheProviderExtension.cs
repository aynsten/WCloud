using Lib.extension;
using Lib.ioc;
using System;
using System.Threading.Tasks;

namespace Lib.cache
{
    public static class ICacheProviderExtension
    {
        public static ICacheProvider ResolveDistributedCache_(this IServiceProvider provider)
        {
            var res = provider.Resolve_<ICacheProvider>();
            return res;
        }

        public static T GetOrSet_<T>(this ICacheProvider provider, string key, Func<T> source, TimeSpan expire, Func<T, bool> cache_when = null)
        {
            //默认都缓存
            cache_when ??= (x => true);
            try
            {
                var res = provider.Get_<T>(key);
                if (res.Success)
                    return res.Data;

                var data = source.Invoke();

                //判断是否需要写入缓存
                var cache_data = cache_when.Invoke(data);
                if (cache_data)
                    provider.Set_(key, data, expire);

                return data;
            }
            catch (Exception e)
            {
                try
                {
                    //读取缓存失败就尝试移除缓存
                    provider.Remove(key);
                }
                catch (Exception ex)
                {
                    provider.Logger.AddErrorLog($"删除错误缓存key异常-缓存key:{key}", ex);
                }
                provider.Logger.AddErrorLog($"读取缓存异常-缓存key:{key}", e);
                //缓存错误
                return source.Invoke();
            }
        }

        public static async Task<T> GetOrSetAsync_<T>(this ICacheProvider provider, string key, Func<Task<T>> source, TimeSpan expire, Func<T, bool> cache_when = null)
        {
            //默认都缓存
            cache_when ??= (x => true);
            try
            {
                var res = await provider.GetAsync_<T>(key);
                if (res.Success)
                    return res.Data;

                var data = await source.Invoke();

                //判断是否需要写入缓存
                var cache_data = cache_when.Invoke(data);
                if (cache_data)
                    await provider.SetAsync_(key, data, expire);

                return data;
            }
            catch (Exception e)
            {
                try
                {
                    //读取缓存失败就尝试移除缓存
                    await provider.RemoveAsync(key);
                }
                catch (Exception ex)
                {
                    provider.Logger.AddErrorLog($"删除错误缓存key异常-缓存key:{key}", ex);
                }
                provider.Logger.AddErrorLog($"读取缓存异常-缓存key:{key}", e);
                //缓存错误
                return await source.Invoke();
            }
        }
    }
}
