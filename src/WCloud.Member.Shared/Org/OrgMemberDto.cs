using System;
using System.Collections.Generic;
using System.Text;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Shared.Org
{
    public class OrgMemberDto : DtoBase
    {
        public virtual string OrgUID { get; set; }

        public virtual string UserUID { get; set; }

        /// <summary>
        /// 权限/角色
        /// </summary>
        public virtual int Flag { get; set; }

        public virtual int IsOwner { get; set; }

        /// <summary>
        /// 会员同意
        /// </summary>
        public virtual int MemberApproved { get; set; } = 1;
    }
}
