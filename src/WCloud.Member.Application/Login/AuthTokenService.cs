using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Login;

namespace WCloud.Member.Application.Service.impl
{
    public class AuthTokenService : IAuthTokenService
    {
        protected readonly ICacheProvider _cache;
        protected readonly ICacheKeyManager _keyManager;
        protected readonly IMSRepository<AuthTokenEntity> _tokenRepo;

        protected virtual int TokenExpireDays => 30;

        public AuthTokenService(
            ICacheProvider _cache,
            ICacheKeyManager _keyManager,
            IMSRepository<AuthTokenEntity> _tokenRepo)
        {
            this._cache = _cache;
            this._keyManager = _keyManager;
            this._tokenRepo = _tokenRepo;
        }

        protected virtual TokenModel Parse(AuthTokenEntity token) => new TokenModel()
        {
            UserUID = token.UserUID,
            AccessToken = token.Id,

            ExtData = token.ExtData,

            ExpireUtc = token.ExpiryTimeUtc,
        };

        /// <summary>
        /// 剩余过期时间小于总有效时间的一半就自动刷新
        /// </summary>
        /// <param name="token"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        protected virtual bool NeedRefresh(AuthTokenEntity token, DateTime now) =>
            Math.Abs((token.ExpiryTimeUtc - now).TotalDays) < (this.TokenExpireDays / 2.0);

        public virtual async Task<TokenModel> CreateAccessTokenAsync(string user_uid)
        {
            user_uid.Should().NotBeNullOrEmpty("create access token subject id");

            var now = DateTime.UtcNow;

            //create new token
            var token = new AuthTokenEntity()
            {
                ExpiryTimeUtc = now + TimeSpan.FromDays(this.TokenExpireDays),
                RefreshToken = Com.GetUUID(),
                UserUID = user_uid
            }.InitSelf("token");

            if (!token.IsValid(out var msg))
                throw new MsgException(msg);

            await this._tokenRepo.InsertAsync(token);

            var token_data = this.Parse(token);

            return token_data;
        }

        public virtual async Task<TokenModel> GetUserIdByTokenAsync(string access_token)
        {
            access_token.Should().NotBeNullOrEmpty("get user id by token,token");

            var now = DateTime.UtcNow;
            var token = await this._tokenRepo.QueryOneAsync(x => x.Id == access_token);

            if (token == null || token.ExpiryTimeUtc < now)
                return null;

            //自动刷新过期时间
            if (this.NeedRefresh(token, now))
                await this.RefreshToken(token.Id);

            var token_data = this.Parse(token);

            return token_data;
        }

        public virtual async Task RemoveCacheAsync(CacheBundle data)
        {
            data.Should().NotBeNull("remove token cache data");

            var keys = new List<string>();

            keys.AddList_(data.TokenUID?.Select(x => this._keyManager.AuthToken(x)));
            keys.AddList_(data.UserUID?.Select(x => this._keyManager.UserInfo(x)));

            foreach (var key in keys.Where(x => x?.Length > 0).Distinct())
            {
                await this._cache.RemoveAsync(key);
            }
        }

        public async Task DeleteUserTokensAsync(string user_uid)
        {
            user_uid.Should().NotBeNullOrEmpty("delete user tokens,user uid");

            var batch_count = 300;

            while (true)
            {
                var list = await this._tokenRepo.QueryManyAsync(x => x.UserUID == user_uid, count: batch_count);
                var token_uids = list.Select(x => x.Id).Distinct().ToArray();
                if (!token_uids.Any())
                    return;

                await this.DeleteTokensAsync(token_uids);
            }
        }

        public async Task DeleteTokensAsync(string[] token_uids)
        {
            token_uids.Should().NotBeNullOrEmpty("delete tokens token uids");

            var db = this._tokenRepo.Database;

            var token_query = db.Set<AuthTokenEntity>();

            token_query.RemoveRange(token_query.Where(x => token_uids.Contains(x.Id)));

            await db.SaveChangesAsync();

            foreach (var token in token_uids)
            {
                await this._cache.RemoveAsync(this._keyManager.AuthToken(token));
            }
        }

        public async Task RefreshToken(string access_token)
        {
            access_token.Should().NotBeNullOrEmpty("refresh token,token");

            var now = DateTime.UtcNow;
            var token = await this._tokenRepo.QueryOneAsync(x => x.Id == access_token);
            if (token == null || token.ExpiryTimeUtc < now)
                return;

            //更新过期时间
            token.ExpiryTimeUtc = now.AddDays(this.TokenExpireDays);
            token.UpdateTimeUtc = now;
            token.RefreshTimeUtc = now;

            await this._tokenRepo.UpdateAsync(token);
        }
    }
}
