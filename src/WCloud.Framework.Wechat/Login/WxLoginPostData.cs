using System;

namespace WCloud.Framework.Wechat.Login
{
    public class WxLoginPostData
    {
        public string code { get; set; }
        public string encryptedData { get; set; }
        public string iv { get; set; }
        public string avatar_url { get; set; }
    }
}
