using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.MongoDB;
using WCloud.Member.Domain;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.DataAccess.MongoDB
{
    public interface IMemberMongoRepository<T> : IMongoRepository<T>, IMemberRepository<T> where T : EntityBase, IMemberShipDBTable
    {
        //
    }
    public class MemberShipMongoRepository<T> : MongoRepository<T, MongoConnectionWrapper>, IMemberMongoRepository<T>, IMemberRepository<T>
        where T : EntityBase, IMemberShipDBTable
    {
        public MemberShipMongoRepository(IServiceProvider provider) : base(provider)
        { }
    }

    public class MongoDatabaseHelper : IDatabaseHelper
    {
        private readonly IWCloudContext _context;
        public MongoDatabaseHelper(IWCloudContext<MongoDatabaseHelper> _context)
        {
            this._context = _context;
        }

        public async Task CreateDatabase()
        {
            var repo = this._context.Provider.Resolve_<IMemberMongoRepository<AdminEntity>>();

            repo.Collection.GetAllIndexNames().Contains("");

        }
    }
}
