using eID.PIVR.Contracts.Enums;
using eID.PIVR.Contracts.Results;

namespace eID.PIVR.Service.Entities;

/// <summary>
/// Entity for MoI import table IDChanges
/// Попълва се при смяна на ЕГН и при преминаване от ЛНЧ към ЕГН
/// </summary>
public class IdChange : IdChangeResult
{
    /// <summary>
    /// Record Id, auto fill
    /// </summary>
    public int Id { get; set; }

    public string OldPersonalId { get; set; } = string.Empty;

    public UidType OldUidType { get; set; }

    public string NewPersonalId { get; set; } = string.Empty;

    public UidType NewUidType { get; set; }

    /// <summary>
    /// Date of change
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Created on, auto fill
    /// </summary>
    public DateTime CreatedOn { get; set; }
}
