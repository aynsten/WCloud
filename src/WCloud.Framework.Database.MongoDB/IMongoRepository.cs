using Lib.helper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions;

namespace WCloud.Framework.Database.MongoDB
{
    public interface IMongoRepository<T> : ILinqRepository<T>, IRepository<T> where T : MongoEntityBase
    {
        IMongoCollection<T> Collection { get; }

        T GetByKeys(string key);

        Task<T> GetByKeysAsync(string key);

        List<T> QueryNearBy(Expression<Func<T, bool>> where, int page, int pagesize,
            Expression<Func<T, object>> field, GeoInfo point,
            double? max_distance = null, double? min_distance = null);
    }
}
