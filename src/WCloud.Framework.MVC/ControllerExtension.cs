using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using WCloud.Core;

namespace WCloud.Framework.MVC
{
    public static class ControllerExtension
    {
        class ControllerExtension__ { }

        static object __get_model_template__<T>(IServiceProvider provider)
        {
            var logger = provider.ResolveLogger<ControllerExtension__>();
            var target_type = typeof(T);
            try
            {
                var obj = Activator.CreateInstance<T>();

                return obj;
            }
            catch (Exception e) //when (e is MissingMethodException)
            {
                logger.AddWarningLog($"{target_type.FullName}可能没有默认构造函数", e);
                return null;
            }
        }

        /// <summary>
        /// json转为实体
        /// </summary>
        public static T JsonToEntity_<T>(this ControllerBase controller, string json, string msg = null) where T : class
        {
            try
            {
                return json?.JsonToEntityOrDefault<T>() ?? throw new ArgumentException();
            }
            catch (Exception err)
            {
                var target_type = typeof(T);

                throw new NoParamException(msg ?? "参数错误", inner: err)
                {
                    ResponseTemplate = new
                    {
                        controller.Request.Path,
                        json,
                        target_instance_type = target_type.FullName,
                        template = __get_model_template__<T>(controller.HttpContext.RequestServices),
                    }
                };
            }
        }

        public static object PagerDataMapper_<T, Result>(this PagerData<T> data, Func<T, Result> selector)
        {
            data.DataList.Should().NotBeNull();

            var res = new
            {
                data.Page,
                data.PageCount,
                data.PageSize,
                data.ItemCount,
                DataList = data.DataList.Select(selector).ToArray()
            };

            return res;
        }

        /// <summary>
        /// 读取request body数据
        /// </summary>
        /// <returns>The to entity.</returns>
        /// <param name="msg">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T BodyJsonToEntity_<T>(this ControllerBase contoller, string msg = null) where T : class
        {
            var stream = contoller.Request.Body;
            if (stream != null)
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);

                    if (ms.CanSeek && ms.Position > 0)
                        ms.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(ms))
                    {
                        var json = reader.ReadToEnd();
                        var res = JsonToEntity_<T>(contoller, json, msg: msg);
                        return res;
                    }
                }
            }

            throw new EndOfStreamException("无法读取流");
        }
    }
}
