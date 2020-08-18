using Lib.data;
using Microsoft.EntityFrameworkCore;

namespace WCloud.Framework.Database.EntityFrameworkCore
{
    public abstract class EFContextBase : DbContext
    {
        public EFContextBase() { }

        public EFContextBase(DbContextOptions option) : base(option) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// 附加一个实体，如果已经存在就直接返回
        /// </summary>
        //protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : ModelBase, new()
        //{
        //    //little hack here until Entity Framework really supports stored procedures
        //    //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
        //    var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
        //    if (alreadyAttached == null)
        //    {
        //        //attach new entity
        //        Set<TEntity>().Attach(entity);
        //        return entity;
        //    }

        //    //entity is already loaded
        //    return alreadyAttached;
        //}

        /// <summary>
        /// 实体集合
        /// new的用法搜索 override new
        /// </summary>
        public new DbSet<T> Set<T>() where T : class, IDBTable
        {
            return base.Set<T>();
        }

    }
}
