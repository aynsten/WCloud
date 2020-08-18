using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.Login;

namespace WCloud.Member.DataAccess.EF.Login
{
    public class AdminAccountRepository : MemberShipRepository<AdminEntity>, IAdminAccountRepository
    {
        private readonly IWCloudContext _context;
        public AdminAccountRepository(IWCloudContext<AdminAccountRepository> _context) : base(_context.Provider)
        {
            this._context = _context;
        }

        public async Task<bool> IsUserNameExist(string user_name)
        {
            var query = this.NoTrackingQueryable.IgnoreQueryFilters().Where(x => x.UserName == user_name);
            var res = await query.AnyAsync();
            return res;
        }

        public async Task RecoverByIdAsync(string[] uids)
        {
            await RepositoryExtension.RecoverByIdAsync(this, uids);
        }

        public async Task RemoveByIdAsync(string[] uids)
        {
            await RepositoryExtension.RemoveByIdAsync(this, uids);
        }
    }
}
