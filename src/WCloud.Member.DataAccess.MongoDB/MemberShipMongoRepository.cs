using System;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.MongoDB;
using WCloud.Member.Domain;

namespace WCloud.Member.DataAccess.MongoDB
{
    public class MemberShipMongoRepository<T> : MongoRepository<T, MongoConnectionWrapper>, IMemberRepository<T>
        where T : EntityBase, IMemberShipDBTable
    {
        public MemberShipMongoRepository(IServiceProvider provider) : base(provider)
        { }
    }
}
