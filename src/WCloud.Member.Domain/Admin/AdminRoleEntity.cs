using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Admin
{
    /// <summary>
    /// 用户角色关联
    /// </summary>
    [Table("tb_admin_role")]
    public class AdminRoleEntity : EntityBase, IMemberShipDBTable
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [StringLength(100), Required]
        public virtual string AdminUID { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [StringLength(100), Required]
        public virtual string RoleUID { get; set; }
    }
}
