using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Framework.Socket.RegistrationCenter
{
    public class RedisRegistrationProvider : IRegistrationProvider
    {
        private readonly int expired_mins = 10;

        private readonly IRedisDatabaseSelector redisDatabaseSelector;
        private readonly IMessageSerializer messageSerializer;
        private readonly IRedisKeyManager redisKeyManager;
        public RedisRegistrationProvider(
            IRedisDatabaseSelector redisDatabaseSelector,
            IMessageSerializer messageSerializer,
            IRedisKeyManager redisKeyManager)
        {
            this.redisDatabaseSelector = redisDatabaseSelector;
            this.messageSerializer = messageSerializer;
            this.redisKeyManager = redisKeyManager;
        }

        /// <summary>
        /// userid=device:data
        /// groupid=serverinstanceid:data
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task RegisterUserInfo(UserRegistrationInfo info)
        {
            info.Should().NotBeNull();
            info.UserID.Should().NotBeNullOrEmpty();
            info.DeviceType.Should().NotBeNullOrEmpty();
            info.Payload.Should().NotBeNull();

            var data = this.messageSerializer.SerializeBytes(info.Payload);

            var user_reg_key = this.redisKeyManager.UserRegInfoKey(info.UserID);
            var device_reg_key = this.redisKeyManager.UserDeviceRegHashKey(info.DeviceType);

            var db = this.redisDatabaseSelector.Database;

            await db.HashSetAsync(user_reg_key, device_reg_key, data);

            //五分钟没有心跳就删除key
            await db.KeyExpireAsync(user_reg_key, TimeSpan.FromMinutes(this.expired_mins));
        }

        public async Task RegisterGroupInfo(GroupRegistrationInfo info)
        {
            info.Should().NotBeNull();
            info.Payload.Should().NotBeNull();

            var key = this.redisKeyManager.GroupRegInfoKey(info.GroupUID);
            var hash_key = this.redisKeyManager.GroupServerHashKey(info.ServerInstance);

            var db = this.redisDatabaseSelector.Database;

            var data = this.messageSerializer.SerializeBytes(info.Payload);
            await db.HashSetAsync(key, hash_key, data);

            //五分钟没有心跳就删除key
            await db.KeyExpireAsync(key, TimeSpan.FromMinutes(this.expired_mins));
        }

        public async Task<string[]> GetUserServerInstances(string user_uid)
        {
            user_uid.Should().NotBeNullOrEmpty();

            var key = this.redisKeyManager.UserRegInfoKey(user_uid);

            var db = this.redisDatabaseSelector.Database;

            var entry = await db.HashGetAllAsync(key);

            var data = entry.Select(x => this.messageSerializer.Deserialize<UserRegistrationInfoPayload>((byte[])x.Value)).ToArray();

            var reg_time_expired_ = DateTime.UtcNow.AddMinutes(-this.expired_mins);

            var server_instance_id_arr = data.Where(x => x.PingTimeUtc > reg_time_expired_).Select(x => x.ServerInstanceID).Distinct().ToArray();

            return server_instance_id_arr;
        }

        public async Task RemoveRegisterInfo(string user_uid, string device)
        {
            var user_reg_key = this.redisKeyManager.UserRegInfoKey(user_uid);
            var device_reg_key = this.redisKeyManager.UserDeviceRegHashKey(device);

            await this.redisDatabaseSelector.Database.HashDeleteAsync(user_reg_key, device_reg_key);
        }
    }
}
