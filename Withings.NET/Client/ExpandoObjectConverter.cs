using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Withings.NET.Client
{
    public class ExpandoObjectConverter : JsonConverter<ExpandoObject>
    {
        public override ExpandoObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var expando = new ExpandoObject();
                    var dictionary = (IDictionary<string, object>)expando;
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            return expando;

                        if (reader.TokenType != JsonTokenType.PropertyName)
                            throw new JsonException();

                        var propertyName = reader.GetString();
                        reader.Read();
                        dictionary[propertyName] = ReadValue(ref reader, options);
                    }
                    throw new JsonException();
                default:
                    throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
        {
             JsonSerializer.Serialize(writer, (IDictionary<string, object>)value, options);
        }

        private object ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                     return Read(ref reader, typeof(ExpandoObject), options);
                case JsonTokenType.StartArray:
                    var list = new List<object>();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                            return list;
                        list.Add(ReadValue(ref reader, options));
                    }
                    throw new JsonException();
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out long l))
                        return l;
                    if (reader.TryGetDecimal(out decimal d))
                        return d;
                    return reader.GetDouble();
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.Null:
                    return null;
                default:
                    throw new JsonException();
            }
        }
    }
}
