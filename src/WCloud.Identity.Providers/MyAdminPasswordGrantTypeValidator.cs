using IdentityServer4.Models;
using IdentityServer4.Validation;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Application.Login;
using WCloud.Member.Authentication;

namespace WCloud.Identity.Providers
{
    public class MyAdminPasswordGrantTypeValidator : IExtensionGrantValidator
    {
        private readonly ILogger logger;
        private readonly IAdminLoginService loginService;

        public MyAdminPasswordGrantTypeValidator(ILogger<MyAdminPasswordGrantTypeValidator> logger, IAdminLoginService loginService)
        {
            this.logger = logger;
            this.loginService = loginService;
        }

        public string GrantType => ConfigSet.Identity.AdminPwdGrantType;

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                var user_name = context.Request.Raw["username"];
                var password = context.Request.Raw["password"];

                if (ValidateHelper.IsEmpty(user_name))
                    throw new MsgException("请输入用户名");

                if (ValidateHelper.IsEmpty(password))
                    throw new MsgException("请输入密码");

                var res = await this.loginService.ValidUserPassword(user_name, password);
                if (res.Error)
                {
                    throw new MsgException(res.ErrorMsg);
                }

                var model = res.Data;
                var subject = model.Id;

                var identity = new ClaimsIdentity(model.ToClaims());
                identity.SetAccountType("admin").SetCreateTimeUtc(DateTime.UtcNow);

                //这个返回还没用到
                var response = new Dictionary<string, object>()
                {
                    ["time_utc"] = DateTime.UtcNow
                };

                context.Result = new GrantValidationResult(
                 subject: subject,
                 claims: identity.Claims,
                 authenticationMethod: "custom",
                 customResponse: response);
            }
            catch (MsgException e)
            {
                var msg = e.Message ?? nameof(MyResourceOwnerPasswordValidator);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, msg);
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "服务器发生错误");
            }
        }
    }
}
