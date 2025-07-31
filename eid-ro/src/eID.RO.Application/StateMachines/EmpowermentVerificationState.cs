using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using eID.RO.Service.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace eID.RO.Application.StateMachines;

public class EmpowermentVerificationState :
SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid OriginCorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid? CurrentRequestId { get; set; }
    public DateTime ReceivedDateTime { get; set; }
    public DateTime? FinishedAt { get; set; }

    public Guid? EmpowermentValidationCheckExpirationTokenId { get; set; }

    public Guid EmpowermentId { get; set; }
    public OnBehalfOf OnBehalfOf { get; set; }
    [EncryptProperty]
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IssuerPosition { get; set; } = string.Empty;

    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; } = Enumerable.Empty<AuthorizerIdentifierData>();
    public IEnumerable<UserIdentifier> EmpoweredUids { get; set; } = Enumerable.Empty<UserIdentifierData>();
    public EmpowermentsDenialReason DenialReason { get; set; }
}
public class EmpowermentVerificationStateMap :
    SagaClassMap<EmpowermentVerificationState>
{
    protected override void Configure(EntityTypeBuilder<EmpowermentVerificationState> entity, ModelBuilder model)
    {
        model.UseEncryption();
        entity.ToTable("Sagas.EmpowermentVerifications");
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
