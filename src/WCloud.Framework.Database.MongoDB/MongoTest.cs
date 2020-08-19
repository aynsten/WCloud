#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
    class Mongoxx<T> where T : EntityBase
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<T> _set;

        public IQueryable<T> Queryable => this._set.AsQueryable();

        class counter_entity : EntityBase
        {
            public string Category { get; set; }
            public int MaxId { get; set; }
        }

        /// <summary>
        /// 自增id
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        async Task<int> counter(string category)
        {
            IMongoCollection<counter_entity> collection = null;

            var builder = new UpdateDefinitionBuilder<counter_entity>();
            var update = builder.Inc(x => x.MaxId, 1);
            var entity = await collection.FindOneAndUpdateAsync(x => x.Category == category, update);
            if (entity == null)
            {
                entity = new counter_entity() { Category = category, MaxId = 1 };
                await collection.InsertOneAsync(entity);
            }
            return entity.MaxId;
        }

        void test()
        {
            var set = this._set;

            //map reduce
            set.MapReduce<_>(
                new BsonJavaScript("function(){emit(this.user_id,this.age);}"),
                new BsonJavaScript("function(user_id,age){return Array.avg(age);}"),
                new MapReduceOptions<T, _>() { });

            //geo index
            var index = Builders<T>.IndexKeys.Geo2D(x => x.Id).Geo2DSphere(x => x.Id);
            set.Indexes.CreateOne(new CreateIndexModel<T>(index, new CreateIndexOptions()
            {
                Name = "p"
            }));
            set.Indexes.DropOne("p");
            set.Indexes.DropAll();
            /*
             { "v" : 1, "key" : { "_id" : 1 }, "name" : "_id_", "ns" : "mydb.test" }
             { "v" : 1, "key" : { "i" : 1 }, "name" : "i_1", "ns" : "mydb.test" }
             */
            set.Indexes.List().ToList().Any(x => x["name"] == "123");


            //agg
            var filter = Builders<T>.Filter.Where(x => x.Id == null);
            var group = Builders<T>.Projection.Exclude(x => x.Id).Include(x => x.Id);
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

    class xx : EntityBase { }

    [ConnectionStringName("xx")]
    class MongoDbContextTest : AbpMongoDbContext
    {
        public IMongoCollection<xx> xx => this.Collection<xx>();

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
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

    class MongoRepo<T> : MongoDbRepository<MongoDbContextTest, T> where T : EntityBase
    {
        public MongoRepo(IMongoDbContextProvider<MongoDbContextTest> dbContextProvider) : base(dbContextProvider)
        {
            //
        }
    }
}

#endif