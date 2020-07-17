using Microsoft.Extensions.Configuration;

namespace WCloud.Member.Authentication
{
    public class OAuthConfig
    {
        private readonly IConfiguration _config;
        public OAuthConfig(IConfiguration configuration)
        {
            this._config = configuration;
        }

        string get_config(string key) => this._config[$"OAuth:{key}"];

        public string ClientId => this.get_config("client_id");

        public string ClientSecret => this.get_config("client_secret");

        public string Scope => this.get_config("scope");
    }
}
