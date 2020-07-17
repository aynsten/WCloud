using System;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Service;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;

namespace WCloud.Framework.Database.EntityFrameworkCore.Service
{
    public abstract class BasicServiceRemovable<T> : BasicService<T>, IBasicServiceRemovable<T> where T : BaseEntity, ILogicalDeletion
    {
        public BasicServiceRemovable(IServiceProvider provider, IEFRepository<T> _repo) : base(provider, _repo)
        {
            //
        }

        public async Task RecoverByUIDs(params string[] uids)
        {
            await this._repo.RecoverByUIDs(uids);
        }

        public async Task RemoveByUIDs(params string[] uids)
        {
            await this._repo.RemoveByUIDs(uids);
        }
    }
}
