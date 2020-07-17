using AspectCore.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace WCloud.Framework.Aop
{
    public static class AopExtension
    {
        public static InterceptorCollection InterceptorServiceMethods<InterceptorType>(this InterceptorCollection interceptor,
            Assembly[] ass, Func<MethodInfo, bool> excludeFilter = null)
            where InterceptorType : class, AspectCore.DynamicProxy.IInterceptor
        {
            excludeFilter ??= (x => false);

            bool __filter__(MethodInfo x)
            {
                if (!ass.Contains(x.DeclaringType.Assembly))
                {
                    return false;
                }
                if (excludeFilter.Invoke(x))
                {
                    return false;
                }
                return true;
            }

            //这里的x是接口定义不是实现
            interceptor.AddTyped<InterceptorType>(x => __filter__(x));

            return interceptor;
        }
    }
}
