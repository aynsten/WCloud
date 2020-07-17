using System;
using System.Text;

namespace Lib.helper
{
    public static partial class ConvertHelper
    {
        /*
            string username = "chenxizhang";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(username); //这是把字符串转成字节数组
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer)); //这是把字节数组再转回到字符串

            Console.WriteLine(BitConverter.ToString(buffer)); // 这是把字节数组当作字符串输出（长度较长）
            Console.WriteLine(Convert.ToBase64String(buffer)); //这是把字节数组当作一种所谓的Base64的字符串格式输出
         */

        [Obsolete]
        public static string GetBitString(string str, Encoding encoding)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            var bs = encoding.GetBytes(str);
            return BitConverter.ToString(bs);
        }

        public static string BytesToBase64(byte[] b)
        {
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            return Convert.ToBase64String(b);
        }

        public static byte[] Base64ToBytes(string b64)
        {
            if (b64 == null)
                throw new ArgumentNullException(nameof(b64));

            return Convert.FromBase64String(b64);
        }

        /// <summary>
        /// 将字符串编码为Base64字符串
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        public static string Base64Encode(string str, Encoding encoding)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            var bs = encoding.GetBytes(str);
            return BytesToBase64(bs);
        }

        /// <summary>
        /// 将Base64字符串解码为普通字符串
        /// </summary>
        /// <param name="b64">要解码的字符串</param>
        public static string Base64Decode(string b64, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            var bs = Base64ToBytes(b64);
            return encoding.GetString(bs);
        }
    }
}
