using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace eID.RO.Application.StateMachines;

public class SagasDbContext :
    SagaDbContext
{
    public SagasDbContext(DbContextOptions<SagasDbContext> options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { 
            yield return new EmpowermentActivationStateMap();
            yield return new SignaturesCollectionStateMap();
            yield return new WithdrawalsCollectionStateMap();
        }
    }
}
