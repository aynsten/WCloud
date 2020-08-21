using System;

namespace WCloud.Core.DataSerializer
{
    public interface IDataSerializer
    {
        /// <summary>
        /// 反序列化为数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string[] DeserializeArray(string data);

        /// <summary>
        /// 序列化
        /// </summary>
        byte[] SerializeToBytes(object item);
        string SerializeToString(object item);

        /// <summary>
        /// 反序列化
        /// </summary>
        T DeserializeFromBytes<T>(byte[] serializedObject);
        T DeserializeFromString<T>(string serializedObject);

        T DeserializeFromStringOrDefault<T>(string serializedObject);
        /// <summary>
        /// 反序列化
        /// </summary>
        object DeserializeFromBytes(byte[] serializedObject, Type target);
        object DeserializeFromString(string serializedObject, Type target);
    }
}
