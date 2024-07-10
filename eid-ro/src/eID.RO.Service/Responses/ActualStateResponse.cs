using Newtonsoft.Json.Linq;

namespace eID.RO.Service.Responses;

public class LegalEntityActualState : ActualState<ActualStateResponse>
{
}

public class ForeignIdentityActualState : ActualState<ActualForeignIdentityInfoResponseType>
{
}

public class ActualForeignIdentityInfoResponseType
{
    public ForeignIdentityInfoResponseType? ForeignIdentityInfoResponse { get; set; }
}

public class ActualStateResponse
{
    public ActualStateResponseV3? ActualStateResponseV3 { get; set; }
}

public class ActualStateResponseV3
{
    public Deed? Deed { get; set; }
    public DateTime DataValidForDate { get; set; }
    public string? DataFound { get; set; }
}

public class Deed
{
    public string? CompanyName { get; set; }
    public string? UIC { get; set; }
    public string? LegalForm { get; set; }
    public int CaseNo { get; set; }
    public int CaseYear { get; set; }
    public string? DeedStatus { get; set; }
    public Subdeeds? Subdeeds { get; set; }
}

public class Subdeeds
{
    public IEnumerable<Subdeed> Subdeed { get; set; } = new List<Subdeed>();
}
public class Subdeed
{
    public Records? Records { get; set; }
}

public class Records
{
    public IEnumerable<RecordItem> Record { get; set; } = new List<RecordItem>();
}

public class RecordItem
{
    public JObject? RecordData { get; set; }
    public string? RecordId { get; set; }
    public string? IncomingId { get; set; }
    public string? FieldIdent { get; set; }
    public MainField? MainField { get; set; }
    public DateTime FieldActionDate { get; set; }
}

public class MainField
{
    public int GroupId { get; set; }
    public string? GroupName { get; set; }
    public int SectionId { get; set; }
    public string? SectionName { get; set; }
    public string? MainFieldIdent { get; set; }
    public string? MainFieldCode { get; set; }
    public string? MainFieldName { get; set; }
}

public class RepresentativeData
{
    public Representative? Representative { get; set; }

}

public class Representative
{
    public Person? Subject { get; set; }
    public Person? Person { get; set; }
}

public class Person
{
    public string? Indent { get; set; }
    public string? Name { get; set; }
    public string? IndentType { get; set; }
    public string? CountryID { get; set; }
    public string? CountryName { get; set; }
}

public class WayOfRepresentationData
{
    public WayOfRepresentation? WayOfRepresentation { get; set; }
}

public class WayOfRepresentation
{
    public bool Jointly { get; set; }
    public bool Severally { get; set; }
    public bool OtherWay { get; set; }
    public string? Value { get; set; }
}
