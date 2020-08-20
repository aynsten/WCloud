using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using WCloud.Core;

namespace WCloud.Member.Authentication
{
    public static class AuthenticationStartup
    {
        [Obsolete("for test")]
        static IServiceCollection AddJwtAuthentication(this IServiceCollection collection)
        {
            var scheme = ConfigSet.Identity.UserLoginScheme;
            var authBuilder = collection.AddAuthentication(option =>
            {
                option.DefaultScheme = scheme;
            });

            authBuilder.AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = "http://localhost:5000",//Audience
                    ValidIssuer = "http://localhost:5000",//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123456789123456789123456789"))//拿到SecurityKey
                };
            });

            return collection;
        }

        /// <summary>
        /// 使用oauth服务器颁发的token
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        static IServiceCollection AddIdentityServerTokenAuthentication(this IServiceCollection collection, IConfiguration config)
        {
            collection.AddTransient<OAuthConfig>();

            var scheme = ConfigSet.Identity.UserLoginScheme;
            var identity_server = config.GetIdentityServerAddressOrThrow();

            var authBuilder = collection.AddAuthentication(option =>
            {
                option.DefaultScheme = scheme;
            });
            authBuilder.AddIdentityServerAuthentication(scheme, option =>
            {
                option.Authority = identity_server;
                option.ApiName = "water";
                //只有使用reference token才需要配置api secret
                option.ApiSecret = "456";
                option.RequireHttpsMetadata = false;
            });
            return collection;
        }

        public static IServiceCollection AddIdentityServerTokenValidation(this IServiceCollection collection, IConfiguration config)
        {
            AddIdentityServerTokenAuthentication(collection, config);
            return collection;
        }

        /// <summary>
        /// 使用cookie验证
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        static IServiceCollection AddAdminCookieLoginAuthentication(this IServiceCollection collection, Action<AuthenticationBuilder> builder = null)
        {
            var scheme = ConfigSet.Identity.AdminLoginScheme;

            var authBuilder = collection.AddAuthentication(option =>
            {
                option.DefaultScheme = scheme;
            });
            authBuilder.AddCookie(scheme, option =>
            {
                option.ExpireTimeSpan = TimeSpan.FromDays(30);
                option.Cookie.Name = $"admin_cookie_{scheme}";
                //这个属性在core 3.x中不能用了
                //option.Cookie.Expiration = option.ExpireTimeSpan; 
                option.Cookie.MaxAge = option.ExpireTimeSpan;
            });

            builder?.Invoke(authBuilder);

            return collection;
        }

        public static IServiceCollection AddAdminAuth(this IServiceCollection collection)
        {
            AddAdminCookieLoginAuthentication(collection);

            return collection;
        }

        /// <summary>
        /// 配置identity server基于cookie的登陆验证
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddOAuthServerCookieAuthentication_(this IServiceCollection collection)
        {
            var cookie_scheme = ConfigSet.Identity.UserLoginScheme;
            var cookie_external_scheme = ConfigSet.Identity.ExternalLoginScheme;

            var authBuilder = collection.AddAuthentication(option =>
            {
                option.DefaultScheme = cookie_scheme;
            });
            authBuilder.AddCookie(cookie_scheme, option =>
            {
                option.Cookie.Name = $"ids_web_login_{cookie_scheme}";
                option.ExpireTimeSpan = TimeSpan.FromDays(30);
                option.LoginPath = "/Account/Login";
                option.LogoutPath = "/Account/Logout";
            })
            .AddCookie(cookie_external_scheme, option =>
            {
                option.Cookie.Name = $"ids_web_external_login_{cookie_external_scheme}";
                //三方登陆的信息只保存10分钟，请尽快引导用户登陆自己账号体系的账号
                option.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            });
            return authBuilder;
        }
    }
}