using System;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;
using WCloud.Member.Domain;

namespace WCloud.Member.DataAccess.EF
{
    /// <summary>
    /// 会员中心仓储实现
    /// </summary>
    public class MemberShipRepository<T> : WCloudEFRepository<T, MemberShipDbContext>, IMSRepository<T>
        where T : BaseEntity, IMemberShipDBTable
    {
        public MemberShipRepository(IServiceProvider provider) : base(provider) { }
    }
}
