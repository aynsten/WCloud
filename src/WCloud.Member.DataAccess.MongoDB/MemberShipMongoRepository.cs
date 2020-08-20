using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.MongoDB;
using WCloud.Framework.Database.MongoDB.Mapping;
using WCloud.Member.Domain;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.User;
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

    public class MongoDatabaseHelper : IMemberDatabaseHelper
    {
        private readonly IWCloudContext _context;
        public MongoDatabaseHelper(IWCloudContext<MongoDatabaseHelper> _context)
        {
            this._context = _context;
        }

        public async Task CreateDatabase()
        {
            var repo = this._context.Provider.Resolve_<IMemberMongoRepository<AdminEntity>>();
            //admin
            var names = repo.Collection.GetAllIndexNames();
            var index_name = "_unique_name_";
            if (!names.Contains(index_name))
            {
                var index = Builders<AdminEntity>.IndexKeys.Descending(x => x.UserName);
                var option = new CreateIndexOptions()
                {
                    Unique = true,
                    Name = index_name
                };
                await repo.Collection.Indexes.CreateOneAsync(new CreateIndexModel<AdminEntity>(index, option));
            }
            //user
            var user_collection = repo.Database.GetCollection<UserEntity>(this._context.Provider);
            names = user_collection.GetAllIndexNames();
            index_name = "_unique_name_";
            if (!names.Contains(index_name))
            {
                var index = Builders<UserEntity>.IndexKeys.Descending(x => x.UserName);
                var option = new CreateIndexOptions()
                {
                    Unique = true,
                    Name = index_name
                };
                await user_collection.Indexes.CreateOneAsync(new CreateIndexModel<UserEntity>(index, option));
            }
        }
    }
}
