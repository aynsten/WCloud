using System.Collections.Generic;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Domain
{
    public static class MemberShipExtension
    {
        /// <summary>
        /// 移除敏感信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static UserEntity RemoveSensitiveData(this UserEntity model)
        {
            model.PassWord = null;
            model.LastPasswordUpdateTimeUtc = null;
            return model;
        }

        /// <summary>
        /// 移除敏感信息 比如密码什么的
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<UserEntity> RemoveSensitiveData(this IEnumerable<UserEntity> list)
        {
            foreach (var m in list)
            {
                m.RemoveSensitiveData();
            }
            return list;
        }
    }
}
