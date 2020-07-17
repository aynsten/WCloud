using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lib.helper
{
    /// <summary>
    /// 帮助类
    /// https://gitee.com/hiwjcn/GodSharp.Encryption
    /// </summary>
    public static class SecureHelper
    {
        private static Encoding _encoding { get => Encoding.UTF8; }

        private static string BsToStr(byte[] bs) => string.Join(string.Empty, bs.Select(x => x.ToString("x2"))).Replace("-", string.Empty);

        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bs = _encoding.GetBytes(str);
                bs = md5.ComputeHash(bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 获取md5
        /// </summary>
        /// <param name="_bs"></param>
        /// <returns></returns>
        public static string GetMD5(byte[] _bs)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bs = md5.ComputeHash(_bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 读取文件MD5
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileMD5(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bs = md5.ComputeHash(file);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 获取sha1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetSHA1(string str)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var bs = _encoding.GetBytes(str);
                bs = sha1.ComputeHash(bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string GetSHA256(string str)
        {
            using (var Sha256 = new SHA256CryptoServiceProvider())
            {
                var bs = _encoding.GetBytes(str);
                bs = Sha256.ComputeHash(bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 获取hmac md5
        /// </summary>
        /// <param name="str"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetHMACMD5(string str, string password)
        {
            using (var hmac_md5 = new HMACMD5())
            {
                hmac_md5.Key = _encoding.GetBytes(password);
                var bs = hmac_md5.ComputeHash(_encoding.GetBytes(str));
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 已测试
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetHMACSHA256(string str, string key)
        {
            using (var hmac_sha256 = new HMACSHA256(_encoding.GetBytes(key)))
            {
                var data = hmac_sha256.ComputeHash(_encoding.GetBytes(str));
                return BitConverter.ToString(data);
            }
        }

        /// <summary>
        /// 获取hmac sha1
        /// </summary>
        /// <param name="str"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetHMACSHA1(string str, string password)
        {
            using (var hmac_sha1 = new HMACSHA1())
            {
                hmac_sha1.Key = _encoding.GetBytes(password);
                var bs = hmac_sha1.ComputeHash(_encoding.GetBytes(str));
                return BsToStr(bs);
            }
        }
    }
}
