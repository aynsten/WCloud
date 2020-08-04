using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Login
{
    /// <summary>
    /// token/不是oauth token
    /// </summary>
    [Table("tb_login_token")]
    public class AuthTokenEntity : BaseEntity, IMemberShipDBTable, IUpdateTime
    {
        [Required]
        public virtual string RefreshToken { get; set; }

        [Required]
        public virtual string UserUID { get; set; }

        public virtual string ExtData { get; set; }

        public virtual DateTime ExpiryTimeUtc { get; set; }

        public virtual DateTime? RefreshTimeUtc { get; set; }

        public virtual DateTime? UpdateTimeUtc { get; set; }
    }
}
