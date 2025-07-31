using eID.MIS.Contracts.SEV.Commands;
using eID.MIS.Service;
using MassTransit;

namespace eID.MIS.Application.Consumers;

public class EDeliveryRequestsConsumer : BaseConsumer,
    IConsumer<CreatePassiveIndividualProfile>,
    IConsumer<SearchUserProfile>,
    IConsumer<GetUserProfile>,
    IConsumer<SendMessageOnBehalf>,
    IConsumer<SendMessage>,
    IConsumer<GetDeliveries>
{
    private readonly EDeliveryRequestsService _eDeliveryRequestsService;

    public EDeliveryRequestsConsumer(
        ILogger<EDeliveryRequestsConsumer> logger,
        EDeliveryRequestsService eDeliveryRequestsService) : base(logger)
    {
        _eDeliveryRequestsService = eDeliveryRequestsService ?? throw new ArgumentNullException(nameof(eDeliveryRequestsService));
    }

    public async Task Consume(ConsumeContext<CreatePassiveIndividualProfile> context)
    {
        await ExecuteMethodAsync(context, () => _eDeliveryRequestsService.CreatePassiveIndividualProfileAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SearchUserProfile> context)
    {
        await ExecuteMethodAsync(context, () => _eDeliveryRequestsService.SearchUserProfileAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetUserProfile> context)
    {
        await ExecuteMethodAsync(context, () => _eDeliveryRequestsService.GetUserProfileAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SendMessageOnBehalf> context)
    {
        await ExecuteMethodAsync(context, () => _eDeliveryRequestsService.SendMessageOnBehalfAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<SendMessage> context)
    {
        await ExecuteMethodAsync(context, () => _eDeliveryRequestsService.SendMessageAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetDeliveries> context)
    {
        await ExecuteMethodAsync(context, () => _eDeliveryRequestsService.GetDeliveriesAsync(context.Message));
    }
}

public class EDeliveryRequestsConsumerDefinition : ConsumerDefinition<EDeliveryRequestsConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EDeliveryRequestsConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromHours(1), TimeSpan.FromHours(12), TimeSpan.FromHours(24)));
        endpointConfigurator.UseMessageRetry(r =>
        {
            r.Interval(5, TimeSpan.FromSeconds(10));
        });

        endpointConfigurator.DiscardSkippedMessages();
    }
}
