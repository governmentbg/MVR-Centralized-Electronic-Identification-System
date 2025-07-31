using System.Text;
using System.Xml.Serialization;
using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Service.Entities;

[Serializable]
public class ProviderTimestampData
{
    public Guid Id { get; set; }
    public string IssuerUid { get; set; } = string.Empty;
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; } = string.Empty;
    public string CreatedByAdministratorId { get; set; } = string.Empty;
    public string CreatedByAdministratorName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Bulstat { get; set; } = string.Empty;
    public string Headquarters { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public ProviderStatus Status { get; set; }
    public ProviderType Type { get; set; }
    public DateTime CreatedOn { get; set; }
    [XmlArray]
    public List<UserTimestampData> Users { get; set; } = new List<UserTimestampData>();

    public static ProviderTimestampData Create(Provider provider)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        var result = new ProviderTimestampData
        {
            Id = provider.Id,
            IssuerUid = provider.IssuerUid,
            IssuerUidType = provider.IssuerUidType,
            IssuerName = provider.IssuerName,
            CreatedByAdministratorId = provider.CreatedByAdministratorId,
            CreatedByAdministratorName = provider.CreatedByAdministratorName,
            Name = provider.Name,
            Bulstat = provider.Bulstat,
            Headquarters = provider.Headquarters,
            Address = provider.Address,
            Email = provider.Email,
            Phone = provider.Phone,
            Status = provider.Status,
            Type = provider.Type,
            CreatedOn = provider.CreatedOn
        };

        result.Users.AddRange(provider.Users.Select(d => 
            new UserTimestampData 
            { 
                Id = d.Id,
                Name = d.Name,
                CreatedOn = d.CreatedOn,
                EID = d.EID,
                Email = d.Email,
                Phone = d.Phone,
                IsAdministrator = d.IsAdministrator,
                Uid = d.Uid,
                UidType = d.UidType,
                IsDeleted = d.IsDeleted
            }));

        return result;
    }

    public string Serialize()
    {
        var serializer = new XmlSerializer(typeof(ProviderTimestampData));
        using var memoryStream = new MemoryStream();
        serializer.Serialize(memoryStream, this);

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public static ProviderTimestampData? Deserialize(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException($"'{nameof(data)}' cannot be null or whitespace.", nameof(data));
        }

        var serializer = new XmlSerializer(typeof(ProviderTimestampData));
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var memoryStream = new MemoryStream(dataBytes);
        var result = serializer.Deserialize(memoryStream);

        if (result == null || result is not ProviderTimestampData)
        {
            return default;
        }

        return (ProviderTimestampData)result;
    }
}

public class UserTimestampData
{
    public Guid Id { get; set; }
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    /// <summary>
    /// Electronic identity Id
    /// </summary>
    public string EID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsAdministrator { get; set; }
    public DateTime CreatedOn { get; set; }
    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }
}
