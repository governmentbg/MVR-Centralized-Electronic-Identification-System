using eID.RO.Contracts.Commands;
using eID.RO.Service;
using MassTransit;

namespace eID.RO.Application.Consumers
{
    public class NotificationsConsumer : BaseConsumer,
        IConsumer<Batch<NotifyUids>>
    {
        private readonly NotificationSenderService _notificationSenderService;

        public NotificationsConsumer(
            ILogger<NotificationsConsumer> logger,
            NotificationSenderService notificationSenderService) : base(logger)
        {
            _notificationSenderService = notificationSenderService ?? throw new ArgumentNullException(nameof(notificationSenderService));
        }

        public async Task Consume(ConsumeContext<Batch<NotifyUids>> context)
        {
            for (int i = 0; i < context.Message.Length; i++)
            {
                ConsumeContext<NotifyUids> message = context.Message[i];
                // TODO: Implement notifications sending
                var serviceResult = await _notificationSenderService.SendAsync(message.Message);

                await context.RespondAsync(serviceResult);
            }
        }
    }
    public class NotificationsConsumerDefinition : ConsumerDefinition<NotificationsConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<NotificationsConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
            endpointConfigurator.DiscardSkippedMessages();
        }
    }
}
