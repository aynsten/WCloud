using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Application.Service
{
    public interface IAdminService : IAutoRegistered
    {
        Task<AdminEntity> GetUserByUID(string uid);
        Task<PagerData<AdminDto>> QueryAdmin(QueryAdminParameter filter, int page, int pagesize);
        [Obsolete]
        Task<PagerData<AdminEntity>> QueryUserList(string name = null, string email = null, string keyword = null, int? isremove = null, int page = 1, int pagesize = 20);

        Task<List<AdminEntity>> QueryTopUser(string q = null, string[] role_uid = null, int size = 20);

        Task UpdateUser(AdminEntity model);

        Task<_<AdminEntity>> AddAdmin(AdminEntity model);

        Task<AdminEntity> GetUserByUserName(string name);

        Task<IEnumerable<AdminEntity>> LoadRoles(IEnumerable<AdminEntity> list);
    }
}
