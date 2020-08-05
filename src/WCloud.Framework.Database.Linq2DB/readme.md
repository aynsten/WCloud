# linq2db

```csharp
    public interface ILinq2DBRepository<T> : ILinqRepository<T>, IRepository<T> where T : class, IDBTable
    {
        IDataContext TryGetNewDatabaseOrThrow();

        IDataContext Database { get; }
    }
```

```csharp
    public abstract partial class Linq2DBRepositoryBase<T> : ILinq2DBRepository<T>
        where T : class, IDBTable
    {
        private readonly Func<IDataContext> _db_getter;
        private readonly IDataContext _context;

        protected Linq2DBRepositoryBase(Func<IDataContext> db_getter)
        {
            this._db_getter = db_getter ?? throw new ArgumentNullException(nameof(db_getter));
            this._context = this._db_getter.Invoke() ?? throw new ArgumentNullException(nameof(this._context));
        }

        public IDataContext TryGetNewDatabaseOrThrow()
        {
            var db = this._db_getter.Invoke();
            if (object.ReferenceEquals(db, this.Database))
                throw new NotSupportedException("当前实现不支持创建新的连接实例");

            return db;
        }

        public IDataContext Database => this._context;
        public IQueryable<T> Queryable => this._context.GetTable<T>();

        public virtual void Dispose()
        {
            this._context?.Dispose();
        }
    }
```

```csharp
    public abstract partial class Linq2DBRepositoryBase<T>
    {
        public int Insert(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.Insert(model);
        }

        public Task<int> InsertAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.InsertAsync(model);
        }

        public int Delete(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.Delete(model);
        }

        public Task<int> DeleteAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.DeleteAsync(model);
        }

        public int DeleteWhere(Expression<Func<T, bool>> where)
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            return this.Database.Delete(where);
        }

        public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            return this.Database.DeleteAsync(where);
        }

        public bool Exist(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).Any();
        }

        public Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).AnyAsync();
        }

        public int QueryCount(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).Count();
        }

        public Task<int> QueryCountAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).CountAsync();
        }

        public T QueryOne(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).FirstOrDefault();
        }

        public Task<T> QueryOneAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).FirstOrDefaultAsync();
        }

        public List<T> QueryMany(Expression<Func<T, bool>> where, int? count = null)
        {
            return this.QueryList<object>(where: where, count: count);
        }

        public Task<List<T>> QueryManyAsync(Expression<Func<T, bool>> where, int? count = null)
        {
            return this.QueryListAsync<object>(where: where, count: count);
        }

        public List<T> QueryList<OrderByColumnType>(Expression<Func<T, bool>> where, Expression<Func<T, OrderByColumnType>> orderby = null, bool Desc = true, int? start = null, int? count = null)
        {
            var query = this.Queryable;
            query = query.WhereIfNotNull(where);
            if (orderby != null)
            {
                if (Desc)
                    query = query.OrderByDescending(orderby);
                else
                    query = query.OrderBy(orderby);
            }
            if (start != null)
            {
                if (orderby == null)
                    throw new ArgumentNullException("必须先排序");
                query = query.Skip(start.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }
            return query.ToList();
        }

        public Task<List<T>> QueryListAsync<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null,
            bool Desc = true, int? start = null, int? count = null)
        {
            var query = this.Queryable;
            query = query.WhereIfNotNull(where);
            if (orderby != null)
            {
                if (Desc)
                    query = query.OrderByDescending(orderby);
                else
                    query = query.OrderBy(orderby);
            }
            if (start != null)
            {
                if (orderby == null)
                    throw new ArgumentNullException("必须先排序");
                query = query.Skip(start.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }
            return query.ToListAsync();
        }

        public int Update(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.Update(model);
        }

        public Task<int> UpdateAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.UpdateAsync(model);
        }
    }
```
