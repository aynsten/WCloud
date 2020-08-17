using Lib.data;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Database.Abstractions;

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
    public interface IMemberRepository<T> : ILinqRepository<T>, IAutoRegistered where T : class, IMemberShipDBTable
    {
        //
    }
}
