using System;

namespace Lib.data
{
    public interface ISerializeProvider
    {
        /// <summary>
        /// 序列化
        /// </summary>
        byte[] Serialize(object item);

        /// <summary>
        /// 反序列化
        /// </summary>
        T Deserialize<T>(byte[] serializedObject);

        /// <summary>
        /// 反序列化
        /// </summary>
        object Deserialize(byte[] serializedObject, Type target);

        /// <summary>
        /// 获取对象序列化的二进制版本
        /// </summary>
        byte[] BinaryFormatterSerialize(object obj);

        /// <summary>
        /// 从已序列化数据中(byte[])获取对象实体
        /// </summary>
        T BinaryFormatterDeserialize<T>(byte[] bs);

        /// <summary>
        /// 从已序列化数据中(byte[])获取对象实体
        /// </summary>
        object BinaryFormatterDeserialize(byte[] bs);
    }
}
