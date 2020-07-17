using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace WCloud.Test
{
    [TestClass]
    public class linq_test
    {
        public static void AddOrUpdate<T>(Expression<Func<T, Task>> methodCall, Func<string> cronExpression, TimeZoneInfo timeZone = null, string queue = "default")
        {
            //
        }

        [TestMethod]
        public void hgha()
        {
            var m = typeof(linq_test).GetMethods().FirstOrDefault(x => x.Name == "AddOrUpdate");
            var p = m.GetParameters().FirstOrDefault().ParameterType;
        }

        [TestMethod]
        public void method_generator()
        {
            var t = typeof(string);
            var m = t.GetMethods().FirstOrDefault(x => x.Name == "ToUpper");
            "".ToUpper();
            var p = Expression.Parameter(t, "x");
            var call = Expression.Call(p, m);
            Expression<Action<string>> exp = Expression.Lambda<Action<string>>(call, p);
            var d = Expression.Lambda(typeof(Action<string>), call, p);

            var sd = d as Expression<Action<string>>;
        }

        class user { public string name { get; set; } }
        [TestMethod]
        public void member_access_expression_test()
        {
            Expression<Func<user, string>> exp = x => x.name;
            var p = ((exp.Body as MemberExpression).Member as PropertyInfo);

            var u = new user();

            p.SetValue(u, "wj");

            u.name.Should().Be("wj");
        }

        [TestMethod]
        public void join_test()
        {
            var user_query = new[]
            {
                new
                {
                    id=1,
                    name="wj"
                }
            };
            var map_query = new[]
            {
                new
                {
                    user_id=1,
                    group_id=9
                },
                new
                {
                    user_id=1,
                    group_id=9
                },
                new
                {
                    user_id=2,
                    group_id=9
                }
            };

            var query = from map in map_query
                        join user in user_query on map.user_id equals user.id// into joined
                        select new
                        {
                            map,
                        };

            var data = query.ToArray();

            data = map_query.Join(user_query, x => x.user_id, x => x.id, (map, user) => new { map }).ToArray();
        }

    }
}
