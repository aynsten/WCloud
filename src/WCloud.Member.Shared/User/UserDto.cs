using System;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Shared.User
{
    public class UserDto : DtoBase, ILoginEntity
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

        public virtual DateTime? UpdateTimeUtc { get; set; }
    }
}
