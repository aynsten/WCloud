using Lib.data;
using WCloud.Framework.Database.Abstractions;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain
{
    /// <summary>
    /// 会员中心数据表约束
    /// </summary>
    public interface IMemberShipDBTable : IDBTable { }

    /// <summary>
    /// 基础crud和queryable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMemberRepository<T> : ILinqRepository<T>, IRepository<T> where T : EntityBase, IMemberShipDBTable
    {
        //
    }
}
