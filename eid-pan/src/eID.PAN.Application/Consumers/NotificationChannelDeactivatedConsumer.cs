using eID.PAN.Contracts.Events;
using eID.PAN.Service;
using MassTransit;

namespace eID.PAN.Application.Consumers;

public class NotificationChannelDeactivatedConsumer : BaseConsumer,
    IConsumer<NotificationChannelDeactivated>
{
    public NotificationChannelDeactivatedConsumer(
        ILogger<CommunicationsService> logger) : base(logger)
    {
    }

    public async Task Consume(ConsumeContext<NotificationChannelDeactivated> context)
    {
        // TODO: ако даден канал се деактивира,
        // трябва да се изпрати служебна нотификация към гражданите (по default канала),
        // че желаният от тях канал вече го няма и трябва да влязат да оправят настройките
        // като default-ен за момента ще е имейл.
        await ExecuteMethodAsync(context, () => throw new NotImplementedException());
    }
}
