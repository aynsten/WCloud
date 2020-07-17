using FluentAssertions;
using Lib.core;
using Lib.extension;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WCloud.Framework.Wechat.Models;

namespace WCloud.Framework.Wechat.Login
{
    /// <summary>
    /// 微信小程序获取手机号码登陆
    /// </summary>
    public class UserWxLoginService : IUserWxLoginService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        private readonly string wx_appid;
        private readonly string wx_appsecret;

        private readonly string wx_api_server = "https://api.weixin.qq.com";

        public string LoginProvider => "wx";

        public UserWxLoginService(
            WxConfig config,
            ILogger<UserWxLoginService> logger,
            IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;

            this._client = httpClientFactory.CreateClient("wx_login");

            this.wx_appid = config.AppID;
            this.wx_appid.Should().NotBeNullOrEmpty("请配置微信appid");

            this.wx_appsecret = config.AppSecret;
            this.wx_appsecret.Should().NotBeNullOrEmpty("请配置微信appsecret");
        }

        public async Task<WxOpenIDResponse> __get_wx_openid__(string code)
        {
            var p = $"appid={this.wx_appid}&secret={this.wx_appsecret}&js_code={code}&grant_type=authorization_code";
            var url = $"{this.wx_api_server}/sns/jscode2session?{p}";

            string json = null;
            try
            {
                using (var res = await this._client.GetAsync(url))
                {
                    res.EnsureSuccessStatusCode();

                    json = await res.Content.ReadAsStringAsync();
                    json.Should().NotBeNullOrEmpty("微信返回了空数据");

                    var response = json.JsonToEntity<WxOpenIDResponse>();

                    response.Should().NotBeNull();
                    response.openid.Should().NotBeNullOrEmpty();

                    return response;
                }
            }
            catch (Exception e)
            {
                var info = new
                {
                    url,
                    json
                };

                this._logger.AddErrorLog(info.ToJson(), e);

                throw new MsgException("获取openid失败");
            }
        }

        /// <summary>
        /// https://developers.weixin.qq.com/miniprogram/dev/framework/open-ability/signature.html#%E5%8A%A0%E5%AF%86%E6%95%B0%E6%8D%AE%E8%A7%A3%E5%AF%86%E7%AE%97%E6%B3%95
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public string __extract_mobile_or_throw__(string encryptedData, string iv, string sessionKey)
        {
            var list = new List<string>();

            string json = null;
            try
            {
                try
                {
                    json = MyWechatEncrypHelper.AESDecrypt(encrypt_data: encryptedData, iv: iv, session_key: sessionKey);
                    list.Add($"json1:{json}");

                    json.Should().NotBeNullOrEmpty("解密json为空");
                }
                catch
                {
                    //用其他办法
                    json = WechatEncrypHelper.DecodeEncryptedData(sessionKey, encryptedData, iv);
                    list.Add($"json2:{json}");

                    json.Should().NotBeNullOrEmpty("解密json为空");
                }

                var data = json.JsonToEntity<WxPhoneModel>();

                (data?.watermark?.appid == this.wx_appid).Should().BeTrue("appid不匹配");

                data.purePhoneNumber.Should().NotBeNullOrEmpty("拿不到手机号");

                return data.purePhoneNumber;
            }
            catch (Exception e)
            {
                var info = new
                {
                    encryptedData,
                    iv,
                    sessionKey,
                    json,
                    list
                };

                this._logger.AddErrorLog(info.ToJson(), e);

                throw new MsgException("解密失败");
            }
        }
    }
}
