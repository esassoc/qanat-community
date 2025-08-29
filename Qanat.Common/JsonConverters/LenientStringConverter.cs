using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qanat.Common.JsonConverters;

public class LenientStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                return reader.GetString();
            }
            case JsonTokenType.Number:
            {
                if (reader.TryGetInt64(out var l))
                {
                    return l.ToString(CultureInfo.InvariantCulture);
                }

                if (reader.TryGetDecimal(out var m))
                {
                    return m.ToString(CultureInfo.InvariantCulture);
                }

                var d = reader.GetDouble();
                return d.ToString("G17", CultureInfo.InvariantCulture);
            }
            case JsonTokenType.True:
            {
                return bool.TrueString;
            }
            case JsonTokenType.False:
            {
                return bool.FalseString;
            }
            case JsonTokenType.Null:
            {
                return null;
            }
            default:
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                return doc.RootElement.GetRawText();
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value);
    }
}