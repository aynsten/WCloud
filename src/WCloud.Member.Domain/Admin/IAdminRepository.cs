using Lib.helper;
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
        Task<PagerData<AdminEntity>> QueryAdmin(QueryAdminParameter filter, int page, int pagesize);
    }
}
