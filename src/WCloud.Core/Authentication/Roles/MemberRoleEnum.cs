using System;
using System.ComponentModel;

namespace WCloud.Core.Authentication.Roles
{
    [Flags]
    public enum MemberRoleEnum : int
    {
        管理员 = 1 << 0,

        普通成员 = 1 << 3,

        [Description("观察者-只能看")]
        观察者 = 1 << 4
    }
}
