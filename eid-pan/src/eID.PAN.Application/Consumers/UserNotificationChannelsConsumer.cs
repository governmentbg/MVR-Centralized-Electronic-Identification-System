using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using MassTransit;

namespace eID.PAN.Application.Consumers;

public class UserNotificationChannelsConsumer : BaseConsumer,
    IConsumer<GetUserNotificationChannelsByFilter>,
    IConsumer<GetUserNotificationChannels>,
    IConsumer<RegisterUserNotificationChannels>
{
    private readonly UserNotificationChannelsService _service;

    public UserNotificationChannelsConsumer(ILogger<UserNotificationChannelsConsumer> logger, UserNotificationChannelsService service)
        : base(logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task Consume(ConsumeContext<GetUserNotificationChannelsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _service.GetByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetUserNotificationChannels> context)
    {
        await ExecuteMethodAsync(context, () => _service.GetSelectedAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RegisterUserNotificationChannels> context)
    {
        await ExecuteMethodAsync(context, () => _service.RegisterSelectedAsync(context.Message));
    }
}
