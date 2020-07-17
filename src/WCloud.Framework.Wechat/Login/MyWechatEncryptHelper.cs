using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lib.encryption.Symmetric;

namespace WCloud.Framework.Wechat.Login
{
    public static class MyWechatEncrypHelper
    {
        public static string AESDecrypt(string encrypt_data, string iv, string session_key)
        {
            var bs = AESHelper.Decrypt(
                     encryptedData: Convert.FromBase64String(encrypt_data),
                     key: Convert.FromBase64String(session_key),
                     iv: Convert.FromBase64String(iv),
                     keySize: 128,
                     blockSize: 128,
                     mode: System.Security.Cryptography.CipherMode.CBC,
                     padding: System.Security.Cryptography.PaddingMode.PKCS7);

            var data = Encoding.UTF8.GetString(bs);

            return data;
        }
    }

    /// <summary>
    /// https://github.com/JeffreySu/WeiXinMPSDK/blob/74ef58e1e667669db257807d89ce27c8131d8ad3/src/Senparc.Weixin.WxOpen/src/Senparc.Weixin.WxOpen/Senparc.Weixin.WxOpen/Helpers/EncryptHelper.cs
    /// </summary>
    public static class WechatEncrypHelper
    {
        /// <summary>
        /// 解密所有消息的基础方法
        /// </summary>
        /// <param name="sessionKey">储存在 SessionBag 中的当前用户 会话 SessionKey</param>
        /// <param name="encryptedData">接口返回数据中的 encryptedData 参数</param>
        /// <param name="iv">接口返回数据中的 iv 参数，对称解密算法初始向量</param>
        /// <returns></returns>
        public static string DecodeEncryptedData(string sessionKey, string encryptedData, string iv)
        {
            var aesCipher = Convert.FromBase64String(encryptedData);
            var aesKey = Convert.FromBase64String(sessionKey);
            var aesIV = Convert.FromBase64String(iv);

            var result = AES_Decrypt(encryptedData, aesIV, aesKey);
            var resultStr = Encoding.UTF8.GetString(result);
            return resultStr;
        }

        private static byte[] decode2(byte[] decrypted)
        {
            int pad = (int)decrypted[decrypted.Length - 1];
            if (pad < 1 || pad > 32)
            {
                pad = 0;
            }
            byte[] res = new byte[decrypted.Length - pad];
            Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
            return res;
        }

        private static byte[] AES_Decrypt(String Input, byte[] Iv, byte[] Key)
        {
#if NET45
            RijndaelManaged aes = new RijndaelManaged();
#else
            SymmetricAlgorithm aes = Aes.Create();
#endif
            aes.KeySize = 128;//原始：256
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Key;
            aes.IV = Iv;
            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] xBuff = null;

            //using (ICryptoTransform decrypt = aes.CreateDecryptor(aes.Key, aes.IV) /*aes.CreateDecryptor()*/)
            //{
            //    var src = Convert.FromBase64String(Input); 
            //    byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
            //    return dest;
            //    //return Encoding.UTF8.GetString(dest);
            //}


            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        //cs.Read(decryptBytes, 0, decryptBytes.Length);
                        //cs.Close();
                        //ms.Close();

                        //cs.FlushFinalBlock();//用于解决第二次获取小程序Session解密出错的情况


                        byte[] xXml = Convert.FromBase64String(Input);
                        byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                        Array.Copy(xXml, msg, xXml.Length);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    //cs.Dispose();
                    xBuff = decode2(ms.ToArray());
                }
            }
            catch (System.Security.Cryptography.CryptographicException e)
            {
                //Padding is invalid and cannot be removed.
                Console.WriteLine("===== CryptographicException =====");

                using (var ms = new MemoryStream())
                {
                    //cs 不自动释放，用于避免“Padding is invalid and cannot be removed”的错误    —— 2019.07.27 Jeffrey
                    var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write);
                    {
                        //cs.Read(decryptBytes, 0, decryptBytes.Length);
                        //cs.Close();
                        //ms.Close();

                        //cs.FlushFinalBlock();//用于解决第二次获取小程序Session解密出错的情况

                        byte[] xXml = Convert.FromBase64String(Input);
                        byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                        Array.Copy(xXml, msg, xXml.Length);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    //cs.Dispose();
                    xBuff = decode2(ms.ToArray());
                }
            }
            return xBuff;
        }
    }
}
