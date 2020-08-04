using Lib.core;
using Lib.extension;
using Lib.helper;
using Lib.ioc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.MVC.Extension;

namespace WCloud.Framework.MVC.Attribute_
{
    /// <summary>
    /// 验证签名
    /// </summary>
    public class ValidateSignAttribute : _ActionFilterBaseAttribute
    {
        /// <summary>
        /// 配置文件里的key
        /// </summary>
        public string ConfigKey { get; set; } = "sign_key";
        public string SignKey { get; set; } = "sign";

        /// <summary>
        /// 时间戳误差
        /// </summary>
        public int DeviationSeconds { get; set; } = 10;

        public override async Task OnActionExecutionAsync(ActionExecutingContext _context, ActionExecutionDelegate next)
        {
            var config = _context.HttpContext.RequestServices.ResolveConfig_();

            var salt = config[ConfigKey];
            if (ValidateHelper.IsEmpty(salt))
                throw new ConfigNotExistException($"没有配置签名的约定key({ConfigKey})");
            var context = _context.HttpContext;

            var allparams = context.PostAndGet();

            #region 验证时间戳
            var client_timestamp = ConvertHelper.GetInt64(allparams.GetValueOrDefault("timestamp") ?? "-1", -1);
            if (client_timestamp < 0)
            {
                _context.Result = new JsonResult(new _() { }.SetErrorMsg("缺少时间戳"));
                return;
            }
            var server_timestamp = DateTimeHelper.GetTimeStamp();
            //取绝对值
            if (Math.Abs(server_timestamp - client_timestamp) > Math.Abs(DeviationSeconds))
            {
                _context.Result = new JsonResult(new _() { }.SetSuccessData(new
                {
                    client_timestamp,
                    server_timestamp
                }).SetErrorMsg("请求时间戳已经过期"));
                return;
            }
            #endregion

            #region 验证签名
            var client_sign = ConvertHelper.GetString(allparams.GetValueOrDefault(SignKey)).ToUpper();
            if (!ValidateHelper.IsAllNotEmpty(client_sign))
            {
                _context.Result = new JsonResult(new _() { }.SetErrorMsg("请求被拦截，获取不到签名"));
                return;
            }

            var reqparams = SignHelper.FilterAndSort(allparams, SignKey, new MyStringComparer());
            var (server_sign, sign_data) = SignHelper.CreateSign(reqparams, salt);

            if (client_sign != server_sign)
            {
                _context.Result = new JsonResult(new _() { }.SetSuccessData(new
                {
                    server_sign,
                    client_sign,
                    sign_data
                }).SetErrorMsg("签名错误"));
                return;
            }
            #endregion

            await next.Invoke();
        }

        Task Sign_1(ActionExecutingContext _context)
        {
            throw new MsgException("获取不到签名");
        }

        Task Sign_2(ActionExecutingContext _context)
        {
            throw new MsgException("获取不到签名");
        }

        async Task xx(ActionExecutingContext _context, ActionExecutionDelegate next)
        {
            foreach (var handler in new Func<ActionExecutingContext, Task>[] { Sign_1, Sign_2 })
            {
                try
                {
                    await handler.Invoke(_context);

                    _context.Result = null;
                    await next.Invoke();
                    return;
                }
                catch (MsgException e)
                {
                    _context.Result = new JsonResult(new _() { }.SetErrorMsg(e.Message));
                }
            }
        }
    }
}
