using WCloud.Framework.Database.EntityFrameworkCore.Repository;
using WCloud.Member.Domain;

namespace WCloud.Member.DataAccess.EF
{
    /// <summary>
    /// 会员中心仓储接口
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// 这里不应该依赖lib.ef，应该剥离repo和efrepo
    /// 但是！！！不依赖ef，application层很多代码要重写，
    /// 只使用repo做数据操作不显示，约束很多
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    public interface IMSRepository<T> : IEFRepository<T> where T : class, IMemberShipDBTable
    {
        //
    }
}
