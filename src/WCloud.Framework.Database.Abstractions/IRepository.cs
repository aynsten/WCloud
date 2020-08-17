using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lib.data;

namespace WCloud.Framework.Database.Abstractions
{
    public interface ILinqRepository<T> : IQueryableRepository<T>, IRepository<T> where T : IDBTable
    {
        //
    }

    public interface IQueryableRepository<T> : IDisposable where T : IDBTable
    {
        IQueryable<T> Queryable { get; }
    }

    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IDisposable where T : IDBTable
    {
        #region 添加
        /// <summary>
        /// 添加多个model
        /// </summary>
        int Insert(T model);

        /// <summary>
        /// 异步添加
        /// </summary>
        Task<int> InsertAsync(T model);
        #endregion

        #region 删除
        /// <summary>
        /// 删除多个model
        /// </summary>
        int Delete(T model);

        /// <summary>
        /// 异步删除 
        /// </summary>
        Task<int> DeleteAsync(T model);

        /// <summary>
        /// 按照条件删除
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int DeleteWhere(Expression<Func<T, bool>> where);

        /// <summary>
        /// 按照条件删除
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where);
        #endregion

        #region 修改
        /// <summary>
        /// 更新多个model
        /// </summary>
        int Update(T model);

        /// <summary>
        /// 异步更新
        /// </summary>
        Task<int> UpdateAsync(T model);

        #endregion

        #region 查询

        /// <summary>
        /// 获取list
        /// </summary>
        T[] QueryMany<OrderByColumn>(Expression<Func<T, bool>> where,
            int? count = null, int? skip = null,
            Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true);

        /// <summary>
        /// 异步获取list
        /// </summary>
        Task<T[]> QueryManyAsync<OrderByColumn>(Expression<Func<T, bool>> where,
            int? count = null, int? skip = null,
            Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true);

        /// <summary>
        /// 查询第一个
        /// </summary>
        T QueryOne(Expression<Func<T, bool>> where);

        /// <summary>
        /// 异步获取第一个
        /// </summary>
        Task<T> QueryOneAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 查询记录数（判断记录是否存在请使用Exist方法，效率更高）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int QueryCount(Expression<Func<T, bool>> where);

        /// <summary>
        /// 异步获取count
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<int> QueryCountAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 查询是否存在 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Exist(Expression<Func<T, bool>> where);

        /// <summary>
        /// 异步查询是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(Expression<Func<T, bool>> where);
        #endregion
    }
}
