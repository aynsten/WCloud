using FluentAssertions;
using WCloud.Core.Cache;
using Lib.extension;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WCloud.Identity.Providers
{
    public class IdentityServerDistributedCache<T> : IdentityServer4.Services.ICache<T> where T : class
    {
        private readonly ILogger _logger;
        private readonly ICacheProvider _cache;

        public IdentityServerDistributedCache(ILogger<IdentityServerDistributedCache<T>> logger, ICacheProvider cache)
        {
            this._logger = logger;
            this._cache = cache;
        }

        public async Task<T> GetAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();
            try
            {
                var res = await this._cache.GetAsync_<T>(key);
                if (!res.Success)
                {
                    this._logger.LogDebug($"ids read cache error:{key}");
                }
                return res.Data;
            }
            catch (Exception e)
            {
                this._logger.AddErrorLog(e.Message, e);
                return default;
            }
        }

        public async Task SetAsync(string key, T item, TimeSpan expiration)
        {
            key.Should().NotBeNullOrEmpty();
            await this._cache.SetAsync_(key, item, expiration);
        }
    }
}
