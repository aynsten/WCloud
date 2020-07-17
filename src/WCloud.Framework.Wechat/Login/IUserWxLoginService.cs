using System.Threading.Tasks;
using WCloud.Framework.Wechat.Models;

namespace WCloud.Framework.Wechat.Login
{
    public interface IUserWxLoginService
    {
        string LoginProvider { get; }
        Task<WxOpenIDResponse> __get_wx_openid__(string code);
        string __extract_mobile_or_throw__(string encryptedData, string iv, string sessionKey);
    }
}
