using System.Xml.Serialization;

namespace eID.PIVR.Service.NAIF.OtherServices;

[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class Envelope
{
    [XmlElement(ElementName = "Body")]
    public Body? Body { get; set; }
}

public class Body
{
    [XmlElement(ElementName = "Message", Namespace = "http://general.service.ict.mvr.bg/")]
    public Message? Message { get; set; }
}

public class Message
{
    [XmlText]
    public string? CData { get; set; }
}

[XmlRoot(ElementName = "OtherServicesResponse", Namespace = "http://bg.mvr.ict/types")]
public class OtherServicesResponse
{
    [XmlElement(ElementName = "ReturnInformation")]
    public ReturnInformation? ReturnInformation { get; set; }

    [XmlElement(ElementName = "Header")]
    public Header? Header { get; set; }

    [XmlElement(ElementName = "ResponseData")]
    public ResponseData? ResponseData { get; set; }

    public bool PersonIsProhibited () => ResponseData?.PersonData?.Prohibition == 1 || ResponseData?.PersonData?.Prohibition == 2;
    public bool PersonIsDead() => ResponseData?.PersonData?.DeathDate != null;
    public bool PersonHasRevokedParentalRights() => ResponseData?.PersonData?.Prohibition == 3;
}

public class ReturnInformation
{
    [XmlElement(ElementName = "ReturnCode", Namespace = "http://bg.mvr.ict/common/types")]
    public string? ReturnCode { get; set; }

    [XmlElement(ElementName = "ServiceInformation", Namespace = "http://bg.mvr.ict/common/types")]
    public ServiceInformation? ServiceInformation { get; set; }
}

public class ServiceInformation
{
    [XmlElement(ElementName = "ServiceInfo", Namespace = "http://bg.mvr.ict/common/types")]
    public ServiceInfo? ServiceInfo { get; set; }
}

public class ServiceInfo
{
    [XmlElement(ElementName = "ServiceCode", Namespace = "http://bg.mvr.ict/common/types")]
    public string? ServiceCode { get; set; }

    [XmlElement(ElementName = "Info", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Info { get; set; }
}

public class Header
{
    [XmlElement(ElementName = "MessageID", Namespace = "http://bg.mvr.ict/common/types")]
    public string? MessageID { get; set; }

    [XmlElement(ElementName = "MessageRefID", Namespace = "http://bg.mvr.ict/common/types")]
    public string? MessageRefID { get; set; }

    [XmlElement(ElementName = "DateTime", Namespace = "http://bg.mvr.ict/common/types")]
    public string? DateTime { get; set; }
}

public class ResponseData
{
    [XmlElement(ElementName = "PersonData", Namespace = "http://bg.mvr.ict/common/types")]
    public PersonData? PersonData { get; set; }

    [XmlElement(ElementName = "Address", Namespace = "http://bg.mvr.ict/common/types")]
    public List<Address>? Addresses { get; set; }

    [XmlElement(ElementName = "Document", Namespace = "http://bg.mvr.ict/common/types")]
    public List<Document>? Documents { get; set; }

    [XmlElement(ElementName = "NotDeliveredAND", Namespace = "http://bg.mvr.ict/common/types")]
    public bool? NotDeliveredAND { get; set; }
}

public class PersonData
{
    [XmlElement(ElementName = "PersonIdentification", Namespace = "http://bg.mvr.ict/common/types")]
    public PersonIdentification? PersonIdentification { get; set; }

    [XmlElement(ElementName = "BirthPlace", Namespace = "http://bg.mvr.ict/common/types")]
    public string? BirthPlace { get; set; }

    [XmlElement(ElementName = "Height", Namespace = "http://bg.mvr.ict/common/types")]
    public int? Height { get; set; }

    [XmlElement(ElementName = "EyesColor", Namespace = "http://bg.mvr.ict/common/types")]
    public EyesColor? EyesColor { get; set; }

    [XmlElement(ElementName = "MaritalStatus", Namespace = "http://bg.mvr.ict/common/types")]
    public MaritalStatus? MaritalStatus { get; set; }

    /// <summary>
    /// Запрещение (0- няма; 1- пълно; 2- ограничено запр., 3- отнети родителски права)
    /// </summary>
    [XmlElement(ElementName = "Prohibition", Namespace = "http://bg.mvr.ict/common/types")]
    public int? Prohibition { get; set; }

    [XmlElement(ElementName = "DeathDate", Namespace = "http://bg.mvr.ict/common/types")]
    public DateTime? DeathDate { get; set; }
}

public class PersonIdentification
{
    [XmlElement(ElementName = "PersonIdentificationBG", Namespace = "http://bg.mvr.ict/common/types")]
    public PersonIdentificationBG? PersonIdentificationBG { get; set; }
    [XmlElement(ElementName = "PersonIdentificationF", Namespace = "http://bg.mvr.ict/common/types")]
    public PersonIdentificationF? PersonIdentificationF { get; set; }
}

public class PersonIdentificationBG
{
    [XmlElement(ElementName = "PIN", Namespace = "http://bg.mvr.ict/common/types")]
    public string? PIN { get; set; }

    [XmlElement(ElementName = "Names", Namespace = "http://bg.mvr.ict/common/types")]
    public Names? Names { get; set; }

    [XmlElement(ElementName = "BirthDate", Namespace = "http://bg.mvr.ict/common/types")]
    public string? BirthDate { get; set; }

    [XmlElement(ElementName = "Sex", Namespace = "http://bg.mvr.ict/common/types")]
    public Sex? Sex { get; set; }
}

public class Names
{
    [XmlElement(ElementName = "FirstName", Namespace = "http://bg.mvr.ict/common/types")]
    public Name? FirstName { get; set; }

    [XmlElement(ElementName = "Surname", Namespace = "http://bg.mvr.ict/common/types")]
    public Name? Surname { get; set; }

    [XmlElement(ElementName = "Family", Namespace = "http://bg.mvr.ict/common/types")]
    public Name? Family { get; set; }
}

public class Name
{
    [XmlElement(ElementName = "Cyrillic", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Cyrillic { get; set; }

    [XmlElement(ElementName = "Latin", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Latin { get; set; }
}

public class Sex
{
    [XmlAttribute(AttributeName = "code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class EyesColor
{
    [XmlAttribute(AttributeName = "code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class MaritalStatus
{
    [XmlAttribute(AttributeName = "code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class Address
{
    [XmlAttribute(AttributeName = "id")]
    public string? ID { get; set; }

    [XmlElement(ElementName = "District", Namespace = "http://bg.mvr.ict/common/types")]
    public District? District { get; set; }

    [XmlElement(ElementName = "Municipality", Namespace = "http://bg.mvr.ict/common/types")]
    public Municipality? Municipality { get; set; }

    [XmlElement(ElementName = "Settlement", Namespace = "http://bg.mvr.ict/common/types")]
    public Settlement? Settlement { get; set; }

    [XmlElement(ElementName = "Location", Namespace = "http://bg.mvr.ict/common/types")]
    public Location? Location { get; set; }

    [XmlElement(ElementName = "BuildingNumber", Namespace = "http://bg.mvr.ict/common/types")]
    public string? BuildingNumber { get; set; }

    [XmlElement(ElementName = "PoliceDepartment", Namespace = "http://bg.mvr.ict/common/types")]
    public PoliceDepartment? PoliceDepartment { get; set; }
}

public class District
{
    [XmlAttribute(AttributeName = "Code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class Municipality
{
    [XmlAttribute(AttributeName = "Code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class Settlement
{
    [XmlAttribute(AttributeName = "Code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class Location
{
    [XmlAttribute(AttributeName = "Code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class PoliceDepartment
{
    [XmlAttribute(AttributeName = "Code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class Document
{
    [XmlElement(ElementName = "DocumentType", Namespace = "http://bg.mvr.ict/common/types")]
    public DocumentType? DocumentType { get; set; }

    [XmlElement(ElementName = "Number", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Number { get; set; }

    [XmlElement(ElementName = "IssueDate", Namespace = "http://bg.mvr.ict/common/types")]
    public DateTime? IssueDate { get; set; }

    [XmlElement(ElementName = "ExpiryDate", Namespace = "http://bg.mvr.ict/common/types")]
    public DateTime? ExpiryDate { get; set; }

    [XmlElement(ElementName = "Issuer", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Issuer { get; set; }

    [XmlElement(ElementName = "CurrentStatus", Namespace = "http://bg.mvr.ict/common/types")]
    public CurrentStatus? CurrentStatus { get; set; }
}

public class DocumentType
{
    [XmlElement(ElementName = "Type", Namespace = "http://bg.mvr.ict/common/types")]
    public Type? Type { get; set; }
}

public class Type
{
    [XmlAttribute(AttributeName = "Code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class CurrentStatus
{
    [XmlElement(ElementName = "StatusType", Namespace = "http://bg.mvr.ict/common/types")]
    public StatusType? StatusType { get; set; }

    [XmlElement(ElementName = "StatusDate", Namespace = "http://bg.mvr.ict/common/types")]
    public DateTime? StatusDate { get; set; }

    [XmlElement(ElementName = "PoliceDepartment", Namespace = "http://bg.mvr.ict/common/types")]
    public PoliceDepartment? PoliceDepartment { get; set; }
}

public class StatusType
{
    [XmlAttribute(AttributeName = "Code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}
public class PersonIdentificationF
{
    [XmlElement(ElementName = "LNC", Namespace = "http://bg.mvr.ict/common/types")]
    public string? LNC { get; set; }

    [XmlElement(ElementName = "Names", Namespace = "http://bg.mvr.ict/common/types")]
    public Names? Names { get; set; }

    [XmlElement(ElementName = "BirthDate", Namespace = "http://bg.mvr.ict/common/types")]
    public string? BirthDate { get; set; }

    [XmlElement(ElementName = "Sex", Namespace = "http://bg.mvr.ict/common/types")]
    public Sex? Sex { get; set; }

    [XmlElement(ElementName = "Statut", Namespace = "http://bg.mvr.ict/common/types")]
    public Statut? Statut { get; set; }

    [XmlElement(ElementName = "Nationality", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Nationality { get; set; }

    [XmlElement(ElementName = "NationalDocument", Namespace = "http://bg.mvr.ict/common/types")]
    public NationalDocument? NationalDocument { get; set; }
}

public class Statut
{
    [XmlAttribute(AttributeName = "code")]
    public string? Code { get; set; }

    [XmlText]
    public string? Value { get; set; }
}

public class NationalDocument
{
    [XmlElement(ElementName = "DocumentType", Namespace = "http://bg.mvr.ict/common/types")]
    public DocumentType? DocumentType { get; set; }

    [XmlElement(ElementName = "Number", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Number { get; set; }

    [XmlElement(ElementName = "IssuingCountry", Namespace = "http://bg.mvr.ict/common/types")]
    public string? IssuingCountry { get; set; }

    [XmlElement(ElementName = "IssueDate", Namespace = "http://bg.mvr.ict/common/types")]
    public DateTime? IssueDate { get; set; }

    [XmlElement(ElementName = "ValidDate", Namespace = "http://bg.mvr.ict/common/types")]
    public DateTime? ValidDate { get; set; }
}

public class AddressExtension
{
    [XmlElement(ElementName = "Entrance", Namespace = "http://bg.mvr.ict/common/types")]
    public string? Entrance { get; set; }

    [XmlElement(ElementName = "Floor", Namespace = "http://bg.mvr.ict/common/types")]
    public int? Floor { get; set; }

    [XmlElement(ElementName = "Apartment", Namespace = "http://bg.mvr.ict/common/types")]
    public int? Apartment { get; set; }
}

