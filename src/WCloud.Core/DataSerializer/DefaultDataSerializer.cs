using FluentAssertions;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WCloud.Core.DataSerializer
{

    public class DefaultDataSerializer : IDataSerializer
    {
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
                var res = data.JsonToEntity<string[]>();
                return res;
            }
            catch (Exception e)
            {
                this._logger.AddWarningLog(msg: $"反序列化失败,{data}=>{typeof(string[]).FullName}", e: e);

                return new string[] { };
            }
        }

        public string SerializeArray(string[] permissions)
        {
            permissions.Should().NotBeNull();

            var res = permissions.ToJson();

            return res;
        }

        private Encoding _encoding => ConfigHelper.Instance.SystemEncoding;

        public virtual byte[] Serialize(object item)
        {
            item.Should().NotBeNull();

            var jsonString = item.ToJson();
            return this._encoding.GetBytes(jsonString);
        }

        public virtual T Deserialize<T>(byte[] serializedObject)
        {
            serializedObject.Should().NotBeNullOrEmpty();

            var json = this._encoding.GetString(serializedObject);
            return JsonHelper.JsonToEntity<T>(json);
        }

        public object Deserialize(byte[] serializedObject, Type target)
        {
            serializedObject.Should().NotBeNullOrEmpty();
            target.Should().NotBeNull();

            var json = this._encoding.GetString(serializedObject);
            var obj = JsonHelper.JsonToEntity(json, target);
            return obj;
        }

        #region binary formatter

        public virtual byte[] BinaryFormatterSerialize(object obj)
        {
            obj.Should().NotBeNull();

            using (var ms = new MemoryStream())
            {
                var formartter = new BinaryFormatter();
                formartter.Serialize(ms, obj);

                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public virtual T BinaryFormatterDeserialize<T>(byte[] bs)
        {
            var obj = this.BinaryFormatterDeserialize(bs);

            return (T)obj;
        }

        public object BinaryFormatterDeserialize(byte[] bs)
        {
            bs.Should().NotBeNullOrEmpty();

            using (var ms = new MemoryStream(bs))
            {
                var formatter = new BinaryFormatter();

                var obj = formatter.Deserialize(ms);
                obj.Should().NotBeNull();
                return obj;
            }
        }
        #endregion
    }
}
