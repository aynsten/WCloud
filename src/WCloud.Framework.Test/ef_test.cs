using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;

namespace WCloud.Test
{
    [TestClass]
    public class ef_test
    {
        [Table(nameof(UserTest))]
        public class UserTest : EntityBase
        {
            public string Name { get; set; }
        }

        public class db_context : EFContextBase
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseMySql("Server=mysql;Database=wp-db;Uid=test;Pwd=123;port=3306;CharSet=utf8");
            }

            public DbSet<UserTest> UserTest { get; set; }
        }

        public class Repo<T> : EFRepositoryFromNew<T, db_context>
            where T : class, Lib.data.IDBTable
        { }

        [TestMethod]
        public void repo_test()
        {
            using (var db = new db_context())
            {
                db.Database.EnsureCreated();

                Assert.IsTrue(db.UserTest == db.Set<UserTest>(), "error1");

                var model = new UserTest()
                {
                    Name = Lib.helper.Com.GetUUID()
                };

                Assert.IsTrue(db.Entry(model).State == EntityState.Detached);
                db.Set<UserTest>().Attach(model);
                Assert.IsTrue(db.Entry(model).State == EntityState.Added);
                model.Name = "fa";
                Assert.IsTrue(db.Entry(model).State == EntityState.Added);
                db.Set<UserTest>().Remove(model);
                Assert.IsTrue(db.Entry(model).State == EntityState.Detached);

                db.UserTest.Add(model);
                Assert.IsTrue(db.Entry(model).State == EntityState.Added);
                db.SaveChanges();
                Assert.IsTrue(db.Entry(model).State == EntityState.Unchanged);

                model.Name = "xx";
                Assert.IsTrue(db.Entry(model).State == EntityState.Modified);
                db.SaveChanges();
                Assert.IsTrue(db.Entry(model).State == EntityState.Unchanged);

                db.UserTest.Remove(model);
                Assert.IsTrue(db.Entry(model).State == EntityState.Deleted);
                db.SaveChanges();
                Assert.IsTrue(db.Entry(model).State == EntityState.Detached);

                var new_user = db.Set<UserTest>().AsNoTrackingQueryable().FirstOrDefault(x => x.Name == "xx");
                Assert.IsTrue(db.Entry(new_user).State == EntityState.Detached);

                new_user = db.Set<UserTest>().AsTracking().FirstOrDefault(x => x.Name == "xx");
                Assert.IsTrue(db.Entry(new_user).State == EntityState.Unchanged);
            }

            var collection = new ServiceCollection();
            collection.AddTransient(typeof(IEFRepository<>), typeof(Repo<>));

            var provider = collection.BuildServiceProvider().SetAsRootServiceProvider();

            using (var user_repo = provider.Resolve_<IEFRepository<UserTest>>())
            {
                user_repo.Insert(new UserTest() { Name = "xx" });
                user_repo.Insert(new UserTest() { Name = "xx" });
                user_repo.Insert(new UserTest() { Name = "xx" });
                user_repo.Insert(new UserTest() { Name = "xx" });

                var m = user_repo.QueryOne(x => x.Name == "xx");
                m.Name += "-dd";
                var effected = user_repo.Update(m);
                Assert.IsTrue(effected > 0);
            }
        }
    }
}
