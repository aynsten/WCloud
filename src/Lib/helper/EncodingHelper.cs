using System;
using System.Text;
using System.Web;

namespace Lib.helper
{
    public static class EncodingHelper
    {
        public static string HtmlDecode(string s)
        {
#if NETFRAMEWORK
            return System.Net.WebUtility.HtmlDecode(s);
#else
            return HttpUtility.HtmlDecode(s);
#endif
        }

        public static string HtmlEncode(string s)
        {
#if NETFRAMEWORK
            return System.Net.WebUtility.HtmlEncode(s);
#else
            return HttpUtility.HtmlEncode(s);
#endif
        }

        public static string UrlDecode(string s)
        {
#if NETFRAMEWORK
            return System.Net.WebUtility.UrlDecode(s);
#else
            return HttpUtility.UrlEncode(s);
#endif
        }

        public static string UrlEncode(string s)
        {
#if NETFRAMEWORK
            return System.Net.WebUtility.UrlEncode(s);
#else
            return HttpUtility.UrlEncode(s);
#endif
        }
    }

    [Obsolete("untested")]
    public static class Native2AsciiUtils
    {
        /** 
         * prefix of ascii string of native character 
         */
        private const String PREFIX = "\\u";

        /** 
         * Native to ascii string. It's same as execut native2ascii.exe. 
         * @param str native string 
         * @return ascii string 
         */
        public static String native2Ascii(String str)
        {
            char[] chars = str.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                sb.Append(char2Ascii(chars[i]));
            }
            return sb.ToString();
        }

        /** 
         * Native character to ascii string. 
         * @param c native character 
         * @return ascii string 
         */
        private static String char2Ascii(char c)
        {
            if (c > 255)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PREFIX);
                int code = (c >> 8);
                String tmp = code.ToString("X");
                if (tmp.Length == 1)
                {
                    sb.Append("0");
                }
                sb.Append(tmp);
                code = (c & 0xFF);
                tmp = code.ToString("X");
                if (tmp.Length == 1)
                {
                    sb.Append("0");
                }
                sb.Append(tmp);
                return sb.ToString();
            }
            else
            {
                return c.ToString();
            }
        }

        /** 
         * Ascii to native string. It's same as execut native2ascii.exe -reverse. 
         * @param str ascii string 
         * @return native string 
         */
        public static String ascii2Native(String str)
        {
            StringBuilder sb = new StringBuilder();
            int begin = 0;
            int index = str.IndexOf(PREFIX);
            while (index != -1)
            {
                sb.Append(str.Substring(begin, index));
                sb.Append(ascii2Char(str.Substring(index, index + 6)));
                begin = index + 6;
                index = str.IndexOf(PREFIX, begin);
            }
            sb.Append(str.Substring(begin));
            return sb.ToString();
        }

        /** 
         * Ascii to native character. 
         * @param str ascii string 
         * @return native character 
         */
        private static char ascii2Char(String str)
        {
            if (str.Length != 6)
            {
                throw new ArgumentException(
                        "Ascii string of a native character must be 6 character.");
            }
            if (!PREFIX.Equals(str.Substring(0, 2)))
            {
                throw new ArgumentException(
                        "Ascii string of a native character must start with \"\\u\".");
            }
            String tmp = str.Substring(2, 4);
            int code = Convert.ToInt32(tmp, 16) << 8;
            tmp = str.Substring(4, 6);
            code += Convert.ToInt32(tmp, 16);
            return (char)code;
        }
    }
}
