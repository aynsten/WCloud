using FluentAssertions;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.MongoDB.Mapping;

namespace WCloud.Framework.Database.MongoDB
{
    public class MongoRepository<T, ConnectionWrapper> : IMongoRepository<T>
        where T : EntityBase
        where ConnectionWrapper : MongoConnectionWrapper
    {
        private readonly IServiceProvider provider;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<T> _collection;

        public IQueryable<T> Queryable => this._collection.AsQueryable();
        public IMongoCollection<T> Collection => this._collection;

        public MongoRepository(IServiceProvider provider) : this(provider, provider.Resolve_<ConnectionWrapper>())
        { }

        public MongoRepository(IServiceProvider provider, ConnectionWrapper wrapper)
        {
            this.provider = provider;
            this._client = wrapper.Client;

            this._db = this._client.GetDatabase(wrapper.DatabaseName);

            var collection_name = this.provider.GetMongoEntityCollectionName<T>();
            this._collection = this._db.GetCollection<T>(collection_name);
        }

        public virtual List<T> QueryNearBy(Expression<Func<T, bool>> where, int page, int pagesize,
            Expression<Func<T, object>> field, GeoInfo point,
            double? max_distance = null, double? min_distance = null)
        {
            var condition = Builders<T>.Filter.Empty;
            condition &= Builders<T>.Filter.Near(field, point.Lat, point.Lon, max_distance, min_distance);
            if (where != null)
            {
                condition &= Builders<T>.Filter.Where(where);
            }

            return this._collection.Find(condition).QueryPage(page, pagesize).ToList();
        }

        public virtual int Insert(T model)
        {
            model.Should().NotBeNull();

            this._collection.InsertOne(model);
            return 1;
        }

        public virtual async Task<int> InsertAsync(T model)
        {
            model.Should().NotBeNull();

            await this._collection.InsertOneAsync(model);
            return 1;
        }

        public virtual int InsertBulk(IEnumerable<T> models)
        {
            models.Should().NotBeNullOrEmpty();

            this._collection.InsertMany(models);

            return models.Count();
        }

        public virtual async Task<int> InsertBulkAsync(IEnumerable<T> models)
        {
            models.Should().NotBeNullOrEmpty();

            await this._collection.InsertManyAsync(models);

            return models.Count();
        }

        public virtual int Delete(T model)
        {
            model.Should().NotBeNull();

            var filter = Builders<T>.Filter.Where(x => x.Id == model.Id);
            return (int)this._collection.DeleteMany(filter).DeletedCount;
        }

        public virtual async Task<int> DeleteAsync(T model)
        {
            model.Should().NotBeNull();

            var filter = Builders<T>.Filter.Where(x => x.Id == model.Id);
            return (int)(await this._collection.DeleteManyAsync(filter)).DeletedCount;
        }

        public virtual int DeleteWhere(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();

            return (int)this._collection.DeleteMany(where).DeletedCount;
        }

        public virtual async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();

            return (int)(await this._collection.DeleteManyAsync(where)).DeletedCount;
        }

        public virtual bool Exist(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();
            return this._collection.Find(where).Take(1).FirstOrDefault() != null;
        }

        public virtual async Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();
            return (await this._collection.FindAsync(where)).FirstOrDefault() != null;
        }

        public virtual int QueryCount(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();
            return (int)this._collection.CountDocuments(where);
        }

        public virtual async Task<int> QueryCountAsync(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();
            return (int)(await this._collection.CountDocumentsAsync(where));
        }

        public virtual T QueryOne(Expression<Func<T, bool>> where)
        {
            return this.QueryMany<object>(where, 1).FirstOrDefault();
        }

        public virtual async Task<T> QueryOneAsync(Expression<Func<T, bool>> where)
        {
            return (await this.QueryManyAsync<object>(where, 1)).FirstOrDefault();
        }

        protected virtual IFindFluent<T, T> __query_many__<OrderByColumnType>(Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null, bool Desc = true,
            int? start = null, int? count = null)
        {
            var condition = Builders<T>.Filter.Empty;
            if (where != null)
            {
                condition &= where;
            }

            var query = this._collection.Find(condition);
            if (orderby != null)
            {
                var sort = Builders<T>.Sort.Sort_(orderby, Desc);
                query = query.Sort(sort);
            }

            if (start != null)
            {
                query = query.Skip(start.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }
            return query;
        }

        public virtual int Update(T model)
        {
            model.Should().NotBeNull();

            var set = this._collection;
            return (int)set.ReplaceOne(x => x.Id == model.Id, model).ModifiedCount;
        }

        public virtual async Task<int> UpdateAsync(T model)
        {
            model.Should().NotBeNull();

            var set = this._collection;
            return (int)(await set.ReplaceOneAsync(x => x.Id == model.Id, model)).ModifiedCount;
        }

        public virtual T QueryOneByKey(string key)
        {
            key.Should().NotBeNull();

            return this._collection.Find(x => x.Id == key).FirstOrDefault();
        }

        public virtual async Task<T> QueryOneByKeyAsync(string key)
        {
            key.Should().NotBeNull();

            return await this._collection.Find(x => x.Id == key).FirstOrDefaultAsync();
        }

        public virtual void Dispose() { }

        public virtual T[] QueryMany<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var query = this.__query_many__(where, order_by, desc, skip, count);
            var res = query.ToList().ToArray();
            return res;
        }

        public virtual async Task<T[]> QueryManyAsync<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var query = this.__query_many__(where, order_by, desc, skip, count);
            var res = await query.ToListAsync();
            return res.ToArray();
        }
    }
}
