using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.extension
{
    public static class ConvertExtension
    {
        private static readonly IReadOnlyCollection<string> bool_string_list =
            new List<string>() {
                "1", "true", "yes", "on", "success", "ok",
                true.ToString().ToLower()
            }.AsReadOnly();

        /// <summary>
        /// 大于0是true，否则false
        /// </summary>
        public static bool ToBool(this int data) => data > 0;

        /// <summary>
        /// true是1，false是0
        /// </summary>
        public static int ToBoolInt(this bool data) => data ? 1 : 0;

        /// <summary>
        /// 转换为布尔值
        /// </summary>
        public static bool ToBool(this string data) =>
            bool_string_list.Contains(data.ToLower());

        /// <summary>
        /// true为1，false为0
        /// </summary>
        public static int ToBoolInt(this string data) =>
            data.ToBool().ToBoolInt();
    }
}
