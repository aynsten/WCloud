using System;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;

namespace WCloud.CommonService.Application
{
    public interface ICommonServiceEntity : Lib.data.IDBTable { }

    public interface ICommonServiceRepository<T> : IEFRepository<T> where T : EntityBase, ICommonServiceEntity
    {
        //
    }

    public class CommonServiceRepository<T> : EFRepository<T, CommonServiceDbContext>, ICommonServiceRepository<T>
        where T : EntityBase, ICommonServiceEntity
    {
        public CommonServiceRepository(IServiceProvider provider) : base(provider) { }
    }
}
