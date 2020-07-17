using Lib.core;
using System;

namespace WCloud.Core
{
    /// <summary>
    /// 传参错误
    /// </summary>
    public class NoParamException : ArgumentNullException
    {
        /// <summary>
        /// 接口返回模板
        /// </summary>
        public object ResponseTemplate { get; set; }

        public NoParamException() : base("参数错误") { }

        public NoParamException(string msg) : base(msg) { }

        public NoParamException(string msg, Exception inner) : base(msg, inner) { }
    }

    /// <summary>
    /// 数据库中不存在相应记录
    /// </summary>
    public class DataNotExistException : Exception
    {
        public DataNotExistException(string msg = null) : base(msg ?? "数据不存在")
        {
            //
        }
    }

    /// <summary>
    /// 没有组织参数
    /// </summary>
    public class NoOrgException : Exception
    {
        public NoOrgException(string msg = null) : base(msg ?? "没有选择组织")
        {
            //
        }
    }

    public class NotInCurrentOrgException : Exception
    {
        public NotInCurrentOrgException(string msg = null) : base(msg ?? "不在当前组织") { }
    }

    /// <summary>
    /// 没有小组信息
    /// </summary>
    public class NoProjectException : Exception
    {
        public NoProjectException(string msg = null) : base(msg ?? "没有选择小组")
        {
            //
        }
    }

    /// <summary>
    /// 在组织内没有权限操作
    /// </summary>
    public class NoPermissionInOrgException : Exception
    {
        public NoPermissionInOrgException(string msg = null) : base(msg ?? "没有许可进行操作")
        {
            //
        }
    }

    /// <summary>
    /// 没有权限
    /// </summary>
    public class NoPermissionException : Exception
    {
        public NoPermissionException(string msg = null) : base(msg ?? "没有权限")
        {
            //
        }
    }

    /// <summary>
    /// 没有scope
    /// </summary>
    public class NoScopeException : Exception
    {
        public NoScopeException(string msg = null) : base(msg ?? "没有scope")
        {
            //
        }
    }

    /// <summary>
    /// 没有登录
    /// </summary>
    public class NoLoginException : Exception
    {
        public NoLoginException(string msg = null) : base(msg ?? "没有登录")
        {
            //
        }
    }

    /// <summary>
    /// 账号没有激活
    /// </summary>
    public class NotActiveException : Exception
    {
        public NotActiveException(string msg = null) : base(msg ?? "没有激活")
        {
            //
        }
    }

    /// <summary>
    /// 账号被禁用
    /// </summary>
    public class AccountBlockedException : Exception
    {
        public AccountBlockedException(string msg = null) : base(msg ?? "账号被禁用")
        {
            //
        }
    }

    /// <summary>
    /// 没有同意服务协议
    /// </summary>
    public class DisAggreeLisenceException : Exception
    {
        public DisAggreeLisenceException(string msg = null) : base(msg ?? "未接受服务协议")
        {
            //
        }
    }

    public class LoginException : BaseException
    {
        public LoginException(string msg, Exception inner) : base(msg, inner)
        { }
    }
}
