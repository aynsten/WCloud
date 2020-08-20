using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace WCloud.Core.DataSerializer
{
    public class DefaultDataSerializer : IDataSerializer
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ILogger _logger;

        public DefaultDataSerializer(ILogger<DefaultDataSerializer> logger)
        {
            this._logger = logger;
        }

        public string[] DeserializeArray(string data)
        {
            try
            {
                data ??= "[]";
                var res = this.DeserializeFromString<string[]>(data);
                return res;
            }
            catch (Exception e)
            {
                this._logger.AddErrorLog("序列化错误，异常只做记录，不会抛出", e);

                return new string[] { };
            }
        }

        public virtual byte[] SerializeToBytes(object item)
        {
            var jsonString = this.SerializeToString(item);

            var res = this._encoding.GetBytes(jsonString);

            return res;
        }

        public virtual T DeserializeFromBytes<T>(byte[] serializedObject)
        {
            serializedObject.Should().NotBeNullOrEmpty();

            var json = this._encoding.GetString(serializedObject);

            var res = this.DeserializeFromString<T>(json);

            return res;
        }

        public object DeserializeFromBytes(byte[] serializedObject, Type target)
        {
            serializedObject.Should().NotBeNullOrEmpty();

            var json = this._encoding.GetString(serializedObject);

            var obj = this.DeserializeFromString(json, target);

            return obj;
        }

        public string SerializeToString(object item)
        {
            item.Should().NotBeNull();
            var res = JsonHelper.ObjectToJson(item);
            return res;
        }

        public T DeserializeFromString<T>(string serializedObject)
        {
            serializedObject.Should().NotBeNullOrEmpty();

            try
            {
                var res = JsonHelper.JsonToEntity<T>(serializedObject);
                return res;
            }
            catch (Exception e)
            {
                throw new Exception($"反序列化失败,{serializedObject}=>{typeof(T).FullName}", e);
            }
        }

        public object DeserializeFromString(string serializedObject, Type target)
        {
            serializedObject.Should().NotBeNullOrEmpty();
            target.Should().NotBeNull();

            try
            {
                var res = JsonHelper.JsonToEntity(serializedObject, target);
                return res;
            }
            catch (Exception e)
            {
                throw new Exception($"反序列化失败,{serializedObject}=>{target.FullName}", e);
            }
        }

        public T DeserializeFromStringOrDefault<T>(string serializedObject)
        {
            try
            {
                var res = this.DeserializeFromString<T>(serializedObject);
                return res;
            }
            catch (Exception e)
            {
                this._logger.AddErrorLog(e.Message, e);
                return default;
            }
        }
    }
}
