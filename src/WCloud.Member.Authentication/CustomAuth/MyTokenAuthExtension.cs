using WCloud.Member.Application.Service;
using Lib.helper;
using Lib.ioc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WCloud.Member.Authentication.CustomAuth
{
    public static class MyTokenAuthExtension
    {
        public const string IDENTITY_TOKEN_KEY = "__identity_token_key__";

        public static string GetToken(this ClaimsPrincipal principal)
        {
            return principal.Claims?.FirstOrDefault(x => x.Type == IDENTITY_TOKEN_KEY)?.Value;
        }

        public static ClaimsIdentity SetToken(this ClaimsIdentity identity, string token)
        {
            identity.SetOrReplaceClaim_(IDENTITY_TOKEN_KEY, token);
            return identity;
        }

        public static async Task<string> TokenLoginAsync(this HttpContext context, string scheme, string subject_id)
        {
            if (ValidateHelper.IsEmpty(scheme))
                throw new ArgumentNullException(nameof(scheme));

            if (ValidateHelper.IsEmpty(subject_id))
                throw new ArgumentNullException(nameof(subject_id));

            var tokenService = context.RequestServices.Resolve_<IAuthTokenService>();

            var token = await tokenService.CreateAccessTokenAsync(subject_id);

            var identity = new ClaimsIdentity();
            identity.SetSubjectID(subject_id).SetToken(token.AccessToken);

            var principal = new ClaimsPrincipal(identity: identity);

            await context.SignInAsync(scheme: scheme, principal: principal);

            return token.AccessToken;
        }
    }
}
