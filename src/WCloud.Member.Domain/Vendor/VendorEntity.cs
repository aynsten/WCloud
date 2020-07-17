using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Vendor
{
    [Table("tb_vendor")]
    public class VendorEntity : BaseEntity, IMemberShipDBTable
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ManagerRealName { get; set; }

        public string ShopName { get; set; }

        public string Address { get; set; }

        public int IsOpen { get; set; }
    }
}
