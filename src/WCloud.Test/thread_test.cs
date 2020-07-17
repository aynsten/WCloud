using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Threading;

namespace WCloud.Test
{
    [TestClass]
    public class thread_test
    {
        void __wait__() => Thread.Sleep(TimeSpan.FromSeconds(1));

        [TestMethod]
        public void Parallel_test1()
        {
            var data = Lib.helper.Com.Range(5);

            var res = Parallel.ForEach(data, x =>
            {
                this.__wait__();
            });
        }

        [TestMethod]
        public void Parallel_test2()
        {
            var data = Lib.helper.Com.Range(5);

            data.AsParallel().ForAll(x =>
            {
                this.__wait__();
            });
        }

        [TestMethod]
        public void semaphore_test()
        {
            Action release_without_wait = () =>
            {
                using (var _sem = new SemaphoreSlim(1, 1))
                    _sem.Release();
            };

            release_without_wait.Should().Throw<SemaphoreFullException>();

            Action release_with_wait = () =>
            {
                using (var _sem = new SemaphoreSlim(1, 1))
                {
                    _sem.Wait();
                    _sem.Release();
                }
            };

            release_with_wait.Should().NotThrow();
        }
    }
}
