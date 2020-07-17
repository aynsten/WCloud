using System;

namespace Lib.helper
{
    /// <summary>
    /// 类型帮助类
    /// </summary>
    public static partial class ConvertHelper
    {
        /// <summary>
        /// 获取非空字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetString(object obj, string str = StringHelper.Empty) => obj?.ToString() ?? str;

        /// <summary>
        /// 通用数据转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="deft"></param>
        /// <returns></returns>
        public static T ChangeType<T>(object obj, T deft = default)
        {
            try
            {
                var o = Convert.ChangeType(obj, typeof(T));
                if (o is T re)
                {
                    return re;
                }
                //强转
                return (T)o;
            }
            catch
            {
                return deft;
            }
        }
    }

}