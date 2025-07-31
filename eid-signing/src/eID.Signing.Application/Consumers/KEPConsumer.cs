using eID.Signing.Contracts.Commands;
using eID.Signing.Service;
using MassTransit;

namespace eID.Signing.Application.Consumers;

public class KEPConsumer : BaseConsumer,
    IConsumer<KEPGetDataToSign>,
    IConsumer<KEPSignData>,
    IConsumer<KEPGetDigestToSign>,
    IConsumer<KEPSignDigest>
{
    private readonly KEPSigningService _kepService;

    public KEPConsumer(ILogger<KEPConsumer> logger, KEPSigningService kepService)
        : base(logger)
    {
        _kepService = kepService ?? throw new ArgumentNullException(nameof(kepService));
    }

    public async Task Consume(ConsumeContext<KEPGetDataToSign> context)
    {
        await ExecuteMethodAsync(context, () => _kepService.GetDataToSignAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<KEPSignData> context)
    {
        await ExecuteMethodAsync(context, () => _kepService.SignDataAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<KEPGetDigestToSign> context)
    {
        await ExecuteMethodAsync(context, () => _kepService.GetDigestToSignAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<KEPSignDigest> context)
    {
        await ExecuteMethodAsync(context, () => _kepService.SignDigestAsync(context.Message));
    }
}

public class KEPConsumerDefinition : ConsumerDefinition<KEPConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<KEPConsumer> consumerConfigurator)
    {
        endpointConfigurator.DiscardFaultedMessages();
        endpointConfigurator.DiscardSkippedMessages();
    }
}
