using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Tenant
{
    /// <summary>
    /// 租户和成员的关联
    /// </summary>
    [Table("tb_org_member")]
    public class OrgMemberEntity : EntityBase, IMemberShipDBTable
    {
        [Required]
        public virtual string OrgUID { get; set; }

        [Required]
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
