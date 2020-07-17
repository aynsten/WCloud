using FluentAssertions;
using Lib.encryption.Symmetric;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using System.Text;
using WCloud.Framework.Wechat.Login;

namespace WCloud.Test
{
    [TestClass]
    public class encryption_test
    {
        [TestMethod]
        public void aes_test()
        {
            var encoding = Encoding.UTF8;

            var data = "123";

            var pass = AESHelper.GenericIvKey();

            var encrypt_data = AESHelper.Encrypt(encoding.GetBytes(data), pass.key, pass.iv);

            var decrypt_data = AESHelper.Decrypt(encrypt_data, pass.key, pass.iv);

            encoding.GetString(decrypt_data).Should().Be(data);
        }

        [TestMethod]
        public void aes_wechat_test()
        {
            var encrypt_data = "oJSnj4dB3dHyYsunyMEnt03rgDt3ucdQu9NvA0z+n8vyJ73Z13kbrPUPa+OtOZkLUoRPudRjLC4C7/y9oYN1FmU2fdty6Rq8nAnE0CwHxOJESeNMzYplF5SBxKL+bu0R0lVe3KDLvSrpbSKd9pPuWFz3jpf2bp5BXpQpdqtkxrxFmMpSWhU9TSLC2tLlYvDMhVhiI/VLG97RUeWBcCawnQ==";
            var iv = "S3VmEObmBoFV7OTR//2NZw==";
            var session_key = "4Nh2jR3Afc3CMgkDl76zpQ==";

            void test()
            {
                var data = MyWechatEncrypHelper.AESDecrypt(encrypt_data, iv, session_key);
            }

            //这里为啥会抛异常呢
            new Action(test).Should().Throw<Exception>();

            encrypt_data = "0XruAccdfr92mDhQrIkpa9oAJClIQbT3Y3d3pLSVxvbFYsDGNozzI6SqyDk9VWgwUPAV1AGxrTzmwYLiaPA9JLjztKzQXZ6g/1Y6wFnByKoKY94P+GS2ViWm+dgGOnbwf9BKHIMAJ2OyHX97c6wSatWnbsTteQhXbu6sgE5o7a9+h3ssv370Ezac2TzDF3yGxEcvUJvOw6TnpKC4IXVyDA==";
            iv = "h9rJ3t4fEs9YNaGM7UeA9Q==";
            session_key = "4Nh2jR3Afc3CMgkDl76zpQ==";

            new Action(test).Should().NotThrow();

            encrypt_data = "8GtyjAsh8hnycvxhxfM3aiz5orMEZzJgm4j3nlrG39gE+AfIGvRpUcEhPUe65Bhji0sUSqIOUDpFZoFJGeI1q7VoXM/U1Td/amul8UIV93IimqUyzG4ko2Rfw3SymZeX4sjnyKFjSZ7DGfB3Go7EpKdWbcUVe4scP/bRLBizV6hCd87YolrSJJdujeIlTWCllvv/Ug0Q6yWT1hxqemKLo68DaiDTfVMJfhT0QFvpoR7Xnq2z1MErvz+4OJy6pCsmzBi16fejyFo1to7/X4Q3Aw4bYFdUPuhvVnmu8RUOM1SBzoKuCTBv/7tX5QgDzqCBccLDtXLCVGXIrTa7OrahJrKPfD7lZSHBVrOYZk1JQjf1xTsOz7Ci7zhJZNKycBu0rpV2YUyHQT/32bFQCf1aPU1Rmf5suNc3/wMA+X/lCb4+mvp1aRSpFcx5hd1pLwVyRnj4PucNlanmAA5oGLeTRp+tBPlL0pL6HZHkcK9bH10=";
            iv = "321jEZN/tHvSPSIBCtQjQg==";
            session_key = "BaYayBiO971vGvYiiLq5og==";

            test();
        }

        [TestMethod]
        public void TestMethod1()
        {

            var encryp = "0XruAccdfr92mDhQrIkpa9oAJClIQbT3Y3d3pLSVxvbFYsDGNozzI6SqyDk9VWgwUPAV1AGxrTzmwYLiaPA9JLjztKzQXZ6g/1Y6wFnByKoKY94P+GS2ViWm+dgGOnbwf9BKHIMAJ2OyHX97c6wSatWnbsTteQhXbu6sgE5o7a9+h3ssv370Ezac2TzDF3yGxEcvUJvOw6TnpKC4IXVyDA==";
            var iv = "h9rJ3t4fEs9YNaGM7UeA9Q==";
            var key = "4Nh2jR3Afc3CMgkDl76zpQ==";

//            var text = "sb sb ha ha ha";
            //对text,key iv 进行base64加密
            var enKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            var enIv = Convert.ToBase64String(Encoding.UTF8.GetBytes(iv));
            //var enText = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
            //加密
            //var encryp = AesEncryp(enText, enKey, enIv);
            //解密
            var decryp = AesDecryp(encryp, enKey, enIv);



        }
        /// <summary>
        /// 加密密text
        /// 王剑锋 2018年7月7日09:58:34
        /// </summary>
        /// <param name="text">明文</param>
        /// <param name="key">秘钥,Base64串</param>
        /// <param name="iv">向量,Base64串</param>
        /// <returns></returns>
        public string AesEncryp(string text, string key, string iv)
        {
            var aes = Aes.Create();
            //AES-128-CBC PKCS#7
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            //Base64解密text,KEY,IV,不够16位填充0
            aes.Key = Padding16(Convert.FromBase64String(key), 0);
            aes.IV = Padding16(Convert.FromBase64String(iv), 0);
            //获取该算法规则下的加密器
            var encryp = aes.CreateEncryptor();
            //加密之前加一个Base64解密 ,要加密的数据
            var dataBytes = Convert.FromBase64String(text);
            //获取AES-128-CBC PKCS#7 密文
            var enData = encryp.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Convert.ToBase64String(enData);
        }
        /// <summary>
        /// 解密text
        /// 王剑锋 2018年7月7日09:58:16
        /// </summary>
        /// <param name="text">密文</param>
        /// <param name="key">秘钥,Base64串</param>
        /// <param name="iv">向量,Base64串</param>
        /// <returns></returns>
        public string AesDecryp(string text, string key, string iv)
        {
            var aes = Aes.Create();
            //AES-128-CBC PKCS#7
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            //Base64解密text,KEY,IV
            aes.Key = Padding16(Convert.FromBase64String(key), 0);
            aes.IV = Padding16(Convert.FromBase64String(iv), 0);
            //获取该算法规则下的解密器
            var encryp = aes.CreateDecryptor();
            //加密之前加一个Base64解密 ,要加密的数据
            var dataBytes = Convert.FromBase64String(text);
            //获取AES-128-CBC PKCS#7 明文
            var enData = encryp.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Encoding.UTF8.GetString(enData);
        }
        /// <summary>
        /// 不够16位的整数倍,已指定字符填充
        /// </summary>
        /// <param name="array"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public byte[] Padding16(byte[] array, byte padding)
        {
            int group = (array.Length + 15) / 16;
            var newArray = new byte[group * 16];
            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = (i < array.Length ? array[i] : padding);
            }
            return newArray;
        }
    }
}
