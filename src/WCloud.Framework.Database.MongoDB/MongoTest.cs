using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.MongoDB
{
    public class Mongoxx<T> where T:MongoEntityBase
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<T> _set;

        public IQueryable<T> Queryable => this._set.AsQueryable();

        void test()
        {
            var set = this._set;

            //map reduce
            set.MapReduce<_>(
                new BsonJavaScript("function(){emit(this.user_id,this.age);}"),
                new BsonJavaScript("function(user_id,age){return Array.avg(age);}"),
                new MapReduceOptions<T, _>() { });

            //geo index
            var index = Builders<T>.IndexKeys.Geo2D(x => x._id).Geo2DSphere(x => x._id);
            set.Indexes.CreateOne(new CreateIndexModel<T>(index, new CreateIndexOptions()
            {
                Name = "p"
            }));
            set.Indexes.DropOne("p");
            set.Indexes.DropAll();

            //agg
            var filter = Builders<T>.Filter.Where(x => x._id == null);
            var group = Builders<T>.Projection.Exclude(x => x._id).Include(x => x._id);
            var agg = set.Aggregate().Match(filter).Group(group).SortByCount(x => x.AsObjectId).ToList();
        }

        void geo_example(Expression<Func<T, object>> field)
        {
            var condition = Builders<T>.Filter.Empty;
            //附近
            condition &= Builders<T>.Filter.Near(field, 32, 43, maxDistance: 323, minDistance: 4434);
            //交集
            condition &= Builders<T>.Filter.GeoIntersects(field,
                new GeoJsonMultiPolygon<GeoJson2DCoordinates>(new GeoJsonMultiPolygonCoordinates<GeoJson2DCoordinates>(new List<GeoJsonPolygonCoordinates<GeoJson2DCoordinates>>() { })));
            //范围内
            condition &= Builders<T>.Filter.GeoWithin(field, new GeoJsonPolygon<GeoJson2DCoordinates>(null));
        }
    }
    public class xx : BaseEntity { }

    [ConnectionStringName("xx")]
    public class MongoDbContextTest : AbpMongoDbContext
    {
        public IMongoCollection<xx> xx => this.Collection<xx>();

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
        }

        void test()
        {
            var update = new UpdateDefinitionBuilder<xx>();
            update.Inc(x => x.Id, 1);


            this.xx.UpdateOne(x => true, null);
        }
    }

    [DependsOn(typeof(AbpMongoDbModule))]
    class module : AbpModule
    {
        void __add_db_context__(IServiceCollection collection)
        {
            collection.AddMongoDbContext<MongoDbContextTest>(builder =>
            {
                builder.AddDefaultRepositories();
            });
        }
    }

    class MongoRepo<T> : MongoDbRepository<MongoDbContextTest, T> where T : BaseEntity
    {
        public MongoRepo(IMongoDbContextProvider<MongoDbContextTest> dbContextProvider) : base(dbContextProvider)
        {
            //
        }
    }
}
