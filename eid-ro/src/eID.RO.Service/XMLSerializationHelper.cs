using System.Text;
using System.Xml;
using System.Xml.Serialization;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;

namespace eID.RO.Service;

public static class XMLSerializationHelper
{
    public static string SerializeEmpowermentStatementItem(EmpowermentStatementItem item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
            Encoding = Encoding.UTF8
        };
        using MemoryStream ms = new MemoryStream();
        XmlWriter writer = XmlWriter.Create(ms, settings);
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        // In order to load the object into the memory stream we need to instantiate a serializer
        // and serialize it using the configured writer.
        var EmpowermentStatementSerializer = new XmlSerializer(typeof(EmpowermentStatementItem));
        EmpowermentStatementSerializer.Serialize(writer, item, ns);
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public static EmpowermentStatementItem DeserializeEmpowermentStatementItem(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException($"'{nameof(data)}' cannot be null or whitespace.", nameof(data));
        }

        var xmlSerializer = new XmlSerializer(typeof(EmpowermentStatementItem));

        var deserialized = xmlSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        if (deserialized == null)
        {
            return new EmpowermentStatementItem();
        }

        if (deserialized is not EmpowermentStatementItem result)
        {
            return new EmpowermentStatementItem();
        }

        return result;
    }
}

[Serializable]
public class EmpowermentStatementItem
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string OnBehalfOf { get; set; } = string.Empty;
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
    [XmlArray]
    [XmlArrayItem(ElementName = "Uid")]
    public AuthorizerIdentifierData[] AuthorizerUids { get; set; } = Array.Empty<AuthorizerIdentifierData>();
    [XmlArray]
    [XmlArrayItem(ElementName = "Uid")]
    public UserIdentifierData[] EmpoweredUids { get; set; } = Array.Empty<UserIdentifierData>();
    public string ProviderId { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string TypeOfEmpowerment { get; set; } = string.Empty;
    [XmlArray]
    [XmlArrayItem(ElementName = "Item")]
    public VolumeOfRepresentationItem[] VolumeOfRepresentation { get; set; } = Array.Empty<VolumeOfRepresentationItem>();
    public DateTime CreatedOn { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

[Serializable]
public class VolumeOfRepresentationItem
{
    public string Name { get; set; } = string.Empty;
}
