using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WCloud.Member.Shared;

namespace WCloud.Member.Application
{
    public interface ILoginService<T> : IAutoRegistered where T : class, ILoginEntity
    {
        string EncryptPassword(string pwd);

        Task<bool> IsUserNameExist(string user_name);
        /// <summary>
        /// 禁用或者激活用户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        Task ActiveOrDeActiveUser(string uid, bool active);

        /// <summary>
        /// 通过用户名密码认证
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<_<T>> ValidUserPassword(string user_name, string password);
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<_<T>> AddAccount(T model, string specific_uid = null);

        /// <summary>
        /// 修改密码
        /// </summary>
        Task SetPassword(string uid, string pwd);

        Task SetUserName(string uid, string user_name);

        /// <summary>
        /// 读取用户
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<T> GetUserByUID(string uid);

        /// <summary>
        /// 读取用户
        /// </summary>
        /// <param name="user_name"></param>
        /// <returns></returns>
        Task<T> GetUserByUserName(string user_name);
    }
}
