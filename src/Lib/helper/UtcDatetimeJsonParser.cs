using Lib.extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.helper
{
    public class UtcDatetimeJsonParser : JsonConverter //Newtonsoft.Json.JsonConverter<DateTime>
    {
        static readonly IReadOnlyList<Type> _allow_list = new List<Type>()
        {
            typeof(DateTime),typeof(DateTime?),
            typeof(DateTimeOffset),typeof(DateTimeOffset?),
        }.AsReadOnly();

        public override bool CanConvert(Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            var allow = _allow_list.Contains(objectType);
            return allow;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType.IsNullable())
                    return null;
                else
                    throw new NotSupportedException($"{objectType.FullName}不能接受null");
            }

            if (reader.TokenType != JsonToken.String)
                throw new NotSupportedException("字段必须是字符串类型");

            var value = ((string)reader.Value) ?? throw new ArgumentNullException(nameof(JsonReader));

            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                var time = DateTime.Parse(value);
                return time;
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else if (value is DateTime dt)
            {
                Nullable<DateTime> d = null;
            }
            throw new NotImplementedException();
        }
    }

    [Obsolete]
    class _xx : Newtonsoft.Json.Converters.DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    [Obsolete]
    class __dd : Newtonsoft.Json.Converters.UnixDateTimeConverter { }

    [Obsolete]
    class __mm : Newtonsoft.Json.Converters.IsoDateTimeConverter { }
}
