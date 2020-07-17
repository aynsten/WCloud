using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Framework.Wechat.Models
{
    /*
     * 

        对称解密使用的算法为 AES-128-CBC，数据采用PKCS#7填充。
对称解密的目标密文为 Base64_Decode(encryptedData)。
对称解密秘钥 aeskey = Base64_Decode(session_key), aeskey 是16字节。
对称解密算法初始向量 为Base64_Decode(iv)，其中iv由数据接口返回。


     {
"phoneNumber": "13580006666",
"purePhoneNumber": "13580006666",
"countryCode": "86",
"watermark":
{
    "appid":"APPID",
    "timestamp": TIMESTAMP
}
}
         */
    public class WxPhoneModel
    {
        public class WaterMarkModel
        {
            public string appid { get; set; }
        }
        public string phoneNumber { get; set; }
        public string purePhoneNumber { get; set; }
        public string countryCode { get; set; }
        public WaterMarkModel watermark { get; set; }
    }
}
