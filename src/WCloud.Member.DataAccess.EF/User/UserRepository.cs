using Microsoft.EntityFrameworkCore;
using System.Linq;
using WCloud.Core;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;

namespace WCloud.Member.DataAccess.EF.User
{
    public class UserRepository : MemberShipRepository<UserEntity>, IUserRepository
    {
        private readonly IWCloudContext _context;
        public UserRepository(IWCloudContext<UserRepository> _context) : base(_context.Provider)
        {
            this._context = _context;
        }

        public IQueryable<UserPhoneEntity> UserPhoneQueryable => this.Database.Set<UserPhoneEntity>().AsNoTracking();
    }
}
