using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    /// <summary>
    /// 永远不会修改iid，uid，create time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="DbContextType"></typeparam>
    public class WCloudEFRepository<T, DbContextType> : EFRepository<T, DbContextType>
        where T : BaseEntity
        where DbContextType : DbContext
    {
        public WCloudEFRepository(IServiceProvider provider) : base(provider) { }

        protected override EntityEntry<T> __TrackEntity__(T model)
        {
            var tracker = base.__TrackEntity__(model);

            tracker.Property(x => x.Id).IsModified = false;
            tracker.Property(x => x.UID).IsModified = false;
            tracker.Property(x => x.CreateTimeUtc).IsModified = false;

            return tracker;
        }

    }
}
