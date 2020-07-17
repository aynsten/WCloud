using System.Threading.Tasks;
using WCloud.Core.Authentication.Model;

namespace WCloud.Member.Authentication.UserContext
{
    /// <summary>
    /// 贯穿整个请求的登录用户上下文
    /// 会缓存登陆状态
    /// </summary>
    //[System.Obsolete("直接注入登录的model")]
    public interface ILoginContext<T> where T : class, ILoginModel
    {
        [WCloud.Core.Apm.Apm]
        Task<T> GetLoginContextAsync();
    }
}
