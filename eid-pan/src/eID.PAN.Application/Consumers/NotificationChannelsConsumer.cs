using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using MassTransit;

namespace eID.PAN.Application.Consumers;

public class NotificationChannelsConsumer : BaseConsumer,
    IConsumer<GetAllNotificationChannels>,
    IConsumer<RegisterNotificationChannel>,
    IConsumer<ModifyNotificationChannel>,
    IConsumer<ApproveNotificationChannel>,
    IConsumer<RejectNotificationChannel>,
    IConsumer<ArchiveNotificationChannel>,
    IConsumer<RestoreNotificationChannel>,
    IConsumer<TestNotificationChannel>
{
    private readonly NotificationChannelsService _notificationChannelsService;

    public NotificationChannelsConsumer(
        ILogger<NotificationChannelsConsumer> logger,
        NotificationChannelsService notificationChannelsService) : base(logger)
    {
        _notificationChannelsService = notificationChannelsService ?? throw new ArgumentNullException(nameof(notificationChannelsService));
    }

    public async Task Consume(ConsumeContext<GetAllNotificationChannels> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.GetAllChannelsAsync());
    }

    public async Task Consume(ConsumeContext<RegisterNotificationChannel> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.RegisterChannelAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ModifyNotificationChannel> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.ModifyChannelAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ApproveNotificationChannel> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.ApproveChannelAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RejectNotificationChannel> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.RejectChannelAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ArchiveNotificationChannel> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.ArchiveChannelAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RestoreNotificationChannel> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.RestoreChannelAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<TestNotificationChannel> context)
    {
        await ExecuteMethodAsync(context, () => _notificationChannelsService.TestChannelAsync(context.Message));
    }
}

public class NotificationChannelsConsumerConsumerDefinition : ConsumerDefinition<NotificationChannelsConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<NotificationChannelsConsumer> consumerConfigurator)
    {
        endpointConfigurator.DiscardFaultedMessages();
        endpointConfigurator.DiscardSkippedMessages();
    }
}

