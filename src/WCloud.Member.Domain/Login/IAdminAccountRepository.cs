using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Domain.Login
{
    public interface IAdminAccountRepository : IMemberRepository<AdminEntity>
    {
        Task RecoverByIdAsync(string[] uids);
        Task RemoveByIdAsync(string[] uids);
        Task<bool> IsUserNameExist(string user_name);
    }
}
