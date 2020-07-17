using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Login
{
    [Table("tb_user_phone")]
    public class UserPhoneEntity : BaseEntity, IMemberShipDBTable
    {
        public string UserUID { get; set; }

        public string Phone { get; set; }
    }
}
