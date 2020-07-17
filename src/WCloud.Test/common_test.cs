using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using WCloud.Member.Application.Entity;
using WCloud.Member.Domain.User;

namespace WCloud.Test
{
    public static class xxEx
    {
        public static void StrEx(this string s) { }
    }

    [TestClass]
    public class common_test
    {
        [TestMethod]
        public void fdsafdsfadsfsagsdf()
        {
            int __discount__(int a, double rate)
            {
                var dd = 1 - rate;

                var res = a * dd;

                return (int)res;
            }

            var d = __discount__(570000, 0.9);
            d.Should().NotBe(57000);
        }


        [TestMethod]
        public void dict_json()
        {
            var dict = new Dictionary<int, int>()
            {
                [1] = 2,
                [2] = 44
            };
            var json = dict.ToJson();

            var res = json.JsonToEntity<Dictionary<int, int>>();
        }

        [TestMethod]
        public void linked_list_test()
        {
            var list = new LinkedList<int>();
        }

        class attr1Attribute : Attribute { }
        class attr2Attribute : Attribute { }
        abstract class a
        {
            [attr1]
            public virtual void test() { }
        }
        class b : a
        {
            [attr2]
            public override void test()
            {
                base.test();
            }
        }

        [TestMethod]
        public void attr_inherit_test()
        {
            var m = typeof(b).GetMethods().FirstOrDefault(x => x.Name == "test");
            m.Should().NotBeNull();
            m.GetCustomAttributes(inherit: true).Length.Should().Be(2);
            m.GetCustomAttributes(inherit: false).Length.Should().Be(1);
        }

        [TestMethod]
        public void json_to_interface_test()
        {
            new Action(() => "[]".JsonToEntity<IEnumerable<string>>()).Should().NotThrow();
            "[]".JsonToEntity<IEnumerable<string>>().GetType().IsGenericType_(typeof(List<>)).Should().BeTrue();
        }

        [TestMethod]
        public void operator_test____()
        {
            var user = new UserEntity();

            object.Equals(user, null).Should().BeFalse();
            object.Equals(null, user).Should().BeFalse();

            object.Equals(user, new UserEntity()).Should().BeFalse();

            user.UID = "1";
            object.Equals(user, new UserEntity() { UID = "1" }).Should().BeTrue();

            (user == new UserEntity() { UID = "1" }).Should().BeTrue();

            (user is null).Should().BeFalse();
        }

        [TestMethod]
        public void operator_test_()
        {
            int? a = null;
            object.ReferenceEquals(a, null).Should().BeTrue();
            a = 0;
            object.ReferenceEquals(a, null).Should().BeFalse();

            object.ReferenceEquals(null, null).Should().BeTrue();

            a = null;
            (a is null).Should().BeTrue();
        }

        [TestMethod]
        public void json_test()
        {
            var obj = JObject.Parse("{\"name\":\"wj\"}");
            var field = obj["name"];

            var value = field.Value<string>();
            value.Should().Be("wj");
        }

        [TestMethod]
        public void compare_test()
        {
            int a = 1, b = 3;

            new Action(() => a.CompareTo(b)).Should().NotThrow();
            new Action(() => a.CompareTo(new { })).Should().Throw<Exception>();
            new Action(() => a.CompareTo(1f)).Should().Throw<Exception>();

            object c = b;
            new Action(() => a.CompareTo(c)).Should().NotThrow();
        }

        [TestMethod]
        public void enum_test()
        {
            var id = (int)CrudEnum.Delete;

            var e = (CrudEnum)id;
            e.ToString().Should().Be("Delete");

            ((CrudEnum)4564).ToString().Should().Be("4564");
        }

        [TestMethod]
        public void null_ex_test()
        {
            Action action = () =>
            {
                string data = null;
                data.StrEx();
            };
            action.Should().NotThrow();
        }

        [TestMethod]
        public void timezone_test()
        {
            var all_timezone = TimeZoneInfo.GetSystemTimeZones().ToArray();

            var timezone = TimeZoneInfo.CreateCustomTimeZone("中国/上海东八区", TimeSpan.FromHours(8), "中国东八区", "中国东八区");

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh");
            var all_cultrues = CultureInfo.GetCultures(CultureTypes.AllCultures);
        }

        [TestMethod]
        public void is_emoji()
        {
            //ValidateHelper._IsEmoji("😑").Should().Be(true);
        }

        interface idb { }

        class db : idb { }

        [TestMethod]
        public void generic_test()
        {
            Type test<T>(Func<T> factory) => typeof(T);

            var _db = new db();
            idb _db_ = _db;

            test(() => _db).IsInterface.Should().Be(false);
            test(() => _db_).IsInterface.Should().Be(true);
        }

        public int should_value { get; set; }

        [TestMethod]
        public void shouldly_test()
        {
            try
            {
                var error_msg = @"

错误信息如下：


Test method WCloud.Test.common_test.shouldly_test threw exception: 
Shouldly.ShouldAssertException: new /*---啦啦啦，抓的stack---*/common_test()/*faf*/.should_value
    should be
9
    but was
0

";
                Console.Out.Write(error_msg);

                new /*---啦啦啦，抓的stack---*/common_test()/*faf*/.should_value.Should().Be(9);
            }
            catch (Exception e)
            {
                e.DebugInfo();
                //throw;
            }
        }

        [TestMethod]
        public void extension_test()
        {
            var str = default(string);
            str.IsNotEmpty().Should().Be(false);

            new[] { "1", null, "2", "3", null }.ConcatString().Should().Be("123");
        }

        [TestMethod]
        public void time_zone_test()
        {
            var now = DateTime.Now;
            now.Kind.Should().Be(DateTimeKind.Local);
            var utc_now = now.ToUniversalTime();
            utc_now.Kind.Should().Be(DateTimeKind.Utc);

            //这个是代码运行机器的时区
            var zone = TimeZoneInfo.Local;
            var converted_local_time = TimeZoneInfo.ConvertTimeFromUtc(utc_now, zone);
            now.Should().Be(converted_local_time);

            var utc_time_str = utc_now.ToString();
            var time_str = "2018-01-25 15:23:44";

            var signTime = DateTime.Parse(time_str);
            signTime.Kind.Should().Be(DateTimeKind.Unspecified);
            var dateTimeUtc = DateTime.SpecifyKind(signTime, DateTimeKind.Utc);
            dateTimeUtc.Kind.Should().Be(DateTimeKind.Utc);

            var timeoffset = DateTimeOffset.Parse(time_str);
            //

            var utc_time_ = DateTime.SpecifyKind(signTime, DateTimeKind.Utc);
        }

        [TestMethod]
        public void nullable_test()
        {
            Nullable<int> a = 1;
            (a is int).Should().Be(true);

            a = null;
            (a is int).Should().Be(false);
        }

        [TestMethod]
        public void any_all_test()
        {
            var data = new string[] { };

            var res = data.Any();
            var res_ = data.Any(x => x != null);

            var res__ = data.All(x => x != null);
        }

        [TestMethod]
        public void join_test()
        {
            var fake_data = Lib.helper.Com.Range((int)'a', (int)'z').Select(x => ((char)x).ToString()).ToArray();
            var ran = new Random((int)DateTime.Now.Ticks);
            var spliter = ran.Choice(fake_data);

            var simple = ran.Sample(fake_data, fake_data.Length);

            var res2 = StringHelper.Join_(spliter, simple);
            var res3 = string.Join(spliter, simple);
            res2.Should().Be(res3);
        }

        [TestMethod]
        public void InnerExceptionTest()
        {
            var user = new User().MockData();
            object d = user;
            d.Should().Be(user);

            var e = new ArgumentNullException("a", new NotSupportedException("b", new TimeoutException("c")));
            var msg = string.Join(",", e.GetInnerExceptionAsList());
            msg.Should().Be("a,b,c");
        }

        class xx
        {
            public int age { get; set; }
        }

        [TestMethod]
        public void swap_test()
        {
            var a = 1;
            var b = 2;
            Lib.helper.Com.Swap(ref a, ref b);
            a.Should().Be(2);
            b.Should().Be(1);

            var model_a = new xx() { age = 1 };
            var model_b = new xx() { age = 2 };
            Lib.helper.Com.Swap(ref model_a, ref model_b);
            model_a.age.Should().Be(2);
            model_b.age.Should().Be(1);
        }

        [TestMethod]
        public void xx_error()
        {
            var ran = new Random((int)DateTime.Now.Ticks);
        }

        /// <summary>
        /// 为啥获取私有方法要指定 instance？
        /// </summary>
        /// <returns>The method.</returns>
        [TestMethod]
        public async Task InvokeMethod___()
        {
            var obj = new common_test();
            var methods = obj.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            bool IsAsync(Type t)
            {
                return t.IsAssignableTo_<Task>();
            }

            async Task Invoke(string name)
            {
                var m = methods.FirstOrDefault(x => x.Name == name);
                var res = m.Invoke(obj, new object[] { });
                if (IsAsync(m.ReturnType))
                {
                    var task = (Task)res;
                    await task;
                }
            }

            await Invoke(nameof(xx_1));
            await Invoke(nameof(xx_2));
            await Invoke(nameof(xx_3));
        }

        void xx_1() => Console.WriteLine(string.Empty);

        async Task<string> xx_2() => await Task.FromResult(string.Empty);

        async Task xx_3() => await Task.FromResult(string.Empty);

        [TestMethod]
        public void expression_test()
        {
            var mike = new User();

            Expression<Func<User, bool>> exp =
                x => x.Name == "����" && (x.Age > 18 || x.Age != mike.Age || x.Age == null) || x.Address.Contains("��ͨ");
            var body = exp.Body;
            //
        }

        [TestMethod]
        public void di_test()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<User>(new User().MockData());
            collection.AddSingleton<User>(new User().MockData());
            collection.AddSingleton<User>(new User().MockData());

            collection.AddSingleton<Group, Group>();


            var provider = collection.BuildServiceProvider();

            var obj = provider.GetService<Group>();
        }

        public class User
        {
            public User MockData()
            {
                var f = new Bogus.Faker();
                this.IID = f.UniqueIndex;
                this.UID = f.Person.Random.Uuid().ToString();
                this.Name = f.Name.FullName();
                this.Age = f.Random.Int(10, 50);
                this.Address = f.Address.FullAddress();
                this.CreateTime = f.Date.Past();

                return this;
            }

            public virtual long IID { get; set; }

            public virtual string UID { get; set; }

            public virtual string Name { get; set; }

            public virtual int? Age { get; set; }

            public virtual string Address { get; set; }

            public virtual DateTime CreateTime { get; set; }
        }

        public class Group
        {
            public Group(IServiceProvider provider)
            {
                var data = provider.GetServices<User>().ToList();
            }
        }

        [TestMethod]
        public void data_mock_test()
        {
            var user = new User().MockData();
            //print user
        }
    }
}
