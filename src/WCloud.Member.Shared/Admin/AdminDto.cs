using System;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Shared.Admin
{
    public class AdminDto : DtoBase, ILoginEntity
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

        /// <summary>
        /// 头像链接
        /// </summary>
        public virtual string UserImg { get; set; }

        public virtual string ContactPhone { get; set; }

        public virtual string ContactEmail { get; set; }

        public virtual DateTime? LastPasswordUpdateTimeUtc { get; set; }

        public virtual int Sex { get; set; }

        public virtual int IsDeleted { get; set; }

        public virtual DateTime? UpdateTimeUtc { get; set; }
    }
}
