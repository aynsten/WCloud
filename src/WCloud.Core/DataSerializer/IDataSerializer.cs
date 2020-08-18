using Microsoft.Extensions.DependencyInjection;
using System;

namespace WCloud.Core.DataSerializer
{
    public interface IDataSerializer : IAutoRegistered
    {
        /// <summary>
        /// 序列化数组
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        string SerializeArray(string[] permissions);

        /// <summary>
        /// 反序列化为数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string[] DeserializeArray(string data);

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
