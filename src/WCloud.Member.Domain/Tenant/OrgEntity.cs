using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Domain.Tenant
{
    /// <summary>
    /// 组织/租户
    /// </summary>
    [Table("tb_org")]
    public class OrgEntity : EntityBase, IMemberShipDBTable, ILogicalDeletion, IUpdateTime
    {
        [Required]
        public virtual string OrgName { get; set; }

        public virtual string OrgDescription { get; set; }

        public virtual string OrgImage { get; set; }

        public virtual string OrgWebSite { get; set; }

        public virtual string Phone { get; set; }

        /// <summary>
        /// 租户过期时间
        /// </summary>
        public virtual DateTime? ExpiredTimeUtc { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public virtual string UserUID { get; set; }

        public virtual int MemeberCount { get; set; }

        public virtual int IsDeleted { get; set; }

        [NotMapped]
        public virtual List<UserEntity> Owners { get; set; }
        public virtual DateTime? UpdateTimeUtc { get; set; }
    }
}
