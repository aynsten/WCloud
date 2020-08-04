using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Employee
{
    [Table("tb_employee")]
    public class EmployeeEntity : EntityBase, IMemberShipDBTable
    {
        public string UserName { get; set; }

        public string RealName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        public string VendorUID { get; set; }
    }
}
