using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 依赖注入对象管理容器
    /// </summary>
    public class IocContext : IDisposable
    {
        public static readonly IocContext Instance = new IocContext();

        private IServiceProvider _root { get; set; }

        public IServiceProvider Container => this._root ?? throw new Exception("没有设置依赖注入容器");

        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool Inited => this._root != null;

        public IocContext SetRootContainer(IServiceProvider root)
        {
            this._root = root;
            return this;
        }

        /// <summary>
        /// 不建议手动调用
        /// </summary>
        /// <returns></returns>
        public IServiceScope Scope() => this.Container.CreateScope();

        /// <summary>
        /// 销毁所有组件
        /// </summary>
        public void Dispose()
        {
            if (!this.Inited)
                return;

            //dispose single instances
            using (var s = this.Scope())
            {
                //释放
                var coms = s.ServiceProvider.ResolveAll_<ISingleInstanceService>();
                foreach (var com in coms.OrderBy(x => x.DisposeOrder))
                {
                    try
                    {
                        //dispose by using syntax
                        using (com) { }
                    }
                    catch (Exception e)
                    {
                        e.DebugInfo();
                    }
                }
            }

            /*
            if (this.Container is ServiceProvider)
                ((ServiceProvider)this.Container).Dispose();
                */

            //回收内存
            GC.Collect();
        }

        ~IocContext()
        {
            try
            {
                using (this) { }
            }
            catch (Exception e)
            {
                e.GetInnerExceptionAsJson().DebugInfo();
            }
        }
    }
}
