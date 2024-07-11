using System.ComponentModel.DataAnnotations.Schema;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;

namespace eID.RO.Service.Entities;

public class EmpowermentStatement : EmpowermentStatementResult, EmpowermentStatementFromMeResult
{
    public Guid Id { get; set; }
    /// <summary>
    /// Uid of the individual/legal entity that empowers someone.
    /// </summary>
    public string Uid { get; set; }
    /// <summary>
    /// When OnBehalfOf.LegalEntity the value doesn't matter.
    /// If OnBehalfOf.Individual this is taken from the token.
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
    public ICollection<AuthorizerUid> AuthorizerUids { get; set; } = new List<AuthorizerUid>();
    /// <summary>
    /// Uids of people that are being empowered
    /// </summary>
    public ICollection<EmpoweredUid> EmpoweredUids { get; set; } = new List<EmpoweredUid>();
    public string SupplierId { get; set; }
    /// <summary>
    /// Extended reference. Used for display, sorting and filtering
    /// </summary>
    public string SupplierName { get; set; }
    public int ServiceId { get; set; }
    /// <summary>
    /// Extended reference. Used for display, sorting and filtering
    /// </summary>
    public string ServiceName { get; set; }
    /// <summary>
    /// Extended reference. Used for display, sorting and filtering
    /// </summary>
    public ICollection<VolumeOfRepresentation> VolumeOfRepresentation { get; set; } = new List<VolumeOfRepresentation>();
    public EmpowermentStatementStatus Status { get; set; }
    /// <summary>
    /// UTC. On this date, once verified and signed, the empowerment can be considered active.
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// UTC. When ExpiryDate is set to null the empowerment will be active until withdrawn.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
    public string XMLRepresentation { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    /// <summary>
    /// Name of the position the issuer has in the legal entity
    /// </summary>
    public string? IssuerPosition { get; set; }
    public ICollection<EmpowermentWithdrawal> EmpowermentWithdrawals { get; set; } = new List<EmpowermentWithdrawal>();
    public ICollection<EmpowermentDisagreement> EmpowermentDisagreements { get; set; } = new List<EmpowermentDisagreement>();
    public ICollection<EmpowermentSignature> EmpowermentSignatures { get; set; } = new List<EmpowermentSignature>();
    public ICollection<StatusHistoryRecord> StatusHistory { get; set; } = new List<StatusHistoryRecord>();
    public EmpowermentTimestamp? Timestamp { get; set; }
    public EmpowermentsDenialReason DenialReason { get; set; }
    public string? DenialReasonComment { get; set; }
    [NotMapped]
    public CalculatedEmpowermentStatus CalculatedStatusOn { get; set; }
    IEnumerable<UidResult> EmpowermentStatementResult.AuthorizerUids { get => AuthorizerUids; set => throw new NotImplementedException(); }
    IEnumerable<UidResult> EmpowermentStatementResult.EmpoweredUids { get => EmpoweredUids; set => throw new NotImplementedException(); }
    IEnumerable<VolumeOfRepresentationResult> EmpowermentStatementResult.VolumeOfRepresentation { get => VolumeOfRepresentation; set => throw new NotImplementedException(); }
    IEnumerable<EmpowermentSignatureResult> EmpowermentStatementFromMeResult.EmpowermentSignatures { get => EmpowermentSignatures; }
    IEnumerable<EmpowermentWithdrawResult> EmpowermentStatementResult.EmpowermentWithdrawals { get => EmpowermentWithdrawals; set => throw new NotImplementedException(); }
    IEnumerable<EmpowermentDisagreementResult> EmpowermentStatementResult.EmpowermentDisagreements { get => EmpowermentDisagreements; set => throw new NotImplementedException(); }
    IEnumerable<StatusHistoryResult> EmpowermentStatementResult.StatusHistory { get => StatusHistory; set => throw new NotImplementedException(); }
}
