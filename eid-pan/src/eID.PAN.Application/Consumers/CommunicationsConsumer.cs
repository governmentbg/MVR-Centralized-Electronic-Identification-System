using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using MassTransit;

namespace eID.PAN.Application.Consumers;

public class CommunicationsConsumer : BaseConsumer,
    IConsumer<SendEmail>,
    IConsumer<SendPushNotification>,
    IConsumer<SendSms>,
    IConsumer<SendHttpCallbackAsync>,
    IConsumer<SendDirectEmail>
{
    private readonly CommunicationsService _communicationsService;

    public CommunicationsConsumer(
        ILogger<CommunicationsService> logger,
        CommunicationsService communicationsService) : base(logger)
    {
        _communicationsService = communicationsService ?? throw new ArgumentNullException(nameof(communicationsService));
    }

    public async Task Consume(ConsumeContext<SendEmail> context)
    {
        await ExecuteMethodAsync(context, () => _communicationsService.SendEmailAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SendPushNotification> context)
    {
        await ExecuteMethodAsync(context, () => _communicationsService.SendPushNotificationAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SendSms> context)
    {
        await ExecuteMethodAsync(context, () => _communicationsService.SendSmsAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SendHttpCallbackAsync> context)
    {
        await ExecuteMethodAsync(context, () => _communicationsService.SendHttpCallbackAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SendDirectEmail> context)
    {
        await ExecuteMethodAsync(context, () => _communicationsService.SendDirectEmailAsync(context.Message));
    }
}
