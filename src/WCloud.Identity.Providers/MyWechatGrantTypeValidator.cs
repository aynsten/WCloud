using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Lib.core;
using Lib.extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MessageBus;
using WCloud.Framework.Wechat.Login;
using WCloud.Framework.Wechat.Models;
using WCloud.Member.Application.Login;
using WCloud.Member.Authentication;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;
using WCloud.Member.Shared.MessageBody;

namespace WCloud.Identity.Providers
{
    /// <summary>
    /// 使用微信小程序授权登录
    /// </summary>
    public class MyWechatGrantTypeValidator : IExtensionGrantValidator
    {
        private readonly ILogger _logger;
        private readonly IMessagePublisher _publisher;
        private readonly IConfiguration _configuration;
        private readonly IUserLoginService _login;

        private readonly IUserWxLoginService userWxLoginService;

        public MyWechatGrantTypeValidator(ILogger<MyWechatGrantTypeValidator> logger,
            IUserWxLoginService userWxLoginService,
            IMessagePublisher publisher,
            IConfiguration configuration,
            IUserLoginService login)
        {
            this._logger = logger;
            this.userWxLoginService = userWxLoginService;

            this._publisher = publisher;
            this._configuration = configuration;
            this._login = login;
        }

        public string GrantType => ConfigSet.Identity.WechatGrantType;

        /// <summary>
        /// code换token
        /// token换openid
        /// openid查找关联用户
        /// 如果没有用户就创建并关联
        /// 用此用户的userid作为subject颁发token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var list = new List<string>();
            try
            {
                list.Add(context.Request.Raw.ToDict().ToJson());

                var code = context.Request.Raw["code"];
                var nick_name = context.Request.Raw["nick_name"];
                var avatar_url = context.Request.Raw["avatar_url"];
                code.Should().NotBeNullOrEmpty("未传微信code");

                var openid_response = await this.__get_openid__(code);

                var user = await this.__get_user_or_create__(openid_response.openid, nick_name, avatar_url);

                list.Add(user.ToJson());

                var subject = user.UID;

                var identity = new ClaimsIdentity(user.ToClaims()).SetAccountType("user").SetCreateTimeUtc(DateTime.UtcNow);

                //这个返回还没用到
                var response = new Dictionary<string, object>()
                {
                    ["user_name"] = user.UserName
                };

                context.Result = new GrantValidationResult(
                 subject: subject,
                 claims: identity.Claims,
                 authenticationMethod: "custom",
                 customResponse: response);
            }
            catch (MsgException e)
            {
                list.Add(e.Message);

                var msg = e.Message ?? nameof(MyWechatGrantTypeValidator);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, msg);
            }
            catch (Exception e)
            {
                list.Add(e.GetInnerExceptionAsJson());

                this._logger.AddErrorLog($"{e.Message}", e);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "服务器发生错误");
            }
            finally
            {
                var log = string.Join("=>", list);
                this._logger.LogInformation(message: log);
            }
        }


        async Task<WxOpenIDResponse> __get_openid__(string code)
        {
            try
            {
                var res = await this.userWxLoginService.__get_wx_openid__(code);
                return res;
            }
            catch (Exception e)
            {
                this._logger.AddErrorLog("获取微信openid异常", e);
                throw new MsgException("code已过期");
            }
        }

        async Task<UserEntity> __get_user_or_create__(string openid, string nick_name, string avatar_url)
        {
            var map = await this._login.FindExternalLoginByOpenID(this.userWxLoginService.LoginProvider, openid);

            if (map == null)
            {
                var user = new UserEntity()
                {
                    UserName = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    NickName = nick_name,
                    UserImg = avatar_url
                };
                var res = await this._login.AddAccount(user);
                res.ThrowIfNotSuccess();

                await this._publisher.PublishAsync(new CopyAvatarMessage() { UserUID = res.Data.UID, AvatarUrl = avatar_url });

                var map_res = await this._login.SaveExternalProviderMapping(new ExternalLoginMapEntity()
                {
                    OpenID = openid,
                    ProviderKey = this.userWxLoginService.LoginProvider,
                    UserID = res.Data.UID
                });
                map_res.ThrowIfNotSuccess();

                return user;
            }
            else
            {
                var user = await this._login.GetUserByUID(map.UserID);
                if (user == null)
                {
                    await this._login.RemoveExternalLogin(map.UserID, new[] { this.userWxLoginService.LoginProvider });
                    throw new MsgException("此微信绑定的用户被禁用，现已解除绑定");
                }

                return user;
            }
        }
    }
}
