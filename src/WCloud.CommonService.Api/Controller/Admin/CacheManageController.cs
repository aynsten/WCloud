using FluentAssertions;
using WCloud.Core.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Authentication.Filters;

namespace WCloud.CommonService.Api.Controller
{
    /// <summary>
    /// 管理缓存
    /// </summary>
    [CommonServiceRoute("admin")]
    public class CacheManageController : WCloudBaseController, IAdminController
    {
        private readonly ICacheProvider _cache;
        private readonly ILogger _logger;

        public CacheManageController(
            ICacheProvider cache,
            ILogger<CacheManageController> logger)
        {
            this._cache = cache;
            this._logger = logger;
        }

        static readonly IReadOnlyCollection<Type> _cacheManagerTypes = new List<Type>()
        {
            typeof(ICacheKeyManager)
        }.AsReadOnly();

        object __manager_description__(Type tp)
        {
            object __get_method_info__(MethodInfo m)
            {
                return new
                {
                    m.Name,
                    Params = m.GetParameters().Select(x => new
                    {
                        x.Name,
                        TypeName = x.ParameterType.Name,
                        FullTypeName = x.ParameterType.FullName,
                    })
                };
            }

            var list = from x in tp.GetMethods()
                       where x.ReturnType == typeof(string)
                       select x;

            var data = list.Select(__get_method_info__).ToArray();

            return new
            {
                TypeName = tp.FullName,
                Methods = data
            };
        }

        /// <summary>
        /// 获取缓存key
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin]
        public async Task<IActionResult> Query()
        {
            var loginuser = await this.GetLoginAdminAsync();

            var data = _cacheManagerTypes.Select(__manager_description__).ToArray();

            return SuccessJson(data);
        }

        IEnumerable<object> __get_param_value__(MethodInfo method, string param_data)
        {
            var param = JObject.Parse(param_data);

            foreach (var p in method.GetParameters().OrderBy(x => x.Position))
            {
                var token = param[p.Name];
                token.Should().NotBeNull();

                var param_value = token.ToObject(p.ParameterType);

                yield return param_value;
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> RemoveCacheKey(
            [FromForm]string type_name,
            [FromForm]string method_name,
            [FromForm]string param_data)
        {
            type_name.Should().NotBeNullOrEmpty();
            method_name.Should().NotBeNullOrEmpty();
            param_data.Should().NotBeNullOrEmpty();

            var tp = _cacheManagerTypes.FirstOrDefault(x => x.FullName == type_name);
            tp.Should().NotBeNull();

            var tp_instance = this.HttpContext.RequestServices.GetService(tp);
            tp_instance.Should().NotBeNull();

            var methods_list = tp.GetMethods().Where(x => x.Name == method_name).ToArray();

            methods_list.Length.Should().Be(1, "有且只有一个方法");

            var method = methods_list.First();

            var method_params_data = this.__get_param_value__(method, param_data).ToArray();

            var data = method.Invoke(tp_instance, method_params_data);

            if (!(data is string key))
                throw new ArgumentException("方法返回值不是字符串");

            key.Should().NotBeNullOrEmpty();

            var loginuser = await this.GetLoginAdminAsync();

            await this._cache.RemoveAsync(key);

            this._logger.LogWarning(message: $"用户：{loginuser.UserID}删除了缓存key：{key}");

            return SuccessJson();
        }

    }
}