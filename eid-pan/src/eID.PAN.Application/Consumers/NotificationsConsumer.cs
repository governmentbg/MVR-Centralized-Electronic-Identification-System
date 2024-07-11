using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using MassTransit;

namespace eID.PAN.Application.Consumers;

public class NotificationsConsumer : BaseConsumer,
    IConsumer<GetNotificationsByFilter>,
    IConsumer<RegisterSystem>,
    IConsumer<ModifyEvent>,
    IConsumer<GetSystemById>,
    IConsumer<RejectSystem>,
    IConsumer<ApproveSystem>,
    IConsumer<ArchiveSystem>,
    IConsumer<RestoreSystem>,
    IConsumer<SendNotification>,
    IConsumer<GetRegisteredSystemsRejected>,
    IConsumer<GetSystemsByFilter>
{
    private readonly NotificationsService _notificationsService;

    public NotificationsConsumer(ILogger<NotificationsConsumer> logger, NotificationsService notificationsService)
        : base(logger)
    {
        _notificationsService = notificationsService ?? throw new ArgumentNullException(nameof(notificationsService));
    }

    public async Task Consume(ConsumeContext<GetNotificationsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.GetByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RegisterSystem> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.RegisterSystemAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ModifyEvent> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.ModifyEventAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetSystemById> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.GetSystemByIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SendNotification> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.SendAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RejectSystem> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.RejectSystemAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ApproveSystem> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.ApproveSystemAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ArchiveSystem> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.ArchiveSystemAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RestoreSystem> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.RestoreSystemAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetRegisteredSystemsRejected> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.GetRegisteredSystemsRejectedAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetSystemsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _notificationsService.GetSystemsByFilterAsync(context.Message));
    }
}
