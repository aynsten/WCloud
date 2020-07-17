using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Admin
{
    [Table("tb_dept")]
    public class DepartmentEntity : TreeEntityBase, IMemberShipDBTable
    {
        [Required, Column("dept_name")]
        public virtual string NodeName { get; set; }

        public virtual string Description { get; set; }
    }
}
