using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace eID.Signing.Contracts.Results;

public class BoricaDocumentSigningResult
{
    public string ResponseCode { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public BoricaSignDocumentsData Data { get; set; }
}

public class BoricaSignDocumentsData
{
    public string CallbackId { get; set; }

    [JsonConverter(typeof(MicrosecondEpochConverter))]
    public DateTime Validity { get; set; }
}

public class MicrosecondEpochConverter : DateTimeConverterBase
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteRawValue(((long)((DateTime)value - DateTime.UnixEpoch).TotalMilliseconds).ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null) { return null; }

        return DateTime.UnixEpoch.AddMilliseconds((long)reader.Value);
    }
}
