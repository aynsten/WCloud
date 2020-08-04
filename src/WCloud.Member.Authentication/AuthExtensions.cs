using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using Lib.extension;
using Lib.helper;
using Lib.ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Core.Authentication.Model;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.Filters;
using WCloud.Member.Domain;

namespace WCloud.Member.Authentication
{
    public static class AuthExtensions
    {
        public static async Task<DiscoveryDocumentResponse> __disco__(this HttpClient httpClient, IConfiguration config)
        {
            var identity_server = config["identity_server"];
            identity_server.Should().NotBeNullOrEmpty("请配置授权服务器");

            var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = identity_server,
                Policy = new DiscoveryPolicy() { RequireHttps = false, }
            });
            disco.IsError.Should().BeFalse(disco.Error ?? string.Empty);

            return disco;
        }

        public static _<TokenModel> ToTokenModel(this TokenResponse tokenResponse)
        {
            tokenResponse.Should().NotBeNull();

            var res = new _<TokenModel>();

            if (tokenResponse.IsError)
            {
                var err = tokenResponse.ErrorDescription ?? tokenResponse.Error ?? "登陆失败";
                return res.SetErrorMsg(err);
            }

            var model = new TokenModel()
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpireUtc = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
            };

            return res.SetSuccessData(model);
        }

        public static string GetSubjectID(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
        }

        public static string GetSubjectID(this ClaimsPrincipal principal)
        {
            return principal.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
        }

        public static ClaimsIdentity SetSubjectID(this ClaimsIdentity identity, string subject_id)
        {
            identity.SetOrReplaceClaim_(JwtClaimTypes.Subject, subject_id);
            return identity;
        }

        public const string claims_account_type_key = "__account_type__";

        public static ClaimsIdentity SetAccountType(this ClaimsIdentity identity, string account_type)
        {
            account_type.Should().NotBeNullOrEmpty();
            identity.SetOrReplaceClaim_(claims_account_type_key, account_type);
            return identity;
        }

        public static string GetAccountType(this IEnumerable<Claim> claims)
        {
            var res = claims.FirstOrDefault(x => x.Type == claims_account_type_key)?.Value;
            return res;
        }

        public const string claims_create_time_utc_key = "__claims_create_time_utc__";

        public static ClaimsIdentity SetCreateTimeUtc(this ClaimsIdentity identity, DateTime? time_utc = null)
        {
            time_utc ??= DateTime.UtcNow;
            var json = new _<DateTime>().SetSuccessData(time_utc.Value).ToJson();
            identity.SetOrReplaceClaim_(claims_create_time_utc_key, json);
            return identity;
        }

        public static DateTime? GetCreateTimeUtc(this IEnumerable<Claim> claims)
        {
            var json = claims.FirstOrDefault(x => x.Type == claims_create_time_utc_key)?.Value ?? "{}";
            var time = json.JsonToEntityOrDefault<_<DateTime?>>();
            return time?.Data;
        }

        public static ClaimsPrincipal ToPrincipal(this ILoginEntity model, string scheme, Action<ClaimsIdentity> handler = null)
        {
            var identity = new ClaimsIdentity(authenticationType: scheme);
            identity.AddClaims(ToClaims(model));

            if (handler != null)
            {
                handler.Invoke(identity);
            }

            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

        public static Claim[] ToClaims(this ILoginEntity model)
        {
            var list = new List<Claim>();

            list.Add(new Claim(JwtClaimTypes.Subject, model.UID));

            if (ValidateHelper.IsNotEmpty(model.UserName))
            {
                list.Add(new Claim(JwtClaimTypes.Name, model.UserName));
            }

            return list.ToArray();
        }

        /// <summary>
        /// 先删除，再添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="claim_type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T SetOrReplaceClaim_<T>(this T model, string claim_type, string data) where T : ClaimsIdentity
        {
            var matched = model.FindAll(x => x.Type == claim_type).ToArray();
            foreach (var m in matched)
            {
                model.RemoveClaim(m);
            }

            return model.AddClaim_(claim_type, data);
        }

        /// <summary>
        /// 添加claim
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="claim_type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T AddClaim_<T>(this T model, string claim_type, string data) where T : ClaimsIdentity
        {
            model.AddClaim(new Claim(claim_type, data));
            return model;
        }

        /// <summary>
        /// 获取bearer token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetBearerToken(this HttpContext context)
        {
            var bearer = "Bearer" + ' '.ToString();
            var token = ((string)context.Request.Headers["Authorization"]) ?? string.Empty;
            if (token.StartsWith(bearer, StringComparison.OrdinalIgnoreCase))
            {
                return token.Substring(bearer.Length);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取bearer token或者header.auth.token
        /// </summary>
        public static string GetAuthToken(this HttpContext context)
        {
            var token = context.GetBearerToken();

            if (ValidateHelper.IsEmpty(token))
                token = context.Request.Headers["auth.token"];

            return token;
        }

        /// <summary>
        /// 获取这个程序集中所用到的所有权限
        /// </summary>
        public static Dictionary<Type, string[]> ScanAllAssignedPermissionOnThisAssembly(this Assembly ass)
        {
            var data = new Dictionary<Type, string[]>();

            var tps = ass.GetTypes().Where(x => x.IsNormalClass() && x.IsAssignableTo_<ControllerBase>()).ToArray();
            foreach (var t in tps)
            {
                var attrs = t.GetMethods()
                    .Where(x => x.IsPublic)
                    .Select(x => x.GetCustomAttribute<AuthAdminAttribute>())
                    .Where(x => x != null);

                var pers = attrs.SelectMany(x => x.Permission?.Split(',') ?? new string[] { })
                    .WhereNotEmpty().ToArray();

                if (ValidateHelper.IsNotEmpty(pers))
                {
                    data[t] = pers;
                }
            }

            return data;
        }

        public static IApplicationBuilder UseUndefinedPermission(this IApplicationBuilder app, Assembly[] ass)
        {
            ass.Should().NotBeNullOrEmpty();
            var data = ass.SelectMany(x => x.ScanAllAssignedPermissionOnThisAssembly().Values.SelectMany(d => d)).ToArray();

            app.Map("/permission-define", option =>
            {
                option.Run(async context =>
                {
                    var all_permissions = context.RequestServices.Resolve_<IPermissionService>().AllPermissions();

                    var not_define = data.Except(all_permissions).ToJson();

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(not_define);
                });
            });

            return app;
        }
    }
}
