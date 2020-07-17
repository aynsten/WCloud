using Lib.helper;

namespace Lib.extension
{
    public static class JsonExtension
    {
        /// <summary>
        /// 转为json 
        /// </summary>
        public static string ToJson<T>(this T data)
            where T : class
            => JsonHelper.ObjectToJson(data);

        /// <summary>
        /// json转为实体
        /// </summary>
        public static T JsonToEntity<T>(this string json, bool throwIfException = true, T deft = default)
            where T : class
            => JsonHelper.JsonToEntity(json, throwIfException: throwIfException, deft: deft);

        /// <summary>
        /// json转为实体，出现异常就返回默认值
        /// </summary>
        public static T JsonToEntityOrDefault<T>(this string json)
            where T : class
            => json.JsonToEntity<T>(throwIfException: false);
    }
}
