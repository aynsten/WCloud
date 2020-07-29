using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.MongoDB
{
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
