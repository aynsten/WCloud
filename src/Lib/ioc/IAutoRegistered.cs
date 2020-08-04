using Lib.core;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// autofac 自动查找注册，
    /// 默认使用瞬时生命周期，可以使用attribute来指定
    /// </summary>
    public interface IAutoRegistered : IFinder { }

    public interface ISingleInstance { }

    public interface IScopedInstance { }

    /// <summary>
    /// 检查重复注册实例
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RepeatCheckAttribute : Attribute { }

    /// <summary>
    /// 单例
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleInstanceAttribute : Attribute { }

    /// <summary>
    /// scope
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedInstanceAttribute : Attribute { }

    /// <summary>
    /// 拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InterceptInstanceAttribute : Attribute { }

    /// <summary>
    /// 不注册IOC
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NotRegIocAttribute : Attribute { }
}
