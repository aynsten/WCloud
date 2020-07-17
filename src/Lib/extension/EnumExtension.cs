using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Lib.extension
{
    public static class EnumExtension
    {
        static void GetEnumFieldsValues<T>(this Type t) where T : Enum { }

        /// <summary>
        /// 用来获取枚举成员
        /// </summary>
        public static Dictionary<string, object> GetEnumFieldsValues(this Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{t.FullName}must be enum");

            string GetFieldName(MemberInfo m)
            {
                var res = m.GetCustomAttribute<DescriptionAttribute>()?.Description ?? m.Name;
                return res;
            }

            var fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);

            return fields.ToDictionary_(x => GetFieldName(x), x => x.GetValue(null));
        }
    }
}
