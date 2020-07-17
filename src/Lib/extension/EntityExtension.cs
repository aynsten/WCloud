using Lib.core;
using Lib.data;
using Lib.extension;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Lib.extension
{
    public static class EntityExtension
    {

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static (string table_name, Dictionary<string, string> keys,
            Dictionary<string, string> auto_generated_columns, Dictionary<string, string> columns)
            GetTableStructure<T>(this T model) where T : IDBTable
        {
            return model.GetType().GetTableStructure();
        }

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static (string table_name, Dictionary<string, string> keys,
            Dictionary<string, string> auto_generated_columns, Dictionary<string, string> columns)
            GetTableStructure(this Type t)
        {
            var pps = t.GetColumnsProperties();
            //表名
            var table_name = t.GetTableName();

            //读取字段名和sql的placeholder
            (string column, string placeholder) GetColumn(PropertyInfo p) => p.GetColumnInfo();

            //主键
            var key_props = pps.Where(x => x.IsPrimaryKey()).ToList();
            if (ValidateHelper.IsEmpty(key_props))
            {
                throw new NoPrimaryKeyException("Model没有设置主键");
            }
            var keys = key_props.Select(x => GetColumn(x)).ToDictionary(x => x.column, x => x.placeholder);

            //自动生成的字段
            var auto_generated_props = pps.Where(x => x.IsGeneratedInDatabase()).ToList();
            var auto_generated_columns = auto_generated_props.Select(x => GetColumn(x)).ToDictionary(x => x.column, x => x.placeholder);

            //普通字段
            var column_props = pps.Where(x => !keys.Values.Contains(x.Name)).ToList();
            column_props = pps.Where(x => !auto_generated_columns.Values.Contains(x.Name)).ToList();
            if (ValidateHelper.IsEmpty(column_props))
            {
                throw new NoValidFieldsException("无法提取到有效字段");
            }
            var columns = column_props.Select(x => GetColumn(x)).ToDictionary(x => x.column, x => x.placeholder);

            return (table_name, keys, auto_generated_columns, columns);
        }

        /// <summary>
        /// 获取插入SQL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetInsertSql<T>(this T model) where T : IDBTable
        {
            var structure = model.GetTableStructure();

            var k = string.Join(",", structure.columns.Keys);
            var v = string.Join(",", structure.columns.Values.Select(x => $"@{x}"));
            var sql = $"INSERT INTO {structure.table_name} ({k}) VALUES ({v})";
            return sql;
        }

        /// <summary>
        /// 获取更新sql
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetUpdateSql<T>(this T model) where T : IDBTable
        {
            var structure = model.GetTableStructure();

            var set = string.Join(",", structure.columns.Select(x => $"{x.Key}=@{x.Value}"));
            var where = string.Join(" AND ", structure.keys.Select(x => $"{x.Key}=@{x.Value}"));
            var sql = $"UPDATE {structure.table_name} SET {set} WHERE {where}";
            return sql;
        }

        /// <summary>
        /// 这个字段是数据库自动生成
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsGeneratedInDatabase(this PropertyInfo prop)
        {
            return prop.GetCustomAttributes<DatabaseGeneratedAttribute>().Where(m => m.DatabaseGeneratedOption != DatabaseGeneratedOption.None).Any();
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetTableName(this Type t)
        {
            return t.GetCustomAttribute<TableAttribute>()?.Name ?? t.Name;
        }

        /// <summary>
        /// 获取字段对应的属性
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetColumnsProperties(this Type t)
        {
            return t.GetProperties().Where(x => x.CanRead && x.CanWrite).Where(x => !x.GetCustomAttributes<NotMappedAttribute>().Any());
        }

        /// <summary>
        /// 获取字段属性的信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static (string column, string placeholder) GetColumnInfo(this PropertyInfo p)
        {
            var column = p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name;
            var placeholder = p.Name;
            return (column, placeholder);
        }

        /// <summary>
        /// 是否是主键
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsPrimaryKey(this PropertyInfo p) => p.GetCustomAttributes<KeyAttribute>().Any();
    }
}
