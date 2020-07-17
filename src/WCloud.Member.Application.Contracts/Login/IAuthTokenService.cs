using Lib.ioc;
using System.Threading.Tasks;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;

namespace WCloud.Member.Application.Service
{
    /// <summary>
    /// 对外提供的服务（webapi，wcf）
    /// </summary>
    public interface IAuthTokenService : IAutoRegistered
    {
        /// <summary>
        /// 用code换token
        /// </summary>
        Task<TokenModel> CreateAccessTokenAsync(string user_uid);

        /// <summary>
        /// 用token换取登录信息
        /// </summary>
        Task<TokenModel> GetUserIdByTokenAsync(string access_token);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task RemoveCacheAsync(CacheBundle data);

        /// <summary>
        /// 删除token，清空缓存
        /// </summary>
        /// <param name="user_uid"></param>
        /// <returns></returns>
        Task DeleteUserTokensAsync(string user_uid);

        /// <summary>
        /// 删除多个token
        /// </summary>
        /// <param name="token_uids"></param>
        /// <returns></returns>
        Task DeleteTokensAsync(string[] token_uids);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        Task RefreshToken(string access_token);
    }
}
