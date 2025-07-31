using eID.Signing.Contracts.Commands;
using eID.Signing.Service;
using MassTransit;

namespace eID.Signing.Application.Consumers;

public class BoricaConsumer : BaseConsumer,
    IConsumer<BoricaGetFileStatusByTransactionId>,
    IConsumer<BoricaDownloadFileByTransactionId>,
    IConsumer<BoricaSignDocument>,
    IConsumer<BoricaCheckUserByUid>,
    IConsumer<BoricaSendConsent>,
    IConsumer<BoricaCheckConsentStatus>,
    IConsumer<BoricaARSSignDocument>,
    IConsumer<BoricaGetAccessTokens>,
    IConsumer<BoricaAddAccessToken>
{
    private readonly BoricaSigningService _boricaService;

    public BoricaConsumer(ILogger<BoricaConsumer> logger, BoricaSigningService boricaService)
        : base(logger)
    {
        _boricaService = boricaService ?? throw new ArgumentNullException(nameof(boricaService));
    }

    public async Task Consume(ConsumeContext<BoricaGetFileStatusByTransactionId> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.GetFileStatusAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<BoricaDownloadFileByTransactionId> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.DownloadFileAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<BoricaSignDocument> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.SignDocumentAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<BoricaCheckUserByUid> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.CheckUserAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<BoricaSendConsent> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.SendConsentAsync());
    }
    public async Task Consume(ConsumeContext<BoricaCheckConsentStatus> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.CheckConsentAsync(context.Message.CallbackId));
    }
    public async Task Consume(ConsumeContext<BoricaARSSignDocument> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.AutomaticRemoteSignDocumentAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<BoricaGetAccessTokens> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.GetAccessTokensAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<BoricaAddAccessToken> context)
    {
        await ExecuteMethodAsync(context, () => _boricaService.AddAccessTokenAsync(context.Message));
    }
}

public class BoricaConsumerDefinition : ConsumerDefinition<BoricaConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<BoricaConsumer> consumerConfigurator)
    {
        endpointConfigurator.DiscardFaultedMessages();
        endpointConfigurator.DiscardSkippedMessages();
    }
}
