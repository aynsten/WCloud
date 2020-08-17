using System.Linq;
using WCloud.Member.Domain.Login;

namespace WCloud.Member.Domain.User
{
    public interface IUserRepository : IMemberRepository<UserEntity>
    {
        IQueryable<UserPhoneEntity> UserPhoneQueryable { get; }
    }
}
