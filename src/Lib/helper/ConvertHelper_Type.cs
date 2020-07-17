using System;

namespace Lib.helper
{
    public static partial class ConvertHelper
    {
        /// <summary>
        /// 转换为整型
        /// </summary>
        public static int GetInt(string data, int? deft = default(int))
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (int.TryParse(data, out var res))
                return res;

            return deft ?? throw new ArgumentException($"{data}无法转为{typeof(int).FullName}");
        }

        /// <summary>
        /// 转为长整型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="deft"></param>
        /// <returns></returns>
        public static long GetLong(string data, long? deft = default(long))
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (long.TryParse(data, out var res))
                return res;

            return deft ?? throw new ArgumentException($"{data}无法转为{typeof(long).FullName}");
        }

        /// <summary>
        /// 转为float
        /// </summary>
        public static float GetFloat(string data, float? deft = default(float))
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (float.TryParse(data, out var res))
                return res;

            return deft ?? throw new ArgumentException($"{data}无法转为{typeof(float).FullName}");
        }

        public static Int64 GetInt64(string data, Int64? deft = default(Int64))
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (Int64.TryParse(data, out var res))
                return res;

            return deft ?? throw new ArgumentException($"{data}无法转为{typeof(Int64).FullName}");
        }

        /// <summary>
        /// 转换为双精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        public static double GetDouble(string data, int? digits = null, double? deft = default(double))
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            double ConvertData()
            {
                if (double.TryParse(data, out var res))
                    return res;

                return deft ?? throw new ArgumentException($"{data}无法转为{typeof(double).FullName}");
            }

            var db = ConvertData();
            if (digits != null)
            {
                return Math.Round(db, digits.Value);
            }
            return db;
        }

        /// <summary>
        /// 转换为高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        public static decimal GetDecimal(string data, int? digits = null, decimal? deft = default(decimal))
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            decimal ConvertData()
            {
                if (decimal.TryParse(data, out var res))
                    return res;

                return deft ?? throw new ArgumentException($"{data}无法转为{typeof(decimal).FullName}");
            }

            var dec = ConvertData();
            if (digits != null)
            {
                return Math.Round(dec, digits.Value);
            }
            return dec;
        }

        /// <summary>
        /// 转换为日期
        /// </summary>
        public static DateTime GetDateTime(string data, DateTime? deft)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (DateTime.TryParse(data, out var res))
                return res;

            return deft ?? throw new ArgumentException($"{data}无法转为{typeof(DateTime).FullName}");
        }

        /// <summary>
        /// 转为日期，默认值为当前时间
        /// </summary>
        public static DateTime GetDateTime(string data)
        {
            return ConvertHelper.GetDateTime(data, DateTime.Now);
        }
    }
}
