using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Tenant
{
    /// <summary>
    /// 租户内定义的角色
    /// </summary>
    [Table("tb_org_role")]
    public class OrgRoleEntity : EntityBase, IMemberShipDBTable
    {
        [Required, StringLength(20)]
        public virtual string RoleName { get; set; }

        [Required]
        public virtual string OrgUID { get; set; }

        public virtual string PermissionJson { get; set; } = "[]";

    }
}
