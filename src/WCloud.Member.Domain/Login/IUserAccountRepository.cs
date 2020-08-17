using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Domain.Login
{
    public interface IUserAccountRepository : IMemberRepository<UserEntity>
    {
        Task RecoverByIdAsync(string[] uids);
        Task RemoveByIdAsync(string[] uids);
        Task<bool> IsUserNameExist(string user_name);
        Task<IEnumerable<UserPhoneEntity>> GetUserPhone(string user_uid);
        Task AddVadlidationCode(ValidationCodeEntity model);
        Task<UserEntity> GetUserByPhone(string phone);
        Task<ExternalLoginMapEntity> FindExternalLoginByOpenID(string provider, string openid);
        Task<_<ExternalLoginMapEntity>> SaveExternalProviderMapping(ExternalLoginMapEntity model);
        Task<ExternalLoginMapEntity> FindExternalLoginByUserID(string provider, string user_uid);
        Task RemoveExternalLogin(string user_uid, string[] provider);
        Task<bool> IsPhoneExist(string phone);
        Task<_<UserPhoneEntity>> SetPhone(string uid, string phone);
        Task<ValidationCodeEntity> GetValidationCode(Expression<Func<ValidationCodeEntity, bool>> where);
    }
}