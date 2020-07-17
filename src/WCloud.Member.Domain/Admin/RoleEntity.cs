using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Admin
{
    /// <summary>
    /// 平台角色
    /// </summary>
    [Table("tb_role")]
    public class RoleEntity : TreeEntityBase, IMemberShipDBTable
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [Column("role_name")]
        [StringLength(200)]
        [Required]
        public virtual string NodeName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public virtual string RoleDescription { get; set; }

        public virtual string PermissionJson { get; set; } = "[]";
    }
}
