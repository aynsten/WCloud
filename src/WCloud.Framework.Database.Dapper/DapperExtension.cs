using Dapper;
using Lib.core;
using Lib.core;
using Lib.extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace WCloud.Framework.Database.Dapper
{
    public static class DapperExtension
    {
        /// <summary>
        /// HowToUseParametersInDapperFramework
        /// </summary>
        /// <param name="con"></param>
        public static DynamicParameters ToDapperParameters(this IDictionary<string, object> dict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));

            var args = new DynamicParameters(new { });
            foreach (var kv in dict)
            {
                args.Add(name: kv.Key, value: kv.Value);
            }
            return args;
        }

        /// <summary>
        /// 把dbparameter转换为dapper的参数（测试可用）
        /// </summary>
        /// <param name="con"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        public static DynamicParameters ToDapperParameters(this IEnumerable<DbParameter> dbParameters)
        {
            if (dbParameters == null)
                throw new ArgumentNullException(nameof(dbParameters));

            if (dbParameters.GroupBy(x => x.ParameterName).Any(x => x.Count() > 1))
            {
                throw new ArgumentException("存在重复参数");
            }
            var dict = dbParameters.ToDictionary(x => x.ParameterName, x => x.Value);
            return DapperExtension.ToDapperParameters(dict: dict);
        }

        /// <summary>
        /// 插入数据（使用System.ComponentModel.DataAnnotations.Schema配置数据表映射）
        /// </summary>
        /// <param name="con"></param>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Insert_<T>(this IDbConnection con, T model,
            IDbTransaction transaction = null, int? commandTimeout = default) where T : IDBTable
        {
            var sql = model.GetInsertSql();
            try
            {
                return con.Execute(sql, model, transaction: transaction, commandTimeout: commandTimeout);
            }
            catch (Exception e)
            {
                throw new Exception($"无法执行SQL:{sql}", e);
            }
        }

        /// <summary>
        /// 通过主键更新数据，自动生成字段和主键不更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Update_<T>(this IDbConnection con, T model,
            IDbTransaction transaction = null, int? commandTimeout = default) where T : IDBTable
        {
            var sql = model.GetUpdateSql();
            try
            {
                return con.Execute(sql, model, transaction: transaction, commandTimeout: commandTimeout);
            }
            catch (Exception e)
            {
                throw new Exception($"无法执行SQL:{sql}", e);
            }
        }

        public static T FindByPrimaryKey_<T>(this IDbConnection con, object[] keys,
            IDbTransaction transaction = null, int? commandTimeout = default)
        {
            var structure = typeof(T).GetTableStructure();
            if (keys.Length != structure.keys.Count)
                throw new Exception("传入主键数量和数据表不一致");
            var where = string.Join(" AND ", structure.keys.Select(x => $"{x.Key}=@{x.Value}"));

            var sql = $"SELECT * FROM {structure.table_name} WHERE {where}";

            var param_dict = new Dictionary<string, object>();
            var index = 0;
            foreach (var row in structure.keys.AsTupleEnumerable())
            {
                param_dict[row.value] = keys[index++];
            }

            return con.Query<T>(sql, param_dict.ToDapperParameters(),
                transaction: transaction, commandTimeout: commandTimeout).FirstOrDefault();
        }
    }
}
