using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Http;
using System;

namespace WCloud.Framework.MVC.Extension
{
    public static class SessionExtension
    {
        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            var data = (value ?? throw new Exception("null不能转为json，并存入session")).ToJson();
            session.SetString(key, data);
        }


        /// <summary>
        /// 获取session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSession<T>(this ISession session, string key) =>
            JsonHelper.JsonToEntity<T>(session.GetString(key) ?? string.Empty, throwIfException: false);

        /// <summary>
        /// 删除session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="keys"></param>
        public static void RemoveSession(this ISession session, params string[] keys) =>
            keys.ForEach_(session.Remove);
    }
}
