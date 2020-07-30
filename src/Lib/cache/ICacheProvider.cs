using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lib.cache
{
    public interface ICacheProvider
    {
        ILogger Logger { get; }

        bool IsSet(string key);
        Task<bool> IsSetAsync(string key);

        void Remove(string key);
        Task RemoveAsync(string key);

        CacheResult<T> Get_<T>(string key);
        void Set_<T>(string key, T data, TimeSpan expire);

        Task<CacheResult<T>> GetAsync_<T>(string key);
        Task SetAsync_<T>(string key, T data, TimeSpan expire);
    }
}
