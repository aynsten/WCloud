using FluentAssertions;
using Lib.extension;
using System.Text;

namespace WCloud.Framework.Socket
{
    public interface IMessageSerializer
    {
        T DeserializeOrDefault<T>(string data) where T : class;
        T Deserialize<T>(string data) where T : class;
        T DeserializeOrDefault<T>(byte[] data) where T : class;
        T Deserialize<T>(byte[] data) where T : class;
        string Serialize<T>(T data) where T : class;
        byte[] SerializeBytes<T>(T data) where T : class;
    }

    public class DefaultMessageSerializer : IMessageSerializer
    {
        private readonly Encoding _encoding = Encoding.UTF8;

        public T Deserialize<T>(string data) where T : class
        {
            data.Should().NotBeNullOrEmpty();
            var res = data.JsonToEntity<T>();
            return res;
        }

        public T Deserialize<T>(byte[] data) where T : class
        {
            var json = this._encoding.GetString(data);
            return this.Deserialize<T>(json);
        }

        public T DeserializeOrDefault<T>(string data) where T : class
        {
            try
            {
                return this.Deserialize<T>(data);
            }
            catch
            {
                return default;
            }
        }

        public T DeserializeOrDefault<T>(byte[] data) where T : class
        {
            var json = this._encoding.GetString(data);
            return this.DeserializeOrDefault<T>(json);
        }

        public string Serialize<T>(T data) where T : class
        {
            data.Should().NotBeNull();
            var json = data.ToJson();
            return json;
        }

        public byte[] SerializeBytes<T>(T data) where T : class
        {
            var json = this.Serialize(data);
            var res = this._encoding.GetBytes(json);
            return res;
        }
    }
}
