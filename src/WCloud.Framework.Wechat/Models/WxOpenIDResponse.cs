using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Framework.Wechat.Models
{
    public class WxOpenIDResponse
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
        public int? errcode { get; set; }
        public string errmsg { get; set; }
    }
}
