using System.Text;
using eID.PIVR.Contracts.Enums;
using eID.PIVR.Contracts.Results;

namespace eID.PIVR.Service.Entities;

public class DateOfProhibition : DateOfProhibitionResult
{
    public int Id { get; set; }
    public string PersonalId { get; set; } = string.Empty;
    /// <summary>
    /// Occurrence date
    /// </summary>
    public DateTime? Date { get; set; }
    public DateTime CreatedOn { get; set; }
    /// <summary>
    /// Uid type: EGN or LNCh
    /// </summary>
    public UidType UidType { get; set; }
    /// <summary>
    /// Prohibition type: Full or Partial
    /// </summary>
    public ProhibitionType TypeOfProhibition { get; set; }
    public string DescriptionOfProhibition { get; set; } = string.Empty;

    public static string GetCacheKey(string personalId, UidType uidType)
    {
        if (string.IsNullOrWhiteSpace(personalId))
        {
            throw new ArgumentException($"'{nameof(personalId)}' cannot be null or whitespace.", nameof(personalId));
        }

        if (uidType == UidType.None)
        {
            throw new ArgumentException($"'{nameof(uidType)}' cannot be {nameof(UidType.None)}.", nameof(uidType));
        }

        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{personalId}:{uidType}"));
            return string.Join(":",
                        "eID:PIVR:DatesOfProhibition",
                        BitConverter.ToString(hashBytes).Replace("-", string.Empty));
        }
    }
}
