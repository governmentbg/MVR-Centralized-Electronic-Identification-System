using eID.PIVR.Contracts.Enums;
using eID.PIVR.Contracts.Results;

namespace eID.PIVR.Service.Entities;

/// <summary>
/// Entity for MoI import table StatutChanges
/// Тук се попълват данни когато:
///  - Когато български гражданин се откаже от своето българско гражданство
///  - Когато на чуждестранен гражданин се отнеме статут
/// </summary>
public class StatutChange : StatutChangeResult
{
    /// <summary>
    /// Record Id, auto fill
    /// </summary>
    public int Id { get; set; }

    public string PersonalId { get; set; } = string.Empty;

    public UidType UidType { get; set; }

    /// <summary>
    /// Date of change
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Created on, auto fill
    /// </summary>
    public DateTime CreatedOn { get; set; }
}
