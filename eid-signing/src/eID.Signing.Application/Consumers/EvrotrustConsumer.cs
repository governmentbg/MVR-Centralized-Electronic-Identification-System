using eID.Signing.Contracts.Commands;
using eID.Signing.Service;
using MassTransit;

namespace eID.Signing.Application.Consumers;

public class EvrotrustConsumer : BaseConsumer,
    IConsumer<EvrotrustGetFileStatusByTransactionId>,
    IConsumer<EvrotrustDownloadFileByTransactionId>,
    IConsumer<EvrotrustSignDocument>,
    IConsumer<EvrotrustCheckUserByUid>
{
    private readonly EvrotrustSigningService _evrotrustService;

    public EvrotrustConsumer(ILogger<EvrotrustConsumer> logger, EvrotrustSigningService evrotrustService)
        : base(logger)
    {
        _evrotrustService = evrotrustService ?? throw new ArgumentNullException(nameof(evrotrustService));
    }

    public async Task Consume(ConsumeContext<EvrotrustGetFileStatusByTransactionId> context)
    {
        await ExecuteMethodAsync(context, () => _evrotrustService.GetFileStatusAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<EvrotrustDownloadFileByTransactionId> context)
    {
        await ExecuteMethodAsync(context, () => _evrotrustService.DownloadFileAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<EvrotrustSignDocument> context)
    {
        await ExecuteMethodAsync(context, () => _evrotrustService.SignDocumentAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<EvrotrustCheckUserByUid> context)
    {
        await ExecuteMethodAsync(context, () => _evrotrustService.CheckUserAsync(context.Message));
    }
}

public class EvrotrustConsumerDefinition : ConsumerDefinition<EvrotrustConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EvrotrustConsumer> consumerConfigurator)
    {
        endpointConfigurator.DiscardFaultedMessages();
        endpointConfigurator.DiscardSkippedMessages();
    }
}
