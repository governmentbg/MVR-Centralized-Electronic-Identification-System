using eID.RO.Contracts.Results;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
    public IEnumerable<UserIdentifierWithName> AuthorizerUids { get; set; } = Enumerable.Empty<UserIdentifierWithNameData>();
    public List<UserIdentifierWithName> SignedUids { get; set; } = new List<UserIdentifierWithName>();
}
public class SignaturesCollectionStateMap :
    SagaClassMap<SignaturesCollectionState>
{
    protected override void Configure(EntityTypeBuilder<SignaturesCollectionState> entity, ModelBuilder model)
    {
        entity.ToTable("Sagas.SignaturesCollections");
        entity.Property(x => x.CurrentState).HasMaxLength(64);

        entity.Property(x => x.AuthorizerUids).HasJsonConversion();
        entity.Property(x => x.SignedUids).HasJsonConversion();
    }
}
