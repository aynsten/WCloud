using Lib.helper;
using Lib.ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Application.Service
{
    public interface IUserService : IBasicService<UserEntity>, IAutoRegistered
    {
        Task<_<UserEntity>> SetIdCard(string user_uid, string idcard, string real_name);

        Task<IEnumerable<UserEntity>> LoadUserPhone(IEnumerable<UserEntity> data);

        Task<IEnumerable<UserEntity>> GetTopMatchedUsers(string keyword, int max_count);

        Task<UserEntity> GetUserByUID(string uid);

        Task<PagerData<UserEntity>> QueryUserList(string keyword = null, bool? isremove = null, int page = 1, int pagesize = 20);

        Task<_<UserEntity>> UpdateUser(UserEntity model);

        Task<_<UserEntity>> UpdateUserAvatar(string user_uid, string avatar_url);

        Task<UserEntity> GetUserByUserName(string name);
    }
}
