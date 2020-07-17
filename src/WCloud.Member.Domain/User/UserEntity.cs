using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Member.Domain.Login;

namespace WCloud.Member.Domain.User
{
    /// <summary>
    ///用户的账户模型
    /// </summary>
    [Table("tb_user")]
    public class UserEntity : BaseEntity, IMemberShipDBTable,
        ILogicalDeletion, IUpdateTime, ILoginEntity
    {
        /// <summary>
        /// 用于登陆，唯一标志
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// 昵称，用于展示
        /// </summary>
        public virtual string NickName { get; set; }

        /// <summary>
        /// md5加密的密码
        /// </summary>
        public virtual string PassWord { get; set; }

        public virtual string CurrentLoginPhone { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [StringLength(1000)]
        public virtual string UserImg { get; set; }

        public virtual string IdCard { get; set; }

        public virtual string RealName { get; set; }

        public virtual int IdCardConfirmed { get; set; }

        public virtual DateTime? LastPasswordUpdateTimeUtc { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public virtual int UserSex { get; set; } = (int)SexEnum.Unknow;

        public virtual int IsDeleted { get; set; }

        /*
        [NotMapped]
        public virtual string SexName
        {
            get
            {
                try
                {
                    return ((SexEnum)this.UserSex).ToString();
                }
                catch
                {
                    return this.UserSex.ToString();
                }
            }
        }*/

        /// <summary>
        /// 是否是租户拥有人
        /// </summary>
        [NotMapped]
        public virtual bool IsOrgOwner { get; set; }

        [NotMapped]
        public virtual string[] OrgRoleUIDs { get; set; }

        [NotMapped]
        public virtual UserPhoneEntity UserPhone { get; set; }

        public virtual DateTime? UpdateTimeUtc { get; set; }
    }
}