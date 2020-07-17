using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.Abstractions.Service
{
    public interface IBasicServiceRemovable<T> : IBasicService<T> where T : ILogicalDeletion
    {
        Task RemoveByUIDs(params string[] uids);

        Task RecoverByUIDs(params string[] uids);
    }
}
