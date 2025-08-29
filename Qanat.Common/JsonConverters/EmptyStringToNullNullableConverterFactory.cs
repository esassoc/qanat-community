using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qanat.Common.JsonConverters;

public sealed class EmptyStringToNullNullableConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return Nullable.GetUnderlyingType(typeToConvert) is not null;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var underlying = Nullable.GetUnderlyingType(typeToConvert)!;
        var converterType = typeof(EmptyStringToNullNullableConverter<>).MakeGenericType(underlying);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class EmptyStringToNullNullableConverter<T> : JsonConverter<T?> where T : struct
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s))
                {
                    return null;
                }
            }

            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}