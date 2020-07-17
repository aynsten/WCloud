using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace WCloud.Framework.Database.Abstractions
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBTypeEnum : int
    {
        None = -1,
        MySQL = 1,
        SqlServer = 2,
        Oracle = 3,
        DB2 = 4,
        PostgreSQL = 5,
        Sqlite = 6
    }

    /// <summary>
    /// 获取数据库链接
    /// </summary>
    public static class DBHelper
    {
        /// <summary>
        /// c#类型转换为dbtype
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DbType ConvertToDbType<T>()
        {
            return ConvertToDbType(typeof(T));
        }

        /// <summary>
        /// c#类型转换为dbtype
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DbType ConvertToDbType(Type t)
        {
            if (!Type2DBTypeMapper.ContainsKey(t))
                throw new NotSupportedException($"{Type2DBTypeMapper}:不支持的类型转换");

            return Type2DBTypeMapper[t];
        }

        /// <summary>
        /// type和dbtype的映射表
        /// </summary>
        public static readonly ReadOnlyDictionary<Type, DbType> Type2DBTypeMapper = new ReadOnlyDictionary<Type, DbType>(new Dictionary<Type, DbType>()
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            [typeof(TimeSpan?)] = DbType.Time,
            [typeof(object)] = DbType.Object
        });
    }
}
