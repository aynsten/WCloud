using Lib.helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Lib.extension
{
    /// <summary>
    /// 对Ado的扩展
    /// </summary>
    public static class AdoExtension
    {
        /// <summary>
        /// 如果没有打开链接就打开链接
        /// </summary>
        public static IDbConnection OpenIfClosedWithRetry(this IDbConnection con, int retryCount = 1)
        {
            var retry = retryCount;
            while (true)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    break;
                }
                catch when (--retry > 0)
                {
                    //捕捉到错误，进入下次尝试
                }
            }
            return con;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="con"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IDbTransaction StartTransaction(this IDbConnection con, IsolationLevel? level = null)
        {
            if (level == null)
            {
                return con.BeginTransaction();
            }
            else
            {
                return con.BeginTransaction(level.Value);
            }
        }

        /// <summary>
        /// 复制参数
        /// </summary>
        public static T[] CloneParams<T>(IEnumerable<T> list) where T : DbParameter
            => Com.CloneParams(list.ToList());

        [Obsolete("强烈推荐使用Dapper")]
        public static T CurrentModel<T>(this IDataReader reader) where T : class, new()
        {
            return MapperHelper.GetModelFromReader<T>(reader);
        }

        /// <summary>
        /// 读取list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        [Obsolete("强烈推荐使用Dapper")]
        public static List<T> WholeList<T>(this IDataReader reader) where T : class, new()
        {
            return MapperHelper.GetListFromReader<T>(reader);
        }

        /// <summary>
        /// 获取Json（测试可用）
        /// </summary>
        public static string GetJson(this IDataReader reader)
        {
            var fields = new List<string>();
            for (int i = 0; i < reader.FieldCount; ++i)
            {
                fields.Add(reader.GetName(i));
            }

            var arr = new JArray();
            while (reader.Read())
            {
                var jo = new JObject();
                fields.ForEach(x =>
                {
                    var val = JToken.FromObject(reader[x]);
                    jo[x] = val;
                });

                arr.Add(jo);
            }
            return arr.ToString();
        }

        /// <summary>
        /// 转为实体
        /// </summary>
        public static List<T> ToEntityList_<T>(this IDataReader reader)
        {
            return reader.GetJson().JsonToEntity<List<T>>();
        }

        /// <summary>
        /// 多种方式绑定参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        [Obsolete("强烈推荐使用Dapper")]
        public static void AddParamsSmartly(this IDbCommand cmd, params object[] parameters)
        {
            if (ValidateHelper.IsEmpty(parameters)) { return; }

            if (parameters.All(x => x is IDataParameter))
            {
                parameters.ToList().ForEach(x => cmd.Parameters.Add(x));
            }
            else if (parameters.Length == 1)
            {
                var p = parameters[0];
                var dict = Com.ObjectToSqlParamsDict(p);
                foreach (var key in dict.Keys)
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"@{key}";
                    param.Value = dict[key];
                    cmd.Parameters.Add(param);
                }
            }
            else
            {
                throw new ArgumentException("只能有一个object参数");
            }
        }

        /// <summary>
        /// AddParameters
        /// </summary>
        /// <param name="command"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [Obsolete("添加参数")]
        public static void AddParameters(this IDbCommand command, string key, object value)
        {
            var p = command.CreateParameter();
            p.ParameterName = key;
            p.Value = value ?? System.DBNull.Value;
            command.Parameters.Add(p);
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("强烈推荐使用Dapper")]
        public static int ExecuteSql(this IDbConnection con, string sql, params object[] parameters)
        {
            var count = 0;
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.AddParamsSmartly(parameters);
                count = cmd.ExecuteNonQuery();
            }
            return count;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="con"></param>
        /// <param name="procedure_name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("强烈推荐使用Dapper")]
        public static int ExecuteProcedure(this IDbConnection con, string procedure_name, params object[] parameters)
        {
            var count = 0;
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedure_name;
                cmd.AddParamsSmartly(parameters);
                count = cmd.ExecuteNonQuery();
            }
            return count;
        }

        /// <summary>
        /// 读取第一行第一个记录
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("强烈推荐使用Dapper")]
        public static object ExecuteScalar(this IDbConnection con, string sql, params object[] parameters)
        {
            object obj = null;
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.AddParamsSmartly(parameters);
                obj = cmd.ExecuteScalar();
            }
            return obj;
        }


        /// <summary>
        /// 用reader读取list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("强烈推荐使用Dapper")]
        public static List<T> GetList<T>(this IDbConnection con, string sql, params object[] parameters) where T : class, new()
        {
            List<T> list = null;
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.AddParamsSmartly(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    list = reader.WholeList<T>();
                }
            }
            return list;
        }

        /// <summary>
        /// 读取reader的json格式（测试可用）
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("强烈推荐使用Dapper")]
        public static string GetJson(this IDbConnection con, string sql, params object[] parameters)
        {
            var json = string.Empty;
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.AddParamsSmartly(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    json = reader.GetJson();
                }
            }
            return json;
        }

        /// <summary>
        /// 读取reader第一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("强烈推荐使用Dapper")]
        public static T GetFirst<T>(this IDbConnection con, string sql, params object[] parameters) where T : class, new()
        {
            T model = default;
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.AddParamsSmartly(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader != null && reader.Read())
                    {
                        model = reader.CurrentModel<T>();
                    }
                }
            }
            return model;
        }

    }
}
