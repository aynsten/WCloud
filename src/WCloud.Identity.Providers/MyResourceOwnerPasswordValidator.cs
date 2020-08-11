using IdentityServer4.Models;
using IdentityServer4.Validation;
using Lib.core;
using Lib.extension;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Member.Application;
using WCloud.Member.Authentication;
using WCloud.Member.Domain.User;

namespace WCloud.Identity.Providers
{
    public class MyResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger _logger;
        private readonly ILoginService<UserEntity> _userLogin;

        public MyResourceOwnerPasswordValidator(ILogger<MyResourceOwnerPasswordValidator> logger, ILoginService<UserEntity> userLogin)
        {
            this._logger = logger;
            this._userLogin = userLogin;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var res = await this._userLogin.ValidUserPassword(context.UserName, context.Password);
                if (res.Error)
                {
                    throw new MsgException(res.ErrorMsg);
                }

                var model = res.Data;
                var subject = model.Id;

                var identity = new ClaimsIdentity(model.ToClaims());
                identity.SetAccountType("user").SetCreateTimeUtc(DateTime.UtcNow);

                //这个返回还没用到
                var response = new Dictionary<string, object>()
                {
                    ["new_user"] = true,
                    ["password"] = "pwd",
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
                this._logger.AddErrorLog(e.Message, e);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "服务器发生错误");
            }
        }
    }
}
