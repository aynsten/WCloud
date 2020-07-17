using System;

namespace WCloud.Core.Authentication.Model
{
    /// <summary>
    /// 租户独享数据库
    /// </summary>
    public class OrgExclusiveDatabase
    {
        public string ConnectionString { get; set; }

        public static implicit operator bool(OrgExclusiveDatabase database) => database?.ConnectionString?.Length > 0;
    }

    /// <summary>
    /// 租户信息
    /// </summary>
    public class OrgInfo
    {
        public string UID { get; set; }

        public OrgExclusiveDatabase Database { get; set; }

        /// <summary>
        /// 当前用户在租户的角色
        /// </summary>
        public int? UserFlag { get; set; }

        /// <summary>
        /// 我是组织所有人
        /// </summary>
        public bool IsOwner { get; set; }

        public DateTime Expired { get; set; }
    }
}
