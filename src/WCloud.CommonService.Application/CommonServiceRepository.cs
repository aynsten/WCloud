using System;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;

namespace WCloud.CommonService.Application
{
    public interface ICommonServiceEntity : Lib.data.IDBTable { }

    public interface ICommonServiceRepository<T> : IEFRepository<T> where T : BaseEntity, ICommonServiceEntity
    {
        //
    }

    public class CommonServiceRepository<T> : EFRepository<T, CommonServiceDbContext>, ICommonServiceRepository<T>
        where T : BaseEntity, ICommonServiceEntity
    {
        public CommonServiceRepository(IServiceProvider provider) : base(provider) { }
    }
}
