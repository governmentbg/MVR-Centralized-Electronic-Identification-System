using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eID.PIVR.Service.Converters;

public class SingleObjectToArrayConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(List<T>) || objectType == typeof(T[]));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.Array)
        {
            var array = token.ToObject<T[]>();
            return array?.ToList() ?? new List<T>();
        }

        T singleObject = token.ToObject<T>();
        return new List<T> { singleObject };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var list = (value as List<T>) ?? (value as T[])?.ToList();

        if (list is null)
        {
            throw new InvalidCastException($"Expected List<{typeof(T).Name}> or {typeof(T).Name}[], but got {value.GetType()}");
        }

        serializer.Serialize(writer, list);
    }
}
