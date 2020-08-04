using System;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace WCloud.MetroAd
{
    public interface IMetroAdDbFactory : IAutoRegistered
    {
        IDbConnection GetMetroAdDatabase();
    }

    public class MetroAdDatabaseFactory : IMetroAdDbFactory
    {
        private readonly IConfiguration _config;

        public MetroAdDatabaseFactory(IServiceProvider provider)
        {
            this._config = provider.ResolveConfig_();
        }

        public IDbConnection GetMetroAdDatabase()
        {
            var constr = this._config.GetMetroAdConnectionStringOrThrow();

            var con = new MySqlConnection(constr);
            con.Open();

            return con;
        }
    }
}
