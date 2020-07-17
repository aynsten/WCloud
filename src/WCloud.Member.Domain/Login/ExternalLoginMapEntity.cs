using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Login
{
    /// <summary>
    /// 外部账号关联登陆
    /// </summary>
    [Table("tb_login_external_account")]
    public class ExternalLoginMapEntity : BaseEntity, IMemberShipDBTable
    {
        [Required]
        public virtual string ProviderKey { get; set; }

        public virtual string ProviderName { get; set; }

        [Required]
        public virtual string OpenID { get; set; }

        [Required]
        public virtual string UserID { get; set; }

        public virtual string AccessToken { get; set; }

        public virtual string RefreshToken { get; set; }

        public virtual DateTime? AccessTokenExpireAtUtc { get; set; }

        public virtual DateTime? RefreshTokenExpireAtUtc { get; set; }
    }
}
