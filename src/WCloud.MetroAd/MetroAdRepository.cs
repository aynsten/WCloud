using Lib.data;
using System;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;

namespace WCloud.MetroAd
{
    public interface IMetroAdTable : IDBTable { }

    public interface IMetroAdRepository<T> : IEFRepository<T> where T : class, IMetroAdTable
    {
        //
    }

    public class MetroAdRepository<T> : EFRepository<T, MetroAdDbContext>, IMetroAdRepository<T> where T : class, IMetroAdTable
    {
        public MetroAdRepository(IServiceProvider provider) : base(provider)
        {
            //
        }
    }
}
