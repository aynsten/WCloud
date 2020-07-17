using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System
{
    /// <summary>
    /// 是否
    /// </summary>
    public enum YesOrNoEnum : int
    {
        No = 0,
        YES = 1
    }

    /// <summary>
    /// 男女未知
    /// </summary>
    public enum SexEnum : int
    {
        Male = 1,
        FeMale = 0,
        Unknow = -1
    }

    /// <summary>
    /// 增删改查
    /// </summary>
    public enum CrudEnum : int
    {
        Add = 1 << 0,
        Delete = 1 << 1,
        Update = 1 << 2,
        Query = 1 << 3
    }

    /// <summary>
    /// 平台
    /// </summary>
    public enum PlatformEnum : int
    {
        [Description("未知")]
        Unknow = 0,
        IOS = 1,
        Android = 2,
        H5 = 3,
        Web = 4,
        MiniProgram = 5
    }

    /// <summary>
    /// 请求方法
    /// </summary>
    public enum RequestMethodEnum : int
    {
        GET = 1,
        POST = 2,
        PUT = 3,
        DELETE = 4
    }

    /// <summary>
    /// 常用类型
    /// </summary>
    public static class TypeEnum
    {
        /// <summary>
        /// Object 类型
        /// </summary>
        public static readonly Type Object = typeof(Object);

        /// <summary>
        /// Type 类型
        /// </summary>
        public static readonly Type Type = typeof(Type);

        /// <summary>
        /// Stirng 类型
        /// </summary>
        public static readonly Type String = typeof(String);

        /// <summary>
        /// Char 类型
        /// </summary>
        public static readonly Type Char = typeof(Char);

        /// <summary>
        /// Boolean 类型
        /// </summary>
        public static readonly Type Boolean = typeof(Boolean);

        /// <summary>
        /// Byte 类型
        /// </summary>
        public static readonly Type Byte = typeof(Byte);


        /// <summary>
        /// Byte 数组类型
        /// </summary>
        public static readonly Type ByteArray = typeof(Byte[]);

        /// <summary>
        /// SByte 类型
        /// </summary>
        public static readonly Type SByte = typeof(SByte);

        /// <summary>
        /// Int16 类型
        /// </summary>
        public static readonly Type Int16 = typeof(Int16);

        /// <summary>
        /// UInt16 类型
        /// </summary>
        public static readonly Type UInt16 = typeof(UInt16);

        /// <summary>
        /// Int32 类型
        /// </summary>
        public static readonly Type Int32 = typeof(Int32);

        /// <summary>
        /// UInt32 类型
        /// </summary>
        public static readonly Type UInt32 = typeof(UInt32);

        /// <summary>
        /// Int64 类型
        /// </summary>
        public static readonly Type Int64 = typeof(Int64);

        /// <summary>
        /// UInt64 类型
        /// </summary>
        public static readonly Type UInt64 = typeof(UInt64);

        /// <summary>
        /// Double 类型
        /// </summary>
        public static readonly Type Double = typeof(Double);

        /// <summary>
        /// Single 类型
        /// </summary>
        public static readonly Type Single = typeof(Single);

        /// <summary>
        /// Decimal 类型
        /// </summary>
        public static readonly Type Decimal = typeof(Decimal);

        /// <summary>
        /// Guid 类型
        /// </summary>
        public static readonly Type Guid = typeof(Guid);

        /// <summary>
        /// DateTime 类型
        /// </summary>
        public static readonly Type DateTime = typeof(DateTime);

        /// <summary>
        /// TimeSpan 类型
        /// </summary>
        public static readonly Type TimeSpan = typeof(TimeSpan);

        /// <summary>
        /// Nullable 类型
        /// </summary>
        public static readonly Type Nullable = typeof(Nullable<>);

        /// <summary>
        /// ValueType 类型
        /// </summary>
        public static readonly Type ValueType = typeof(ValueType);

        /// <summary>
        /// void 类型
        /// </summary>
        public static readonly Type Void = typeof(void);

        /// <summary>
        /// DBNull 类型
        /// </summary>
        public static readonly Type DBNull = typeof(DBNull);

        /// <summary>
        /// Delegate 类型
        /// </summary>
        public static readonly Type Delegate = typeof(Delegate);

        /// <summary>
        /// ByteEnumerable 类型
        /// </summary>
        public static readonly Type ByteEnumerable = typeof(IEnumerable<Byte>);

        /// <summary>
        /// IEnumerable 类型
        /// </summary>
        public static readonly Type IEnumerableofT = typeof(System.Collections.Generic.IEnumerable<>);

        /// <summary>
        /// IEnumerable 类型
        /// </summary>
        public static readonly Type IEnumerable = typeof(System.Collections.IEnumerable);

        /// <summary>
        /// IListSource 类型
        /// </summary>
        public static readonly Type IListSource = typeof(System.ComponentModel.IListSource);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionary = typeof(System.Collections.IDictionary);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionaryOfT = typeof(IDictionary<,>);
        /// <summary>
        /// Dictionary 类型
        /// </summary>
        public static readonly Type DictionaryOfT = typeof(Dictionary<,>);

        /// <summary>
        /// StringDictionary 类型
        /// </summary>
        public static readonly Type StringDictionary = typeof(StringDictionary);

        /// <summary>
        /// NameValueCollection 类型
        /// </summary>
        public static readonly Type NameValueCollection = typeof(NameValueCollection);

        /// <summary>
        /// IDataReader 类型
        /// </summary>
        public static readonly Type IDataReader = typeof(System.Data.IDataReader);

        /// <summary>
        /// DataTable 类型
        /// </summary>
        public static readonly Type DataTable = typeof(System.Data.DataTable);

        /// <summary>
        /// DataRow 类型
        /// </summary>
        public static readonly Type DataRow = typeof(System.Data.DataRow);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionaryOfStringAndObject = typeof(IDictionary<string, object>);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionaryOfStringAndString = typeof(IDictionary<string, string>);

    }
}
