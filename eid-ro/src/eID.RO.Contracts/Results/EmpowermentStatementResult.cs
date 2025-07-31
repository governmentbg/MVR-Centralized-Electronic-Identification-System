using System.Diagnostics.CodeAnalysis;
using eID.RO.Contracts.Enums;

namespace eID.RO.Contracts.Results;

public interface EmpowermentStatementResult
{
    /// <summary>
    /// Empowerment statement Id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Empowerment number. Template: РОx/dd.mm.yyyy. x is a integer, dd.mm.yyyy the date of action.
    /// </summary>
    public string Number { get; set; }
    /// <summary>
    /// On this date, once verified and signed, the empowerment can be considered active.
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// When ExpiryDate is set to null the empowerment will be active until withdrawn.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
    /// <summary>
    /// Current status
    /// </summary>
    public EmpowermentStatementStatus Status { get; set; }
    /// <summary>
    /// Uid of the individual/legal entity that empowers someone.
    /// </summary>
    public string Uid { get; set; }
    /// <summary>
    /// When OnBehalfOf.LegalEntity the value doesn't matter.
    /// If OnBehalfOf.Individual this is EGN or LNCh.
    /// </summary>
    public IdentifierType UidType { get; set; }
    /// <summary>
    /// Name of the individual/legal entity
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Indicates what's behind Uid 
    /// </summary>
    public OnBehalfOf OnBehalfOf { get; set; }
    /// <summary>
    /// Uids of people that need to sign the empowerment
    /// </summary>
    public IEnumerable<UidResult> AuthorizerUids { get; set; }
    /// <summary>
    /// Uids of people that are being empowered
    /// </summary>
    public IEnumerable<UidResult> EmpoweredUids { get; set; }
    /// <summary>
    /// Extended reference.
    /// </summary>
    public string ProviderId { get; set; }
    /// <summary>
    /// Extended reference. Used for display, sorting and filtering
    /// </summary>
    public string ProviderName { get; set; }
    /// <summary>
    /// Extended reference. Matches IISDA Service Id.
    /// </summary>
    public int ServiceId { get; set; }
    /// <summary>
    /// Extended reference. Used for display, sorting and filtering
    /// </summary>
    public string ServiceName { get; set; }
    /// <summary>
    /// Extended reference. Used for display, sorting and filtering
    /// </summary>
    public IEnumerable<VolumeOfRepresentationResult> VolumeOfRepresentation { get; set; }
    /// <summary>
    /// Created on
    /// </summary>
    public DateTime? CreatedOn { get; set; }
    /// <summary>
    /// Created name
    /// </summary>
    public string CreatedBy { get; set; }
    /// <summary>
    /// Position of issuer in legal entity
    /// </summary>
    public string IssuerPosition { get; set; }
    /// <summary>
    /// XML representation of empowerment
    /// </summary>
    public string XMLRepresentation { get; set; }
    /// <summary>
    /// Denial reason if the Empowerment statement is denied.
    /// </summary>
    public EmpowermentsDenialReason DenialReason { get; set; }
    /// <summary>
    /// Withdrawal information
    /// </summary>
    public IEnumerable<EmpowermentWithdrawResult> EmpowermentWithdrawals { get; set; }
    /// <summary>
    /// Disagreement information
    /// </summary>
    public IEnumerable<EmpowermentDisagreementResult> EmpowermentDisagreements { get; set; }
    /// <summary>
    /// Collection of status history containing records for all statuses that the empowerment.
    /// </summary>
    public IEnumerable<StatusHistoryResult> StatusHistory { get; set; }
    /// <summary>
    /// Status relative to a specific moment in time. This calculation must occur before the property is returned to the client.
    /// </summary>
    public CalculatedEmpowermentStatus CalculatedStatusOn { get; set; }
}

public static class Extensions
{
    /// <summary>
    /// Sets <see cref="EmpowermentStatementResult.CalculatedStatusOn"/> relative to <see cref="statusCheckDate"/>
    /// </summary>
    public static void CalculateStatusOn(this EmpowermentStatementResult statement, DateTime statusCheckDate)
    {
        var statusAtStatusCheckDate = statement.StatusHistory
            .FirstOrDefault(sh => sh.DateTime < statusCheckDate.ToUniversalTime());

        if (statusAtStatusCheckDate is null)
        {
            statement.CalculatedStatusOn = CalculatedEmpowermentStatus.None;
            return;
        }

        switch (statusAtStatusCheckDate.Status)
        {
            case EmpowermentStatementStatus.Created:
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.Created;
                return;
            case EmpowermentStatementStatus.CollectingAuthorizerSignatures:
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.CollectingAuthorizerSignatures;
                return;
            case EmpowermentStatementStatus.Denied:
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.Denied;
                return;
            case EmpowermentStatementStatus.Withdrawn:
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.Withdrawn;
                return;
            case EmpowermentStatementStatus.DisagreementDeclared:
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.DisagreementDeclared;
                return;
            case EmpowermentStatementStatus.Unconfirmed:
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.Unconfirmed;
                return;

            case EmpowermentStatementStatus.Active:
                if (IsStatementExpired(statement, statusCheckDate))
                {
                    statement.CalculatedStatusOn = CalculatedEmpowermentStatus.Expired;
                    return;
                }
                if (IsStatementUpComing(statement, statusCheckDate))
                {
                    statement.CalculatedStatusOn = CalculatedEmpowermentStatus.UpComing;
                    return;
                }
                if (IsStatementActive(statement, statusCheckDate))
                {
                    statement.CalculatedStatusOn = CalculatedEmpowermentStatus.Active;
                    return;
                }
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.None;
                return;
            default:
                statement.CalculatedStatusOn = CalculatedEmpowermentStatus.None;
                return;
        }

    }
    private static bool IsStatementExpired(EmpowermentStatementResult statement, DateTime statusCheckDate)
    {
        if (statement is null)
        {
            throw new ArgumentNullException(nameof(statement));
        }

        //check if statement actually has expired date
        if (!statement.ExpiryDate.HasValue)
        {
            return false;
        }

        //returns true if status check date is after empowerment expiry date 
        return statement.ExpiryDate.Value.Date < statusCheckDate.Date;
    }

    private static bool IsStatementUpComing(EmpowermentStatementResult statement, DateTime statusCheckDate)
    {
        if (statement is null)
        {
            throw new ArgumentNullException(nameof(statement));
        }

        // returns true if status check date is before statement start date and after statement activation date
        var activeStatusHistory = statement.StatusHistory.FirstOrDefault(s => s.Status == EmpowermentStatementStatus.Active);
        return statusCheckDate < statement.StartDate && statusCheckDate >= activeStatusHistory?.DateTime;
    }

    private static bool IsStatementActive(EmpowermentStatementResult statement, DateTime statusCheckDate)
    {
        if (statement is null)
        {
            throw new ArgumentNullException(nameof(statement));
        }

        //returns true if status check date is after empowerment start date 
        return statusCheckDate.ToUniversalTime() >= statement.StartDate;
    }
}


/// <summary>
/// Describe empowerment disagreement
/// </summary>
public interface EmpowermentDisagreementResult
{
    /// <summary>
    /// When the disagreement is activated
    /// </summary>
    public DateTime ActiveDateTime { get; set; }
    /// <summary>
    /// Who is started disagreement
    /// </summary>
    public string IssuerUid { get; set; }
    /// <summary>
    /// IssuerUid type
    /// </summary>
    public IdentifierType IssuerUidType { get; set; }
    /// <summary>
    /// Reason of disagreement
    /// </summary>
    public string Reason { get; set; }
}

/// <summary>
/// Contains Uid
/// </summary>
public interface UidResult
{
    /// <summary>
    /// Current Uid
    /// </summary>
    string Uid { get; }
    IdentifierType UidType { get; }
    string Name { get; }
}

public class UidResultComparer : IEqualityComparer<UidResult>
{
    public bool Equals(UidResult? x, UidResult? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Uid == y.Uid && x.UidType == y.UidType && x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode([DisallowNull] UidResult obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        // Combine hash codes of Uid and UidType
        int hashUid = obj.Uid.GetHashCode();
        int hashUidType = obj.UidType.GetHashCode();
        int name = obj.Name.GetHashCode();

        unchecked
        {
            // Simple hash code combination algorithm
            return ((hashUid << 5) + hashUid) ^ hashUidType ^ name;
        }
    }
}
