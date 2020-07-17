using System;
using WCloud.Member.Application.Login;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.Application.Service.impl
{
    public class AdminLoginService : LoginServiceBase<AdminEntity>,IAdminLoginService
    {
        public AdminLoginService(IServiceProvider provider,
            IPasswordHelper passHelper,
            IMobilePhoneFormatter _mobileFormatter,
            IMSRepository<AdminEntity> _userRepo) :
            base(provider,passHelper, _mobileFormatter, _userRepo)
        {
            //
        }
    }
}
