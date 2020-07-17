using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Framework.Database.Dapper
{
    /// <summary>
    /// 针对不同数据库的sql方言
    /// </summary>
    public interface ISqlDialect
    {
        string SelectFields(string[] fields);

        string OrderBy(IDictionary<string, string> sort);

        /// <summary>
        /// 比较有几种情况
        /// 字段和值比较
        /// 字段和字段比较
        /// 值和值比较
        /// </summary>
        /// <param name="field_1"></param>
        /// <param name="_operator"></param>
        /// <param name="field_2"></param>
        /// <returns></returns>
        string Compare(string field_1, string _operator, string field_2);

        string Limit(int? skip, int? take);
    }
}
