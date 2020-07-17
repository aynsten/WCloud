using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Application.Login
{
    public interface IUserLoginService : ILoginService<UserEntity>
    {
        Task<bool> IsPhoneExist(string phone);

        Task<_<UserPhoneEntity>> SetPhone(string uid, string phone);

        Task<IEnumerable<UserPhoneEntity>> GetUserPhone(string user_uid);

        /// <summary>
        /// 读取用户
        /// </summary>
        Task<UserEntity> GetUserByPhone(string phone);

        /// <summary>
        /// 保存验证码
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task AddVadlidationCode(ValidationCodeEntity data);

        Task<ValidationCodeEntity> GetValidationCode(Expression<Func<ValidationCodeEntity, bool>> where);

        /// <summary>
        /// 通过三方账号找到关联的账号
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        Task<ExternalLoginMapEntity> FindExternalLoginByOpenID(string provider, string openid);

        /// <summary>
        /// 查找用户的外部关联账号
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="user_uid"></param>
        /// <returns></returns>
        Task<ExternalLoginMapEntity> FindExternalLoginByUserID(string provider, string user_uid);

        /// <summary>
        /// 保存关联关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<_<ExternalLoginMapEntity>> SaveExternalProviderMapping(ExternalLoginMapEntity model);

        /// <summary>
        /// 删除外部账号关联
        /// </summary>
        /// <param name="user_uid"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        Task RemoveExternalLogin(string user_uid, string[] provider);
    }
}
