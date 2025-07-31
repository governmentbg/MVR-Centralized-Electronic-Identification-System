using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eID.RO.Service.Converters;

public class SingleObjectToArrayConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(List<T>));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.Array)
        {
            return token.ToObject<List<T>>();
        }

        T singleObject = token.ToObject<T>();
        return new List<T> { singleObject };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var list = (List<T>)value;
        serializer.Serialize(writer, list);
    }
}
