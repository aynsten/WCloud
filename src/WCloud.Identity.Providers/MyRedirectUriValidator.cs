using IdentityServer4.Models;
using IdentityServer4.Validation;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Identity.Providers
{
    //class dd : IdentityServer4.Validation.IApiSecretValidator { }

    public class MyDevRedirectUriValidator : IRedirectUriValidator
    {
        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(true);
        }
    }

    public class MyProductionRedirectUriValidator : IRedirectUriValidator
    {
        private readonly IConfiguration configuration;
        private readonly string[] _allowed_domains;
        public MyProductionRedirectUriValidator(IConfiguration configuration)
        {
            this.configuration = configuration;

            var domain_config = this.configuration["allowed_redirect_url_domains"] ?? string.Empty;
            this._allowed_domains = domain_config.Split(',').WhereNotNull().Distinct().ToArray();
        }

        public async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (ValidateHelper.IsEmpty(requestedUri))
            {
                return false;
            }

            var urls = client?.PostLogoutRedirectUris ?? new string[] { };
            if (urls.Contains(requestedUri))
            {
                return true;
            }

            if (this._allowed_domains.Any())
            {
                if (this._allowed_domains.Any(x => requestedUri.Contains(x)))
                {
                    return true;
                }
            }

            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (ValidateHelper.IsEmpty(requestedUri))
            {
                return false;
            }

            var urls = client?.RedirectUris ?? new string[] { };
            if (urls.Contains(requestedUri))
            {
                return true;
            }

            if (this._allowed_domains.Any())
            {
                if (this._allowed_domains.Any(x => requestedUri.Contains(x)))
                {
                    return true;
                }
            }

            await Task.CompletedTask;
            return false;
        }
    }
}
