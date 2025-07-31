using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace eID.RO.Application.StateMachines;

public class SignaturesCollectionState :
SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid OriginCorrelationId { get; set; }
    public string CurrentState { get; set; }
    public DateTime ReceivedDateTime { get; set; }
    public DateTime SignaturesCollectionDeadline { get; set; }

    public Guid? SignatureCollectionTimeoutTokenId { get; set; }

    public Guid EmpowermentId { get; set; }
    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; } = Enumerable.Empty<AuthorizerIdentifierData>();
    public List<AuthorizerIdentifier> SignedUids { get; set; } = new List<AuthorizerIdentifier>();
    public bool IsEmpowermentWithdrawn { get; set; } = false;
}
public class SignaturesCollectionStateMap :
    SagaClassMap<SignaturesCollectionState>
{
    protected override void Configure(EntityTypeBuilder<SignaturesCollectionState> entity, ModelBuilder model)
    {
        entity.ToTable("Sagas.SignaturesCollections");
        entity.Property(x => x.CurrentState).HasMaxLength(64);

        entity.Property(x => x.AuthorizerUids).HasConversion(
            x => EncryptionHelper.Encrypt(JsonConvert.SerializeObject(x)),
            x => JsonConvert.DeserializeObject<IEnumerable<AuthorizerIdentifierData>>(EncryptionHelper.Decrypt(x))
        );
        entity.Property(x => x.SignedUids).HasConversion(
            x => EncryptionHelper.Encrypt(JsonConvert.SerializeObject(x)),
            x => JsonConvert.DeserializeObject<IEnumerable<AuthorizerIdentifierData>>(EncryptionHelper.Decrypt(x)).ToList<AuthorizerIdentifier>()
        );
    }
}
