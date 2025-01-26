using System.Text.Json;
using System.Text.Json.Serialization;
using Messy.Requests;

namespace Messy.Helpers;

public class JsonCreateChatRequestConverter : JsonConverter<CreateChatRequest>
{
    public override CreateChatRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);

        var type = doc.RootElement.GetProperty("Type").GetInt16();

        return type switch
        {
            (short)ChatType.ManyToMany => JsonSerializer.Deserialize<CreateManyToManyRequest>(doc.RootElement.GetRawText(), options),
            (short)ChatType.OneToOne => JsonSerializer.Deserialize<CreateOneToOneRequest>(doc.RootElement.GetRawText(), options),
            _ => JsonSerializer.Deserialize<CreateChatRequest>(doc.RootElement.GetRawText(), options),
        };
    }

    public override void Write(Utf8JsonWriter writer, CreateChatRequest value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}