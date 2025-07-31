using eID.RO.Application.StateMachines;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eID.RO.Application.Consumers;

public class SagaChecksConsumer : BaseConsumer,
    IConsumer<CheckForEmpowermentsVerification>
{
    private readonly SagasDbContext _dbContext;


    public SagaChecksConsumer(
        ILogger<SagaChecksConsumer> logger,
        SagasDbContext dbContext) : base(logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task Consume(ConsumeContext<CheckForEmpowermentsVerification> context)
    {
        var empowermentIds = context.Message.EmpowermentIds.ToHashSet();

        var existingSagas = await _dbContext
                        .Set<EmpowermentVerificationState>()
                        .Where(x => empowermentIds.Contains(x.EmpowermentId))
                        .ToListAsync();

        var existingFinishedSagasIds = existingSagas
                                    .Where(x => x.CurrentState == "Final")
                                    .Select(x => x.EmpowermentId).ToHashSet();

        var allSagasExistAndFinished = empowermentIds.SetEquals(existingFinishedSagasIds);
        var existingSagasIds = existingSagas.Select(x => x.EmpowermentId).ToHashSet();
        var missingIds = empowermentIds.Except(existingSagasIds);

        await context.RespondAsync<EmpowermentsVerificationSagasCheckResult>(new
        {
            AllSagasExistAndFinished = allSagasExistAndFinished,
            MissingIds = missingIds
        });
    }
}
