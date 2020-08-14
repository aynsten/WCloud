using Lib.helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCloud.Member.Domain.Admin
{
    /// <summary>
    /// 分页查询管理员的参数
    /// </summary>
    public class QueryAdminParameter
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Keyword { get; set; }
        public int? IsDeleted { get; set; }
    }

    public interface IAdminRepository : IMemberRepository<AdminEntity>
    {
        Task<PagerData<AdminEntity>> QueryUserList(string name = null, string email = null, string keyword = null, int? isremove = null, int page = 1, int pagesize = 20);
        Task<PagerData<AdminEntity>> QueryAdmin(QueryAdminParameter filter, int page, int pagesize);
        Task<IEnumerable<AdminEntity>> LoadRoles(IEnumerable<AdminEntity> list);
        Task<List<AdminEntity>> QueryTopUser(string q, string[] role_uid, int size);
    }
}
