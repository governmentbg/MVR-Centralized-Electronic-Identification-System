using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service;
using MassTransit;

namespace eID.PIVR.Application.Consumers;

public class DateOfDeathConsumer : BaseConsumer,
    IConsumer<GetDateOfDeath>,
    IConsumer<GetDeceasedByPeriod>
{
    private readonly DateOfDeathService _dateOfDeathService;

    public DateOfDeathConsumer(
        ILogger<DateOfDeathConsumer> logger,
        DateOfDeathService dateOfDeathService) : base(logger)
    {
        _dateOfDeathService = dateOfDeathService ?? throw new ArgumentNullException(nameof(dateOfDeathService));
    }

    public async Task Consume(ConsumeContext<GetDateOfDeath> context)
    {
        await ExecuteMethodAsync(context, () => _dateOfDeathService.GetByPersonalIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetDeceasedByPeriod> context)
    {
        await ExecuteMethodAsync(context, () => _dateOfDeathService.GetDeceasedByPeriodAsync(context.Message));
    }
}

public class DateOfDeathConsumerDefinition : ConsumerDefinition<DateOfDeathConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DateOfDeathConsumer> consumerConfigurator)
    {
        endpointConfigurator.DiscardFaultedMessages();
        endpointConfigurator.DiscardSkippedMessages();
    }
}

