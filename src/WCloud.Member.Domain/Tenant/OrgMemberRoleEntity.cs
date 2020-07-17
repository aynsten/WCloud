using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Tenant
{
    /// <summary>
    /// 租户成员的角色关联
    /// </summary>
    [Table("tb_org_member_role")]
    public class OrgMemberRoleEntity : BaseEntity, IMemberShipDBTable
    {
        [Required]
        public virtual string OrgUID { get; set; }

        [Required]
        public virtual string RoleUID { get; set; }

        [Required]
        public virtual string UserUID { get; set; }
    }
}
