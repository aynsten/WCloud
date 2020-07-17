using Lib.extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Test
{
    [TestClass]
    public class dynamic_call_test
    {
        class tagAttribute : Attribute { }

        public class user { }

        [tag]
        public void xx(string a) { }

        [tag]
        public async Task dd(user u) => await Task.CompletedTask;

        [tag]
        public async Task<int> yy(user u) => await Task.FromResult(4);

        [TestMethod]
        public async Task run()
        {
            var ms = typeof(dynamic_call_test).GetMethods().ToList();
            ms = ms.Where(x => x.HasCustomAttributes_<tagAttribute>()).ToList();

            foreach (var m in ms)
            {
                var res = m.Invoke(this, new object[] { null });
                if (res?.GetType().IsAssignableTo_<Task>() ?? false)
                {
                    await (Task)res;
                }
            }
            //ActivatorUtilities.GetServiceOrCreateInstance(null, this.GetType());
        }
    }
}
