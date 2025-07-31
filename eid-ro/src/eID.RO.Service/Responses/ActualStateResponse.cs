using System.Text.RegularExpressions;
using eID.RO.Service.Converters;
using Newtonsoft.Json;

namespace eID.RO.Service.Responses;

public class LegalEntityActualState : ActualState<ActualStateResponse>
{
    public bool MatchCompanyData(string companyName, string companyUic)
    {
        return MatchCompanyName(companyName) && MatchCompanyUic(companyUic);
    }

    public bool MatchCompanyName(string companyName)
    {
        return companyName == Response?.ActualStateResponseV3?.Deed?.CompanyName;
    }

    public bool MatchCompanyUic(string companyUic)
    {
        return companyUic == Response?.ActualStateResponseV3?.Deed?.UIC;
    }

    public bool IsAmongRepresentatives(string uid, string name)
    {
        var representatives = Response?.ActualStateResponseV3?.GetRepresentatives() ?? Enumerable.Empty<Person>();
        return representatives.Any(r => r.Name?.ToLower().Trim() == name.ToLower().Trim() && r.Indent == uid);
    }

    public bool ContainsRepresentativesData()
    {
        var representatives = Response?.ActualStateResponseV3?.GetRepresentatives() ?? Enumerable.Empty<Person>();
        return representatives.Any();
    }

    public bool HasValidDeedStatus()
    {
        return !string.IsNullOrWhiteSpace(Response?.ActualStateResponseV3?.Deed?.DeedStatus);
    }

    public bool IsInactive()
    {
        //C - Нова партида затворена
        //L - Пререгистрирана фирма по Булстат затворена
        string[] _inctiveStatuses = { "C", "L" };
        return _inctiveStatuses.Contains(Response?.ActualStateResponseV3?.Deed?.DeedStatus ?? string.Empty);
    }

    public bool IsToBeCheckedInBulstat()
    {
        //Е - Пререгистрирана фирма по Булстат
        //L - Пререгистрирана фирма по Булстат затворена
        string[] _checkInBulstatStatuses = { "E", "L" };
        return _checkInBulstatStatuses.Contains(Response?.ActualStateResponseV3?.Deed?.DeedStatus ?? string.Empty);
    }

    public WayOfRepresentation GetWayOfRepresentation()
    {
        string[] _wayOfRepresentationFieldIds = new[] { "00110", "00430", "05350" };
        var wayOfRepresentation = Response?.ActualStateResponseV3?.Deed?.Subdeeds?
            .Subdeed?
            .SelectMany(subdeed => subdeed.Records?.Record ?? Enumerable.Empty<RecordItem>())
            .Where(record => record != null && _wayOfRepresentationFieldIds.Contains(record.FieldIdent))
            .Select(record => record.RecordData?.WayOfRepresentation)
            .FirstOrDefault(wayOfRepresentation => !string.IsNullOrWhiteSpace(wayOfRepresentation));

        return new WayOfRepresentation(wayOfRepresentation);
    }

    public IEnumerable<Person> GetRepresentatives()
    {
        return Response?.ActualStateResponseV3?.GetRepresentatives() ?? Enumerable.Empty<Person>();
    }
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
    private readonly HashSet<string> _representativeFields = "00020,00070,00071,00100,00101,00102,00110,00120,00121,00123,00130,00131,00133,00134,00140,00142,00143,00150,00151,00190,00200,00210,00230,00430,00540,00541,05340,05350,05371,05381,05500,01160".Split(",").ToHashSet();
    public Deed? Deed { get; set; }
    public DateTime DataValidForDate { get; set; }
    public string? DataFound { get; set; }

    public IEnumerable<Person> GetRepresentatives()
    {
        return Deed?.Subdeeds?.Subdeed?
            .SelectMany(subdeed => subdeed.Records?.Record ?? Enumerable.Empty<RecordItem>())
            .Where(record => record != null)
            .SelectMany(record => new[]
            {
            _representativeFields.Contains(record.FieldIdent) ? record.RecordData?.Manager?.Person : null,
            _representativeFields.Any(f => record.FieldIdent.StartsWith(f)) ? record.RecordData?.Manager?.Person : null, // Sometimes fields contain the main category with additions. e.g. 00070 -> 0007000
            _representativeFields.Any(f => record.FieldIdent.StartsWith(f)) ? record.RecordData?.Representative?.Subject : null, // Sometimes fields contain the main category with additions. e.g. 00070 -> 0007000
            })
            .Where(person => person != null)
            .Distinct()
            .Cast<Person>()
            ?? Enumerable.Empty<Person>();
    }
}

public class Deed
{
    public string? CompanyName { get; set; }
    public string? UIC { get; set; }
    public string? LegalForm { get; set; }
    public int CaseNo { get; set; }
    public int CaseYear { get; set; }
    public string GUID { get; set; }
    public string CourtNo { get; set; }
    /// <summary>
    //N - нова
    //Е - Пререгистрирана фирма по Булстат
    //C - Нова партида затворена
    //L - Пререгистрирана фирма по Булстат затворена
    /// </summary>
    public string? DeedStatus { get; set; }
    public Subdeeds? Subdeeds { get; set; }
}

public class Subdeeds
{
    [JsonConverter(typeof(SingleObjectToArrayConverter<Subdeed>))]
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
    public RecordData RecordData { get; set; }
    public string? RecordId { get; set; }
    public string? IncomingId { get; set; }
    public string? FieldIdent { get; set; }
    public MainField? MainField { get; set; }
    public DateTime FieldActionDate { get; set; }
    public string FieldEntryNumber { get; set; }
}
public class RecordData
{
    public string Company { get; set; }
    public string LegalForm { get; set; }
    public string Transliteration { get; set; }
    public string SubjectOfActivity { get; set; }
    public ActualStateUIC? UIC { get; set; }
    public Seat? Seat { get; set; }
    public Manager? Manager { get; set; }
    public string WayOfRepresentation { get; set; }
    public SoleCapitalOwner? SoleCapitalOwner { get; set; }
    public string Funds { get; set; }
    public string DepositedFunds { get; set; }
    public NonMonetaryDeposit? NonMonetaryDeposit { get; set; }
    public OffshoreDirectControlCompany? OffshoreDirectControlCompany { get; set; }
    public ActualOwner? ActualOwner { get; set; }
    public Person? Partner { get; set; }
    public Representative? Representative { get; set; }
}

public class ActualStateUIC
{
    public string CompanyControl { get; set; }
    public BulstatDeed? BulstatDeed { get; set; }
}

public class NonMonetaryDeposit
{
    public string Description033 { get; set; }
    public string Value { get; set; }
}

public class OffshoreDirectControlCompany
{
    public string Name { get; set; }
    public ActualStateAddress? Address { get; set; }
}

public class ActualOwner
{
    public Person? Person { get; set; }
    public ActualStateAddress? Address { get; set; }
    public OwnedRightsDetails? OwnedRightsDetails { get; set; }
}

public class OwnedRightsDetails
{
    public OwnedRightsDetail? OwnedRightsDetail { get; set; }
}

public class OwnedRightsDetail
{
    public string OwnedRightCode { get; set; }
    public string OwnedRightName { get; set; }
    public string OwnedRightSize { get; set; }
    public string OwnedRightDescription { get; set; }
}

public class BulstatDeed
{
    public string Deed { get; set; }
    public string Year { get; set; }
    public string CourtCode { get; set; }
}

public class Seat
{
    public ActualStateAddress? Address { get; set; }
    public Contacts? Contacts { get; set; }
}

public class ActualStateAddress
{
    public string CountryID { get; set; }
    public string CountryCode { get; set; }
    public string Country { get; set; }
    public string IsForeign { get; set; }
    public string DistrictID { get; set; }
    public string DistrictEkatte { get; set; }
    public string District { get; set; }
    public string Municipalityid { get; set; }
    public string MunicipalityEkatte { get; set; }
    public string Municipality { get; set; }
    public string SettlementID { get; set; }
    public string SettlementEKATTE { get; set; }
    public string Settlement { get; set; }
    public string AreaID { get; set; }
    public string Area { get; set; }
    public string AreaEkatte { get; set; }
    public string PostCode { get; set; }
    public string ForeignPlace { get; set; }
    public string HousingEstate { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string Block { get; set; }
    public string Entrance { get; set; }
    public string Floor { get; set; }
    public string Apartment { get; set; }
}

public class Contacts
{
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string EMail { get; set; }
    public string URL { get; set; }
}

public class Manager
{
    public Person? Person { get; set; }
}

public class SoleCapitalOwner
{
    public ActualStateSubject? Subject { get; set; }
}
public class ActualStateSubject
{
    public string Indent { get; set; }
    public string Name { get; set; }
    public string IndentType { get; set; }
    public string CountryID { get; set; }
    public string CountryName { get; set; }
}

public class MainField
{
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
    public string? SectionId { get; set; }
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

public class Person : IEquatable<Person>
{
    public string? Indent { get; set; }
    public string? Name { get; set; }
    public string? IndentType { get; set; }
    public string? CountryID { get; set; }
    public string? CountryName { get; set; }
    public override bool Equals(object? obj)
    {
        return Equals(obj as Person);
    }

    public bool Equals(Person? other)
    {
        if (other == null) return false;

        return string.Equals(Indent, other.Indent, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
            && string.Equals(IndentType, other.IndentType, StringComparison.OrdinalIgnoreCase)
            && string.Equals(CountryID, other.CountryID, StringComparison.OrdinalIgnoreCase)
            && string.Equals(CountryName, other.CountryName, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Indent?.ToLowerInvariant(),
            Name?.ToLowerInvariant(),
            IndentType?.ToLowerInvariant(),
            CountryID?.ToLowerInvariant(),
            CountryName?.ToLowerInvariant()
        );
    }
}

public class WayOfRepresentation
{
    private const string JOINTLY = "Само заедно";
    private const string SEVERALLY = "Заедно и поотделно";
    public WayOfRepresentation() { }
    public WayOfRepresentation(string rawResponse)
    {
        var value = string.Empty;
        if (!string.IsNullOrWhiteSpace(rawResponse))
        {
            value = rawResponse.Trim();
        }
        Value = Regex.Replace(value, @"\s+", " ");
        Jointly = Value.Equals(JOINTLY, StringComparison.InvariantCultureIgnoreCase);
        Severally = Value.Equals(SEVERALLY, StringComparison.InvariantCultureIgnoreCase);
        OtherWay = !Jointly && !Severally;
    }

    public bool Jointly { get; set; }
    public bool Severally { get; set; }
    public bool OtherWay { get; set; }
    public string? Value { get; set; }
}
