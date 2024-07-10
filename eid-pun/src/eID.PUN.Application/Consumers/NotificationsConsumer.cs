using eID.PUN.Contracts.Commands;
using eID.PUN.Service;
using MassTransit;

namespace eID.PUN.Application.Consumers;

public class NotificationsConsumer : BaseConsumer,
        IConsumer<NotifyEIds>
{
    private readonly NotificationsService _notificationsService;

    public NotificationsConsumer(
        ILogger<NotificationsConsumer> logger,
        NotificationsService notificationsService) : base(logger)
    {
        _notificationsService = notificationsService ?? throw new ArgumentNullException(nameof(notificationsService));
    }

    public async Task Consume(ConsumeContext<NotifyEIds> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.SendAsync(context.Message));
    }
}
