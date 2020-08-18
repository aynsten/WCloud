using Lib.core;
using Lib.extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Lib.helper
{
    /*
     * https://github.com/arasatasaygin/is.js/blob/master/is.js
     * 
         var regexes = {
        affirmative: /^(?:1|t(?:rue)?|y(?:es)?|ok(?:ay)?)$/,
        alphaNumeric: /^[A-Za-z0-9]+$/,
        caPostalCode: /^(?!.*[DFIOQU])[A-VXY][0-9][A-Z]\s?[0-9][A-Z][0-9]$/,
        creditCard: /^(?:(4[0-9]{12}(?:[0-9]{3})?)|(5[1-5][0-9]{14})|(6(?:011|5[0-9]{2})[0-9]{12})|(3[47][0-9]{13})|(3(?:0[0-5]|[68][0-9])[0-9]{11})|((?:2131|1800|35[0-9]{3})[0-9]{11}))$/,
        dateString: /^(1[0-2]|0?[1-9])([\/-])(3[01]|[12][0-9]|0?[1-9])(?:\2)(?:[0-9]{2})?[0-9]{2}$/,
        email: /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i, // eslint-disable-line no-control-regex
        eppPhone: /^\+[0-9]{1,3}\.[0-9]{4,14}(?:x.+)?$/,
        hexadecimal: /^(?:0x)?[0-9a-fA-F]+$/,
        hexColor: /^#?([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$/,
        ipv4: /^(?:(?:\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])\.){3}(?:\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])$/,
        ipv6: /^((?=.*::)(?!.*::.+::)(::)?([\dA-F]{1,4}:(:|\b)|){5}|([\dA-F]{1,4}:){6})((([\dA-F]{1,4}((?!\3)::|:\b|$))|(?!\2\3)){2}|(((2[0-4]|1\d|[1-9])?\d|25[0-5])\.?\b){4})$/i,
        nanpPhone: /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/,
        socialSecurityNumber: /^(?!000|666)[0-8][0-9]{2}-?(?!00)[0-9]{2}-?(?!0000)[0-9]{4}$/,
        timeString: /^(2[0-3]|[01]?[0-9]):([0-5]?[0-9]):([0-5]?[0-9])$/,
        ukPostCode: /^[A-Z]{1,2}[0-9RCHNQ][0-9A-Z]?\s?[0-9][ABD-HJLNP-UW-Z]{2}$|^[A-Z]{2}-?[0-9]{4}$/,
        url: /^(?:(?:https?|ftp):\/\/)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:\/\S*)?$/i,
        usZipCode: /^[0-9]{5}(?:-[0-9]{4})?$/
    };
    */

    /// <summary>
    /// 验证帮助类
    /// </summary>
    public static class ValidateHelper
    {
        private static readonly Regex Regex_Number = new Regex("^[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex Regex_NumberSign = new Regex("^[+-]?[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex Regex_Decimal = new Regex("^[0-9]+[.]?[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex Regex_DecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$", RegexOptions.Compiled); //等价于^[+-]?\d+[.]?\d+$
        private static readonly Regex Regex_Email = new Regex(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$", RegexOptions.Compiled);//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 
        private static readonly Regex Regex_CHZN = new Regex("[\u4e00-\u9fa5]", RegexOptions.Compiled);

        /// <summary>
        /// 判断是否是邮件地址，来自nop的方法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMailAddress(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }

        /// <summary>
        /// 判断是不是url
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsURL(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"[a-zA-z]+://[^s]*");
        }

        /// <summary>
        /// 是手机号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$");
        }

        /// <summary>
        /// 是否为固话号
        /// </summary>
        public static bool IsPhone(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^(\d{3,4}-?)?\d{7,8}$");
        }

        /// <summary>
        /// 是否是域名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDomain(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$");
        }

        /// <summary>
        /// 是否是中文
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsChinese(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^[\u4e00-\u9fa5]{0,}$");
        }

        /// <summary>
        /// 是否是身份证号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIDCardNo(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        /// <summary>
        /// 是否为日期
        /// </summary>
        public static bool IsDate(string s)
        {
            //return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"(\d{4})-(\d{1,2})-(\d{1,2})");
            return DateTime.TryParse(s, out var re);
        }

        /// <summary>
        /// 是否是IP
        /// </summary>
        public static bool IsIP(string s) =>
            IsNotEmpty(s) && RegexHelper.IsMatch(s, @"\d+\.\d+\.\d+\.\d+");

        public static bool IsIPv4(string s) =>
            IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        public static bool IsIPv6(string s) =>
            IsNotEmpty(s) && RegexHelper.IsMatch(s, @"/^((?=.*::)(?!.*::.+::)(::)?([\dA-F]{1,4}:(:|\b)|){5}|([\dA-F]{1,4}:){6})((([\dA-F]{1,4}((?!\3)::|:\b|$))|(?!\2\3)){2}|(((2[0-4]|1\d|[1-9])?\d|25[0-5])\.?\b){4})$/i");

        /// <summary>
        /// 是否是时间
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsTime(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$");
        }

        /// <summary>
        /// 数字或者字母
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNUMBER_OR_CHAR(string s)
        {
            return IsNotEmpty(s) && s.All(x => IsChar(x) || Is0To9(x));
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumber(string s)
        {
            return IsNotEmpty(s) && RegexHelper.IsMatch(s, @"^[0-9]*$");
        }

        /// <summary>
        /// 是否是数值(包括整数和小数)
        /// </summary>
        public static bool IsFloat(string s)
        {
            return float.TryParse(s, out var re);
        }

        /// <summary>
        /// 是否是double
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDouble(string s)
        {
            return double.TryParse(s, out var re);
        }

        /// <summary>
        /// 判断是数字，空返回false（前面可以带负号）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInt(string s)
        {
            return int.TryParse(s, out var re);
        }

        public static bool IsChar(char x)
        {
            var res = (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z');
            return res;
        }

        public static bool Is0To9(char x)
        {
            var res = x >= '0' && x <= '9';
            return res;
        }

        /// <summary>
        /// 判断是否是中文字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChineaseStr(string str)
        {
            if (!IsNotEmpty(str))
            {
                return false;
            }
            return str.ToArray().All(x => x >= 0x4e00 && x <= 0x9fbb);
        }

        /// <summary>
        /// 简单判断是否是json，不一定全部正确
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject(json) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查颜色值是否为3/6位的合法颜色只支持#ffffff格式，rgb(0,0,0)格式不能验证通过
        /// </summary>
        /// <param name="color">待检查的颜色</param>
        /// <returns></returns>
        public static bool IsColor(string color)
        {
            if (!IsNotEmpty(color)) { return false; }
            color = color.Trim();
            if (color.StartsWith("#")) { return false; }
            color = color.Trim('#');

            if (color.Length != 3 && color.Length != 6) { return false; }

            //不包含0-9  a-f以外的字符
            if (!RegexHelper.IsMatch(color, "[^0-9a-f]", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static readonly string[] ImageExtesions = new string[] { ".jpg", ".png", ".gif", ".bmp", ".jpeg" };

        /// <summary>
        /// 是否是图片
        /// </summary>
        /// <param name="urlOrPathOrName"></param>
        /// <returns></returns>
        public static bool IsImage(string urlOrPathOrName)
        {
            if (IsEmpty(urlOrPathOrName))
                return false;

            urlOrPathOrName = urlOrPathOrName.Trim().ToLower();
            foreach (var ext in ImageExtesions)
            {
                if (urlOrPathOrName.EndsWith(ext)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// 是否是imoji字符
        /// </summary>
        [Obsolete("方法有问题")]
        public static bool _IsEmoji(char codePoint)
        {
            return (codePoint == 0x0) ||
                    (codePoint == 0x9) ||
                    (codePoint == 0xA) ||
                    (codePoint == 0xD) ||
                    ((codePoint >= 0x20) && (codePoint <= 0xD7FF)) ||
                    ((codePoint >= 0xE000) && (codePoint <= 0xFFFD)) ||
                    ((codePoint >= 0x10000) && (codePoint <= 0x10FFFF));
        }
        [Obsolete("方法有问题")]
        public static bool _IsEmoji(string codePoint)
        {
            return codePoint?.Any(x => _IsEmoji(x)) ?? false;
        }

        /// <summary>
        /// 判断文件流是否为UTF8字符集
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>判断结果</returns>
        private static bool IsUTF8(FileStream stream)
        {
            byte cOctets = 0;  // octets to go in this UTF-8 encoded character 
            byte chr;
            bool bAllAscii = true;
            for (int i = 0; i < stream.Length; ++i)
            {
                chr = (byte)stream.ReadByte();

                if ((chr & 0x80) != 0) { bAllAscii = false; }

                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        }
                        while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0) { return false; }
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80) { return false; }
                    cOctets--;
                }
            }

            if (cOctets > 0)
            {
                return false;
            }

            if (bAllAscii)
            {
                return false;
            }

            return true;
        }

        #region 判断是否是空数据

        /// <summary>
        /// string dict都是list
        /// </summary>
        public static bool IsNotEmpty<T>(IEnumerable<T> list)
        {
            /*
            IEnumerable<char> x = "fasdfas";
            IEnumerable<string> y = new List<string>();
            IEnumerable<KeyValuePair<string, string>> d = new Dictionary<string, string>();
            */
            var res = list?.Any() ?? false;
            return res;
        }

        /// <summary>
        /// 为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(IEnumerable<T> list) => !IsNotEmpty(list);

        /// <summary>
        /// 判断是否都是非空字符串
        /// </summary>
        public static bool IsAllNotEmpty(params string[] strs)
        {
            if (IsEmpty(strs))
                throw new ArgumentNullException("至少需要一个参数");
            return strs.All(x => IsNotEmpty(x));
        }

        /// <summary>
        /// 判断数组里至少有一个非空字符串
        /// </summary>
        public static bool IsAnyNotEmpty(params string[] strs)
        {
            if (IsEmpty(strs))
                return false;
            return strs.Any(x => IsNotEmpty(x));
        }
        #endregion

        /// <summary>
        /// 判断一个对象是否是某个类型
        /// </summary>
        public static bool Is<T>(object obj) => obj != null && obj is T;

        /// <summary>
        /// 判断是相同引用
        /// </summary>
        public static bool IsReferenceEquals(object obj1, object obj2) => object.ReferenceEquals(obj1, obj2);

        /// <summary>
        /// 根据attribute验证model
        /// </summary>
        public static List<string> CheckEntity_<T>(T model) where T : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var list = new List<string>();

            //checker
            bool CheckProp(IEnumerable<ValidationAttribute> validators, PropertyInfo p)
            {
                var data = p.GetValue(model);
                foreach (var validator in validators)
                {
                    if (!validator.IsValid(data))
                    {
                        var msg = ConvertHelper.GetString(validator.ErrorMessage).Trim();
                        if (ValidateHelper.IsEmpty(msg))
                        {
                            msg = $"字段{p.Name}未通过{validator.GetType().Name}标签的验证";
                        }
                        list.Add(msg);
                        return false;
                    }
                }
                return true;
            };

            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.HasCustomAttributes_<NotMappedAttribute>(inherit: false))
                    continue;

                //忽略父级的验证属性
                var validators = prop.GetCustomAttributes_<ValidationAttribute>(inherit: false);
                if (!CheckProp(validators, prop))
                    continue;
            }

            list = list.SelectNotEmptyAndDistinct(x => x).ToList();

            return list;
        }
    }
}
