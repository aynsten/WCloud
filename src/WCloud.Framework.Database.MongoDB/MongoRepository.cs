using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lib.extension;
using Lib.helper;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WCloud.Framework.Database.MongoDB
{
    public class MongoRepository<T> : IMongoRepository<T> where T : MongoEntityBase
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<T> _set;

        public IQueryable<T> Queryable => this._set.AsQueryable();

        public MongoRepository(MongoConnectionWrapper wrapper)
        {
            this._client = wrapper.Client;

            this._db = this._client.GetDatabase(wrapper.DatabaseName);
            this._set = this._db.GetCollection<T>(typeof(T).GetTableName());
        }

        public List<T> QueryNearBy(Expression<Func<T, bool>> where, int page, int pagesize,
            Expression<Func<T, object>> field, GeoInfo point,
            double? max_distance = null, double? min_distance = null)
        {
            var condition = Builders<T>.Filter.Empty;
            condition &= Builders<T>.Filter.Near(field, point.Lat, point.Lon, max_distance, min_distance);
            if (where != null)
            {
                condition &= Builders<T>.Filter.Where(where);
            }
            var range = PagerHelper.GetQueryRange(page, pagesize);
            return this._set.Find(condition).QueryPage(page, pagesize).ToList();
        }

        public int Add(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            this._set.InsertOne(model);
            return 1;
        }

        public async Task<int> AddAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            await this._set.InsertOneAsync(model);
            return 1;
        }

        public int Delete(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var filter = Builders<T>.Filter.Where(x => x._id == model._id);
            return (int)this._set.DeleteMany(filter).DeletedCount;
        }

        public async Task<int> DeleteAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var filter = Builders<T>.Filter.Where(x => x._id == model._id);
            return (int)(await this._set.DeleteManyAsync(filter)).DeletedCount;
        }

        public int DeleteWhere(Expression<Func<T, bool>> where)
        {
            where = where ?? throw new ArgumentNullException(nameof(where));

            return (int)this._set.DeleteMany(where).DeletedCount;
        }

        public async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            where = where ?? throw new ArgumentNullException(nameof(where));

            return (int)(await this._set.DeleteManyAsync(where)).DeletedCount;
        }

        public bool Exist(Expression<Func<T, bool>> where)
        {
            return this._set.Find(where).Take(1).FirstOrDefault() != null;
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            return (await this._set.FindAsync(where)).FirstOrDefault() != null;
        }

        public int GetCount(Expression<Func<T, bool>> where)
        {
            return (int)this._set.CountDocuments(where);
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> where)
        {
            return (int)(await this._set.CountDocumentsAsync(where));
        }

        public T GetFirst(Expression<Func<T, bool>> where)
        {
            return this.GetList(where, 1).FirstOrDefault();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> where)
        {
            return (await this.GetListAsync(where, 1)).FirstOrDefault();
        }

        public List<T> GetList(Expression<Func<T, bool>> where, int? count = null)
        {
            return this.QueryList<object>(where: where, start: 0, count: count);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where, int? count = null)
        {
            return await this.QueryListAsync<object>(where: where, start: 0, count: count);
        }

        public List<T> QueryList<OrderByColumnType>(Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null, bool Desc = true,
            int? start = null, int? count = null)
        {
            var condition = Builders<T>.Filter.Empty;
            if (where != null)
            {
                condition &= where;
            }

            var query = this._set.Find(condition);
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
            return query.ToList();
        }

        public async Task<List<T>> QueryListAsync<OrderByColumnType>(Expression<Func<T, bool>> where, Expression<Func<T, OrderByColumnType>> orderby = null, bool Desc = true, int? start = null, int? count = null)
        {
            var condition = Builders<T>.Filter.Empty;
            if (where != null)
            {
                condition &= where;
            }

            var query = this._set.Find(condition);
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
            return await query.ToListAsync();
        }

        public int Update(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var set = this._set;
            return (int)set.ReplaceOne(x => x._id == model._id, model).ModifiedCount;
        }

        public async Task<int> UpdateAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var set = this._set;
            return (int)(await set.ReplaceOneAsync(x => x._id == model._id, model)).ModifiedCount;
        }

        public T GetByKeys(string key)
        {
            var id = ObjectId.Parse(key);
            return this._set.Find(x => x._id == id).FirstOrDefault();
        }

        public async Task<T> GetByKeysAsync(string key)
        {
            var id = ObjectId.Parse(key);
            return await this._set.Find(x => x._id == id).FirstOrDefaultAsync();
        }

        public virtual void Dispose() { }
    }
}
