using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using eID.RO.Service.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace eID.RO.Application.StateMachines;

public class WithdrawalsCollectionState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    /// <summary>
    /// In initial state this property should be null
    /// </summary>
    public string CurrentState { get; set; }
    public Guid OriginCorrelationId { get; set; }
    public DateTime ReceivedDateTime { get; set; }
    public DateTime WithdrawalsCollectionsDeadline { get; set; }
    /// <summary>
    /// User who initiated withdraw operation. EGN or LNCh
    /// </summary>
    [EncryptProperty]
    public string IssuerUid { get; set; } = string.Empty;
    /// <summary>
    /// IssuerUid type
    /// </summary>
    public IdentifierType IssuerUidType { get; set; }
    /// <summary>
    /// Withdraw reason
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    /// <summary>
    /// Id of the empowerment undergoing withdrawal process
    /// </summary>
    public Guid EmpowermentId { get; set; }
    public OnBehalfOf OnBehalfOf { get; set; }
    public IEnumerable<UserIdentifier> AuthorizerUids { get; set; } = Enumerable.Empty<UserIdentifierData>();
    /// <summary>
    /// Empowered Uids we use in case of successful operation to notify them
    /// </summary>
    public IEnumerable<UserIdentifier> EmpoweredUids { get; set; } = Enumerable.Empty<UserIdentifierData>();

    public Guid? WithdrawalsCollectionTimeoutTokenId { get; set; }
    public Guid EmpowermentWithdrawalId { get; set; }
    /// <summary>
    /// Uid of the legal entity that empowers someone.
    /// </summary>
    [EncryptProperty]
    public string? LegalEntityUid { get; set; }
    /// <summary>
    /// Name of the legal entity
    /// </summary>
    public string? LegalEntityName { get; set; }
    /// <summary>
    /// Name of the initiator of the operation
    /// </summary>
    public string IssuerName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the position the issuer has in the legal entity
    /// </summary>
    public string? IssuerPosition { get; set; }
}

internal class WithdrawalsCollectionStateMap : SagaClassMap<WithdrawalsCollectionState>
{
    protected override void Configure(EntityTypeBuilder<WithdrawalsCollectionState> entity, ModelBuilder model)
    {
        model.UseEncryption();
        entity.ToTable("Sagas.WithdrawalsCollections");
        entity.Property(x => x.CurrentState).HasMaxLength(64);

        entity.Property(x => x.Reason).HasMaxLength(256);
        entity.Property(x => x.AuthorizerUids).HasConversion(
            x => EncryptionHelper.Encrypt(JsonConvert.SerializeObject(x)),
            x => JsonConvert.DeserializeObject<IEnumerable<AuthorizerIdentifierData>>(EncryptionHelper.Decrypt(x))
        );
        entity.Property(x => x.EmpoweredUids).HasConversion(
            x => EncryptionHelper.Encrypt(JsonConvert.SerializeObject(x)),
            x => JsonConvert.DeserializeObject<IEnumerable<UserIdentifierData>>(EncryptionHelper.Decrypt(x))
        );
    }
}
