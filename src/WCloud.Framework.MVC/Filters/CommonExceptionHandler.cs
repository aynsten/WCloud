using Lib.core;
using Lib.extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Volo.Abp.Authorization;
using WCloud.Core;

namespace WCloud.Framework.Filters
{
    public interface ICommonExceptionHandler
    {
        _<object> Handle(Exception e);
    }

    internal class CommonExceptionHandler : ICommonExceptionHandler
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        private readonly bool _isDev;

        public CommonExceptionHandler(IWebHostEnvironment env, ILogger<CommonExceptionHandler> logger)
        {
            this._env = env;
            this._logger = logger;
            this._isDev = env.IsDevelopment();
        }

        public _<object> Handle(Exception e)
        {
            if (e == null)
            {
                return new _().SetSuccessData("未能捕获异常");
            }
            if (e is MsgException msg_ex)
            {
                return new _().SetErrorMsg(msg_ex.Message);
            }
            if (e is NoOrgException)
            {
                return new _().SetErrorMsg("请选择组织", "-111");
            }
            if (e is NotInCurrentOrgException)
            {
                return new _().SetErrorMsg("你不在当前组织内", "-111.1");
            }
            if (e is NoParamException no_param_ex)
            {
                return new _()
                {
                    ErrorMsg = "参数错误",
                    ErrorCode = "-100",
                    Data = no_param_ex.ResponseTemplate
                }.SetHttpStatusCode(HttpStatusCode.BadRequest);
            }
            if (e is NoLoginException)
            {
                return new _().SetErrorMsg("没有登录", "-401").SetHttpStatusCode(HttpStatusCode.Unauthorized);
            }
            if (e is NoPermissionException || e is NoPermissionInOrgException || e is AbpAuthorizationException)
            {
                return new _().SetErrorMsg("没有权限", "-403").SetHttpStatusCode(HttpStatusCode.Forbidden);
            }
            if (e is NotImplementedException || e is NotSupportedException)
            {
                return new _().SetErrorMsg("该功能未实现或者暂不支持").SetHttpStatusCode(HttpStatusCode.NotImplemented);
            }

            //------------------------------上面是业务异常，下面是程序异常--------------------------------

            //开发环境就抛出所有异常
            if (this._isDev)
                throw e;
            //生产环境写日志，返回友好格式
            this._logger.AddErrorLog(e.Message, e);

            var res = new _();
            res.SetErrorMsg("服务器发生异常").SetHttpStatusCode(HttpStatusCode.InternalServerError);
            return res;
        }
    }
}
