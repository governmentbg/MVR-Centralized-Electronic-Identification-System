using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using MassTransit;

namespace eID.PAN.Application.Consumers;

public class UserNotificationsConsumer : BaseConsumer,
    IConsumer<GetSystemsAndNotificationsByFilter>,
    IConsumer<GetDeactivatedUserNotifications>,
    IConsumer<RegisterDeactivatedEvents>
{
    private readonly UserNotificationsService _service;

    public UserNotificationsConsumer(ILogger<UserNotificationsConsumer> logger, UserNotificationsService service)
        : base(logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    } 

    public async Task Consume(ConsumeContext<GetSystemsAndNotificationsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _service.GetByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetDeactivatedUserNotifications> context)
    {
        await ExecuteMethodAsync(context, () => _service.GetDeactivatedAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RegisterDeactivatedEvents> context)
    {
        await ExecuteMethodAsync(context, () => _service.RegisterDeactivatedEventsAsync(context.Message));
    }
}
