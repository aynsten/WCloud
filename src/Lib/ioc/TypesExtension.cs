using Lib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lib.ioc
{
    public static class TypesExtension
    {
        /// <summary>
        /// 获取可以注册的类
        /// </summary>
        public static IEnumerable<Type> FindAllRegistableClass(this Assembly a)
        {
            var res = a.GetAllNormalClass().Where(x => x.CanRegIoc()).ToArray();
            return res;
        }

        /// <summary>
        /// 服务是否允许注册多个实现
        /// </summary>
        /// <returns><c>true</c>, if repeat was checked, <c>false</c> otherwise.</returns>
        /// <param name="t">T.</param>
        public static bool CheckRepeatRequired(this Type t)
        {
            var res = t.GetCustomAttributes_<RepeatCheckAttribute>().Any();
            return res;
        }

        /// <summary>
        /// 是否注册为单例
        /// </summary>
        public static bool IsSingleInstance(this Type t)
        {
            var res =
                t.GetCustomAttributes_<SingleInstanceAttribute>().Any() ||
                t.IsAssignableTo_<ISingleInstance>();
            return res;
        }

        /// <summary>
        /// scope
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsScopedInstance(this Type t)
        {
            var res =
                t.GetCustomAttributes_<ScopedInstanceAttribute>().Any() ||
                t.IsAssignableTo_<IScopedInstance>();
            return res;
        }

        /// <summary>
        /// 是否拦截实例
        /// </summary>
        public static bool IsInterceptClass(this Type t)
        {
            var res = t.GetCustomAttributes_<InterceptInstanceAttribute>().Any();
            return res;
        }

        /// <summary>
        /// 配置可以注册IOC，
        /// 要public，不是抽象类，不是接口，没有不注册属性，不是数据表model
        /// </summary>
        public static bool CanRegIoc(this Type t)
        {
            var res =
                t.IsPublic &&
                t.IsNormalClass() &&
                t.GetCustomAttributes<NotRegIocAttribute>().IsEmpty_() &&
                !t.IsDatabaseTable();
            return res;
        }
    }
}
