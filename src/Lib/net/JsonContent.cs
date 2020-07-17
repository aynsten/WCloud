using Lib.helper;
using System;
using System.Net.Http;
using System.Text;

namespace Lib.net
{
    public class JsonContent<T> : JsonContent
    {
        public JsonContent(T data, Encoding encoding = null) :
            base(data == null ? null : JsonHelper.ObjectToJson(data), encoding)
        {
            //
        }
    }

    public class JsonContent : StringContent
    {
        private const string MediaType = "application/json";

        public JsonContent(string json, Encoding encoding = null) :
            base(
                json ?? throw new ArgumentNullException(nameof(json)),
                encoding ?? Encoding.UTF8,
                MediaType)
        {
            //
        }
    }
}
