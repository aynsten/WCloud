using System;
using WCloud.Framework.Database.Abstractions.Entity;
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
    [Obsolete("使用通用repo")]
    public interface IMSRepository<T> : IEFRepository<T>, IMemberRepository<T> where T : class, IMemberShipDBTable
    {
        //
    }

    /// <summary>
    /// 会员中心仓储实现
    /// </summary>
    public class MemberShipRepository<T> : WCloudEFRepository<T, MemberShipDbContext>, IMemberRepository<T>, IMSRepository<T>
        where T : EntityBase, IMemberShipDBTable
    {
        public MemberShipRepository(IServiceProvider provider) : base(provider) { }
    }
}
