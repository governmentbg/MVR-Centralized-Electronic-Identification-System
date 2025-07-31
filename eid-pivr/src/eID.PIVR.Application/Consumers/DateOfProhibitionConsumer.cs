using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service;
using MassTransit;

namespace eID.PIVR.Application.Consumers;

public class DateOfProhibitionConsumer : BaseConsumer,
    IConsumer<GetDateOfProhibition>
{
    private readonly DateOfProhibitionService _dateOfProhibitionService;

    public DateOfProhibitionConsumer(
        ILogger<DateOfProhibitionConsumer> logger,
        DateOfProhibitionService dateOfProhibitionService) : base(logger)
    {
        _dateOfProhibitionService = dateOfProhibitionService ?? throw new ArgumentNullException(nameof(dateOfProhibitionService));
    }

    public async Task Consume(ConsumeContext<GetDateOfProhibition> context)
    {
        await ExecuteMethodAsync(context, () => _dateOfProhibitionService.GetByPersonalIdAsync(context.Message));
    }
}

public class DateOfProhibitionConsumerDefinition : ConsumerDefinition<DateOfProhibitionConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DateOfProhibitionConsumer> consumerConfigurator)
    {
        endpointConfigurator.DiscardFaultedMessages();
        endpointConfigurator.DiscardSkippedMessages();
    }
}
