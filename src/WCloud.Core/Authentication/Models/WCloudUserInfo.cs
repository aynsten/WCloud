using System;
using System.Collections.Generic;
using Lib.helper;
using WCloud.Core.Authentication.Roles;

namespace WCloud.Core.Authentication.Model
{
    /// <summary>
    /// 当前登陆用户信息
    /// </summary>
    public class WCloudUserInfo : ILoginModel
    {
        /// <summary>
        /// 登陆人id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 用户名，应该是手机号
        /// </summary>
        public string UserName { get; set; }

        public string NickName { get; set; }

        public string UserImg { get; set; }

        /// <summary>
        /// 当前登陆用户的租户
        /// </summary>
        public OrgInfo Org { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public Dictionary<string, string> ExtraData { get; set; }


        /// <summary>
        /// 在租户中具有某个角色
        /// </summary>
        public bool HasRoleInOrg(MemberRoleEnum role)
        {
            return this.HasRoleInOrg(flag: (int)role);
        }

        /// <summary>
        /// 在租户中具有某个角色
        /// </summary>
        public bool HasRoleInOrg(int flag)
        {
            var role = this.Org?.UserFlag;
            if (role == null)
                throw new NotSupportedException("当前租户上下文未加载成员角色");

            var res = PermissionHelper.HasPermission(user_permission: role.Value, permission_to_valid: flag);
            return res;
        }
    }
}
