using Lib.data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WCloud.Framework.Database.Abstractions
{
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
        int Add(T model);

        /// <summary>
        /// 异步添加
        /// </summary>
        Task<int> AddAsync(T model);
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
        /// expression和func的使用注意点，参见lib的readme
        /// </summary>
        /// <param name="where">where条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">读取条数</param>
        /// <param name="Desc">正序反序</param>
        /// <returns></returns>
        List<T> QueryList<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null,
            bool Desc = true,
            int? start = null,
            int? count = null);

        /// <summary>
        /// 异步查询
        /// </summary>
        /// <typeparam name="OrderByColumnType"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="desc"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<T>> QueryListAsync<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null,
            bool desc = true,
            int? start = null,
            int? count = null);

        /// <summary>
        /// 获取list
        /// </summary>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<T> GetList(Expression<Func<T, bool>> where, int? count = null);

        /// <summary>
        /// 异步获取list
        /// </summary>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> where, int? count = null);

        /// <summary>
        /// 查询第一个
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        T GetFirst(Expression<Func<T, bool>> where);

        /// <summary>
        /// 异步获取第一个
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<T> GetFirstAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 查询记录数（判断记录是否存在请使用Exist方法，效率更高）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int GetCount(Expression<Func<T, bool>> where);

        /// <summary>
        /// 异步获取count
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(Expression<Func<T, bool>> where);

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
