using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Login
{
    /// <summary>
    /// 一次性登录用的code/验证码
    /// </summary>
    public class ValidationCodeEntity : EntityBase, IMemberShipDBTable
    {
        public virtual string UserUID { get; set; }

        public virtual string Code { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Email { get; set; }

        public virtual string Phone { get; set; }

        public virtual string CodeType { get; set; }
    }
}
