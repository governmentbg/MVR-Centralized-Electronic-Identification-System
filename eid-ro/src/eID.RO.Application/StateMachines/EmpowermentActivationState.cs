using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using eID.RO.Service.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace eID.RO.Application.StateMachines;

public class EmpowermentActivationState :
SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid OriginCorrelationId { get; set; }
    public string CurrentState { get; set; }
    public DateTime ReceivedDateTime { get; set; }
    public DateTime ActivationDeadline { get; set; }

    public Guid? EmpowermentActivationTimeoutTokenId { get; set; }

    public Guid EmpowermentId { get; set; }
    public OnBehalfOf OnBehalfOf { get; set; }

    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; } = Enumerable.Empty<AuthorizerIdentifierData>();
    public IEnumerable<UserIdentifier> EmpoweredUids { get; set; } = Enumerable.Empty<UserIdentifierData>();
    [EncryptProperty]
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
    public string IssuerPosition { get; set; } = string.Empty;
    public bool SuccessfulCompletion { get; set; } = false;
    public EmpowermentsDenialReason DenialReason { get; set; }
    public bool LegalEntityCannotBeConfirmed { get; set; } = false;
    public bool IsEmpowermentWithdrawn { get; set; } = false;
}
public class EmpowermentActivationStateMap :
    SagaClassMap<EmpowermentActivationState>
{
    protected override void Configure(EntityTypeBuilder<EmpowermentActivationState> entity, ModelBuilder model)
    {
        model.UseEncryption();
        entity.ToTable("Sagas.EmpowermentActivations");
        entity.Property(x => x.CurrentState).HasMaxLength(64);

        entity.Property(x => x.AuthorizerUids).HasConversion(
            x => EncryptionHelper.Encrypt(JsonConvert.SerializeObject(x)),
            x => JsonConvert.DeserializeObject<IEnumerable<AuthorizerIdentifierData>>(EncryptionHelper.Decrypt(x))
        );
        entity.Property(x => x.EmpoweredUids).HasConversion(
            x => EncryptionHelper.Encrypt(JsonConvert.SerializeObject(x)),
            x => JsonConvert.DeserializeObject<IEnumerable<UserIdentifierData>>(EncryptionHelper.Decrypt(x))
        );
        entity.Property(x => x.IssuerPosition).IsRequired(false);
    }
}
