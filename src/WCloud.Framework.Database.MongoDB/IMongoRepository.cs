using Lib.helper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WCloud.Framework.Database.Abstractions;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.MongoDB
{
    public interface IMongoRepository<T> : ILinqRepository<T>, IBulkInsertRepository<T>, IQueryByKeyRepository<T> where T : EntityBase
    {
        IMongoCollection<T> Collection { get; }

        List<T> QueryNearBy(
            Expression<Func<T, bool>> where, int page, int pagesize,
            Expression<Func<T, object>> field, GeoInfo point,
            double? max_distance, double? min_distance);
    }
}
