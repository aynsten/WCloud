using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.CommonService.Application.KVStore
{
    public interface IKVStoreService : IBasicService<KVStoreEntity>, IAutoRegistered
    {
        Task<string> GetValue(string key);
        Task<T> GetValue<T>(string key) where T : class;
        Task SetValue<T>(string key, T data) where T : class;
    }
}
